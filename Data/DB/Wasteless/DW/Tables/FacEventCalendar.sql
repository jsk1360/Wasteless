CREATE TABLE [DW].[FacEventCalendar] (
    [EventSID]          INT            IDENTITY (1, 1) NOT NULL,
    [DateSID]           INT            NOT NULL,
    [LocationSID]       INT            NOT NULL,
    [EventDesc]         NVARCHAR (255) NOT NULL,
    [PackedLunchCount]  INT            NULL,
    [EffectToMealCount] BIT            NULL,
    CONSTRAINT [PK_FacEventCalendar] PRIMARY KEY CLUSTERED ([EventSID] ASC),
    CONSTRAINT [FK_FacEventCalendar_DimDate] FOREIGN KEY ([DateSID]) REFERENCES [DW].[DimDate] ([DateSID]),
    CONSTRAINT [FK_FacEventCalendar_DimLocation] FOREIGN KEY ([LocationSID]) REFERENCES [DW].[DimLocation] ([LocationSID])
);










GO
EXECUTE sp_addextendedproperty @name = N'Source type', @value = N'Excel', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacEventCalendar';


GO
EXECUTE sp_addextendedproperty @name = N'ETL', @value = N'Event.dtsx', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacEventCalendar';


GO
EXECUTE sp_addextendedproperty @name = N'Description', @value = N'This table includes information of school events.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacEventCalendar';

