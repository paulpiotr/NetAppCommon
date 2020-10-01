CREATE TABLE [%SchemaName%].[%TableName%Audit] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [PK]          NVARCHAR (MAX) NULL,
    [JsonData]    NVARCHAR (MAX) NOT NULL,
    [UserName]    NVARCHAR (256) NOT NULL,
    [AuditDate]   DATETIME       NOT NULL,
    [AuditAction] NVARCHAR (16)  NOT NULL,
    [AuditIP]     NVARCHAR (32)  NOT NULL,
    CONSTRAINT [PK_dbo.%TableName%Audit] PRIMARY KEY CLUSTERED ([Id] ASC)
);