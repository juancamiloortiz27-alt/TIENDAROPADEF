IF OBJECT_ID(N'dbo.Auditorias', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Auditorias]
    (
        [IdAuditoria] int not null identity(1,1) primary key,
        [FechaHoraUtc] datetime2 not null,
        [UsuarioId] int null,
        [CorreoUsuario] nvarchar(256) null,
        [RolUsuario] nvarchar(30) null,
        [Accion] nvarchar(20) not null,
        [Entidad] nvarchar(100) not null,
        [IdRegistro] nvarchar(200) null,
        [Cambios] nvarchar(max) null
    )
END
