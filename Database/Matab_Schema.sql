-- Matab Stage 4: reconstructed schema based on existing WinForms SQL commands
-- Run using SSMS on your intended SQL Server instance.
IF DB_ID(N'Matab') IS NULL CREATE DATABASE [Matab];
GO
USE [Matab];
GO
IF OBJECT_ID(N'dbo.Bemeh', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.[Bemeh] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Mablagh] NVARCHAR(100) NULL,
    [NameBemeh] NVARCHAR(255) NULL,
    [Tozih] NVARCHAR(500) NULL
 );
END;
GO
IF OBJECT_ID(N'dbo.Hazineh', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.[Hazineh] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Mablagh] NVARCHAR(100) NULL,
    [NameH] NVARCHAR(255) NULL,
    [NameMonshi] NVARCHAR(255) NULL,
    [Tarikh] NVARCHAR(255) NULL,
    [Tozih] NVARCHAR(500) NULL
 );
END;
GO
IF OBJECT_ID(N'dbo.Karbar', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.[Karbar] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Access] NVARCHAR(255) NULL,
    [Password] NVARCHAR(255) NULL,
    [UserName] NVARCHAR(255) NULL
 );
END;
GO
IF OBJECT_ID(N'dbo.Khadamat', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.[Khadamat] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Mablagh] NVARCHAR(100) NULL,
    [NameKhadamat] NVARCHAR(255) NULL,
    [Tozih] NVARCHAR(500) NULL
 );
END;
GO
IF OBJECT_ID(N'dbo.Monshi', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.[Monshi] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Mablagh] NVARCHAR(100) NULL,
    [NameKh] NVARCHAR(255) NULL,
    [NameMonshi] NVARCHAR(255) NULL,
    [Tarikh] NVARCHAR(255) NULL,
    [Tel] NVARCHAR(255) NULL,
    [Tozih] NVARCHAR(500) NULL
 );
END;
GO
IF OBJECT_ID(N'dbo.Nobat', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.[Nobat] (
    [Nobat] INT NOT NULL PRIMARY KEY,
    [Id] NVARCHAR(255) NULL,
    [NameBemar] NVARCHAR(255) NULL,
    [NameKh] NVARCHAR(255) NULL,
    [Tarikh] NVARCHAR(255) NULL,
    [Tel] NVARCHAR(255) NULL,
    [Tozih] NVARCHAR(500) NULL
 );
END;
GO
IF OBJECT_ID(N'dbo.Pardakht', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.[Pardakht] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Mablagh] NVARCHAR(100) NULL,
    [NameKh] NVARCHAR(255) NULL,
    [Tarikh] NVARCHAR(255) NULL,
    [Tozih] NVARCHAR(500) NULL
 );
END;
GO
IF OBJECT_ID(N'dbo.Parvandeh', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.[Parvandeh] (
    [CodeP] INT NOT NULL PRIMARY KEY,
    [BemariF] NVARCHAR(255) NULL,
    [BemariG] NVARCHAR(255) NULL,
    [Jensiyat] NVARCHAR(255) NULL,
    [MablaghBemeh] NVARCHAR(100) NULL,
    [NameBemar] NVARCHAR(255) NULL,
    [NameBemeh] NVARCHAR(255) NULL,
    [NameKh] NVARCHAR(255) NULL,
    [Tarikh] NVARCHAR(255) NULL,
    [Tarikh1] NVARCHAR(255) NULL,
    [Tarikh2] NVARCHAR(255) NULL,
    [TarikhTavalod] NVARCHAR(255) NULL,
    [Tozih] NVARCHAR(500) NULL
 );
END;
GO
IF OBJECT_ID(N'dbo.Setting', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.[Setting] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Address] NVARCHAR(255) NULL,
    [Mobile] NVARCHAR(255) NULL,
    [NameMatab] NVARCHAR(255) NULL,
    [NamePezeshk] NVARCHAR(255) NULL,
    [Tel] NVARCHAR(255) NULL
 );
END;
GO
IF OBJECT_ID(N'dbo.Vizit', N'U') IS NULL
BEGIN
 CREATE TABLE dbo.[Vizit] (
    [IdVizit] INT NOT NULL PRIMARY KEY,
    [Mablagh] NVARCHAR(100) NULL,
    [MablaghBemeh] NVARCHAR(100) NULL,
    [MablaghKhadamat] NVARCHAR(100) NULL,
    [NameBemar] NVARCHAR(255) NULL,
    [NameBemeh] NVARCHAR(255) NULL,
    [NameKh] NVARCHAR(255) NULL,
    [NameKhadamat] NVARCHAR(255) NULL,
    [Tarikh] NVARCHAR(255) NULL,
    [Tozih] NVARCHAR(500) NULL
 );
END;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name=N'UX_Matab_Karbar_UserName' AND object_id=OBJECT_ID(N'dbo.Karbar')) CREATE UNIQUE INDEX UX_Matab_Karbar_UserName ON dbo.Karbar(UserName) WHERE UserName IS NOT NULL;
GO