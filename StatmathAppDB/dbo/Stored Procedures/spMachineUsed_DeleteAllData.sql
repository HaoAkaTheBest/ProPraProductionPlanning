CREATE PROCEDURE [dbo].[spMachineUsed_DeleteAllData]
	
AS
begin
	set nocount on;

	delete from dbo.MachineAvailabilities;
	delete from dbo.Machines;
	delete from dbo.Orders;
	delete from dbo.Routings;
	delete from dbo.MachineUsed;
	delete from dbo.MachineUsedOptimized;

end
