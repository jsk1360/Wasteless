
CREATE view [DW].[Wasteless] as 
SELECT  [WasteSID]
      ,a.[DateSID]
      ,[ProductionWasteKg]
      ,[LineWasteKg]
      ,[PlateWasteKg]
      ,[WasteTotalKg]
      ,a.[LocationSID]
      ,[MealCount]
      ,[SpecialMealCount]
      ,[MealTotal]
      ,[Forecast_MealTotal]
      ,[Forecast_SpecialMealCount]
      ,[Forecast_MealCount]
      ,[Forecast_ProductionWasteKg]
      ,[Forecast_LineWasteKg]
      ,[Forecast_PlateWasteKg]
      ,[Forecast_WasteTotalKg]
       , b.Menu
  FROM [DW].[FacWasteless] a
  left join DW.FacMenu b on a.LocationSID = b.LocationSID and a.DateSID = b.DateSID
where (coalesce(a.MealTotal,0) > 0) and a.locationsid = 1