CREATE PROCEDURE [dbo].[spMachineUsed_CreateOptimized]
	@MachineId int,
	@OrderId int,
	@TaskId int,
	@Start int,
	@Duration int
AS
begin
	set nocount on;
	IF NOT EXISTS (SELECT 1 FROM dbo.MachineUsedOptimized WHERE OrderId = @OrderId and MachineId = @MachineId)
    BEGIN
        insert into dbo.MachineUsedOptimized(OrderId ,MachineId, TaskId, Start,Duration)
		values(@OrderId,@MachineId,@TaskId,@Start,@Duration);
    END

end