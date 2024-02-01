CREATE PROCEDURE [dbo].[spOrders_Update]
	@Id int,
    @ProductId int,
    @Deadline datetime2,
    @EarliestStartDate datetime2,
    @OrderDate datetime2
AS
begin

    set nocount on;
    update dbo.Orders
    set
        ProductId = @ProductId,
        Deadline = @Deadline,
        EarliestStartDate = @EarliestStartDate,
        OrderDate = @OrderDate
    where Id = @Id;
end
