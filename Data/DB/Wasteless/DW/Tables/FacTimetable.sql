CREATE TABLE [DW].[FacTimetable] (
    [TimetableSID]                      INT            NOT NULL,
    [DateSID]                           INT            NOT NULL,
    [StartHour]                         INT            NULL,
    [EndHour]                           INT            NULL,
    [DayLen]                            INT            NULL,
    [PELesson]                          INT            NULL,
    [PELessonBeforeLunch]               INT            NULL,
    [PELessonBeforeLunchTurnout]        INT            NULL,
    [HouseholdLesson]                   INT            NULL,
    [HouseholdLessonBeforeLunch]        INT            NULL,
    [HouseholdLessonBeforeLunchTurnout] INT            NULL,
    [LocationSID]                       INT            NOT NULL,
    [ClassID]                           NVARCHAR (255) NOT NULL,
    [ModifiedTime]                      DATETIME       NULL,
    CONSTRAINT [PK_FacTimetable] PRIMARY KEY CLUSTERED ([TimetableSID] ASC),
    CONSTRAINT [FK_FacTimetable_DimDate] FOREIGN KEY ([DateSID]) REFERENCES [DW].[DimDate] ([DateSID]),
    CONSTRAINT [FK_FacTimetable_DimLocation] FOREIGN KEY ([LocationSID]) REFERENCES [DW].[DimLocation] ([LocationSID])
);






GO
EXECUTE sp_addextendedproperty @name = N'Source type', @value = N'Excel', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacTimetable';


GO
EXECUTE sp_addextendedproperty @name = N'ETL', @value = N'Timetable.dtsx', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacTimetable';


GO
EXECUTE sp_addextendedproperty @name = N'Description', @value = N'Table includes information of lessons', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacTimetable';

