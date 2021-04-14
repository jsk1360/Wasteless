CREATE TABLE [DW].[DimLocation] (
    [LocationSID]  INT           NOT NULL,
    [LocationName] NVARCHAR (50) NOT NULL,
    [City]         NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_DimLocation] PRIMARY KEY CLUSTERED ([LocationSID] ASC)
);






GO
EXECUTE sp_addextendedproperty @name = N'Description', @value = N'Manually created table which includes city + eatery info.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'DimLocation';

