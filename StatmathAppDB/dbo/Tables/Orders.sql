﻿CREATE TABLE [dbo].[Orders]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ProductId] INT NOT NULL, 
    [Deadline] DATETIME2 NOT NULL, 
    [EarliestStartDate] DATETIME2 NOT NULL, 
    [OrderDate] DATETIME2 NOT NULL
)
