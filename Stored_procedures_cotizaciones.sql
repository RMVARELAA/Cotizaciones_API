USE [COTIZACIONES];
GO

/* =========================================================
   CLIENTE
   ========================================================= */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_Delete]
(
    @IdCliente INT,
    @UsuarioModificacion NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE dbo.Clientes
        SET Estado = 0,
            FechaModificacion = SYSUTCDATETIME(),
            UsuarioModificacion = @UsuarioModificacion
        WHERE IdCliente = @IdCliente
          AND Estado = 1;

        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 0 AS Resultado;
            RETURN;
        END

        SELECT 1 AS Resultado;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Clientes
    WHERE Estado = 1
    ORDER BY Nombre;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_GetById]
(
    @IdCliente INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Clientes
    WHERE IdCliente = @IdCliente AND Estado = 1;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_GetByIdentidad]
(
    @Identidad NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Clientes
    WHERE Identidad = @Identidad AND Estado = 1;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_Insert]
(
    @Nombre NVARCHAR(180),
    @Identidad NVARCHAR(50),
    @FechaNacimiento DATE = NULL,
    @IdTipoCliente INT,
    @Telefono NVARCHAR(30) = NULL,
    @Email NVARCHAR(150) = NULL,
    @Direccion NVARCHAR(300) = NULL,
    @UsuarioCreacion NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO dbo.Clientes
        (
            Nombre, Identidad, FechaNacimiento, IdTipoCliente,
            Telefono, Email, Direccion,
            FechaCreacion, UsuarioCreacion, Estado
        )
        VALUES
        (
            @Nombre, @Identidad, @FechaNacimiento, @IdTipoCliente,
            @Telefono, @Email, @Direccion,
            SYSUTCDATETIME(), @UsuarioCreacion, 1
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdCliente;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cliente_Update]
(
    @IdCliente INT,
    @Nombre NVARCHAR(180),
    @Identidad NVARCHAR(50),
    @FechaNacimiento DATE = NULL,
    @IdTipoCliente INT,
    @Telefono NVARCHAR(30) = NULL,
    @Email NVARCHAR(150) = NULL,
    @Direccion NVARCHAR(300) = NULL,
    @UsuarioModificacion NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE dbo.Clientes
        SET
            Nombre = @Nombre,
            Identidad = @Identidad,
            FechaNacimiento = @FechaNacimiento,
            IdTipoCliente = @IdTipoCliente,
            Telefono = @Telefono,
            Email = @Email,
            Direccion = @Direccion,
            FechaModificacion = SYSUTCDATETIME(),
            UsuarioModificacion = @UsuarioModificacion
        WHERE IdCliente = @IdCliente
          AND Estado = 1;

        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 0 AS Resultado;
            RETURN;
        END

        SELECT 1 AS Resultado;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

/* =========================================================
   COTIZACION
   ========================================================= */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_Delete]
(
    @IdCotizacion BIGINT,
    @UsuarioModificacion NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Cotizaciones
        SET
            Estado = 0,
            FechaModificacion = SYSUTCDATETIME(),
            UsuarioModificacion = @UsuarioModificacion
        WHERE IdCotizacion = @IdCotizacion
          AND Estado = 1;

        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 0 AS Resultado;
            RETURN;
        END

        SELECT 1 AS Resultado;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_GenerarNumero]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Siguiente BIGINT = NEXT VALUE FOR dbo.Seq_Cotizacion;
    DECLARE @Anio CHAR(4) = YEAR(SYSUTCDATETIME());

    DECLARE @NumeroCotizacion NVARCHAR(50) =
        'COT-' + @Anio + '-' + RIGHT('00000' + CAST(@Siguiente AS VARCHAR(5)), 5);

    SELECT @NumeroCotizacion AS NumeroCotizacion;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_GetById]
