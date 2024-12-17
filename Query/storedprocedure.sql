CREATE PROCEDURE dbo.UpdateCreatedDate
    @TemplateID INT
AS
BEGIN
    -- Update the CreatedDate to the current time for the specified TemplateID
    UPDATE Templates
    SET CreatedDate = GETDATE()
    WHERE TemplateID = @TemplateID;
END;
GO

CREATE PROCEDURE dbo.UpdateUpdatedDate
    @TemplateID INT
AS
BEGIN
    -- Update the UpdatedDate to the current time for the specified TemplateID
    UPDATE Templates
    SET UpdatedDate = GETDATE()
    WHERE TemplateID = @TemplateID;
END;
GO
