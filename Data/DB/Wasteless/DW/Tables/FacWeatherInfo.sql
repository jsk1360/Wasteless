CREATE TABLE [DW].[FacWeatherInfo] (
    [DateSID]      INT            NOT NULL,
    [Cloudiness]   FLOAT (53)     NULL,
    [Temperature]  FLOAT (53)     NULL,
    [WindSpeed]    NVARCHAR (255) NULL,
    [LocationSID]  INT            NULL,
    [ModifiedTime] DATETIME       NULL
);




GO
EXECUTE sp_addextendedproperty @name = N'Source type', @value = N'Azure function.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacWeatherInfo';


GO
EXECUTE sp_addextendedproperty @name = N'ETL', @value = N'Data comes from Azure function.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacWeatherInfo';


GO
EXECUTE sp_addextendedproperty @name = N'Description', @value = N'Table includes weather history and forecast for weather.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacWeatherInfo';

