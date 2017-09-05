
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/05/2017 10:22:20
-- Generated from EDMX file: C:\Users\DURI3L\Source\Repos\test_app\test_app\Sistema.DataModel\EntityMSSQL.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [C:\USERS\DURI3L\SOURCE\REPOS\TEST_APP\TEST_APP\SISTEMA.DATAMODEL\DATABASEMSSQL.MDF];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'usuarios'
CREATE TABLE [dbo].[usuarios] (
    [id] int IDENTITY(1,1) NOT NULL,
    [nombre_formal] varchar(255)  NOT NULL,
    [nombre] varchar(255)  NOT NULL,
    [apellido] varchar(255)  NOT NULL,
    [contrasena] varchar(max)  NOT NULL,
    [correo] varchar(255)  NOT NULL,
    [area] int  NOT NULL,
    [privilegios] int  NOT NULL,
    [priv_sigi] int  NOT NULL,
    [priv_sui] int  NOT NULL,
    [priv_sia] int  NOT NULL,
    [titular] int  NOT NULL,
    [estado] int  NOT NULL,
    [importado] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'usuarios'
ALTER TABLE [dbo].[usuarios]
ADD CONSTRAINT [PK_usuarios]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------