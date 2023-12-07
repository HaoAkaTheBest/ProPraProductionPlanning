CREATE PROCEDURE [dbo].[spRoutings_ReadForOneProduct]
	@ProductId int
AS
begin
	set nocount on;

	select [Id], [ProductId], [StepId], [MachineId], [SetupTimeInSeconds], [ProcessTimeInSeconds]
	from dbo.Routings
	where ProductId = @ProductId;

end
