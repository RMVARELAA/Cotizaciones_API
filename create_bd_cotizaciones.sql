CREATE DATABASE [COTIZACIONES]
GO

USE [COTIZACIONES];
GO

/* =========================================================
   0) ELIMINAR OBJETOS SI EXISTEN
   ========================================================= */

-- Eliminar tablas si existen (en orden contrario a las FKs)
IF OBJECT_ID('dbo.Cotizaciones', 'U') IS NOT NULL DROP TABLE dbo.Cotizaciones;
IF OBJECT_ID('dbo.ReglasTasa', 'U') IS NOT NULL DROP TABLE dbo.ReglasTasa;
IF OBJECT_ID('dbo.Clientes',    'U') IS NOT NULL DROP TABLE dbo.Clientes;
IF OBJECT_ID('dbo.Moneda',      'U') IS NOT NULL DROP TABLE dbo.Moneda;
IF OBJECT_ID('dbo.TipoClientes','U') IS NOT NULL DROP TABLE dbo.TipoClientes;
IF OBJECT_ID('dbo.TipoSeguros', 'U') IS NOT NULL DROP TABLE dbo.TipoSeguros;
GO

-- Eliminar secuencia si existe
IF OBJECT_ID('dbo.Seq_Cotizacion', 'SO') IS NOT NULL
    DROP SEQUENCE dbo.Seq_Cotizacion;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/* =========================================================
   1) TABLA: TipoSeguros
   ========================================================= */
CREATE TABLE dbo.TipoSeguros
(
    IdTipoSeguro   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    NombreSeguro   NVARCHAR(70) NOT NULL,
    Codigo         NVARCHAR(20) NULL,
    Descripcion    NVARCHAR(250) NULL,
    FechaCreacion  DATETIME2(3) NOT NULL 
        CONSTRAINT DF_TipoSeguros_FechaCreacion DEFAULT SYSUTCDATETIME(),
    Estado         BIT NOT NULL 
        CONSTRAINT DF_TipoSeguros_Estado DEFAULT (1)
);
GO

/* =========================================================
   2) TABLA: TipoClientes
   ========================================================= */
CREATE TABLE dbo.TipoClientes
(
    IdTipoCliente     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    NombreTipoCliente NVARCHAR(50) NOT NULL,
    Descripcion       NVARCHAR(250) NULL,
    FechaCreacion     DATETIME2(3) NOT NULL 
        CONSTRAINT DF_TipoClientes_FechaCreacion DEFAULT SYSUTCDATETIME(),
    Estado            BIT NOT NULL 
        CONSTRAINT DF_TipoClientes_Estado DEFAULT (1)
);
GO

/* =========================================================
   3) TABLA: Moneda
   ========================================================= */
CREATE TABLE dbo.Moneda
(
    IdMoneda      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CodigoISO     CHAR(3) NOT NULL,
    Nombre        NVARCHAR(100) NULL,
    Simbolo       NVARCHAR(5) NULL,
    FechaCreacion DATETIME2(3) NOT NULL 
        CONSTRAINT DF_Moneda_FechaCreacion DEFAULT SYSUTCDATETIME(),
    Estado        BIT NOT NULL 
        CONSTRAINT DF_Moneda_Estado DEFAULT (1)
);
GO

-- UNIQUE de CodigoISO
ALTER TABLE dbo.Moneda
ADD CONSTRAINT UQ_Moneda_CodigoISO UNIQUE (CodigoISO);
GO

/* =========================================================
   4) TABLA: Clientes  (depende de TipoClientes)
   ========================================================= */
CREATE TABLE dbo.Clientes
(
    IdCliente          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre             NVARCHAR(180) NOT NULL,
    Identidad          NVARCHAR(50) NOT NULL,
    FechaNacimiento    DATE NULL,
    IdTipoCliente      INT NOT NULL,
    Telefono           NVARCHAR(30) NULL,
    Email              NVARCHAR(150) NULL,
    Direccion          NVARCHAR(300) NULL,
    FechaCreacion      DATETIME2(3) NOT NULL 
        CONSTRAINT DF_Clientes_FechaCreacion DEFAULT SYSUTCDATETIME(),
    FechaModificacion  DATETIME2(3) NULL,
    UsuarioCreacion    NVARCHAR(100) NULL,
    UsuarioModificacion NVARCHAR(100) NULL,
    Estado             BIT NOT NULL 
        CONSTRAINT DF_Clientes_Estado DEFAULT (1)
);
GO

