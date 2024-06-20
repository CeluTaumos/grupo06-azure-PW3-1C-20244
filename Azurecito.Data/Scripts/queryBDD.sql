
CREATE DATABASE AzurecitoFotos;
GO


USE AzurecitoFotos;
GO

--  tabla Usuarios
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Usuarios' and xtype='U')
BEGIN
    CREATE TABLE Usuarios (
        Id INT PRIMARY KEY IDENTITY,
        NombreUsuario NVARCHAR(50) NOT NULL,
        Password NVARCHAR(50) NOT NULL,
        EsAdmin BIT NOT NULL 
    );
END
GO

--  tabla Fotos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Fotos' and xtype='U')
BEGIN
    CREATE TABLE Fotos (
        Id INT PRIMARY KEY IDENTITY,
        UserId INT FOREIGN KEY REFERENCES Usuarios(Id),
        FotoUrl NVARCHAR(200) NOT NULL,
        EstaAprobada BIT NOT NULL
    );
END
GO

-- Insertar cinco usuarios, uno de ellos administrador
INSERT INTO Usuarios (NombreUsuario, Password, EsAdmin) VALUES ('Celu', 'admin123', 1);
INSERT INTO Usuarios (NombreUsuario, Password, EsAdmin) VALUES ('Dari', 'dario', 0);
INSERT INTO Usuarios (NombreUsuario, Password, EsAdmin) VALUES ('Anita', 'ana', 0);
INSERT INTO Usuarios (NombreUsuario, Password, EsAdmin) VALUES ('Emi', 'emiliano', 0);
INSERT INTO Usuarios (NombreUsuario, Password, EsAdmin) VALUES ('Ivi', 'ivan', 0);
GO