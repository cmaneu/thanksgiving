CREATE TABLE [dbo].[Organization]
(
  [Id] INT NOT NULL PRIMARY KEY IDENTITY,
  [Slug] NVARCHAR(50),
  [Name] NVARCHAR(80)
)