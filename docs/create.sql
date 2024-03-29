USE [master]
GO
/****** Object:  Database [OMIdatos]    Script Date: 05/10/2015 15:59:04 ******/
CREATE DATABASE [OMIdatos] ON  PRIMARY 
( NAME = N'OMIdatos', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\OMIdatos.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'OMIdatos_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\OMIdatos_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [OMIdatos] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [OMIdatos].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [OMIdatos] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [OMIdatos] SET ANSI_NULLS OFF
GO
ALTER DATABASE [OMIdatos] SET ANSI_PADDING OFF
GO
ALTER DATABASE [OMIdatos] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [OMIdatos] SET ARITHABORT OFF
GO
ALTER DATABASE [OMIdatos] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [OMIdatos] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [OMIdatos] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [OMIdatos] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [OMIdatos] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [OMIdatos] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [OMIdatos] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [OMIdatos] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [OMIdatos] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [OMIdatos] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [OMIdatos] SET  DISABLE_BROKER
GO
ALTER DATABASE [OMIdatos] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [OMIdatos] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [OMIdatos] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [OMIdatos] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [OMIdatos] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [OMIdatos] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [OMIdatos] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [OMIdatos] SET  READ_WRITE
GO
ALTER DATABASE [OMIdatos] SET RECOVERY SIMPLE
GO
ALTER DATABASE [OMIdatos] SET  MULTI_USER
GO
ALTER DATABASE [OMIdatos] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [OMIdatos] SET DB_CHAINING OFF
GO
USE [OMIdatos]
GO
/****** Object:  Table [dbo].[Estado]    Script Date: 05/10/2015 15:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Estado](
	[clave] [char](3) NOT NULL,
	[nombre] [nchar](30) NOT NULL,
	[sitio] [nchar](70) NULL,
	[delegado] [int] NULL,
 CONSTRAINT [PK_Estado] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DelegacionIOI]    Script Date: 05/10/2015 15:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DelegacionIOI](
	[año] [int] NOT NULL,
	[pais] [nchar](20) NOT NULL,
	[ciudad] [nchar](20) NOT NULL,
	[puntos] [int] NOT NULL,
	[lugar] [int] NOT NULL,
	[media] [decimal](10, 2) NOT NULL,
 CONSTRAINT [PK_DelegacionIOI] PRIMARY KEY CLUSTERED 
(
	[año] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Persona]    Script Date: 05/10/2015 15:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Persona](
	[clave] [int] IDENTITY(1000,3) NOT NULL,
	[nombre] [nchar](60) NOT NULL,
	[nacimiento] [char](8) NULL,
	[facebook] [nchar](50) NULL,
	[twitter] [nchar](50) NULL,
	[sitio] [nvarchar](100) NULL,
	[usuario] [nchar](20) NULL,
	[password] [nvarchar](50) NULL,
	[admin] [bit] NULL,
	[nombreHash] [nvarchar](50) NULL,
	[genero] [char](1) NULL,
	[foto] [nvarchar](50) NULL,
	[correo] [nchar](50) NULL,
 CONSTRAINT [PK_Persona] PRIMARY KEY CLUSTERED 
(
	[clave] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Olimpiada]    Script Date: 05/10/2015 15:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Olimpiada](
	[numero] [char](6) NOT NULL,
	[ciudad] [nchar](30) NOT NULL,
	[estado] [char](3) NOT NULL,
	[año] [char](6) NOT NULL,
	[inicio] [char](8) NOT NULL,
	[fin] [char](8) NOT NULL,
	[media] [decimal](10, 2) NOT NULL,
	[mediana] [int] NOT NULL,
 CONSTRAINT [PK_Olimpiada] PRIMARY KEY CLUSTERED 
(
	[numero] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MiembroDelegacionIOI]    Script Date: 05/10/2015 15:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MiembroDelegacionIOI](
	[IOI] [int] NOT NULL,
	[olimpiada] [char](6) NOT NULL,
	[persona] [int] NOT NULL,
	[clave] [nchar](10) NOT NULL,
	[tipo] [nchar](10) NOT NULL,
	[medalla] [int] NOT NULL,
	[estado] [char](3) NOT NULL,
	[IOIid] [nchar](10) NULL,
 CONSTRAINT [PK_MiembroDelegacionIOI] PRIMARY KEY CLUSTERED 
(
	[IOI] ASC,
	[persona] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MiembroDelegacion]    Script Date: 05/10/2015 15:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MiembroDelegacion](
	[olimpiada] [char](6) NOT NULL,
	[estado] [char](3) NOT NULL,
	[clave] [nchar](10) NOT NULL,
	[tipo] [char](4) NOT NULL,
	[persona] [int] NOT NULL,
	[institucion] [nchar](100) NULL,
	[nivel] [nchar](30) NULL,
	[medalla] [int] NOT NULL,
 CONSTRAINT [PK_MiembroDelegacion] PRIMARY KEY CLUSTERED 
(
	[olimpiada] ASC,
	[estado] ASC,
	[persona] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Problema]    Script Date: 05/10/2015 15:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Problema](
	[olimpiada] [char](6) NOT NULL,
	[dia] [int] NOT NULL,
	[numero] [int] NOT NULL,
	[nombre] [nchar](30) NOT NULL,
	[URL] [nchar](50) NULL,
	[media] [decimal](10, 2) NOT NULL,
	[perfectos] [int] NOT NULL,
	[ceros] [int] NOT NULL,
	[mediana] [int] NOT NULL,
 CONSTRAINT [PK_Problema] PRIMARY KEY CLUSTERED 
(
	[olimpiada] ASC,
	[dia] ASC,
	[numero] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ResultadoProblema]    Script Date: 05/10/2015 15:59:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ResultadoProblema](
	[olimpiada] [char](6) NOT NULL,
	[dia] [int] NOT NULL,
	[problema] [int] NOT NULL,
	[concursante] [int] NOT NULL,
	[puntos] [int] NOT NULL,
	[estado] [char](3) NOT NULL,
 CONSTRAINT [PK_ResultadoProblema] PRIMARY KEY CLUSTERED 
(
	[olimpiada] ASC,
	[dia] ASC,
	[problema] ASC,
	[concursante] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  ForeignKey [FK_Olimpiada_Estado]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[Olimpiada]  WITH CHECK ADD  CONSTRAINT [FK_Olimpiada_Estado] FOREIGN KEY([estado])
REFERENCES [dbo].[Estado] ([clave])
GO
ALTER TABLE [dbo].[Olimpiada] CHECK CONSTRAINT [FK_Olimpiada_Estado]
GO
/****** Object:  ForeignKey [FK_MiembroDelegacionIOI_DelegacionIOI]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[MiembroDelegacionIOI]  WITH CHECK ADD  CONSTRAINT [FK_MiembroDelegacionIOI_DelegacionIOI] FOREIGN KEY([IOI])
REFERENCES [dbo].[DelegacionIOI] ([año])
GO
ALTER TABLE [dbo].[MiembroDelegacionIOI] CHECK CONSTRAINT [FK_MiembroDelegacionIOI_DelegacionIOI]
GO
/****** Object:  ForeignKey [FK_MiembroDelegacionIOI_Estado]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[MiembroDelegacionIOI]  WITH CHECK ADD  CONSTRAINT [FK_MiembroDelegacionIOI_Estado] FOREIGN KEY([estado])
REFERENCES [dbo].[Estado] ([clave])
GO
ALTER TABLE [dbo].[MiembroDelegacionIOI] CHECK CONSTRAINT [FK_MiembroDelegacionIOI_Estado]
GO
/****** Object:  ForeignKey [FK_MiembroDelegacionIOI_Olimpiada]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[MiembroDelegacionIOI]  WITH CHECK ADD  CONSTRAINT [FK_MiembroDelegacionIOI_Olimpiada] FOREIGN KEY([olimpiada])
REFERENCES [dbo].[Olimpiada] ([numero])
GO
ALTER TABLE [dbo].[MiembroDelegacionIOI] CHECK CONSTRAINT [FK_MiembroDelegacionIOI_Olimpiada]
GO
/****** Object:  ForeignKey [FK_MiembroDelegacionIOI_Persona]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[MiembroDelegacionIOI]  WITH CHECK ADD  CONSTRAINT [FK_MiembroDelegacionIOI_Persona] FOREIGN KEY([persona])
REFERENCES [dbo].[Persona] ([clave])
GO
ALTER TABLE [dbo].[MiembroDelegacionIOI] CHECK CONSTRAINT [FK_MiembroDelegacionIOI_Persona]
GO
/****** Object:  ForeignKey [FK_MiembroDelegacion_Estado]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[MiembroDelegacion]  WITH CHECK ADD  CONSTRAINT [FK_MiembroDelegacion_Estado] FOREIGN KEY([estado])
REFERENCES [dbo].[Estado] ([clave])
GO
ALTER TABLE [dbo].[MiembroDelegacion] CHECK CONSTRAINT [FK_MiembroDelegacion_Estado]
GO
/****** Object:  ForeignKey [FK_MiembroDelegacion_Estado1]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[MiembroDelegacion]  WITH CHECK ADD  CONSTRAINT [FK_MiembroDelegacion_Estado1] FOREIGN KEY([estado])
REFERENCES [dbo].[Estado] ([clave])
GO
ALTER TABLE [dbo].[MiembroDelegacion] CHECK CONSTRAINT [FK_MiembroDelegacion_Estado1]
GO
/****** Object:  ForeignKey [FK_MiembroDelegacion_Olimpiada]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[MiembroDelegacion]  WITH CHECK ADD  CONSTRAINT [FK_MiembroDelegacion_Olimpiada] FOREIGN KEY([olimpiada])
REFERENCES [dbo].[Olimpiada] ([numero])
GO
ALTER TABLE [dbo].[MiembroDelegacion] CHECK CONSTRAINT [FK_MiembroDelegacion_Olimpiada]
GO
/****** Object:  ForeignKey [FK_MiembroDelegacion_Persona]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[MiembroDelegacion]  WITH CHECK ADD  CONSTRAINT [FK_MiembroDelegacion_Persona] FOREIGN KEY([persona])
REFERENCES [dbo].[Persona] ([clave])
GO
ALTER TABLE [dbo].[MiembroDelegacion] CHECK CONSTRAINT [FK_MiembroDelegacion_Persona]
GO
/****** Object:  ForeignKey [FK_Problema_Olimpiada]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[Problema]  WITH CHECK ADD  CONSTRAINT [FK_Problema_Olimpiada] FOREIGN KEY([olimpiada])
REFERENCES [dbo].[Olimpiada] ([numero])
GO
ALTER TABLE [dbo].[Problema] CHECK CONSTRAINT [FK_Problema_Olimpiada]
GO
/****** Object:  ForeignKey [FK_ResultadoProblema_MiembroDelegacion]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[ResultadoProblema]  WITH CHECK ADD  CONSTRAINT [FK_ResultadoProblema_MiembroDelegacion] FOREIGN KEY([olimpiada], [estado], [concursante])
REFERENCES [dbo].[MiembroDelegacion] ([olimpiada], [estado], [persona])
GO
ALTER TABLE [dbo].[ResultadoProblema] CHECK CONSTRAINT [FK_ResultadoProblema_MiembroDelegacion]
GO
/****** Object:  ForeignKey [FK_ResultadoProblema_Problema]    Script Date: 05/10/2015 15:59:05 ******/
ALTER TABLE [dbo].[ResultadoProblema]  WITH CHECK ADD  CONSTRAINT [FK_ResultadoProblema_Problema] FOREIGN KEY([olimpiada], [dia], [problema])
REFERENCES [dbo].[Problema] ([olimpiada], [dia], [numero])
GO
ALTER TABLE [dbo].[ResultadoProblema] CHECK CONSTRAINT [FK_ResultadoProblema_Problema]
GO
