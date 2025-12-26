CREATE DATABASE [CAMPUSCOIN];
GO
USE [CAMPUSCOIN];
GO

/* ================= TABLES ================= */

CREATE TABLE [dbo].[ADMIN](
    [AdminId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] [varchar](30) NOT NULL,
    [Surname] [varchar](30) NULL,
    [Position] [varchar](20) NOT NULL,
    [Password] [varchar](255) NULL,
    [Username] [varchar](20) NOT NULL,
    [email] [varchar](50) NULL
);

CREATE TABLE [dbo].[STUDENT](
    [StudentId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] [varchar](30) NOT NULL,
    [Surname] [varchar](30) NULL,
    [email] [varchar](50) NULL,
    [Departmant] [varchar](20) NOT NULL,
    [Password] [varchar](255) NULL,
    [Username] [varchar](20) NOT NULL,
    [Coins] [decimal](18, 0) NULL
);

CREATE TABLE [dbo].[ACTIVITY](
    [ActivityId] [int] NOT NULL PRIMARY KEY, -- Note: Not set as Identity in original
    [Title] [varchar](30) NOT NULL,
    [Description] [varchar](255) NULL,
    [Status] [varchar](30) NULL,
    [RewardTokenAmount] [decimal](18, 0) NOT NULL,
    [MaxParticipants] [int] NOT NULL,
    [CreatedDate] [date] NULL,
    [AdminId] [int] NOT NULL,
    [Deadline] [date] NULL,
    [PriorityLevel] [decimal](18, 0) NULL,
    CONSTRAINT [FK_Activity_Admin] FOREIGN KEY([AdminId]) REFERENCES [dbo].[ADMIN] ([AdminId])
);

CREATE TABLE [dbo].[REWARD](
    [RewardId] [int] NOT NULL PRIMARY KEY,
    [RewardName] [varchar](100) NOT NULL,
    [RewardType] [varchar](50) NOT NULL,
    [Cost] [decimal](18, 0) NOT NULL,
    [CreatedDate] [date] NULL,
    [Vendor] [varchar](100) NULL,
    [UniqueCode] [varchar](50) NOT NULL,
    [Status] [varchar](20) NOT NULL,
    [ExpiryDate] [date] NULL,
    CONSTRAINT [UQ_Reward_UniqueCode] UNIQUE ([UniqueCode])
);

CREATE TABLE [dbo].[WALLET](
    [WalletId] [int] NOT NULL PRIMARY KEY,
    [Balance] [decimal](18, 0) NOT NULL,
    [StudentId] [int] NOT NULL,
    [LastUpdated] [date] NULL,
    [CreatedDate] [date] NULL,
    CONSTRAINT [FK_Wallet_Student] FOREIGN KEY([StudentId]) REFERENCES [dbo].[STUDENT] ([StudentId]),
    CONSTRAINT [UQ_Wallet_StudentId] UNIQUE ([StudentId])
);

CREATE TABLE [dbo].[APPLY](
    [StudentId] [int] NOT NULL,
    [ActivityId] [int] NOT NULL,
    [ApplicationDate] [date] NOT NULL,
    [Status] [varchar](20) NOT NULL,
    CONSTRAINT [PK_Apply] PRIMARY KEY ([StudentId], [ActivityId]),
    CONSTRAINT [FK_Apply_Student] FOREIGN KEY([StudentId]) REFERENCES [dbo].[STUDENT] ([StudentId]),
    CONSTRAINT [FK_Apply_Activity] FOREIGN KEY([ActivityId]) REFERENCES [dbo].[ACTIVITY] ([ActivityId])
);

CREATE TABLE [dbo].[COMPLETES](
    [StudentId] [int] NOT NULL,
    [ActivityId] [int] NOT NULL,
    [CompletionDate] [date] NOT NULL,
    [AwardedAmount] [decimal](18, 0) NOT NULL,
    CONSTRAINT [PK_Completes] PRIMARY KEY ([StudentId], [ActivityId]),
    CONSTRAINT [FK_Completes_Student] FOREIGN KEY([StudentId]) REFERENCES [dbo].[STUDENT] ([StudentId]),
    CONSTRAINT [FK_Completes_Activity] FOREIGN KEY([ActivityId]) REFERENCES [dbo].[ACTIVITY] ([ActivityId])
);

