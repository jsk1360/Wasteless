CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id]         INT            NOT NULL,
    [RoleId]     NVARCHAR (450) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL
);

