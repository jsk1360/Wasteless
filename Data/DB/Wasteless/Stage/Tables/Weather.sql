CREATE TABLE [Stage].[Weather] (
    [LocationSID] INT             NOT NULL,
    [DateSID]     INT             NOT NULL,
    [Temperature] DECIMAL (18, 2) NULL,
    [WindSpeed]   DECIMAL (18, 2) NULL,
    [Clouds]      INT             NULL
);

