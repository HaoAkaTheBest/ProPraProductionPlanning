CREATE PROCEDURE [dbo].[spOders_UpdateEarliestStartDate]
    @Id int,
    @EarliestStartDate datetime2
AS
begin
    update dbo.Orders
    set EarliestStartDate = @EarliestStartDate
    where Id = @Id
end