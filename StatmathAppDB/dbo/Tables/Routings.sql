CREATE TABLE [dbo].[Routings]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProductId] INT NOT NULL, 
    [StepId] INT NOT NULL, 
    [MachineId] INT NOT NULL, 
    [SetupTimeInSeconds] INT NOT NULL, 
    [ProcessTimeInSeconds] INT NOT NULL 
)
