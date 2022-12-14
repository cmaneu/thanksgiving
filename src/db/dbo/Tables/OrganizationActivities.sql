CREATE TABLE [dbo].[OrganizationActivities]
(
	[Id] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[OrganizationId] INT NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[CustomFields] NVARCHAR(MAX), 
    CONSTRAINT [FK_OrganizationActivities_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([Id])
)
