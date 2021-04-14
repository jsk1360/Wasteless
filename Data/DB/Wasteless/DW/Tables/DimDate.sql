CREATE TABLE [DW].[DimDate] (
    [DateSID]                  INT           NOT NULL,
    [Date]                     SMALLDATETIME NULL,
    [Year]                     INT           NULL,
    [Month]                    INT           NULL,
    [Day]                      INT           NULL,
    [YearMonth]                INT           NULL,
    [MonthName]                CHAR (30)     NULL,
    [MonthNamePre]             CHAR (30)     NULL,
    [MonthNameShort]           CHAR (30)     NULL,
    [WeekNumber1]              INT           NULL,
    [WeekNumber2]              INT           NULL,
    [DayofWeek]                INT           NULL,
    [DayName]                  CHAR (30)     NULL,
    [DayNameShort]             CHAR (10)     NULL,
    [Trimester]                INT           NULL,
    [Quarter]                  INT           NULL,
    [WorkDay]                  INT           NULL,
    [WorkDaySum]               INT           NULL,
    [WorkDayName]              CHAR (30)     NULL,
    [YearTrimester]            INT           NULL,
    [YearQuarter]              INT           NULL,
    [YearWeekNumber1]          INT           NULL,
    [YearWeekNumber2]          INT           NULL,
    [WeekNumber1DayOfWeek]     CHAR (4)      NULL,
    [WeekNumber2DayOfWeek]     CHAR (4)      NULL,
    [YearWeekNumber1DayOfWeek] DECIMAL (10)  NULL,
    [YearWeekNumber2DayOfWeek] DECIMAL (10)  NULL,
    [PrevYearCompareDate]      SMALLDATETIME NULL,
    [PrevYearCompareWeek]      DECIMAL (10)  NULL,
    [HalfOfYear]               INT           NULL,
    [YearHalfOfYear]           INT           NULL,
    CONSTRAINT [PK_DimDate] PRIMARY KEY CLUSTERED ([DateSID] ASC),
    CONSTRAINT [FK_DimDate_DimDate] FOREIGN KEY ([DateSID]) REFERENCES [DW].[DimDate] ([DateSID])
);








GO
EXECUTE sp_addextendedproperty @name = N'Description', @value = N'Semi manually created table which includes time info.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'DimDate';

