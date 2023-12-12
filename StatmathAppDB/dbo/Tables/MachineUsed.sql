CREATE TABLE [dbo].[MachineUsed]
(
	[Id] INT NOT NULL PRIMARY KEY identity, 
    [MachineId] INT NOT NULL, 
    [StartTime] DATETIME2 NOT NULL, 
    [EndTime] DATETIME2 NOT NULL
)
