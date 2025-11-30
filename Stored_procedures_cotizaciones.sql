USE [COTIZACIONES];
GO

/* ===========================
   CLIENTES
   =========================== */

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
    WHERE IdCliente = @IdCliente;
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
    @IdCliente           INT,
    @Nombre              NVARCHAR(180),
    @Identidad           NVARCHAR(50),
    @FechaNacimiento     DATE = NULL,
    @IdTipoCliente       INT,
    @Telefono            NVARCHAR(30) = NULL,
    @Email               NVARCHAR(150) = NULL,
    @Direccion           NVARCHAR(300) = NULL,
    @UsuarioModificacion NVARCHAR(100) = NULL,
    @Estado              BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE dbo.Clientes
        SET
            Nombre              = @Nombre,
            Identidad           = @Identidad,
            FechaNacimiento     = @FechaNacimiento,
            IdTipoCliente       = @IdTipoCliente,
            Telefono            = @Telefono,
            Email               = @Email,
            Direccion           = @Direccion,
            Estado              = @Estado,
            FechaModificacion   = SYSUTCDATETIME(),
            UsuarioModificacion = @UsuarioModificacion
        WHERE IdCliente = @IdCliente;

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


/* ===========================
   COTIZACIONES - CRUD BÁSICO
   =========================== */

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


/* ===========================
   COTIZACIONES - QUERIES
   =========================== */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_GetAll]
(
    @Desde DATETIME2(3) = NULL,
    @Hasta DATETIME2(3) = NULL,
    @IdTipoSeguro INT = NULL,
    @IdCliente INT = NULL,
    @IdMoneda INT = NULL,
    @NumeroCotizacion NVARCHAR(50) = NULL,
    @Estado BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 1000
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @PageNumber IS NULL OR @PageNumber < 1 SET @PageNumber = 1;
        IF @PageSize IS NOT NULL AND @PageSize < 1 SET @PageSize = 1000;

        DECLARE @OffsetRows BIGINT = 0;
        IF @PageSize IS NOT NULL
            SET @OffsetRows = (CAST(@PageNumber - 1 AS BIGINT) * CAST(@PageSize AS BIGINT));

        ;WITH Filtered AS
        (
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
            WHERE (@Estado IS NULL OR c.Estado = @Estado)
              AND (@Desde IS NULL OR c.FechaCotizacion >= @Desde)
              AND (@Hasta IS NULL OR c.FechaCotizacion <= @Hasta)
              AND (@IdTipoSeguro IS NULL OR c.IdTipoSeguro = @IdTipoSeguro)
              AND (@IdCliente IS NULL OR c.IdCliente = @IdCliente)
              AND (@IdMoneda IS NULL OR c.IdMoneda = @IdMoneda)
              AND (@NumeroCotizacion IS NULL OR c.NumeroCotizacion = @NumeroCotizacion)
        )
        SELECT
            F.*,
            COUNT(1) OVER() AS TotalRows,
            ROW_NUMBER() OVER (ORDER BY F.FechaCotizacion DESC, F.IdCotizacion DESC) AS RowNum
        INTO #CotTmp
        FROM Filtered F;

        IF @PageSize IS NULL
        BEGIN
            SELECT
                IdCotizacion,
                NumeroCotizacion,
                FechaCotizacion,
                IdTipoSeguro,
                NombreTipoSeguro,
                IdMoneda,
                MonedaCodigoISO,
                MonedaNombre,
                IdCliente,
                ClienteNombre,
                DescripcionBien,
                SumaAsegurada,
                Tasa,
                PrimaNeta,
                Observaciones,
                FechaCreacion,
                FechaModificacion,
                UsuarioCreacion,
                UsuarioModificacion,
                Estado,
                TotalRows
            FROM #CotTmp
            ORDER BY FechaCotizacion DESC, IdCotizacion DESC;

            DROP TABLE #CotTmp;
            RETURN;
        END

        SELECT
            IdCotizacion,
            NumeroCotizacion,
            FechaCotizacion,
            IdTipoSeguro,
            NombreTipoSeguro,
            IdMoneda,
            MonedaCodigoISO,
            MonedaNombre,
            IdCliente,
            ClienteNombre,
            DescripcionBien,
            SumaAsegurada,
            Tasa,
            PrimaNeta,
            Observaciones,
            FechaCreacion,
            FechaModificacion,
            UsuarioCreacion,
            UsuarioModificacion,
            Estado,
            TotalRows
        FROM #CotTmp
        WHERE RowNum > @OffsetRows
          AND RowNum <= (@OffsetRows + @PageSize)
        ORDER BY FechaCotizacion DESC, IdCotizacion DESC;

        DROP TABLE #CotTmp;
    END TRY
    BEGIN CATCH
        IF OBJECT_ID('tempdb..#CotTmp') IS NOT NULL DROP TABLE #CotTmp;
        THROW;
    END CATCH
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
CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_GetPaged]
(
    @Desde DATETIME2(3) = NULL,
    @Hasta DATETIME2(3) = NULL,
    @IdTipoSeguro INT = NULL,
    @IdCliente INT = NULL,
    @IdMoneda INT = NULL,
    @NumeroCotizacion NVARCHAR(50) = NULL,
    @Estado BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 25
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber IS NULL OR @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize  IS NULL OR @PageSize  < 1 SET @PageSize  = 25;

    DECLARE @OffsetRows BIGINT = (CAST(@PageNumber - 1 AS BIGINT) * CAST(@PageSize AS BIGINT));

    ;WITH Filtered AS
    (
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
        WHERE (@Estado IS NULL OR c.Estado = @Estado)
          AND (@Desde IS NULL OR c.FechaCotizacion >= @Desde)
          AND (@Hasta IS NULL OR c.FechaCotizacion <= @Hasta)
          AND (@IdTipoSeguro IS NULL OR c.IdTipoSeguro = @IdTipoSeguro)
          AND (@IdCliente IS NULL OR c.IdCliente = @IdCliente)
          AND (@IdMoneda IS NULL OR c.IdMoneda = @IdMoneda)
          AND (@NumeroCotizacion IS NULL OR c.NumeroCotizacion = @NumeroCotizacion)
    )
    -- Result set paginado
    SELECT
        IdCotizacion,
        NumeroCotizacion,
        FechaCotizacion,
        IdTipoSeguro,
        NombreTipoSeguro,
        IdMoneda,
        MonedaCodigoISO,
        MonedaNombre,
        IdCliente,
        ClienteNombre,
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
    FROM
    (
        SELECT F.*, ROW_NUMBER() OVER (ORDER BY F.FechaCotizacion DESC, F.IdCotizacion DESC) AS RowNum
        FROM Filtered F
    ) P
    WHERE P.RowNum > @OffsetRows
      AND P.RowNum <= (@OffsetRows + @PageSize)
    ORDER BY FechaCotizacion DESC, IdCotizacion DESC;

    -- Segundo result set: total de registros
    SELECT COUNT(1) AS TotalRows
    FROM Filtered;
END;
GO


/* ===========================
   COTIZACIONES - REPORTES
   =========================== */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_GetReport]
