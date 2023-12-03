CREATE TABLE [dbo].[Machines]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ShortName] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(50) NOT NULL, 
    [Effectivity] FLOAT NOT NULL, 
    [MachineAlternativityGroup] INT NOT NULL
)
