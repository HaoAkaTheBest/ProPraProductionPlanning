CREATE TABLE [dbo].[Tasks]
(
	[OrderId] INT NOT NULL PRIMARY KEY, 
    [MachineId] INT NOT NULL, 
    [TaskId] INT NOT NULL, 
    [Start] INT NOT NULL, 
    [Duration] INT NOT NULL
)
