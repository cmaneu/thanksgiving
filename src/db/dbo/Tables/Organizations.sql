﻿CREATE TABLE [dbo].[Organizations]
(
  [Id] INT NOT NULL PRIMARY KEY IDENTITY,
  [Slug] NVARCHAR(50) NOT NULL,
  [Name] NVARCHAR(80) NOT NULL, 
  [Visibility] INT NULL DEFAULT 1
)