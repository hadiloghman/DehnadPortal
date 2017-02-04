CREATE TABLE [dbo].[AutochargeMessagesBuffer]
(
[Id] [bigint] NOT NULL IDENTITY(1, 1),
[MobileNumber] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Content] [nvarchar] (max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[ProcessStatus] [int] NOT NULL,
[SentDate] [datetime] NULL,
[PersianSentDate] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[DateAddedToQueue] [datetime] NOT NULL,
[PersianDateAddedToQueue] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[MessageType] [int] NOT NULL,
[AggregatorId] [bigint] NOT NULL,
[ServiceId] [bigint] NOT NULL,
[ReferenceId] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[ContentId] [bigint] NULL,
[MessagePoint] [int] NULL,
[ImiChargeCode] [int] NULL,
[ImiMessageType] [int] NULL,
[SubscriberId] [bigint] NULL,
[ImiChargeKey] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[SubUnSubMoMssage] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
[SubUnSubType] [int] NULL,
[Tag] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AutochargeMessagesBuffer] ADD CONSTRAINT [PK_AutochargeMessagesBuffer] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_AutochargeMessagesBuffer] ON [dbo].[AutochargeMessagesBuffer] ([ContentId], [MobileNumber]) ON [PRIMARY]
GO
