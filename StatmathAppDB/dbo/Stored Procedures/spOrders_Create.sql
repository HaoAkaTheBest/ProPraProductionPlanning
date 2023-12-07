CREATE PROCEDURE [dbo].[spOrders_Create]
	@Id int,
	@ProductId int,
	@Deadline datetime2(7),
	@EarliestStartDate datetime2(7),
	@OrderDate datetime2(7)
AS
begin
	set nocount on;

	insert into dbo.Orders(Id,ProductId,Deadline,EarliestStartDate,OrderDate)
	values(@Id,@ProductId,@Deadline,@EarliestStartDate,@OrderDate);



end