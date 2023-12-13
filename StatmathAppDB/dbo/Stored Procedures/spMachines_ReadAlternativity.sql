CREATE PROCEDURE [dbo].[spMachines_ReadAlternativity]
	@Id int,
	@MachineAlternativityGroup int
AS
begin
	set nocount on;

	select [Id], [ShortName], [Description], [Effectivity], [MachineAlternativityGroup]
	from dbo.Machines
	where MachineAlternativityGroup = @MachineAlternativityGroup and Id <> @Id;

end