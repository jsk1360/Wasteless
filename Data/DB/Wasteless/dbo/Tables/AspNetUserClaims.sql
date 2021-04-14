CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id]         INT            NOT NULL,
    [UserId]     NVARCHAR (450) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL
);

