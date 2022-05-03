
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/05/2022 15:29:39
-- Generated from EDMX file: E:\LSR\LSR\LSR\Models\LSR.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [LSR];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[User_InfoSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_InfoSet];
GO
IF OBJECT_ID(N'[dbo].[MusicSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MusicSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'User_InfoSet'
CREATE TABLE [dbo].[User_InfoSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(max)  NOT NULL,
    [PassWord] nvarchar(max)  NOT NULL,
    [Gender] nvarchar(max)  NOT NULL,
    [UserEmail] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [Birthday] nvarchar(max)  NOT NULL,
    [UploadMusic] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'MusicSet'
CREATE TABLE [dbo].[MusicSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MusicName] nvarchar(max)  NOT NULL,
    [Author] nvarchar(max)  NOT NULL,
    [AlbumName] nvarchar(max)  NOT NULL,
    [MusicDuration] nvarchar(max)  NOT NULL,
    [LyricsPath] nvarchar(max)  NOT NULL,
    [UploadDate] nvarchar(max)  NOT NULL,
    [UploaderName] nvarchar(max)  NOT NULL,
    [FileSize] nvarchar(max)  NOT NULL,
    [MusicFilePath] nvarchar(max)  NOT NULL,
    [MusicFormat] nvarchar(max)  NOT NULL,
    [ImagePath] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'User_InfoSet'
ALTER TABLE [dbo].[User_InfoSet]
ADD CONSTRAINT [PK_User_InfoSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MusicSet'
ALTER TABLE [dbo].[MusicSet]
ADD CONSTRAINT [PK_MusicSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------