USE [EVOBDO_WORLDDB_0001]
GO

/****** Object:  Table [dbo].[TblUser]    Script Date: 11.04.2022 21:28:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TblUser](
	[registerDate] [datetime] NOT NULL,
	[variousDate] [datetime] NOT NULL,
	[userNo] [bigint] NOT NULL,
	[userName] [nvarchar](32) NOT NULL,
	[password] [nvarchar](32) NOT NULL,
	[email] [nvarchar](50) NOT NULL,
	[phone] [nvarchar](12) NOT NULL,
	[ip] [nvarchar](20) NOT NULL,
	[balance] [bigint] NOT NULL,
 CONSTRAINT [PK_TblUser] PRIMARY KEY CLUSTERED 
(
	[userNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


USE [EVOBDO_WORLDDB_0001]
GO

/****** Object:  Table [dbo].[TblPasswordLog]    Script Date: 11.04.2022 23:07:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TblPasswordLog](
	[registerDate] [datetime] NOT NULL,
	[userNo] [bigint] NOT NULL,
	[oldPassword] [nvarchar](32) NOT NULL,
	[newPassword] [nvarchar](32) NOT NULL,
	[reason] [nvarchar](20) NOT NULL,
	[ip] [nvarchar](20) NOT NULL
) ON [PRIMARY]
GO


