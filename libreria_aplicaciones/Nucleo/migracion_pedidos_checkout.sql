USE TIENDADEROPAONLINE2;
GO

ALTER TABLE [Cupones] ALTER COLUMN [PorcentajeDescuento] decimal(5,2) NOT NULL;
GO

IF COL_LENGTH('MetodoPagos', 'Estado') IS NULL
    ALTER TABLE [MetodoPagos] ADD [Estado] bit NOT NULL
        CONSTRAINT [DF_MetodoPagos_Estado] DEFAULT (1) WITH VALUES;
GO

UPDATE [MetodoPagos]
SET [Nombre] = N'Pago en línea', [Descripcion] = N'Pago realizado desde la tienda', [Tipo] = N'Digital'
WHERE [Nombre] = N'Pago simulado';
GO

IF NOT EXISTS (SELECT 1 FROM [MetodoPagos] WHERE [Nombre] = N'Pago en línea')
    INSERT INTO [MetodoPagos] ([Nombre], [Descripcion], [Tipo], [Comision], [Estado])
    VALUES (N'Pago en línea', N'Pago realizado desde la tienda', N'Digital', 0, 1);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'UX_Envios_Pedido' AND [object_id] = OBJECT_ID(N'dbo.Envios'))
    CREATE UNIQUE INDEX [UX_Envios_Pedido] ON [Envios]([Pedido]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [name] = N'UX_Envios_NumeroGuia' AND [object_id] = OBJECT_ID(N'dbo.Envios'))
    CREATE UNIQUE INDEX [UX_Envios_NumeroGuia] ON [Envios]([NumeroGuia]);
GO

ALTER TABLE [Pedidos] ALTER COLUMN [Cupon] int NULL;
GO

IF COL_LENGTH('Pedidos', 'CiudadEnvio') IS NULL
    ALTER TABLE [Pedidos] ADD [CiudadEnvio] nvarchar(30) NULL;
GO

IF COL_LENGTH('Pedidos', 'DireccionEnvio') IS NULL
    ALTER TABLE [Pedidos] ADD [DireccionEnvio] nvarchar(100) NULL;
GO

IF COL_LENGTH('Pedidos', 'CodigoPostalEnvio') IS NULL
    ALTER TABLE [Pedidos] ADD [CodigoPostalEnvio] nvarchar(10) NULL;
GO