-- UNIQUE de Identidad
ALTER TABLE dbo.Clientes
ADD CONSTRAINT UQ_Clientes_Identidad UNIQUE (Identidad);
GO

-- FK hacia TipoClientes
ALTER TABLE dbo.Clientes
ADD CONSTRAINT FK_Clientes_TipoClientes 
FOREIGN KEY (IdTipoCliente) REFERENCES dbo.TipoClientes (IdTipoCliente);
GO

/* =========================================================
   5) TABLA: Cotizaciones (depende de Clientes, Moneda, TipoSeguros)
   ========================================================= */
CREATE TABLE dbo.Cotizaciones
(
    IdCotizacion       BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    NumeroCotizacion   NVARCHAR(50) NOT NULL,
    FechaCotizacion    DATETIME2(3) NOT NULL 
        CONSTRAINT DF_Cotizaciones_FechaCotizacion DEFAULT SYSUTCDATETIME(),
    IdTipoSeguro       INT NOT NULL,
    IdMoneda           INT NOT NULL,
    IdCliente          INT NOT NULL,
    DescripcionBien    NVARCHAR(1000) NULL,
    SumaAsegurada      DECIMAL(18,2) NOT NULL,
    Tasa               DECIMAL(9,6) NOT NULL,
    PrimaNeta          DECIMAL(18,2) NOT NULL,
    Observaciones      NVARCHAR(1000) NULL,
    FechaCreacion      DATETIME2(3) NOT NULL 
        CONSTRAINT DF_Cotizaciones_FechaCreacion DEFAULT SYSUTCDATETIME(),
    FechaModificacion  DATETIME2(3) NULL,
    UsuarioCreacion    NVARCHAR(100) NULL,
    UsuarioModificacion NVARCHAR(100) NULL,
    Estado             BIT NOT NULL 
        CONSTRAINT DF_Cotizaciones_Estado DEFAULT (1),
    CONSTRAINT CK_Cotizaciones_PrimaNeta     CHECK (PrimaNeta     >= 0),
    CONSTRAINT CK_Cotizaciones_SumaAsegurada CHECK (SumaAsegurada >= 0),
    CONSTRAINT CK_Cotizaciones_Tasa          CHECK (Tasa          >= 0)
);
GO

-- UNIQUE de NumeroCotizacion
ALTER TABLE dbo.Cotizaciones
ADD CONSTRAINT UX_Cotizaciones_Numero UNIQUE (NumeroCotizacion);
GO

-- FKs
ALTER TABLE dbo.Cotizaciones
ADD CONSTRAINT FK_Cotizaciones_Cliente 
    FOREIGN KEY (IdCliente) REFERENCES dbo.Clientes (IdCliente);
GO

ALTER TABLE dbo.Cotizaciones
ADD CONSTRAINT FK_Cotizaciones_Moneda 
    FOREIGN KEY (IdMoneda) REFERENCES dbo.Moneda (IdMoneda);
GO

ALTER TABLE dbo.Cotizaciones
ADD CONSTRAINT FK_Cotizaciones_TipoSeguro 
    FOREIGN KEY (IdTipoSeguro) REFERENCES dbo.TipoSeguros (IdTipoSeguro);
GO

/* =========================================================
   6) TABLA: ReglasTasa
   ========================================================= */
CREATE TABLE dbo.ReglasTasa
(
    IdRegla       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdTipoSeguro  INT NOT NULL,
    SumaMin       DECIMAL(18,2) NOT NULL 
        CONSTRAINT DF_ReglasTasa_SumaMin DEFAULT (0),
    SumaMax       DECIMAL(18,2) NULL,
    Tasa          DECIMAL(9,6) NOT NULL,
    FechaCreacion DATETIME2(3) NOT NULL 
        CONSTRAINT DF_ReglasTasa_FechaCreacion DEFAULT SYSUTCDATETIME(),
    Estado        BIT NOT NULL 
        CONSTRAINT DF_ReglasTasa_Estado DEFAULT (1)
);
GO

-- (Opcional) FK hacia TipoSeguros
-- ALTER TABLE dbo.ReglasTasa
-- ADD CONSTRAINT FK_ReglasTasa_TipoSeguros 
--     FOREIGN KEY (IdTipoSeguro) REFERENCES dbo.TipoSeguros (IdTipoSeguro);
-- GO

/* =========================================================
   7) SEQUENCE: Seq_Cotizacion
   ========================================================= */
CREATE SEQUENCE dbo.Seq_Cotizacion
    AS BIGINT
    START WITH 1
    INCREMENT BY 1;
GO
