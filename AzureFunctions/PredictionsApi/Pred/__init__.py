#The function returns the predicted MealTotal, WasteTotalKg, LineWasteKg, PlateWasteKg variables for the next 7 days.

import logging
import azure.functions as func
import os
import pypyodbc as pyodbc
import numpy as np
import pandas as pd
from sklearn import linear_model
pyodbc.lowercase = False

def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')
    date = req.params.get('date')
    if not date:
        today = pd.to_datetime('2020-2-10')
    else:
        try:
            today = pd.to_datetime(date)
        except:
            today = pd.to_datetime('2020-2-10')
            pass
    connectionString = os.environ['WastelessDb']
    cnxn = pyodbc.connect(connectionString)

    #Load pandas dataframes with SQL querys
    df = pd.read_sql_query('select x.DateSID, ProductionWasteKg, LineWasteKg, PlateWasteKg, WasteTotalKg,MealCount, SpecialMealCount, MealTotal, Menu, PELessonBeforeLunchTurnout, HouseholdLessonBeforeLunchTurnout, StudentCountTotal, Cloudiness, Temperature, DayOfWeek, 1 LocationSID from dw.dimDate x left join [DW].[FacWasteless] y on x.datesid = y.datesid and y.LocationSID = 1 left join DW.FacMenu z on z.DateSID = x.DateSID and z.LocationSID = 1 left join DW.FacTimeTableExtra v on v.DateSID = x.DateSID and v.LocationSID = 1 left join DW.FacStudentCountPerDay w on w.DateSID=x.DateSID and w.LocationSID=1 left join DW.FacWeatherInfo q on q.DateSID=x.DateSID and q.LocationSID=1 where x.Date <= dateadd(day,8,getdate()) and x.Date >= convert(datetime, convert(char(8),20171127)) and x.WorkDay = 1 order by datesid asc',cnxn)    
    dfjs = pd.read_sql_query('select x.DateSID, ProductionWasteKg, LineWasteKg, PlateWasteKg, WasteTotalKg,MealCount, SpecialMealCount, MealTotal, Menu, PELessonBeforeLunchTurnout, HouseholdLessonBeforeLunchTurnout, StudentCountTotal, Cloudiness, Temperature, DayOfWeek, 2 LocationSID from dw.dimDate x left join [DW].[FacWasteless] y on x.datesid = y.datesid and y.LocationSID = 2 left join DW.FacMenu z on z.DateSID = x.DateSID and z.LocationSID = 2 left join DW.FacTimeTableExtra v on v.DateSID = x.DateSID and v.LocationSID = 2 left join DW.FacStudentCountPerDay w on w.DateSID=x.DateSID and w.LocationSID=2 left join DW.FacWeatherInfo q on q.DateSID=x.DateSID and q.LocationSID=2 where x.Date <= dateadd(day,8,getdate()) and x.Date >= convert(datetime, convert(char(8),20171127)) and x.WorkDay = 1 order by datesid asc',cnxn)    
    dfevents = pd.read_sql_query('select DateSID, EventDesc, PackedLunchCount, LocationSID from DW.FacEventCalendar', cnxn)
    
    #Create moving average of the MealTotal variable
    for i in range(1,6):
        df['MealTotalLag'+str(i)] = df['MealTotal'].shift(i)
    df['MealTotalMA'] = df.apply(lambda row: np.mean([row['MealTotalLag1'],row['MealTotalLag2'],row['MealTotalLag3'],row['MealTotalLag4'],row['MealTotalLag5']]), axis=1)
    df['MealTotalMA'].fillna(method='ffill', inplace=True)
    df['MealTotalMA'].fillna(method='bfill', inplace=True)
    for i in range(1,6):
        dfjs['MealTotalLag'+str(i)] = df['MealTotal'].shift(i)
    dfjs['MealTotalMA'] = dfjs.apply(lambda row: np.mean([row['MealTotalLag1'],row['MealTotalLag2'],row['MealTotalLag3'],row['MealTotalLag4'],row['MealTotalLag5']]), axis=1)
    dfjs['MealTotalMA'].fillna(method='ffill', inplace=True)
    dfjs['MealTotalMA'].fillna(method='bfill', inplace=True)

    #Replace zeros with nan
    df = df.append(dfjs)
    df['WasteTotalKg'].replace({0:np.nan}, inplace=True)
    
    #Change date type from string to datetime
    df['Date'] = pd.to_datetime(df['DateSID'], format='%Y%m%d')
    dfevents['Date'] = pd.to_datetime(dfevents['DateSID'], format='%Y%m%d')

    #Feature engineering and missing value treatment
    df['PackedLunchCount'] = df.apply(lambda row: np.max(dfevents['PackedLunchCount'].loc[(dfevents['Date']==row['Date']) & (dfevents['LocationSID']==row['LocationSID'])]), axis=1)
    df['PackedLunchCount'].fillna(0, inplace=True)
    df['Event'] = df.apply(lambda row: '-'.join(dfevents['EventDesc'].loc[(dfevents['Date']==row['Date']) & (dfevents['LocationSID']==row['LocationSID'])]), axis=1)
    df['TET'] = df.apply(lambda row: 1 if 'TET' in row['Event'] else 0, axis=1)
    df['Leirikoulu'] = df.apply(lambda row: 1 if 'leirikoulu' in row['Event'] else 0, axis=1)
    df['StudentCountTotal'].fillna(np.mean(df['StudentCountTotal'].dropna()), inplace=True)
    df['HouseholdLessonBeforeLunchTurnout'].fillna(np.mean(df['HouseholdLessonBeforeLunchTurnout'].dropna()), inplace=True)
    df['PELessonBeforeLunchTurnout'].fillna(np.mean(df['PELessonBeforeLunchTurnout'].dropna()), inplace=True)
    df['Cloudiness'].fillna(method='ffill', inplace=True)
    df['Temperature'].fillna(method='ffill', inplace=True)

    #MenuClass feature creation from Menu. Includes some manual string parsing.
    df.dropna(subset=['Menu'], inplace=True)
    df['Menu'] = df.apply(lambda row: row['Menu'] if row['Menu'][0]!=' ' else row['Menu'][1:], axis=1)
    df['MenuClass'] = df.apply(lambda row: row['Menu'].split(',', 1)[0].split('/', 1)[0].split(' ', 1)[0], axis=1)
    df.replace(to_replace=['\nTalon', 'Hedelmäinen', 'Makkara-', 'Kala(kasvis)keitto', 'Ohra-riisipuuro', 'Suikalelihakastike', 'Kappalekala', 'Broileripatukka', 'Kiusaus'+chr(160)+'(kebab', 'Kebab-', 'Kalakasviskeitto', 'Kesäkeitto', 'Kalamurekepihvi', 'Riisi-ohrapuuro',
                       'Vaalea', 'Ohrapuuro', 'Lohikeitto', 'Liha-makaronipata', 'Broilerihoukutus', 'Ranskalainen', 'Juustoinen', 'Tomaattinen', 'Lohikiusaus', 'Kalkkunahöystö', 'Kalakepukka', 'Juuresvoipapupyörykkä', 'Broileri-kasviskastike', 'Kana-kookoskastike', 'Jauheliha-juustoperunavuoka', 'Porkkanapyörykkä', 'Naudan', 'Broileripyo'+chr(776)+'rykka'+chr(776), 'Kanapasta', 'Pinaattiohukkaat', 'Italianpata', 'Broileripastavuoka', 'Broilerivuoka'],
           value=['Talon', 'Broilerikastike', 'Makkarakeitto', 'Kalakeitto', 'Puuro', 'Lihakastike', 'Kalaleike', 'Broilerinugetti', 'Kiusaus', 'Kiusaus', 'Kalakeitto', 'Kasviskeitto', 'Kalamureke', 'Puuro',
                  'Kasviskastike', 'Puuro', 'Kalakeitto', 'Makaronipata', 'Broilerikastike', 'Kalaleike', 'Broilerikeitto', 'Jauhelihavuoka', 'Kiusaus', 'Kalkkunakastike', 'Kalapuikot', 'Kasvispyörykät', 'Broilerikastike', 'Broilerikastike', 'Jauhelihavuoka', 'Kasvispyörykät', 'Jauhelihapihvi', 'Broileripyörykkä', 'Broileripasta', 'Pinaattiohukaiset', 'Makaronipata', 'Broileripasta', 'Broileripasta'], inplace=True)
    df = pd.concat([df, pd.get_dummies(df['MenuClass'], prefix='MenuCode')], axis=1)
    menucols = [col for col in df.columns if 'MenuCode' in col]

    #Filter missing values and outliers from train data for MealTotal
    dftrain = df.dropna(subset=['MealTotal'])
    dftrain.dropna(subset=['MealTotalMA'], inplace=True)
    dftrain = dftrain[dftrain['MealTotal'] < 600]
    dftrain = dftrain[dftrain['MealTotal'] > 10]

    #Choose other features besides menu
    othercols = ['LocationSID', 'StudentCountTotal', 'MealTotalMA', 'TET', 'PackedLunchCount', 'PELessonBeforeLunchTurnout', 'Temperature', 'Cloudiness', 'DayOfWeek']
    Xmenu = dftrain[menucols].values
    Xother = dftrain[othercols].values
    X = np.concatenate((Xmenu, Xother), axis=1)
    y = np.array(dftrain['MealTotal'])

    #Create linear regression model for MealTotal
    modelMeal = linear_model.LinearRegression()
    modelMeal.fit(X,y)
    dftrain['PredictedMealTotal'] = modelMeal.predict(X)

    #Filter missing values from train data for WasteTotalKg
    dftrainwaste = df.dropna(subset=['WasteTotalKg'])
    Xmenu = dftrainwaste[menucols].values
    Xother = dftrainwaste[othercols].values
    X = np.concatenate((Xmenu, Xother), axis=1)
    y = np.array(dftrainwaste['WasteTotalKg'])

    #Create linear regression model for WasteTotalKg
    modelWaste = linear_model.LinearRegression()
    modelWaste.fit(X,y)
    dftrainwaste['PredictedWasteTotalKg'] = modelWaste.predict(X)

    #Create linear regression model for the ratio between line waste and total waste
    dftrainwaste['LineWasteKg'] = dftrainwaste['LineWasteKg'].fillna(0)
    dftrainwaste['LineTotalRatio'] = dftrainwaste.apply(lambda row: 0 if row['WasteTotalKg']==0 else row['LineWasteKg']/row['WasteTotalKg'], axis=1)
    Xmenu = dftrainwaste[menucols].values
    Xother = dftrainwaste[othercols].values
    X = np.concatenate((Xmenu, Xother), axis=1)
    y = np.array(dftrainwaste['LineTotalRatio'])
    modelRatio = linear_model.LinearRegression()
    modelRatio.fit(X,y)
    dftrainwaste['PredictedLineTotalRatio'] = modelRatio.predict(X)
    dftrainwaste['PredictedLineWasteKg'] = dftrainwaste.apply(lambda row: row['PredictedWasteTotalKg']*min(row['PredictedLineTotalRatio'],1), axis=1)
    dftrainwaste['PredictedPlateWasteKg'] = dftrainwaste.apply(lambda row: row['PredictedWasteTotalKg']-row['PredictedLineWasteKg'], axis=1)

    #Create test dataframe (8 days to future) and write the predictions
    dftest = df.loc[(df['Date'] >= today) & (df['Date'] < today + pd.DateOffset(days=8))]
    dftest['MealTotalMA'].fillna(method='ffill', inplace=True)
    Xtestmenu = dftest[menucols].values
    Xtestother = dftest[othercols].values
    Xtest = np.concatenate((Xtestmenu, Xtestother), axis=1)
    dftest['PredictedMealTotal'] = modelMeal.predict(Xtest)
    dftest['PredictedWasteTotalKg'] = modelWaste.predict(Xtest)
    dftest['PredictedLineTotalRatio'] = modelRatio.predict(Xtest)
    dftest['PredictedLineWasteKg'] = dftest.apply(lambda row: row['PredictedWasteTotalKg']*min(row['PredictedLineTotalRatio'],1), axis=1)
    dftest['PredictedPlateWasteKg'] = dftest.apply(lambda row: row['PredictedWasteTotalKg']-row['PredictedLineWasteKg'], axis=1)

    #Filter the test dataframe and return the results as json-file
    dftest = dftest[['LocationSID', 'DateSID', 'PredictedMealTotal', 'PredictedWasteTotalKg', 'PredictedLineWasteKg', 'PredictedPlateWasteKg']]
    dftestjs = dftest.to_json(orient='table', index=False)

    return func.HttpResponse(
             dftestjs,
             status_code=200
    )
