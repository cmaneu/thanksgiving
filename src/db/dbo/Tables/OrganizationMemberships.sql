CREATE TABLE [dbo].[OrganizationMemberships]
(
	[OrganizationId] INT NOT NULL,
	[UserId] INT NOT NULL,
	[Role] INT NOT NULL DEFAULT 1,
	CONSTRAINT [FK_Organizations] FOREIGN KEY ([OrganizationId]) REFERENCES [Organizations]([Id]),
	CONSTRAINT [FK_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]), 
    CONSTRAINT [PK_OrganizationMemberships] PRIMARY KEY ([OrganizationId],[UserId])
	
)
