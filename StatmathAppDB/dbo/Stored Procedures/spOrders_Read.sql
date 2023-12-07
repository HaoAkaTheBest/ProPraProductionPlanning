CREATE PROCEDURE [dbo].[spOrders_Read]
	
AS
begin
	select [Id], [ProductId], [Deadline], [EarliestStartDate], [OrderDate]
	from dbo.Orders;

end
