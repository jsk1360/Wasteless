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
        today = pd.to_datetime('2021-2-12')
    else:
        try:
            today = pd.to_datetime(date)
        except:
            today = pd.to_datetime('2021-2-12')
            pass
    connectionString = os.environ['WastelessDb']
    cnxn = pyodbc.connect(connectionString)

    df = pd.read_sql_query('select x.DateSID, MenuItemSID, ProductionWasteKg, LineWasteKg, PlateWasteKg, WasteTotalKg,MealCount,SpecialMealCount, MealTotal, PELessonBeforeLunchTurnout, HouseholdLessonBeforeLunchTurnout, StudentCountTotal, Cloudiness, Temperature, DayOfWeek, 1 LocationSID from dw.dimDate x left join [DW].[FacWastelessByItemNew] y on x.datesid = y.datesid and y.LocationSID = 1 left join DW.FacTimeTableExtra v on v.DateSID = x.DateSID and v.LocationSID = 1 left join DW.FacStudentCountPerDay w on w.DateSID=x.DateSID and w.LocationSID=1 left join DW.FacWeatherInfo q on q.DateSID=x.DateSID and q.LocationSID=1 where x.Date <= dateadd(day,8,getdate()) and x.Date >= convert(datetime, convert(char(8),20171127)) and x.WorkDay = 1 order by datesid asc',cnxn)    
    dfjs = pd.read_sql_query('select x.DateSID, MenuItemSID, ProductionWasteKg, LineWasteKg, PlateWasteKg, WasteTotalKg,MealCount, SpecialMealCount, MealTotal, PELessonBeforeLunchTurnout, HouseholdLessonBeforeLunchTurnout, StudentCountTotal, Cloudiness, Temperature, DayOfWeek, 2 LocationSID from dw.dimDate x left join [DW].[FacWastelessByItemNew] y on x.datesid = y.datesid and y.LocationSID = 2 left join DW.FacTimeTableExtra v on v.DateSID = x.DateSID and v.LocationSID = 2 left join DW.FacStudentCountPerDay w on w.DateSID=x.DateSID and w.LocationSID=2 left join DW.FacWeatherInfo q on q.DateSID=x.DateSID and q.LocationSID=2 where x.Date <= dateadd(day,8,getdate()) and x.Date >= convert(datetime, convert(char(8),20171127)) and x.WorkDay = 1 order by datesid asc',cnxn)    
    dfevents = pd.read_sql_query('select DateSID, EventDesc, PackedLunchCount, LocationSID from DW.FacEventCalendar', cnxn)
    dfdishes = pd.read_sql_query('select Id, Name, Type from DW.DimMenuItemNew', cnxn)
    
    df['MealTotal'].replace({0:np.nan}, inplace=True)
    df['MealTotal'].fillna(method='ffill', inplace=True)
    df['MealTotal'].fillna(method='bfill', inplace=True)
    dfjs['MealTotal'].replace({0:np.nan}, inplace=True)
    dfjs['MealTotal'].fillna(method='ffill', inplace=True)
    dfjs['MealTotal'].fillna(method='bfill', inplace=True)
    for i in range(1,11):
        df['MealTotalLag'+str(i)] = df['MealTotal'].shift(i)
    df['MealTotalMA'] = df.apply(lambda row: np.mean([row['MealTotalLag1'],row['MealTotalLag2'],row['MealTotalLag3'],row['MealTotalLag4'],row['MealTotalLag5'],row['MealTotalLag6'],row['MealTotalLag7'],row['MealTotalLag8'],row['MealTotalLag9'],row['MealTotalLag10']]), axis=1)
    df['MealTotalMA'].fillna(method='ffill', inplace=True)
    df['MealTotalMA'].fillna(method='bfill', inplace=True)
    for i in range(1,11):
        dfjs['MealTotalLag'+str(i)] = df['MealTotal'].shift(i)
    dfjs['MealTotalMA'] = dfjs.apply(lambda row: np.mean([row['MealTotalLag1'],row['MealTotalLag2'],row['MealTotalLag3'],row['MealTotalLag4'],row['MealTotalLag5'],row['MealTotalLag6'],row['MealTotalLag7'],row['MealTotalLag8'],row['MealTotalLag9'],row['MealTotalLag10']]), axis=1)
    dfjs['MealTotalMA'].fillna(method='ffill', inplace=True)
    dfjs['MealTotalMA'].fillna(method='bfill', inplace=True)

    df = df.append(dfjs)
    df['WasteTotalKg'].replace({0:np.nan}, inplace=True)
    
    df['Date'] = pd.to_datetime(df['DateSID'], format='%Y%m%d')
    dfevents['Date'] = pd.to_datetime(dfevents['DateSID'], format='%Y%m%d')

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
    df.dropna(subset=['MenuItemSID'], inplace=True)
    df['Dish'] = df.apply(lambda row: dfdishes[dfdishes['Id']==row['MenuItemSID']]['Name'].iloc[0], axis=1)