(
    @IdCotizacion BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.IdCotizacion,
        c.NumeroCotizacion,
        c.FechaCotizacion,
        c.IdTipoSeguro,
        ts.NombreSeguro AS NombreTipoSeguro,
        c.IdMoneda,
        m.CodigoISO AS MonedaCodigoISO,
        m.Nombre AS MonedaNombre,
        c.IdCliente,
        cl.Nombre AS ClienteNombre,
        c.DescripcionBien,
        c.SumaAsegurada,
        c.Tasa,
        c.PrimaNeta,
        c.Observaciones,
        c.FechaCreacion,
        c.FechaModificacion,
        c.UsuarioCreacion,
        c.UsuarioModificacion,
        c.Estado
    FROM dbo.Cotizaciones c
    LEFT JOIN dbo.TipoSeguros ts ON c.IdTipoSeguro = ts.IdTipoSeguro
    LEFT JOIN dbo.Moneda m ON c.IdMoneda = m.IdMoneda
    LEFT JOIN dbo.Clientes cl ON c.IdCliente = cl.IdCliente
    WHERE c.IdCotizacion = @IdCotizacion
      AND c.Estado = 1;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_GetByNumero]
(
    @NumeroCotizacion NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdCotizacion,
        NumeroCotizacion,
        FechaCotizacion,
        IdTipoSeguro,
        IdMoneda,
        IdCliente,
        DescripcionBien,
        SumaAsegurada,
        Tasa,
        PrimaNeta,
        Observaciones,
        FechaCreacion,
        FechaModificacion,
        UsuarioCreacion,
        UsuarioModificacion,
        Estado
    FROM dbo.Cotizaciones
    WHERE NumeroCotizacion = @NumeroCotizacion
      AND Estado = 1;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_GetReport]
