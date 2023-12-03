CREATE PROCEDURE [dbo].[spRoutings_Create]
	@ProductId int,
	@StepId int,
	@MachineId int,
	@SetupTimeInSeconds int,
	@ProcessTimeInSeconds int
AS
begin
	set nocount on;

	insert into dbo.Routings(ProductId,StepId,MachineId,SetupTimeInSeconds,ProcessTimeInSeconds)
	values(@ProductId,@StepId,@MachineId,@SetupTimeInSeconds,@ProcessTimeInSeconds);


end

