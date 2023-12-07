CREATE PROCEDURE [dbo].[spMachineAvailabilities_Read]

AS
begin
	select [Id], [MachineId], [PauseStartDate], [PauseEndDate], [Description]
	from dbo.MachineAvailabilities;

end