CREATE TABLE [auth].[AuthorizationTokens]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [UserId] INT NOT NULL, 
    [ExpiresAt] DATETIME NOT NULL
)
