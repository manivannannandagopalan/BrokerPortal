CREATE TABLE dbo.Brokers (
    BrokerId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Brokers PRIMARY KEY,
    ExternalCode NVARCHAR(50) NOT NULL CONSTRAINT UQ_Brokers_ExternalCode UNIQUE,
    Name NVARCHAR(200) NOT NULL,
    CreatedAt DATETIMEOFFSET(7) NOT NULL CONSTRAINT DF_Brokers_CreatedAt DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.Users (
    UserId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    BrokerId UNIQUEIDENTIFIER NOT NULL,
    DisplayName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(320) NOT NULL,
    Role NVARCHAR(40) NOT NULL,
    Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Users_Status DEFAULT 'pending',
    LastSignIn DATETIMEOFFSET(7) NULL,
    CreatedAt DATETIMEOFFSET(7) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIMEOFFSET(7) NOT NULL CONSTRAINT DF_Users_UpdatedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Users_Brokers FOREIGN KEY (BrokerId) REFERENCES dbo.Brokers (BrokerId),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('broker_admin', 'operations', 'viewer')),
    CONSTRAINT CK_Users_Status CHECK (Status IN ('active', 'pending', 'suspended'))
);
CREATE UNIQUE INDEX UX_Users_Email_Broker ON dbo.Users (Email, BrokerId);

CREATE TABLE dbo.AuthorizationPolicies (
    PolicyId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_AuthorizationPolicies PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    Version INT NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    PermissionsJson NVARCHAR(MAX) NOT NULL,
    PublishedAt DATETIMEOFFSET(7) NULL,
    CONSTRAINT CK_Policies_Status CHECK (Status IN ('draft', 'published', 'retired'))
);

CREATE TABLE dbo.AuditEvents (
    AuditEventId UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_AuditEvents PRIMARY KEY,
    EventType NVARCHAR(120) NOT NULL,
    Actor NVARCHAR(200) NOT NULL,
    BrokerScope NVARCHAR(200) NULL,
    CorrelationId NVARCHAR(80) NOT NULL,
    Outcome NVARCHAR(20) NOT NULL,
    DetailsJson NVARCHAR(MAX) NULL,
    OccurredAt DATETIMEOFFSET(7) NOT NULL CONSTRAINT DF_AuditEvents_OccurredAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT CK_AuditEvents_Outcome CHECK (Outcome IN ('success', 'denied', 'failure'))
);
CREATE INDEX IX_AuditEvents_CorrelationId ON dbo.AuditEvents (CorrelationId);
CREATE INDEX IX_AuditEvents_OccurredAt ON dbo.AuditEvents (OccurredAt);
