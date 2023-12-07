CREATE PROCEDURE [dbo].[spMachines_Read]
	
AS
begin
	select [Id], [ShortName], [Description], [Effectivity], [MachineAlternativityGroup]
	from dbo.Machines;
end
