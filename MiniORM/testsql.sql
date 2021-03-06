USE [Adayroi_TMSV2]
GO
/****** Object:  Table [dbo].[A]    Script Date: 4/14/2016 3:41:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[A](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Val] [nvarchar](50) NOT NULL,
	[DId] [int] NULL,
 CONSTRAINT [PK_A] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[B]    Script Date: 4/14/2016 3:41:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[B](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Val] [nvarchar](50) NOT NULL,
	[AId] [int] NOT NULL,
 CONSTRAINT [PK_B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[C]    Script Date: 4/14/2016 3:41:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[C](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Val] [nvarchar](50) NOT NULL,
	[AId] [int] NOT NULL,
 CONSTRAINT [PK_C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[D]    Script Date: 4/14/2016 3:41:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[D](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Val] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [master].[Stations]    Script Date: 4/14/2016 3:41:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [master].[Stations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Code] [nvarchar](10) NOT NULL,
	[IsCashie] [bit] NOT NULL,
	[UIDChiefStation] [int] NOT NULL,
	[CityID] [int] NOT NULL,
	[DistID] [int] NULL,
	[WardID] [int] NULL,
	[StreetID] [bigint] NULL,
	[HouseN0] [nvarchar](500) NULL,
	[Latitude] [float] NOT NULL,
	[Note] [nvarchar](200) NULL,
	[Longitude] [float] NOT NULL,
	[Visible] [bit] NOT NULL,
	[UIDCreate] [int] NULL,
	[CreateDatetime] [datetime] NOT NULL,
	[FullAddress] [nvarchar](500) NULL,
	[Phone] [nvarchar](20) NULL,
	[SOS] [bit] NOT NULL,
	[LongestDistance] [int] NULL,
	[TimeExecute] [int] NULL,
	[StationTypeId] [int] NOT NULL,
 CONSTRAINT [PK_Stations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [master].[StationType]    Script Date: 4/14/2016 3:41:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [master].[StationType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[Visible] [bit] NOT NULL,
	[UIDCreate] [int] NOT NULL,
	[CreateDatetime] [datetime] NOT NULL,
 CONSTRAINT [PK_StationType_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [master].[StationWard]    Script Date: 4/14/2016 3:41:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [master].[StationWard](
	[StationId] [int] NOT NULL,
	[WardId] [int] NOT NULL,
	[CityId] [int] NULL,
 CONSTRAINT [PK_StationWard] PRIMARY KEY CLUSTERED 
(
	[StationId] ASC,
	[WardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[B]  WITH CHECK ADD  CONSTRAINT [FK_B_A] FOREIGN KEY([AId])
REFERENCES [dbo].[A] ([Id])
GO
ALTER TABLE [dbo].[B] CHECK CONSTRAINT [FK_B_A]
GO
ALTER TABLE [dbo].[C]  WITH CHECK ADD  CONSTRAINT [FK_C_A] FOREIGN KEY([AId])
REFERENCES [dbo].[A] ([Id])
GO
ALTER TABLE [dbo].[C] CHECK CONSTRAINT [FK_C_A]
GO
ALTER TABLE [master].[Stations]  WITH CHECK ADD  CONSTRAINT [FK_Stations_StationType] FOREIGN KEY([StationTypeId])
REFERENCES [master].[StationType] ([Id])
GO
ALTER TABLE [master].[Stations] CHECK CONSTRAINT [FK_Stations_StationType]
GO
ALTER TABLE [master].[StationWard]  WITH CHECK ADD  CONSTRAINT [FK_StationWard_Stations] FOREIGN KEY([StationId])
REFERENCES [master].[Stations] ([Id])
GO
ALTER TABLE [master].[StationWard] CHECK CONSTRAINT [FK_StationWard_Stations]
GO
