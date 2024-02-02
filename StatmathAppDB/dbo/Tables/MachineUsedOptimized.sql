CREATE TABLE [dbo].[MachineUsedOptimized]
(
	[MachineId] INT NOT NULL, 
    [OrderId] INT NOT NULL, 
    [TaskId] INT NOT NULL, 
    [Start] INT NOT NULL, 
    [Duration] INT NOT NULL, 
    [Id] INT NOT NULL PRIMARY KEY IDENTITY
)
