CREATE PROCEDURE [dbo].[spMachineUsed_CheckIfBeingUsed]
	@MachineId int,
	@StartTime datetime2(7)
AS
begin
	set nocount on;

	select [OrderId], [MachineId], [StartTime], [EndTime]
	from dbo.MachineUsed
	where MachineId = @MachineId
		and (@StartTime < EndTime and
			 @StartTime >= StartTime);


end

