CREATE PROCEDURE [dbo].[spOrders_ReadOne]
	@Id int
AS
begin
	select [Id], [ProductId], [Deadline], [EarliestStartDate], [OrderDate]
	from dbo.Orders
	where Id = @Id;

end