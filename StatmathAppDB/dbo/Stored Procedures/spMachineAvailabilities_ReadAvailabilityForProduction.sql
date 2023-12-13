CREATE PROCEDURE [dbo].[spMachineAvailabilities_ReadAvailabilityForProduction]
	@MachineId int,
	@StartDate datetime2(7)
AS
begin
	set nocount on;

	select [Id], [MachineId], [PauseStartDate], [PauseEndDate], [Description]
	from dbo.MachineAvailabilities
	where MachineId = @MachineId 
	and (PauseStartDate <= @StartDate and 
		 @StartDate < PauseEndDate);

end
