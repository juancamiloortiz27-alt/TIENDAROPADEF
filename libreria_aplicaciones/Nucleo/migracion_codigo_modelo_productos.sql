IF COL_LENGTH('dbo.Productos', 'CodigoModelo') IS NULL
BEGIN
    ALTER TABLE dbo.Productos ADD CodigoModelo nvarchar(40) NULL;
END
GO

UPDATE dbo.Productos
SET CodigoModelo = CONCAT('PRODUCTO-', IdProducto)
WHERE CodigoModelo IS NULL OR LTRIM(RTRIM(CodigoModelo)) = '';
GO

ALTER TABLE dbo.Productos ALTER COLUMN CodigoModelo nvarchar(40) NOT NULL;
GO
