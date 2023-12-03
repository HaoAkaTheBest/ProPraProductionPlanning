CREATE TABLE [dbo].[MachineAvailabilities]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [MachineId] INT NOT NULL, 
    [PauseStartDate] DATETIME2 NOT NULL, 
    [PauseEndDate] DATETIME2 NOT NULL, 
    [Description] NVARCHAR(200) NOT NULL 
)
