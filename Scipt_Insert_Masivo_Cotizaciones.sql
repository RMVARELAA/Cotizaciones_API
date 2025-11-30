USE [COTIZACIONES];
GO

SET NOCOUNT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    ----------------------------------------------------------------
    -- 1) Catalogos: TipoSeguros (si no existen)
    ----------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.TipoSeguros WHERE Codigo = 'MED')
    BEGIN
        INSERT INTO dbo.TipoSeguros (NombreSeguro, Codigo, Descripcion)
        VALUES
        ('Médico',     'MED',  'Seguro médico / salud'),
        ('Automóvil',  'AUTO', 'Seguro de vehículo automotor'),
        ('Incendio',   'INC',  'Seguro contra incendios y daños'),
        ('Fianzas',    'FIA',  'Fianzas y cauciones financieras');
    END

    ----------------------------------------------------------------
    -- 2) Catalogo: TipoClientes (si no existen)
    ----------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.TipoClientes WHERE NombreTipoCliente = 'Natural')
    BEGIN
        INSERT INTO dbo.TipoClientes (NombreTipoCliente, Descripcion)
        VALUES
        ('Natural',  'Persona natural - individuo'),
        ('Jurídico', 'Persona jurídica - empresa o institución');
    END

    ----------------------------------------------------------------
    -- 3) Catalogo: Moneda (si no existen)
    ----------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.Moneda WHERE CodigoISO = 'HNL')
    BEGIN
        INSERT INTO dbo.Moneda (CodigoISO, Nombre, Simbolo)
        VALUES
        ('HNL', 'Lempira', 'L'),
        ('USD', 'Dólar estadounidense', '$'),
        ('EUR', 'Euro', '€');
    END

    ----------------------------------------------------------------
    -- 4) ReglasTasa (si no existen reglas para cada tipo)
    ----------------------------------------------------------------
    -- Médico (IdTipoSeguro = 1)
    IF NOT EXISTS (SELECT 1 FROM dbo.ReglasTasa r JOIN dbo.TipoSeguros t ON r.IdTipoSeguro = t.IdTipoSeguro WHERE t.Codigo = 'MED')
    BEGIN
        DECLARE @idMed INT = (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo = 'MED');
        INSERT INTO dbo.ReglasTasa (IdTipoSeguro, SumaMin, SumaMax, Tasa)
        VALUES
        (@idMed, 0,        49999.99,   0.035),
        (@idMed, 50000,    149999.99,  0.028),
        (@idMed, 150000,   NULL,       0.022);
    END

    -- Automóvil (AUTO)
    IF NOT EXISTS (SELECT 1 FROM dbo.ReglasTasa r JOIN dbo.TipoSeguros t ON r.IdTipoSeguro = t.IdTipoSeguro WHERE t.Codigo = 'AUTO')
    BEGIN
        DECLARE @idAuto INT = (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo = 'AUTO');
        INSERT INTO dbo.ReglasTasa (IdTipoSeguro, SumaMin, SumaMax, Tasa)
        VALUES
        (@idAuto, 0,        19999.99, 0.050),
        (@idAuto, 20000,    49999.99, 0.038),
        (@idAuto, 50000,    99999.99, 0.028),
        (@idAuto, 100000,   299999.99,0.020),
        (@idAuto, 300000,   NULL,     0.017);
    END

    -- Incendio (INC)
    IF NOT EXISTS (SELECT 1 FROM dbo.ReglasTasa r JOIN dbo.TipoSeguros t ON r.IdTipoSeguro = t.IdTipoSeguro WHERE t.Codigo = 'INC')
    BEGIN
        DECLARE @idInc INT = (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo = 'INC');
        INSERT INTO dbo.ReglasTasa (IdTipoSeguro, SumaMin, SumaMax, Tasa)
        VALUES
        (@idInc, 0,        99999.99,   0.0060),
        (@idInc, 100000,   499999.99,  0.0045),
        (@idInc, 500000,   1999999.99, 0.0032),
        (@idInc, 2000000,  NULL,       0.0020);
    END

    -- Fianzas (FIA)
    IF NOT EXISTS (SELECT 1 FROM dbo.ReglasTasa r JOIN dbo.TipoSeguros t ON r.IdTipoSeguro = t.IdTipoSeguro WHERE t.Codigo = 'FIA')
    BEGIN
        DECLARE @idFia INT = (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo = 'FIA');
        INSERT INTO dbo.ReglasTasa (IdTipoSeguro, SumaMin, SumaMax, Tasa)
        VALUES
        (@idFia, 0,         49999.99,  0.040),
        (@idFia, 50000,     199999.99, 0.030),
        (@idFia, 200000,    999999.99, 0.020),
        (@idFia, 1000000,   NULL,      0.015);
    END

    ----------------------------------------------------------------
    -- 5) Clientes de prueba (si no existen por Identidad)
    ----------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.Clientes WHERE Identidad = '0801199000011')
    BEGIN
        INSERT INTO dbo.Clientes 
        (Nombre, Identidad, FechaNacimiento, IdTipoCliente, Telefono, Email, Direccion, UsuarioCreacion)
        VALUES
        ('Juan Pérez',          '0801199000011', '1990-01-10', (SELECT IdTipoCliente FROM dbo.TipoClientes WHERE NombreTipoCliente='Natural'), '9999-1111', 'juan.perez@mail.com', 'Tegucigalpa', 'system'),
        ('María Gómez',         '0801199500055', '1995-05-15', (SELECT IdTipoCliente FROM dbo.TipoClientes WHERE NombreTipoCliente='Natural'), '8888-2222', 'maria.gomez@mail.com', 'San Pedro Sula', 'system'),
        ('Empresa XYZ S.A.',    '0601198500999', '1985-11-20', (SELECT IdTipoCliente FROM dbo.TipoClientes WHERE NombreTipoCliente='Jurídico'), '2266-1122', 'contacto@xyz.com', 'Choloma', 'system'),
        ('Servicios ABC LLC',   '0501198000666', '1980-08-08', (SELECT IdTipoCliente FROM dbo.TipoClientes WHERE NombreTipoCliente='Jurídico'), '2233-4455', 'info@abc.com', 'El Progreso', 'system');
    END

    ----------------------------------------------------------------
    -- 6) Cotizaciones MASIVAS (20 registros)
    --    Usamos INSERT ... VALUES; calculamos PrimaNeta con expresiones.
    ----------------------------------------------------------------

    -- Evitamos duplicar por NumeroCotizacion: solo insertamos si número no existe.
    -- Listado de inserts: si no existe NumeroCotizacion, insertamos.

    -- Automóvil (AUTO) - 4
    IF NOT EXISTS (SELECT 1 FROM dbo.Cotizaciones WHERE NumeroCotizacion = 'COT-2025-00001')
    BEGIN
        INSERT INTO dbo.Cotizaciones
        (NumeroCotizacion, FechaCotizacion, IdTipoSeguro, IdMoneda, IdCliente,
         DescripcionBien, SumaAsegurada, Tasa, PrimaNeta, Observaciones, UsuarioCreacion)
        VALUES
        ('COT-2025-00001', '2025-01-10', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='AUTO'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0801199000011'),
         'Toyota Corolla 2018', 18000.00, 0.050, ROUND(18000.00 * 0.050, 2), 'Auto usado', 'system'),

        ('COT-2025-00002', '2025-01-15', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='AUTO'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0801199500055'),
         'Mazda 3 2020', 32000.00, 0.038, ROUND(32000.00 * 0.038, 2), 'Auto reciente', 'system'),

        ('COT-2025-00003', '2025-02-01', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='AUTO'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='USD'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0601198500999'),
         'Hilux 2022', 68000.00, 0.028, ROUND(68000.00 * 0.028, 2), 'Pickup nueva', 'system'),

        ('COT-2025-00004', '2025-02-05', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='AUTO'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='USD'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0501198000666'),
         'BMW X5 2023', 120000.00, 0.020, ROUND(120000.00 * 0.020, 2), 'Auto de lujo', 'system');
    END

    -- Médico (MED) - 3
    IF NOT EXISTS (SELECT 1 FROM dbo.Cotizaciones WHERE NumeroCotizacion = 'COT-2025-00005')
    BEGIN
        INSERT INTO dbo.Cotizaciones
        (NumeroCotizacion, FechaCotizacion, IdTipoSeguro, IdMoneda, IdCliente,
         DescripcionBien, SumaAsegurada, Tasa, PrimaNeta, Observaciones, UsuarioCreacion)
        VALUES
        ('COT-2025-00005', '2025-01-12', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='MED'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0801199000011'),
         'Cobertura familiar', 30000.00, 0.035, ROUND(30000.00 * 0.035, 2), 'Plan familiar', 'system'),

        ('COT-2025-00006', '2025-01-20', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='MED'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='USD'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0801199500055'),
         'Cobertura individual', 80000.00, 0.028, ROUND(80000.00 * 0.028, 2), 'Plan base', 'system'),

        ('COT-2025-00007', '2025-01-25', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='MED'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0601198500999'),
         'Cobertura ejecutiva', 200000.00, 0.022, ROUND(200000.00 * 0.022, 2), 'Plan premium', 'system');
    END

    -- Incendio (INC) - 4
    IF NOT EXISTS (SELECT 1 FROM dbo.Cotizaciones WHERE NumeroCotizacion = 'COT-2025-00008')
    BEGIN
        INSERT INTO dbo.Cotizaciones
        (NumeroCotizacion, FechaCotizacion, IdTipoSeguro, IdMoneda, IdCliente,
         DescripcionBien, SumaAsegurada, Tasa, PrimaNeta, Observaciones, UsuarioCreacion)
        VALUES
        ('COT-2025-00008', '2025-02-10', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='INC'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0801199000011'),
         'Casa habitación', 90000.00, 0.0060, ROUND(90000.00 * 0.0060, 2), 'Casa pequeña', 'system'),

        ('COT-2025-00009', '2025-02-12', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='INC'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0801199500055'),
         'Residencia grande', 300000.00, 0.0045, ROUND(300000.00 * 0.0045, 2), 'Casa mediana', 'system'),

        ('COT-2025-00010', '2025-02-15', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='INC'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='USD'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0601198500999'),
         'Local comercial', 800000.00, 0.0032, ROUND(800000.00 * 0.0032, 2), 'Negocio', 'system'),

        ('COT-2025-00011', '2025-02-18', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='INC'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='USD'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0501198000666'),
         'Planta industrial', 3000000.00, 0.0020, ROUND(3000000.00 * 0.0020, 2), 'Industria', 'system');
    END

    -- Fianzas (FIA) - 4
    IF NOT EXISTS (SELECT 1 FROM dbo.Cotizaciones WHERE NumeroCotizacion = 'COT-2025-00012')
    BEGIN
        INSERT INTO dbo.Cotizaciones
        (NumeroCotizacion, FechaCotizacion, IdTipoSeguro, IdMoneda, IdCliente,
         DescripcionBien, SumaAsegurada, Tasa, PrimaNeta, Observaciones, UsuarioCreacion)
        VALUES
        ('COT-2025-00012', '2025-03-01', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='FIA'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0601198500999'),
         'Garantía de cumplimiento', 40000.00, 0.040, ROUND(40000.00 * 0.040, 2), 'Contrato pequeño', 'system'),

        ('COT-2025-00013', '2025-03-05', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='FIA'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0501198000666'),
         'Garantía aduanera', 150000.00, 0.030, ROUND(150000.00 * 0.030, 2), 'Operación mediana', 'system'),

        ('COT-2025-00014', '2025-03-10', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='FIA'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='USD'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0601198500999'),
         'Fianza bancaria', 500000.00, 0.020, ROUND(500000.00 * 0.020, 2), 'Proyecto grande', 'system'),

        ('COT-2025-00015', '2025-03-15', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='FIA'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='USD'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0501198000666'),
         'Fianza de construcción', 1200000.00, 0.015, ROUND(1200000.00 * 0.015, 2), 'Obra civil', 'system');
    END

    -- Extras (5 adicionales)
    IF NOT EXISTS (SELECT 1 FROM dbo.Cotizaciones WHERE NumeroCotizacion = 'COT-2025-00016')
    BEGIN
        INSERT INTO dbo.Cotizaciones
        (NumeroCotizacion, FechaCotizacion, IdTipoSeguro, IdMoneda, IdCliente,
         DescripcionBien, SumaAsegurada, Tasa, PrimaNeta, Observaciones, UsuarioCreacion)
        VALUES
        ('COT-2025-00016', '2025-02-20', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='AUTO'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0801199000011'),
         'Kia Rio 2017', 17000.00, 0.050, ROUND(17000.00 * 0.050, 2), '', 'system'),

        ('COT-2025-00017', '2025-02-22', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='MED'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='USD'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0601198500999'),
         'Cobertura personal', 60000.00, 0.028, ROUND(60000.00 * 0.028, 2), '', 'system'),

        ('COT-2025-00018', '2025-02-25', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='INC'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0801199500055'),
         'Bodega pequeña', 70000.00, 0.0060, ROUND(70000.00 * 0.0060, 2), '', 'system'),

        ('COT-2025-00019', '2025-02-27', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='FIA'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='USD'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0601198500999'),
         'Fianza laboral', 100000.00, 0.030, ROUND(100000.00 * 0.030, 2), '', 'system'),

        ('COT-2025-00020', '2025-03-01', (SELECT IdTipoSeguro FROM dbo.TipoSeguros WHERE Codigo='AUTO'), (SELECT IdMoneda FROM dbo.Moneda WHERE CodigoISO='HNL'), (SELECT IdCliente FROM dbo.Clientes WHERE Identidad='0501198000666'),
         'Toyota Prado 2021', 93000.00, 0.028, ROUND(93000.00 * 0.028, 2), '', 'system');
    END

    ----------------------------------------------------------------
    -- Commit
    ----------------------------------------------------------------
    COMMIT TRANSACTION;
    PRINT 'Inserciones masivas completadas correctamente.';
END TRY
BEGIN CATCH
    DECLARE @errMsg NVARCHAR(MAX) = ERROR_MESSAGE();
    DECLARE @errNum INT = ERROR_NUMBER();
    ROLLBACK TRANSACTION;
    RAISERROR('Error en script masivo: %d - %s', 16, 1, @errNum, @errMsg);
END CATCH;
GO
