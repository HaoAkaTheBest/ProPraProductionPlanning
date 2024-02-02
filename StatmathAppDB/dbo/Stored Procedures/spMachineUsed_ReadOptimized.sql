CREATE PROCEDURE [dbo].[spMachineUsed_ReadOptimized]
	
AS
begin
	set nocount on;

	select[MachineId], [OrderId], [TaskId], [Start], [Duration], [Id]
	from dbo.MachineUsedOptimized;

end