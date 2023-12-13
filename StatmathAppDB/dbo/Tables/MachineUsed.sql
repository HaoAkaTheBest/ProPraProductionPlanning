CREATE TABLE [dbo].[MachineUsed]
(
	[Id] INT NOT NULL PRIMARY KEY identity , 
    [OrderId] INT NOT NULL,
    [MachineId] INT NOT NULL, 
    [StartTime] DATETIME2 NOT NULL, 
    [EndTime] DATETIME2 NOT NULL
)
