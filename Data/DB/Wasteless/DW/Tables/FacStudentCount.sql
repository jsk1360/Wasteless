CREATE TABLE [DW].[FacStudentCount] (
    [StudentCountSID] INT           NOT NULL,
    [SchoolYearSID]   INT           NOT NULL,
    [ClassID]         NVARCHAR (50) NOT NULL,
    [Girls]           INT           NULL,
    [Boys]            INT           NULL,
    [Grade]           NVARCHAR (1)  NULL,
    [ModifiedTime]    DATETIME      NULL,
    CONSTRAINT [PK_FacStudentCount] PRIMARY KEY CLUSTERED ([StudentCountSID] ASC),
    CONSTRAINT [FK_FacStudentCount_DimSchoolYear] FOREIGN KEY ([SchoolYearSID]) REFERENCES [DW].[DimSchoolYear] ([SchoolYearSID])
);






GO
EXECUTE sp_addextendedproperty @name = N'Source type', @value = N'Excel', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacStudentCount';


GO
EXECUTE sp_addextendedproperty @name = N'ETL', @value = N'StudentCount.dtsx', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacStudentCount';


GO
EXECUTE sp_addextendedproperty @name = N'Description', @value = N'Table includes count of students.', @level0type = N'SCHEMA', @level0name = N'DW', @level1type = N'TABLE', @level1name = N'FacStudentCount';

