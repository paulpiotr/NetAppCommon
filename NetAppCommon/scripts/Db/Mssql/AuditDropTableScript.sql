IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[%SchemaName%].[%TableName%Audit]') AND type in (N'U'))
DROP TABLE [%SchemaName%].[%TableName%Audit]