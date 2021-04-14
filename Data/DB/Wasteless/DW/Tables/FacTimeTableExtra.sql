CREATE TABLE [DW].[FacTimeTableExtra] (
    [TimetableExtraSID]                 INT      NOT NULL,
    [DateSID]                           INT      NOT NULL,
    [LocationSID]                       INT      NOT NULL,
    [PELessonBeforeLunchTurnout]        INT      NULL,
    [HouseholdLessonBeforeLunchTurnout] INT      NULL,
    [ModifiedTime]                      DATETIME NULL,
    CONSTRAINT [PK_FacTimeTableExtra] PRIMARY KEY CLUSTERED ([TimetableExtraSID] ASC),
    CONSTRAINT [FK_FacTimeTableExtra_DimDate] FOREIGN KEY ([DateSID]) REFERENCES [DW].[DimDate] ([DateSID]),
    CONSTRAINT [FK_FacTimeTableExtra_DimLocation] FOREIGN KEY ([LocationSID]) REFERENCES [DW].[DimLocation] ([LocationSID])
);






GO
EXECUTE sp_addextendedproperty @name = N'Source type', @value = N'Table (DW.FacTimetable)', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacTimeTableExtra';


GO
EXECUTE sp_addextendedproperty @name = N'ETL', @value = N'Timetable.dtsx', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacTimeTableExtra';


GO
EXECUTE sp_addextendedproperty @name = N'Description', @value = N'Table includes sum level information of lessons.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacTimeTableExtra';

