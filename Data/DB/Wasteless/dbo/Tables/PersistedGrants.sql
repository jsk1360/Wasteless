CREATE TABLE [dbo].[PersistedGrants] (
    [Key]          NVARCHAR (200) NOT NULL,
    [Type]         NVARCHAR (50)  NOT NULL,
    [SubjectId]    NVARCHAR (200) NULL,
    [SessionId]    NVARCHAR (100) NULL,
    [ClientId]     NVARCHAR (200) NOT NULL,
    [Description]  NVARCHAR (200) NULL,
    [CreationTime] DATETIME2 (7)  NOT NULL,
    [Expiration]   DATETIME2 (7)  NULL,
    [ConsumedTime] DATETIME2 (7)  NULL,
    [Data]         NVARCHAR (MAX) NOT NULL
);

