USE TIENDADEROPAONLINE2;
GO

IF COL_LENGTH('dbo.Productos', 'Imagen') IS NULL
    ALTER TABLE dbo.Productos ADD Imagen nvarchar(500) NULL;
GO

IF COL_LENGTH('dbo.Categorias', 'Imagen') IS NULL
    ALTER TABLE dbo.Categorias ADD Imagen nvarchar(500) NULL;
GO

IF COL_LENGTH('dbo.Marcas', 'Imagen') IS NULL
    ALTER TABLE dbo.Marcas ADD Imagen nvarchar(500) NULL;
GO
