CREATE PROCEDURE [dbo].[spMachines_ReadOneMachine]
	@Id int
AS
begin
	set nocount on;

	select [Id], [ShortName], [Description], [Effectivity], [MachineAlternativityGroup]
	from dbo.Machines
	where Id = @Id;

end
