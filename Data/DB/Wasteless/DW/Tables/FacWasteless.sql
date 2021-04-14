CREATE TABLE [DW].[FacWasteless] (
    [WasteSID]                   INT             NOT NULL,
    [DateSID]                    INT             NOT NULL,
    [ProductionWasteKg]          NUMERIC (18, 2) NULL,
    [LineWasteKg]                NUMERIC (18, 2) NULL,
    [PlateWasteKg]               NUMERIC (18, 2) NULL,
    [LocationSID]                INT             NOT NULL,
    [MealCount]                  INT             NULL,
    [SpecialMealCount]           INT             NULL,
    [MealTotal]                  INT             NULL,
    [Forecast_MealTotal]         INT             NULL,
    [Forecast_SpecialMealCount]  INT             NULL,
    [Forecast_MealCount]         INT             NULL,
    [Forecast_ProductionWasteKg] NUMERIC (18, 2) NULL,
    [Forecast_LineWasteKg]       NUMERIC (18, 2) NULL,
    [Forecast_PlateWasteKg]      NUMERIC (18, 2) NULL,
    [ModifiedTime]               DATETIME        NOT NULL,
    [MealCountReserved]          INT             NULL,
    [WasteTotalKg]               NUMERIC (20, 2) NULL,
    [Forecast_WasteTotalKg]      NUMERIC (20, 2) NULL,
    CONSTRAINT [PK_FacWasteless] PRIMARY KEY CLUSTERED ([WasteSID] ASC),
    CONSTRAINT [FK_FacWasteless_DimDate] FOREIGN KEY ([DateSID]) REFERENCES [DW].[DimDate] ([DateSID]),
    CONSTRAINT [FK_FacWasteless_DimLocation] FOREIGN KEY ([LocationSID]) REFERENCES [DW].[DimLocation] ([LocationSID])
);








GO
EXECUTE sp_addextendedproperty @name = N'Source type', @value = N'Azure function and UI.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacWasteless';


GO
EXECUTE sp_addextendedproperty @name = N'ETL', @value = N'There is no ETL process for this table. Forecast comes from Azure function and results from UI.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacWasteless';


GO
EXECUTE sp_addextendedproperty @name = N'Description', @value = N'The main table of project. Includes information of forecast and results.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacWasteless';

