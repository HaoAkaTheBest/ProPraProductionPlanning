CREATE PROCEDURE [dbo].[spMachineUsed_Delete]
	
AS
begin
	set nocount on;
	delete from dbo.MachineUsed;

end