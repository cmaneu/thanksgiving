CREATE TABLE [dbo].[Activities]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[OrganizationId] INT NOT NULL,
	[VolunteerId] INT NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[Date] DATETIME2 NOT NULL,
	[Duration] INT,
	[Status] INT NOT NULL DEFAULT 1,
	[CustomFields] NVARCHAR(MAX),
	CONSTRAINT [FK_Activities_Organization] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([Id]),
	CONSTRAINT [FK_Activities_Volunteer] FOREIGN KEY ([VolunteerId]) REFERENCES [Users]([Id]),
)
