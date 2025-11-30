/* ============================================
   INSERT MASIVO DE CATÁLOGOS (Profesional)
   ============================================ */

-- 1) TipoSeguros
INSERT INTO dbo.TipoSeguros (NombreSeguro, Codigo, Descripcion)
VALUES
('Médico',     'MED',  'Seguro médico / salud'),
('Automóvil',  'AUTO', 'Seguro de vehículo automotor'),
('Incendio',   'INC',  'Seguro contra incendios y daños'),
('Fianzas',    'FIA',  'Fianzas y cauciones financieras');

-- 2) TipoClientes
INSERT INTO dbo.TipoClientes (NombreTipoCliente, Descripcion)
VALUES
('Natural',  'Persona natural - individuo'),
('Jurídico', 'Persona jurídica - empresa o institución');

-- 3) Moneda
INSERT INTO dbo.Moneda (CodigoISO, Nombre, Simbolo)
VALUES
('HNL', 'Lempira', 'L'),
('USD', 'Dólar estadounidense', '$'),
('EUR', 'Euro', '€');

-- 4) ReglasTasa (por TipoSeguro)
-- Médico (1)
INSERT INTO dbo.ReglasTasa (IdTipoSeguro, SumaMin, SumaMax, Tasa) VALUES
(1, 0,        49999.99,   0.035),
(1, 50000,    149999.99,  0.028),
(1, 150000,   NULL,        0.022);

-- Automóvil (2)
INSERT INTO dbo.ReglasTasa (IdTipoSeguro, SumaMin, SumaMax, Tasa) VALUES
(2, 0,        19999.99, 0.050),
(2, 20000,    49999.99, 0.038),
(2, 50000,    99999.99, 0.028),
(2, 100000,   299999.99,0.020),
(2, 300000,   NULL,     0.017);

-- Incendio (3)
INSERT INTO dbo.ReglasTasa (IdTipoSeguro, SumaMin, SumaMax, Tasa) VALUES
(3, 0,        99999.99,   0.0060),
(3, 100000,   499999.99,  0.0045),
(3, 500000,   1999999.99, 0.0032),
(3, 2000000,  NULL,       0.0020);

-- Fianzas (4)
INSERT INTO dbo.ReglasTasa (IdTipoSeguro, SumaMin, SumaMax, Tasa) VALUES
(4, 0,         49999.99,  0.040),
(4, 50000,     199999.99, 0.030),
(4, 200000,    999999.99, 0.020),
(4, 1000000,   NULL,      0.015);
