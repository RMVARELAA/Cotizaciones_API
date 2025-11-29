-- =========================================================
-- Script completo: Crear base de datos y tablas para Cotizaciones
-- Ejecutar en SQL Server (SSMS)
-- =========================================================

/* 1) Crear base de datos */
IF DB_ID('COTIZACIONES') IS NULL
BEGIN
    CREATE DATABASE COTIZACIONES;
END;
GO

USE COTIZACIONES;
GO

SET NOCOUNT ON;
BEGIN TRANSACTION;
SET XACT_ABORT ON;

-- Eliminar tablas si existen (en orden contrario a las FKs)
IF OBJECT_ID('dbo.Cotizaciones', 'U') IS NOT NULL DROP TABLE dbo.Cotizaciones;
IF OBJECT_ID('dbo.ReglasTasa', 'U') IS NOT NULL DROP TABLE dbo.ReglasTasa;
IF OBJECT_ID('dbo.Clientes', 'U') IS NOT NULL DROP TABLE dbo.Clientes;
IF OBJECT_ID('dbo.Moneda', 'U') IS NOT NULL DROP TABLE dbo.Moneda;
IF OBJECT_ID('dbo.TipoClientes', 'U') IS NOT NULL DROP TABLE dbo.TipoClientes;
IF OBJECT_ID('dbo.TipoSeguros', 'U') IS NOT NULL DROP TABLE dbo.TipoSeguros;
GO
	
IF OBJECT_ID('dbo.Seq_Cotizacion', 'SO') IS NOT NULL
    DROP SEQUENCE dbo.Seq_Cotizacion;
GO

CREATE SEQUENCE dbo.Seq_Cotizacion
    AS BIGINT
    START WITH 1
    INCREMENT BY 1;
GO
-- ===========================================
-- Tabla TiposSeguros (catálogo)
-- ===========================================
CREATE TABLE dbo.TipoSeguros (
    IdTipoSeguro INT IDENTITY(1,1) PRIMARY KEY,
    NombreSeguro NVARCHAR(70) NOT NULL,
    Codigo NVARCHAR(20) NULL,
    Descripcion NVARCHAR(250) NULL,
    FechaCreacion DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    Estado BIT NOT NULL DEFAULT(1)
);
GO

INSERT INTO dbo.TipoSeguros (NombreSeguro, Codigo, Descripcion)
VALUES
 ('Médico', 'MED', 'Seguro médico / salud'),
 ('Automóvil', 'AUTO', 'Seguro de vehículo automotor'),
 ('Incendio', 'INC', 'Seguro contra incendios'),
 ('Fianzas', 'FIA', 'Fianzas y cauciones');
GO

-- ===========================================
-- Tabla TipoClientes (catálogo)
-- ===========================================
CREATE TABLE dbo.TipoClientes (
    IdTipoCliente INT IDENTITY(1,1) PRIMARY KEY,
    NombreTipoCliente NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    FechaCreacion DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    Estado BIT NOT NULL DEFAULT(1)
);
GO

INSERT INTO dbo.TipoClientes (NombreTipoCliente, Descripcion)
VALUES
 ('Natural', 'Persona natural - individuo'),
 ('Jurídico', 'Persona jurídica - empresa o entidad');
GO

-- ===========================================
-- Tabla Moneda 
-- ===========================================
CREATE TABLE dbo.Moneda (
    IdMoneda INT IDENTITY(1,1) PRIMARY KEY,
    CodigoISO CHAR(3) NOT NULL UNIQUE, 
    Nombre NVARCHAR(100) NULL,
    Simbolo NVARCHAR(5) NULL,
    FechaCreacion DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    Estado BIT NOT NULL DEFAULT(1)
);
GO

INSERT INTO dbo.Moneda (CodigoISO, Nombre, Simbolo)
VALUES
 ('HNL', 'Lempira', 'L'),
 ('USD', 'Dólar estadounidense', '$'),
 ('EUR', 'Euro', '€');
GO