(
    @Desde DATETIME2(3) = NULL,
    @Hasta DATETIME2(3) = NULL,
    @IdTipoSeguro INT = NULL,
    @IdCliente INT = NULL,
    @IdMoneda INT = NULL,
    @NumeroCotizacion NVARCHAR(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.IdCotizacion,
        c.NumeroCotizacion,
        c.FechaCotizacion,
        ts.IdTipoSeguro,
        ts.NombreSeguro AS TipoSeguro,
        m.IdMoneda,
        m.Nombre AS Moneda,
        cl.IdCliente,
        cl.Nombre AS ClienteNombre,
        c.DescripcionBien,
        c.SumaAsegurada,
        c.Tasa,
        c.PrimaNeta,
        c.Observaciones,
        c.FechaCreacion
    FROM dbo.Cotizaciones c
    INNER JOIN dbo.TipoSeguros ts ON c.IdTipoSeguro = ts.IdTipoSeguro
    INNER JOIN dbo.Moneda m ON c.IdMoneda = m.IdMoneda
    INNER JOIN dbo.Clientes cl ON c.IdCliente = cl.IdCliente
    WHERE c.Estado = 1
      AND (@Desde IS NULL OR c.FechaCotizacion >= @Desde)
      AND (@Hasta IS NULL OR c.FechaCotizacion <= @Hasta)
      AND (@IdTipoSeguro IS NULL OR c.IdTipoSeguro = @IdTipoSeguro)
      AND (@IdCliente IS NULL OR c.IdCliente = @IdCliente)
      AND (@IdMoneda IS NULL OR c.IdMoneda = @IdMoneda)
      AND (@NumeroCotizacion IS NULL OR c.NumeroCotizacion = @NumeroCotizacion)
    ORDER BY c.FechaCotizacion DESC;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_Insert]
(
    @NumeroCotizacion NVARCHAR(50),
    @FechaCotizacion DATETIME2(3),
    @IdTipoSeguro INT,
    @IdMoneda INT,
    @IdCliente INT,
    @DescripcionBien NVARCHAR(1000) = NULL,
    @SumaAsegurada DECIMAL(18,2),
    @Tasa DECIMAL(9,6),
    @PrimaNeta DECIMAL(18,2),
    @Observaciones NVARCHAR(1000) = NULL,
    @UsuarioCreacion NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO dbo.Cotizaciones
        (
            NumeroCotizacion,
            FechaCotizacion,
            IdTipoSeguro,
            IdMoneda,
            IdCliente,
            DescripcionBien,
            SumaAsegurada,
            Tasa,
            PrimaNeta,
            Observaciones,
            FechaCreacion,
            UsuarioCreacion,
            Estado
        )
        VALUES
        (
            @NumeroCotizacion,
            @FechaCotizacion,
            @IdTipoSeguro,
            @IdMoneda,
            @IdCliente,
            @DescripcionBien,
            @SumaAsegurada,
            @Tasa,
            @PrimaNeta,
            @Observaciones,
            SYSUTCDATETIME(),
            @UsuarioCreacion,
            1
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS IdCotizacion;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_Update]
(
    @IdCotizacion BIGINT,
    @IdTipoSeguro INT,
    @IdMoneda INT,
    @IdCliente INT,
    @DescripcionBien NVARCHAR(1000) = NULL,
    @SumaAsegurada DECIMAL(18,2),
    @Tasa DECIMAL(9,6),
    @PrimaNeta DECIMAL(18,2),
    @Observaciones NVARCHAR(1000) = NULL,
    @UsuarioModificacion NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE dbo.Cotizaciones
        SET
            IdTipoSeguro = @IdTipoSeguro,
            IdMoneda = @IdMoneda,
            IdCliente = @IdCliente,
            DescripcionBien = @DescripcionBien,
            SumaAsegurada = @SumaAsegurada,
            Tasa = @Tasa,
            PrimaNeta = @PrimaNeta,
            Observaciones = @Observaciones,
            FechaModificacion = SYSUTCDATETIME(),
            UsuarioModificacion = @UsuarioModificacion
        WHERE IdCotizacion = @IdCotizacion
          AND Estado = 1;

        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 0 AS Resultado;
            RETURN;
        END

        SELECT 1 AS Resultado;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

/* =========================================================
   MONEDA
   ========================================================= */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Moneda_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Moneda WHERE Estado = 1;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Moneda_GetById]
(
    @IdMoneda INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Moneda
    WHERE IdMoneda = @IdMoneda AND Estado = 1;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Moneda_Insert]
(
    @CodigoISO CHAR(3),
    @Nombre NVARCHAR(100),
    @Simbolo NVARCHAR(5)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Moneda(CodigoISO, Nombre, Simbolo, FechaCreacion, Estado)
    VALUES (@CodigoISO, @Nombre, @Simbolo, SYSUTCDATETIME(), 1);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdMoneda;
END;
GO

/* =========================================================
   TASA
   ========================================================= */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Tasa_GetPorTipoYSuma]
(
    @IdTipoSeguro INT,
    @SumaAsegurada DECIMAL(18,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP(1) Tasa
    FROM dbo.ReglasTasa r
    WHERE r.Estado = 1
      AND r.IdTipoSeguro = @IdTipoSeguro
      AND @SumaAsegurada >= r.SumaMin
      AND (r.SumaMax IS NULL OR @SumaAsegurada <= r.SumaMax)
    ORDER BY r.SumaMin DESC;
END;
GO

/* =========================================================
   TIPO CLIENTE
   ========================================================= */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_TipoCliente_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.TipoClientes WHERE Estado = 1;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_TipoCliente_GetById]
(
    @IdTipoCliente INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.TipoClientes
    WHERE IdTipoCliente = @IdTipoCliente AND Estado = 1;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_TipoCliente_Insert]
(
    @NombreTipoCliente NVARCHAR(50),
    @Descripcion NVARCHAR(250)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.TipoClientes(NombreTipoCliente, Descripcion, FechaCreacion, Estado)
    VALUES (@NombreTipoCliente, @Descripcion, SYSUTCDATETIME(), 1);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdTipoCliente;
END;
GO

/* =========================================================
   TIPO SEGURO
   ========================================================= */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_TipoSeguro_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.TipoSeguros
    WHERE Estado = 1;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_TipoSeguro_GetById]
(
    @IdTipoSeguro INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.TipoSeguros
    WHERE IdTipoSeguro = @IdTipoSeguro AND Estado = 1;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_TipoSeguro_Insert]
(
    @NombreSeguro NVARCHAR(70),
    @Codigo NVARCHAR(20),
    @Descripcion NVARCHAR(250)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.TipoSeguros(NombreSeguro, Codigo, Descripcion, FechaCreacion, Estado)
    VALUES (@NombreSeguro, @Codigo, @Descripcion, SYSUTCDATETIME(), 1);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdTipoSeguro;
END;
GO
