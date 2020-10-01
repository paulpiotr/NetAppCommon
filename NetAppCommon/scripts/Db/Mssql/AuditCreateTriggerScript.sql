CREATE TRIGGER [%SchemaName%].[Trigger_%TableName%Audit]
ON [%SchemaName%].[%TableName%]
AFTER UPDATE, INSERT, DELETE
AS
BEGIN
    SET NoCount ON
	DECLARE @AuditAction AS nvarchar(16) = 'Insert';
	IF EXISTS(SELECT * FROM DELETED)
	BEGIN
		SET @AuditAction = CASE WHEN EXISTS(SELECT * FROM INSERTED) THEN 'Update' ELSE 'Delete' END
	END
	IF (@AuditAction = 'Insert' OR @AuditAction = 'Update')
	BEGIN
		INSERT INTO [%SchemaName%].[%TableName%Audit] ([PK] ,[JsonData] ,[UserName] ,[AuditDate] ,[AuditAction] ,[AuditIP])
		(SELECT
			CAST(i.Id AS NVARCHAR(max)),
			CAST(JSON_QUERY((SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)) AS NVARCHAR(max)),
			SUSER_SNAME(),
			GETDATE(),
			@AuditAction,
			(SELECT CONVERT(VARCHAR(48), CONNECTIONPROPERTY('client_net_address')))
		FROM inserted AS i)
	END
	ELSE IF (@AuditAction = 'Delete')
	BEGIN
		INSERT INTO [%SchemaName%].[%TableName%Audit] ([PK] ,[JsonData] ,[UserName] ,[AuditDate] ,[AuditAction] ,[AuditIP])
		(SELECT
			CAST(d.Id AS NVARCHAR(max)),
			CAST(JSON_QUERY((SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)) AS NVARCHAR(max)),
			SUSER_SNAME(),
			GETDATE(),
			@AuditAction,
			(SELECT CONVERT(VARCHAR(48), CONNECTIONPROPERTY('client_net_address')))
		FROM deleted AS d)
	END
END