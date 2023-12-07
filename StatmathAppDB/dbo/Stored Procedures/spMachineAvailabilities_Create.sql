CREATE PROCEDURE [dbo].[spMachineAvailabilities_Create]
	@MachineId int,
	@PauseStartDate datetime2(7),
	@PauseEndDate datetime2(7),
	@Description nvarchar(200)
AS
begin
	set nocount on;

	IF NOT EXISTS (SELECT 1 FROM dbo.MachineAvailabilities WHERE MachineId = @MachineId and PauseStartDate = @PauseStartDate and PauseEndDate = @PauseEndDate and [Description] = @Description )
    BEGIN
        insert into dbo.MachineAvailabilities(MachineId,PauseStartDate,PauseEndDate,[Description])
	    values(@MachineId,@PauseStartDate,@PauseEndDate,@Description);
    END
	ELSE
	Begin
		Declare @ErrorMessage Nvarchar(100);
		Set @ErrorMessage = 'Error: Duplicate entry';
		RAISERROR(@ErrorMessage,16,1);
		
	End

end