CREATE PROCEDURE [dbo].[spMachineUsed_Create]
	@OrderId int,
	@MachineId int,
	@StartTime datetime2(7),
	@EndTime datetime2(7)
AS
begin
	set nocount on;
	IF NOT EXISTS (SELECT 1 FROM dbo.MachineUsed WHERE OrderId = @OrderId and MachineId = @MachineId)
    BEGIN
        insert into dbo.MachineUsed(OrderId ,MachineId, StartTime, EndTime)
		values(@OrderId,@MachineId,@StartTime,@EndTime);
    END

end
