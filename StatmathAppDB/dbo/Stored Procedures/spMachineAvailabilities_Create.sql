CREATE PROCEDURE [dbo].[spMachineAvailabilities_Create]
	@MachineId int,
	@PauseStartDate datetime2(7),
	@PauseEndDate datetime2(7),
	@Description nvarchar(200)
AS
begin
	set nocount on;

	insert into dbo.MachineAvailabilities(MachineId,PauseStartDate,PauseEndDate,[Description])
	values(@MachineId,@PauseStartDate,@PauseEndDate,@Description);


end