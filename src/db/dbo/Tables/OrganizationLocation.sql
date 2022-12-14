CREATE TABLE [dbo].[OrganizationLocation]
(
	[Id] INT NOT NULL IDENTITY (1, 1)  PRIMARY KEY,
	[OrganizationId] INT NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[Address] NVARCHAR(200) NOT NULL,
    CONSTRAINT [FK_OrganizationLocaation_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([Id])
)