-- ===========================================
-- Tabla Clientes (con FK a TipoClientes)
-- ===========================================
CREATE TABLE dbo.Clientes (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(180) NOT NULL,
    Identidad NVARCHAR(50) NOT NULL UNIQUE,
    FechaNacimiento DATE NULL,                 
    IdTipoCliente INT NOT NULL,                
    Telefono NVARCHAR(30) NULL,
    Email NVARCHAR(150) NULL,
    Direccion NVARCHAR(300) NULL,
    FechaCreacion DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    FechaModificacion DATETIME2(3) NULL,
    UsuarioCreacion NVARCHAR(100) NULL,
    UsuarioModificacion NVARCHAR(100) NULL,
    Estado BIT NOT NULL DEFAULT(1),

    CONSTRAINT FK_Clientes_TipoClientes FOREIGN KEY (IdTipoCliente)
        REFERENCES dbo.TipoClientes (IdTipoCliente)
);
GO

CREATE INDEX IX_Clientes_Nombre ON dbo.Clientes (Nombre);
CREATE INDEX IX_Clientes_Estado ON dbo.Clientes (Estado);
GO

-- ===========================================
-- Tabla Cotizaciones (con FKs integradas)
-- ===========================================
CREATE TABLE dbo.Cotizaciones (
    IdCotizacion BIGINT IDENTITY(1,1) PRIMARY KEY,
    NumeroCotizacion NVARCHAR(50) NOT NULL,
    FechaCotizacion DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    IdTipoSeguro INT NOT NULL,                 
    IdMoneda INT NOT NULL,                     
    IdCliente INT NOT NULL,                    
    DescripcionBien NVARCHAR(1000) NULL,
    SumaAsegurada DECIMAL(18,2) NOT NULL CHECK (SumaAsegurada >= 0),
    Tasa DECIMAL(9,6) NOT NULL CHECK (Tasa >= 0),
    PrimaNeta DECIMAL(18,2) NOT NULL CHECK (PrimaNeta >= 0),
    Observaciones NVARCHAR(1000) NULL,
    FechaCreacion DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(),
    FechaModificacion DATETIME2(3) NULL,
    UsuarioCreacion NVARCHAR(100) NULL,
    UsuarioModificacion NVARCHAR(100) NULL,
    Estado BIT NOT NULL DEFAULT(1),

    CONSTRAINT UX_Cotizaciones_Numero UNIQUE (NumeroCotizacion),

    CONSTRAINT FK_Cotizaciones_TipoSeguro FOREIGN KEY (IdTipoSeguro)
        REFERENCES dbo.TipoSeguros (IdTipoSeguro),

    CONSTRAINT FK_Cotizaciones_Moneda FOREIGN KEY (IdMoneda)
        REFERENCES dbo.Moneda (IdMoneda),

    CONSTRAINT FK_Cotizaciones_Cliente FOREIGN KEY (IdCliente)
        REFERENCES dbo.Clientes (IdCliente)
);
GO

-- Índices recomendados para búsquedas frecuentes
CREATE INDEX IX_Cotizaciones_Fecha ON dbo.Cotizaciones (FechaCotizacion);
CREATE INDEX IX_Cotizaciones_IdCliente ON dbo.Cotizaciones (IdCliente);
CREATE INDEX IX_Cotizaciones_IdTipoSeguro ON dbo.Cotizaciones (IdTipoSeguro);
GO

CREATE TABLE [dbo].[ReglasTasa](
	[IdRegla] [int] IDENTITY(1,1) NOT NULL,
	[IdTipoSeguro] [int] NOT NULL,
	[SumaMin] [decimal](18, 2) NOT NULL,
	[SumaMax] [decimal](18, 2) NULL,
	[Tasa] [decimal](9, 6) NOT NULL,
	[FechaCreacion] [datetime2](3) NOT NULL,
	[Estado] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdRegla] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ReglasTasa] ADD  DEFAULT ((0)) FOR [SumaMin]
GO

ALTER TABLE [dbo].[ReglasTasa] ADD  DEFAULT (sysutcdatetime()) FOR [FechaCreacion]
GO

ALTER TABLE [dbo].[ReglasTasa] ADD  DEFAULT ((1)) FOR [Estado]
GO


COMMIT TRANSACTION;
GO

PRINT 'Script ejecutado correctamente. Tablas creadas: TiposSeguros, TipoClientes, Moneda, Clientes, Cotizaciones.';