(
    @Desde            DATETIME2(3) = NULL,
    @Hasta            DATETIME2(3) = NULL,
    @IdTipoSeguro     INT          = NULL,
    @IdCliente        INT          = NULL,
    @IdMoneda         INT          = NULL,
    @NumeroCotizacion NVARCHAR(50) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Normalizar Número de Cotización
    IF @NumeroCotizacion IS NOT NULL
    BEGIN
        SET @NumeroCotizacion = LTRIM(RTRIM(@NumeroCotizacion));
        IF @NumeroCotizacion = N'' SET @NumeroCotizacion = NULL;
    END

    -- Si el caller envía 0 para los Ids, tratarlos como NULL
    IF @IdTipoSeguro = 0 SET @IdTipoSeguro = NULL;
    IF @IdCliente    = 0 SET @IdCliente    = NULL;
    IF @IdMoneda     = 0 SET @IdMoneda     = NULL;

    SELECT
        c.IdCotizacion,
        c.NumeroCotizacion,
        c.FechaCotizacion,
        ts.IdTipoSeguro,
        ts.NombreSeguro AS TipoSeguro,
        m.IdMoneda,
        m.Nombre       AS Moneda,
        cl.IdCliente,
        cl.Nombre      AS ClienteNombre,
        c.DescripcionBien,
        c.SumaAsegurada,
        c.Tasa,
        c.PrimaNeta,
        c.Observaciones,
        c.FechaCreacion,
        c.Estado
    FROM dbo.Cotizaciones c
    INNER JOIN dbo.TipoSeguros ts ON c.IdTipoSeguro = ts.IdTipoSeguro
    INNER JOIN dbo.Moneda m       ON c.IdMoneda     = m.IdMoneda
    INNER JOIN dbo.Clientes cl    ON c.IdCliente    = cl.IdCliente
    WHERE c.Estado = 1
      AND (@Desde IS NULL OR c.FechaCotizacion >= @Desde)
      AND (@Hasta IS NULL OR c.FechaCotizacion < DATEADD(DAY, 1, @Hasta))
      AND (@IdTipoSeguro    IS NULL OR c.IdTipoSeguro    = @IdTipoSeguro)
      AND (@IdCliente       IS NULL OR c.IdCliente       = @IdCliente)
      AND (@IdMoneda        IS NULL OR c.IdMoneda        = @IdMoneda)
      AND (@NumeroCotizacion IS NULL OR c.NumeroCotizacion = @NumeroCotizacion)
    ORDER BY c.FechaCotizacion DESC;
END;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE [dbo].[sp_Cotizacion_GetReportPaged]
(
    @Desde        DATE          = NULL,
    @Hasta        DATE          = NULL,
    @IdTipoSeguro INT           = NULL,
    @Filtro       NVARCHAR(100) = NULL,
    @PageNumber   INT           = 1,
    @PageSize     INT           = 20
)
AS
BEGIN
    SET NOCOUNT ON;

    IF (@PageNumber < 1) SET @PageNumber = 1;
    IF (@PageSize   < 1) SET @PageSize   = 20;

    ;WITH CTE AS
    (
        SELECT
            c.IdCotizacion,
            c.NumeroCotizacion,
            c.FechaCotizacion,
            c.IdTipoSeguro,
            ts.NombreSeguro AS NombreTipoSeguro,
            c.IdMoneda,
            m.CodigoISO     AS MonedaCodigoISO,
            m.Nombre        AS MonedaNombre,
            c.IdCliente,
            cli.Nombre      AS ClienteNombre,
            c.DescripcionBien,
            c.SumaAsegurada,
            c.Tasa,
            c.PrimaNeta,
            c.Observaciones,
            COUNT(*) OVER() AS TotalRows
        FROM  dbo.Cotizaciones c
        INNER JOIN dbo.TipoSeguros ts ON ts.IdTipoSeguro = c.IdTipoSeguro
        INNER JOIN dbo.Moneda m       ON m.IdMoneda      = c.IdMoneda
        INNER JOIN dbo.Clientes cli   ON cli.IdCliente   = c.IdCliente
        WHERE
            (@Desde        IS NULL OR c.FechaCotizacion >= @Desde) AND
            (@Hasta        IS NULL OR c.FechaCotizacion <= @Hasta) AND
            (@IdTipoSeguro IS NULL OR c.IdTipoSeguro = @IdTipoSeguro) AND
            (
                @Filtro IS NULL OR @Filtro = '' OR
                c.NumeroCotizacion LIKE '%' + @Filtro + '%' OR
                cli.Nombre         LIKE '%' + @Filtro + '%' OR
                c.DescripcionBien  LIKE '%' + @Filtro + '%'
            )
    )
    SELECT
        IdCotizacion,
        NumeroCotizacion,
        FechaCotizacion,
        IdTipoSeguro,
        NombreTipoSeguro,
        IdMoneda,
        MonedaCodigoISO,
        MonedaNombre,
        IdCliente,
        ClienteNombre,
        DescripcionBien,
        SumaAsegurada,
        Tasa,
        PrimaNeta,
        Observaciones,
        TotalRows
    FROM CTE
    ORDER BY FechaCotizacion DESC, IdCotizacion DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO


/* ===========================
   MONEDA
   =========================== */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE [dbo].[sp_Moneda_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.Moneda
    WHERE Estado = 1;
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

    INSERT INTO dbo.Moneda (CodigoISO, Nombre, Simbolo, FechaCreacion, Estado)
    VALUES (@CodigoISO, @Nombre, @Simbolo, SYSUTCDATETIME(), 1);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdMoneda;
END;
GO


/* ===========================
   TASA / REGLAS
   =========================== */

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

    SELECT TOP (1) Tasa
    FROM dbo.ReglasTasa r
    WHERE r.Estado = 1
      AND r.IdTipoSeguro = @IdTipoSeguro
      AND @SumaAsegurada >= r.SumaMin
      AND (r.SumaMax IS NULL OR @SumaAsegurada <= r.SumaMax)
    ORDER BY r.SumaMin DESC;
END;
GO


/* ===========================
   TIPO CLIENTE
   =========================== */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE OR ALTER PROCEDURE [dbo].[sp_TipoCliente_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.TipoClientes
    WHERE Estado = 1;
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

    INSERT INTO dbo.TipoClientes (NombreTipoCliente, Descripcion, FechaCreacion, Estado)
    VALUES (@NombreTipoCliente, @Descripcion, SYSUTCDATETIME(), 1);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdTipoCliente;
END;
GO


/* ===========================
   TIPO SEGURO
   =========================== */

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

    INSERT INTO dbo.TipoSeguros (NombreSeguro, Codigo, Descripcion, FechaCreacion, Estado)
    VALUES (@NombreSeguro, @Codigo, @Descripcion, SYSUTCDATETIME(), 1);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdTipoSeguro;
END;
GO
