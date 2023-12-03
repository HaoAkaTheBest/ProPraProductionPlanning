CREATE PROCEDURE [dbo].[spMachines_Create]
	@Id int,
	@ShortName nvarchar(50),
	@Description nvarchar(50),
	@Effectivity float,
	@MachineAlternativityGroup int
AS
begin
	set nocount on;

	insert into dbo.Machines(Id,ShortName,[Description],Effectivity,MachineAlternativityGroup)
	values(@Id,@ShortName,@Description,@Effectivity,@MachineAlternativityGroup);



end