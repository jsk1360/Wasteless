CREATE TABLE [dbo].[DeviceCodes] (
    [UserCode]     NVARCHAR (200) NOT NULL,
    [DeviceCode]   NVARCHAR (200) NOT NULL,
    [SubjectId]    NVARCHAR (200) NULL,
    [SessionId]    NVARCHAR (100) NULL,
    [ClientId]     NVARCHAR (200) NOT NULL,
    [Description]  NVARCHAR (200) NULL,
    [CreationTime] DATETIME2 (7)  NOT NULL,
    [Expiration]   DATETIME2 (7)  NOT NULL,
    [Data]         NVARCHAR (MAX) NOT NULL
);

