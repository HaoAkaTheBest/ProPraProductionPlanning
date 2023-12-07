CREATE PROCEDURE [dbo].[spRoutings_Read]

AS
begin
	select [Id], [ProductId], [StepId], [MachineId], [SetupTimeInSeconds], [ProcessTimeInSeconds]
	from dbo.Routings;

end