#    df['Menu'] = df.apply(lambda row: row['Menu'] if row['Menu'][0]!=' ' else row['Menu'][1:], axis=1)
#    df['MenuClass'] = df.apply(lambda row: row['Menu'].split(',', 1)[0].split('/', 1)[0].split(' ', 1)[0], axis=1)
#    df.replace(to_replace=['\nTalon', 'Hedelmäinen', 'Makkara-', 'Kala(kasvis)keitto', 'Ohra-riisipuuro', 'Suikalelihakastike', 'Kappalekala', 'Broileripatukka', 'Kiusaus'+chr(160)+'(kebab', 'Kebab-', 'Kalakasviskeitto', 'Kesäkeitto', 'Kalamurekepihvi', 'Riisi-ohrapuuro',
#                       'Vaalea', 'Ohrapuuro', 'Lohikeitto', 'Liha-makaronipata', 'Broilerihoukutus', 'Ranskalainen', 'Juustoinen', 'Tomaattinen', 'Lohikiusaus', 'Kalkkunahöystö', 'Kalakepukka', 'Juuresvoipapupyörykkä', 'Broileri-kasviskastike', 'Kana-kookoskastike', 'Jauheliha-juustoperunavuoka', 'Porkkanapyörykkä', 'Naudan', 'Broileripyo'+chr(776)+'rykka'+chr(776), 'Kanapasta', 'Pinaattiohukkaat', 'Italianpata', 'Broileripastavuoka', 'Broilerivuoka'],
#           value=['Talon', 'Broilerikastike', 'Makkarakeitto', 'Kalakeitto', 'Puuro', 'Lihakastike', 'Kalaleike', 'Broilerinugetti', 'Kiusaus', 'Kiusaus', 'Kalakeitto', 'Kasviskeitto', 'Kalamureke', 'Puuro',
#                  'Kasviskastike', 'Puuro', 'Kalakeitto', 'Makaronipata', 'Broilerikastike', 'Kalaleike', 'Broilerikeitto', 'Jauhelihavuoka', 'Kiusaus', 'Kalkkunakastike', 'Kalapuikot', 'Kasvispyörykät', 'Broilerikastike', 'Broilerikastike', 'Jauhelihavuoka', 'Kasvispyörykät', 'Jauhelihapihvi', 'Broileripyörykkä', 'Broileripasta', 'Pinaattiohukaiset', 'Makaronipata', 'Broileripasta', 'Broileripasta'], inplace=True)

    df = pd.concat([df, pd.get_dummies(df['MenuItemSID'], prefix='MenuCode')], axis=1)
    menucols = [col for col in df.columns if 'MenuCode' in col]
    
    dftrain = df.dropna(subset=['MealTotal']) #
    dftrain.dropna(subset=['MealTotalMA'], inplace=True) #
    dftrain = dftrain[dftrain['MealTotal'] < 600] #
    dftrain = dftrain[dftrain['MealTotal'] > 10] #
    
    othercols = ['LocationSID', 'StudentCountTotal', 'MealTotalMA', 'TET', 'PackedLunchCount', 'PELessonBeforeLunchTurnout', 'HouseholdLessonBeforeLunchTurnout', 'Temperature', 'Cloudiness', 'DayOfWeek']

    Xmenu = dftrain[menucols].values #
    Xother = dftrain[othercols].values #
    X = np.concatenate((Xmenu, Xother), axis=1) #
    y = np.array(dftrain['MealTotal']) #

    modelMeal = linear_model.LinearRegression() #
    modelMeal.fit(X,y) #
    dftrain['PredictedMealTotal'] = modelMeal.predict(X) #

    dftrainproductionwaste = df.dropna(subset=['ProductionWasteKg'])
    Xmenu = dftrainproductionwaste[menucols].values
    Xother = dftrainproductionwaste[othercols].values
    X = np.concatenate((Xmenu, Xother), axis=1)
    y = np.array(dftrainproductionwaste['ProductionWasteKg'])
    modelProductionWaste = linear_model.Ridge()
    modelProductionWaste.fit(X,y)
    dftrainproductionwaste['PredictedProductionWasteKg'] = modelProductionWaste.predict(X)

    dftrainlinewaste = df.dropna(subset=['LineWasteKg'])
    Xmenu = dftrainlinewaste[menucols].values
    Xother = dftrainlinewaste[othercols].values
    X = np.concatenate((Xmenu, Xother), axis=1)
    y = np.array(dftrainlinewaste['LineWasteKg'])
    modelLineWaste = linear_model.Ridge()
    modelLineWaste.fit(X,y)
    dftrainlinewaste['PredictedLineWasteKg'] = modelLineWaste.predict(X)

    dftrainplatewaste = df.dropna(subset=['PlateWasteKg'])
    Xmenu = dftrainplatewaste[menucols].values
    Xother = dftrainplatewaste[othercols].values
    X = np.concatenate((Xmenu, Xother), axis=1)
    y = np.array(dftrainplatewaste['PlateWasteKg'])
    modelPlateWaste = linear_model.Ridge()
    modelPlateWaste.fit(X,y)
    dftrainplatewaste['PredictedPlateWasteKg'] = modelPlateWaste.predict(X)

    dftest = df.loc[(df['Date'] >= today) & (df['Date'] < today + pd.DateOffset(days=15))]
    dftest['MealTotalMA'].fillna(method='ffill', inplace=True)
    Xtestmenu = dftest[menucols].values
    Xtestother = dftest[othercols].values
    Xtest = np.concatenate((Xtestmenu, Xtestother), axis=1)
    dftest['PredictedMealTotal'] = modelMeal.predict(Xtest)
    dftest['PredictedProductionWasteKg'] = modelProductionWaste.predict(Xtest)
    dftest['PredictedLineWasteKg'] = modelLineWaste.predict(Xtest)
    dftest['PredictedPlateWasteKg'] = modelPlateWaste.predict(Xtest)
    dftest['PredictedProductionWasteKg'][dftest['PredictedProductionWasteKg']<0] = 0
    dftest['PredictedLineWasteKg'][dftest['PredictedLineWasteKg']<0] = 0
    dftest['PredictedPlateWasteKg'][dftest['PredictedPlateWasteKg']<0] = 0
    dftest['PredictedWasteTotalKg'] = dftest.apply(lambda row: row['PredictedProductionWasteKg']+row['PredictedLineWasteKg']+row['PredictedPlateWasteKg'], axis=1)

    dftest = dftest[['LocationSID', 'DateSID', 'PredictedMealTotal', 'MenuItemSID', 'Dish', 'PredictedWasteTotalKg', 'PredictedProductionWasteKg', 'PredictedLineWasteKg', 'PredictedPlateWasteKg']]
    dftestjs = dftest.to_json(orient='table', index=False)

    return func.HttpResponse(
             dftestjs,
             status_code=200
    )