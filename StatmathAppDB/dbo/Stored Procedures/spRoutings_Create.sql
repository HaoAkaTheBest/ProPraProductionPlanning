CREATE PROCEDURE [dbo].[spRoutings_Create]
	@ProductId int,
	@StepId int,
	@MachineId int,
	@SetupTimeInSeconds int,
	@ProcessTimeInSeconds int
AS
begin
	set nocount on;

	IF NOT EXISTS (SELECT 1 FROM dbo.Routings WHERE ProductId = @ProductId and StepId = @StepId and MachineId = @MachineId and SetupTimeInSeconds = @SetupTimeInSeconds and ProcessTimeInSeconds = @ProcessTimeInSeconds )
    BEGIN
        insert into dbo.Routings(ProductId,StepId,MachineId,SetupTimeInSeconds,ProcessTimeInSeconds)
	    values(@ProductId,@StepId,@MachineId,@SetupTimeInSeconds,@ProcessTimeInSeconds);
    END
	ELSE
	Begin
		Declare @ErrorMessage Nvarchar(100);
		Set @ErrorMessage = 'Error: Duplicate entry';
		RAISERROR(@ErrorMessage,16,1);
		
	End


end

