
create view [DW].[Location] as 
SELECT [LocationName]
      ,[City]
      ,[LocationSID]
  FROM [DW].[DimLocation]