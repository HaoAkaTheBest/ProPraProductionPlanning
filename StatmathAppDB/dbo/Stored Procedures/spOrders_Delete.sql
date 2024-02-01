CREATE PROCEDURE [dbo].[spOrders_Delete]
	@Id int
AS
begin
	set nocount on;

	delete from dbo.Orders
	where Id = @Id;
end