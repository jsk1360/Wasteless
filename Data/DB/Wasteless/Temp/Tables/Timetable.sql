CREATE TABLE [Temp].[Timetable] (
    [ClassID]      NVARCHAR (255) NULL,
    [Hour]         INT            NULL,
    [Mon]          NVARCHAR (10)  NULL,
    [Tue]          NVARCHAR (10)  NULL,
    [Wed]          NVARCHAR (10)  NULL,
    [Thu]          NVARCHAR (10)  NULL,
    [Fri]          NVARCHAR (10)  NULL,
    [City]         NVARCHAR (MAX) NULL,
    [LocationName] NVARCHAR (MAX) NULL,
    [StartDateSID] NVARCHAR (MAX) NULL,
    [EndDateSID]   NVARCHAR (MAX) NULL
);

