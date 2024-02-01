CREATE PROCEDURE [dbo].[spOrders_UpdateDeadline]
    @Id int,
    @Deadline datetime2
AS
begin
    update dbo.Orders
    set Deadline = @Deadline
    where Id = @Id
end
