CREATE TABLE [DW].[FacMenu] (
    [MenuSID]      INT            IDENTITY (1, 1) NOT NULL,
    [Menu]         NVARCHAR (255) NULL,
    [DateSID]      INT            NOT NULL,
    [LocationSID]  INT            NOT NULL,
    [ModifiedTime] DATETIME       NULL,
    CONSTRAINT [PK_FacMenu] PRIMARY KEY CLUSTERED ([MenuSID] ASC),
    CONSTRAINT [FK_FacMenu_DimDate] FOREIGN KEY ([DateSID]) REFERENCES [DW].[DimDate] ([DateSID]),
    CONSTRAINT [FK_FacMenu_DimLocation] FOREIGN KEY ([LocationSID]) REFERENCES [DW].[DimLocation] ([LocationSID])
);






GO
EXECUTE sp_addextendedproperty @name = N'Source type', @value = N'Excel', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacMenu';


GO
EXECUTE sp_addextendedproperty @name = N'ETL', @value = N'Menu.dtsx', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacMenu';


GO
EXECUTE sp_addextendedproperty @name = N'Description', @value = N'This table includes information of menus.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacMenu';