CREATE TABLE [dbo].[LOG](
    [LogId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [StudentId] [int] NOT NULL,
    [ActivityId] [int] NULL,
    [RewardId] [int] NULL,
    [ActionType] [varchar](50) NOT NULL,
    [Timestamp] [datetime2](7) NULL,
    CONSTRAINT [FK_Log_Student] FOREIGN KEY([StudentId]) REFERENCES [dbo].[STUDENT] ([StudentId]),
    CONSTRAINT [FK_Log_Activity] FOREIGN KEY([ActivityId]) REFERENCES [dbo].[ACTIVITY] ([ActivityId]),
    CONSTRAINT [FK_Log_Reward] FOREIGN KEY([RewardId]) REFERENCES [dbo].[REWARD] ([RewardId])
);

CREATE TABLE [dbo].[WALLET_SPENDS_REWARD](
    [TransactionId] [int] NOT NULL PRIMARY KEY,
    [WalletId] [int] NOT NULL,
    [RewardId] [int] NOT NULL,
    [SpentDate] [date] NOT NULL,
    [UsedDate] [date] NOT NULL,
    [Quantity] [int] NOT NULL,
    [StudentId] [int] NULL,
    CONSTRAINT [FK_WSR_Wallet] FOREIGN KEY([WalletId]) REFERENCES [dbo].[WALLET] ([WalletId]),
    CONSTRAINT [FK_WSR_Reward] FOREIGN KEY([RewardId]) REFERENCES [dbo].[REWARD] ([RewardId]),
    CONSTRAINT [FK_WSR_Student] FOREIGN KEY([StudentId]) REFERENCES [dbo].[STUDENT] ([StudentId])
);
GO

/* ================= INDEXES ================= */

CREATE NONCLUSTERED INDEX [idx_student_email] ON [dbo].[STUDENT]([email] ASC);
CREATE NONCLUSTERED INDEX [idx_student_username] ON [dbo].[STUDENT]([Username] ASC);
CREATE NONCLUSTERED INDEX [idx_Log_Timestamp] ON [dbo].[LOG]([Timestamp] DESC);
GO

/* ================= VIEWS ================= */

CREATE VIEW [dbo].[vw_Top3StudentsThisWeek] AS
SELECT
    S.StudentId,
    S.Name + ' ' + S.Surname AS FullName,
    SUM(A.RewardTokenAmount) AS WeeklyCoinsEarned
FROM STUDENT S
INNER JOIN COMPLETES C ON S.StudentId = C.StudentId
INNER JOIN ACTIVITY A ON C.ActivityId = A.ActivityId
WHERE 
    C.CompletionDate >= DATEADD(week, DATEDIFF(week, 0, GETDATE()), 0)
    AND C.CompletionDate < DATEADD(week, DATEDIFF(week, 0, GETDATE()) + 1, 0)
GROUP BY S.StudentId, S.Name, S.Surname;
GO

CREATE VIEW [dbo].[vw_student_completions] AS
SELECT c.StudentId, c.ActivityId, c.CompletionDate, c.AwardedAmount FROM COMPLETES c;
GO

CREATE VIEW [dbo].[vw_activity_details] AS
SELECT 
    a.ActivityId, a.Title, a.Description, a.Status, a.RewardTokenAmount,
    a.MaxParticipants, a.Deadline, ad.Name AS AdminName, ad.Surname AS AdminSurname
FROM ACTIVITY a JOIN ADMIN ad ON a.AdminId = ad.AdminId;
GO

CREATE VIEW [dbo].[vw_student_wallet] AS
SELECT s.StudentId, s.Name, s.Surname, s.Username, w.WalletId, w.Balance, w.LastUpdated
FROM STUDENT s INNER JOIN WALLET w ON s.StudentId = w.StudentId;
GO

/* ================= TRIGGERS ================= */

CREATE TRIGGER [dbo].[logApplyActivity] ON [dbo].[APPLY]
AFTER INSERT AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO LOG (ActionType, StudentId, ActivityId, [Timestamp])
    SELECT 'APPLY', i.StudentId, i.ActivityId, SYSUTCDATETIME() FROM inserted i;
END;
GO

CREATE TRIGGER [dbo].[logCompletesActivity] ON [dbo].[COMPLETES]
AFTER INSERT AS BEGIN 
    SET NOCOUNT ON;
    INSERT INTO LOG (ActionType, StudentId, ActivityId, [Timestamp])
    SELECT 'COMPLETED', i.StudentId, i.ActivityId, SYSUTCDATETIME() FROM inserted i;
END;
GO

CREATE TRIGGER [dbo].[trg_UpdateWalletOnCompletion] ON [dbo].[COMPLETES]
AFTER INSERT AS BEGIN
    UPDATE W
    SET Balance = W.Balance + A.RewardTokenAmount
    FROM WALLET W
    INNER JOIN INSERTED I ON W.StudentId = I.StudentId
    INNER JOIN ACTIVITY A ON I.ActivityId = A.ActivityId
END;
GO

CREATE TRIGGER [dbo].[logSpends] ON [dbo].[WALLET_SPENDS_REWARD]
AFTER INSERT AS BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.LOG (ActionType, StudentId, RewardId, [Timestamp])
    SELECT 'REWARD_SPENT', i.StudentId, i.RewardId, SYSUTCDATETIME() FROM inserted i;
END;
GO