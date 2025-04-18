USE [master]
GO
/****** Object:  Database [DeliveryDatabase]    Script Date: 26/12/2024 13:26:54 ******/
CREATE DATABASE [DeliveryDatabase]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DeliveryDatabase', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MAYAO\MSSQL\DATA\DeliveryDatabase.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DeliveryDatabase_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MAYAO\MSSQL\DATA\DeliveryDatabase_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [DeliveryDatabase] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DeliveryDatabase].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DeliveryDatabase] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET ARITHABORT OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DeliveryDatabase] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DeliveryDatabase] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET  ENABLE_BROKER 
GO
ALTER DATABASE [DeliveryDatabase] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DeliveryDatabase] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET RECOVERY FULL 
GO
ALTER DATABASE [DeliveryDatabase] SET  MULTI_USER 
GO
ALTER DATABASE [DeliveryDatabase] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DeliveryDatabase] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DeliveryDatabase] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DeliveryDatabase] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DeliveryDatabase] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DeliveryDatabase] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'DeliveryDatabase', N'ON'
GO
ALTER DATABASE [DeliveryDatabase] SET QUERY_STORE = ON
GO
ALTER DATABASE [DeliveryDatabase] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [DeliveryDatabase]
GO
/****** Object:  Table [dbo].[Linehaul]    Script Date: 26/12/2024 13:26:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Linehaul](
	[LinehaulID] [char](15) NOT NULL,
	[VehicleNumber] [varchar](10) NOT NULL,
	[NumberOfPackage] [int] NOT NULL,
	[NumberOfOrder] [int] NOT NULL,
	[Seal] [varchar](50) NOT NULL,
	[Operator] [char](10) NOT NULL,
	[Driver] [char](10) NOT NULL,
 CONSTRAINT [PK_Linehaul] PRIMARY KEY CLUSTERED 
(
	[LinehaulID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LINEHAUL_ORDER]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LINEHAUL_ORDER](
	[LinehaulID] [char](15) NOT NULL,
	[OrderID] [char](15) NOT NULL,
	[AddTime] [datetime] NOT NULL,
 CONSTRAINT [PK_LINEHAUL_ORDER] PRIMARY KEY CLUSTERED 
(
	[LinehaulID] ASC,
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Linehaul_Package]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Linehaul_Package](
	[LinehaulID] [char](15) NOT NULL,
	[PackageID] [char](15) NOT NULL,
	[AddTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Linehaul_Package] PRIMARY KEY CLUSTERED 
(
	[LinehaulID] ASC,
	[PackageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[OrderID] [char](15) NOT NULL,
	[SenderName] [nvarchar](50) NOT NULL,
	[SenderAddress] [nvarchar](200) NOT NULL,
	[SenderPhone] [nvarchar](20) NOT NULL,
	[SenderEmail] [nvarchar](50) NULL,
	[ReceiverName] [nvarchar](50) NOT NULL,
	[ReceiverAddress] [nvarchar](200) NOT NULL,
	[ReceiverPhone] [nvarchar](20) NOT NULL,
	[FirstStation] [char](4) NULL,
	[Transit] [char](4) NULL,
	[LastStation] [char](4) NULL,
	[Fee] [float] NOT NULL,
	[Paid] [bit] NOT NULL,
	[TotalWeight] [float] NOT NULL,
	[OrderPrice] [float] NULL,
	[CurrentStationID] [char](4) NOT NULL,
	[Creator] [char](10) NOT NULL,
	[OnDelivering] [bit] NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order_Status]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order_Status](
	[OrderID] [char](15) NOT NULL,
	[StatusID] [char](10) NOT NULL,
	[StationID] [char](4) NOT NULL,
	[Time] [datetime] NOT NULL,
 CONSTRAINT [PK_Order_Status_1] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[StatusID] ASC,
	[StationID] ASC,
	[Time] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderItems]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[OrderID] [char](15) NOT NULL,
	[ItemID] [int] NOT NULL,
	[ItemName] [nvarchar](50) NOT NULL,
	[Quantity] [int] NOT NULL,
	[Weight] [float] NOT NULL,
 CONSTRAINT [PK_OrderItems] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderProblem]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderProblem](
	[OrderID] [char](15) NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[Status] [nvarchar](1000) NOT NULL,
	[IsDisposed] [bit] NOT NULL,
	[Continuable] [bit] NOT NULL,
	[ReportStaff] [char](10) NOT NULL,
	[DisposedStaff] [char](10) NOT NULL,
 CONSTRAINT [PK_OrderProblem] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[UpdateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Package]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Package](
	[PackageID] [char](15) NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[CompleteTime] [datetime] NULL,
	[NumberOfOrder] [int] NOT NULL,
	[StatusID] [char](10) NOT NULL,
	[TotalWeight] [float] NOT NULL,
	[Packer] [char](10) NOT NULL,
	[SendingStation] [char](4) NOT NULL,
	[ReceivingStation] [char](4) NOT NULL,
 CONSTRAINT [PK_Package] PRIMARY KEY CLUSTERED 
(
	[PackageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Package_Order]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Package_Order](
	[PackageID] [char](15) NOT NULL,
	[OrderID] [char](15) NOT NULL,
	[AddTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Package_Order] PRIMARY KEY CLUSTERED 
(
	[PackageID] ASC,
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingRates]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingRates](
	[SendingStation] [char](4) NOT NULL,
	[ReceivingStation] [char](4) NOT NULL,
	[MinPrice] [float] NOT NULL,
	[PricePerKg] [float] NOT NULL,
	[MinPriceForHVO] [float] NULL,
	[PricePerKgForHVO] [float] NULL,
 CONSTRAINT [PK_ShippingRates] PRIMARY KEY CLUSTERED 
(
	[SendingStation] ASC,
	[ReceivingStation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Staff]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Staff](
	[StaffID] [char](10) NOT NULL,
	[Password] [nvarchar](20) NOT NULL,
	[Fullname] [nvarchar](50) NOT NULL,
	[Male] [bit] NOT NULL,
	[Birthday] [date] NOT NULL,
	[StationID] [char](4) NOT NULL,
 CONSTRAINT [PK_Staff] PRIMARY KEY CLUSTERED 
(
	[StaffID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Station]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Station](
	[StationID] [char](4) NOT NULL,
	[StationName] [nvarchar](50) NOT NULL,
	[Address] [nvarchar](200) NOT NULL,
	[IsStation] [bit] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[IsDriver] [bit] NOT NULL,
 CONSTRAINT [PK_Station] PRIMARY KEY CLUSTERED 
(
	[StationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Status]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[StatusID] [char](10) NOT NULL,
	[StatusName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Status] PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vehicle]    Script Date: 26/12/2024 13:26:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vehicle](
	[VehicleNumber] [varchar](10) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Vehicle] PRIMARY KEY CLUSTERED 
(
	[VehicleNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Linehaul]  WITH CHECK ADD  CONSTRAINT [FK_Linehaul_StaffDriving] FOREIGN KEY([Driver])
REFERENCES [dbo].[Staff] ([StaffID])
GO
ALTER TABLE [dbo].[Linehaul] CHECK CONSTRAINT [FK_Linehaul_StaffDriving]
GO
ALTER TABLE [dbo].[Linehaul]  WITH CHECK ADD  CONSTRAINT [FK_Linehaul_StaffOperator] FOREIGN KEY([Operator])
REFERENCES [dbo].[Staff] ([StaffID])
GO
ALTER TABLE [dbo].[Linehaul] CHECK CONSTRAINT [FK_Linehaul_StaffOperator]
GO
ALTER TABLE [dbo].[Linehaul]  WITH CHECK ADD  CONSTRAINT [FK_Linehaul_Vehicle] FOREIGN KEY([VehicleNumber])
REFERENCES [dbo].[Vehicle] ([VehicleNumber])
GO
ALTER TABLE [dbo].[Linehaul] CHECK CONSTRAINT [FK_Linehaul_Vehicle]
GO
ALTER TABLE [dbo].[LINEHAUL_ORDER]  WITH CHECK ADD  CONSTRAINT [FK_LINEHAUL_ORDER_Linehaul] FOREIGN KEY([LinehaulID])
REFERENCES [dbo].[Linehaul] ([LinehaulID])
GO
ALTER TABLE [dbo].[LINEHAUL_ORDER] CHECK CONSTRAINT [FK_LINEHAUL_ORDER_Linehaul]
GO
ALTER TABLE [dbo].[LINEHAUL_ORDER]  WITH CHECK ADD  CONSTRAINT [FK_LINEHAUL_ORDER_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Order] ([OrderID])
GO
ALTER TABLE [dbo].[LINEHAUL_ORDER] CHECK CONSTRAINT [FK_LINEHAUL_ORDER_Order]
GO
ALTER TABLE [dbo].[Linehaul_Package]  WITH CHECK ADD  CONSTRAINT [FK_Linehaul_Package_Linehaul] FOREIGN KEY([LinehaulID])
REFERENCES [dbo].[Linehaul] ([LinehaulID])
GO
ALTER TABLE [dbo].[Linehaul_Package] CHECK CONSTRAINT [FK_Linehaul_Package_Linehaul]
GO
ALTER TABLE [dbo].[Linehaul_Package]  WITH CHECK ADD  CONSTRAINT [FK_Linehaul_Package_Package] FOREIGN KEY([PackageID])
REFERENCES [dbo].[Package] ([PackageID])
GO
ALTER TABLE [dbo].[Linehaul_Package] CHECK CONSTRAINT [FK_Linehaul_Package_Package]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_StaffCreate] FOREIGN KEY([Creator])
REFERENCES [dbo].[Staff] ([StaffID])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_StaffCreate]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_StationCurrent] FOREIGN KEY([CurrentStationID])
REFERENCES [dbo].[Station] ([StationID])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_StationCurrent]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_StationFirst] FOREIGN KEY([FirstStation])
REFERENCES [dbo].[Station] ([StationID])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_StationFirst]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_StationLast] FOREIGN KEY([LastStation])
REFERENCES [dbo].[Station] ([StationID])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_StationLast]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_StationTransit] FOREIGN KEY([Transit])
REFERENCES [dbo].[Station] ([StationID])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_StationTransit]
GO
ALTER TABLE [dbo].[Order_Status]  WITH CHECK ADD  CONSTRAINT [FK_Order_Status_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Order] ([OrderID])
GO
ALTER TABLE [dbo].[Order_Status] CHECK CONSTRAINT [FK_Order_Status_Order]
GO
ALTER TABLE [dbo].[Order_Status]  WITH CHECK ADD  CONSTRAINT [FK_Order_Status_Station] FOREIGN KEY([StationID])
REFERENCES [dbo].[Station] ([StationID])
GO
ALTER TABLE [dbo].[Order_Status] CHECK CONSTRAINT [FK_Order_Status_Station]
GO
ALTER TABLE [dbo].[Order_Status]  WITH CHECK ADD  CONSTRAINT [FK_Order_Status_Status] FOREIGN KEY([StatusID])
REFERENCES [dbo].[Status] ([StatusID])
GO
ALTER TABLE [dbo].[Order_Status] CHECK CONSTRAINT [FK_Order_Status_Status]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD  CONSTRAINT [FK_OrderItems_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Order] ([OrderID])
GO
ALTER TABLE [dbo].[OrderItems] CHECK CONSTRAINT [FK_OrderItems_Order]
GO
ALTER TABLE [dbo].[OrderProblem]  WITH CHECK ADD  CONSTRAINT [FK_OrderProblem_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Order] ([OrderID])
GO
ALTER TABLE [dbo].[OrderProblem] CHECK CONSTRAINT [FK_OrderProblem_Order]
GO
ALTER TABLE [dbo].[OrderProblem]  WITH CHECK ADD  CONSTRAINT [FK_OrderProblem_StaffDisposed] FOREIGN KEY([DisposedStaff])
REFERENCES [dbo].[Staff] ([StaffID])
GO
ALTER TABLE [dbo].[OrderProblem] CHECK CONSTRAINT [FK_OrderProblem_StaffDisposed]
GO
ALTER TABLE [dbo].[OrderProblem]  WITH CHECK ADD  CONSTRAINT [FK_OrderProblem_StaffReport] FOREIGN KEY([ReportStaff])
REFERENCES [dbo].[Staff] ([StaffID])
GO
ALTER TABLE [dbo].[OrderProblem] CHECK CONSTRAINT [FK_OrderProblem_StaffReport]
GO
ALTER TABLE [dbo].[Package]  WITH CHECK ADD  CONSTRAINT [FK_Package_Staff] FOREIGN KEY([Packer])
REFERENCES [dbo].[Staff] ([StaffID])
GO
ALTER TABLE [dbo].[Package] CHECK CONSTRAINT [FK_Package_Staff]
GO
ALTER TABLE [dbo].[Package]  WITH CHECK ADD  CONSTRAINT [FK_Package_StationReceive] FOREIGN KEY([ReceivingStation])
REFERENCES [dbo].[Station] ([StationID])
GO
ALTER TABLE [dbo].[Package] CHECK CONSTRAINT [FK_Package_StationReceive]
GO
ALTER TABLE [dbo].[Package]  WITH CHECK ADD  CONSTRAINT [FK_Package_StationSend] FOREIGN KEY([SendingStation])
REFERENCES [dbo].[Station] ([StationID])
GO
ALTER TABLE [dbo].[Package] CHECK CONSTRAINT [FK_Package_StationSend]
GO
ALTER TABLE [dbo].[Package]  WITH CHECK ADD  CONSTRAINT [FK_Package_Status] FOREIGN KEY([StatusID])
REFERENCES [dbo].[Status] ([StatusID])
GO
ALTER TABLE [dbo].[Package] CHECK CONSTRAINT [FK_Package_Status]
GO
ALTER TABLE [dbo].[Package_Order]  WITH CHECK ADD  CONSTRAINT [FK_Package_Order_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Order] ([OrderID])
GO
ALTER TABLE [dbo].[Package_Order] CHECK CONSTRAINT [FK_Package_Order_Order]
GO
ALTER TABLE [dbo].[Package_Order]  WITH CHECK ADD  CONSTRAINT [FK_Package_Order_Package] FOREIGN KEY([PackageID])
REFERENCES [dbo].[Package] ([PackageID])
GO
ALTER TABLE [dbo].[Package_Order] CHECK CONSTRAINT [FK_Package_Order_Package]
GO
ALTER TABLE [dbo].[ShippingRates]  WITH CHECK ADD  CONSTRAINT [FK_ShippingRates_Station] FOREIGN KEY([ReceivingStation])
REFERENCES [dbo].[Station] ([StationID])
GO
ALTER TABLE [dbo].[ShippingRates] CHECK CONSTRAINT [FK_ShippingRates_Station]
GO
ALTER TABLE [dbo].[ShippingRates]  WITH CHECK ADD  CONSTRAINT [FK_ShippingRates_StationSend] FOREIGN KEY([SendingStation])
REFERENCES [dbo].[Station] ([StationID])
GO
ALTER TABLE [dbo].[ShippingRates] CHECK CONSTRAINT [FK_ShippingRates_StationSend]
GO
ALTER TABLE [dbo].[Staff]  WITH CHECK ADD  CONSTRAINT [FK_Staff_Station] FOREIGN KEY([StationID])
REFERENCES [dbo].[Station] ([StationID])
GO
ALTER TABLE [dbo].[Staff] CHECK CONSTRAINT [FK_Staff_Station]
GO
USE [master]
GO
ALTER DATABASE [DeliveryDatabase] SET  READ_WRITE 
GO
