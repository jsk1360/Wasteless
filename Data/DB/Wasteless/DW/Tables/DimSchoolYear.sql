CREATE TABLE [DW].[DimSchoolYear] (
    [SchoolYearSID]     INT           NOT NULL,
    [SchoolYear]        NVARCHAR (50) NULL,
    [AutumnPeriodStart] DATE          NULL,
    [AutumnPeriodEnd]   DATE          NULL,
    [SpringPeriodStart] DATE          NULL,
    [SpringPeriodEnd]   DATE          NULL,
    [LocationSID]       INT           NOT NULL,
    CONSTRAINT [PK_DimSchoolYear] PRIMARY KEY CLUSTERED ([SchoolYearSID] ASC),
    CONSTRAINT [FK_DimSchoolYear_DimLocation] FOREIGN KEY ([LocationSID]) REFERENCES [DW].[DimLocation] ([LocationSID])
);








GO
EXECUTE sp_addextendedproperty @name = N'Description', @value = N'Manually created table which includes school year info (periods).', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'DimSchoolYear';

