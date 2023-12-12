CREATE PROCEDURE [dbo].[spMachineUsed_Create]
	@MachineId int,
	@StartTime datetime2(7),
	@EndTime datetime2(7)
AS
begin
	set nocount on;

	insert into dbo.MachineUsed(MachineId, StartTime, EndTime)
	values(@MachineId,@StartTime,@EndTime);


end
