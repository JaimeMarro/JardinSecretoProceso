-- =============================================
-- CREACIÓN DE BASE DE DATOS
-- =============================================
CREATE DATABASE JardinSecretoFinal;
GO

USE JardinSecretoFinal;
GO

-- =============================================
-- TABLA DE ADMINISTRADOR
-- =============================================
CREATE TABLE Administrador (
    admin_id INT IDENTITY(1,1) PRIMARY KEY,
    usuario NVARCHAR(50) UNIQUE NOT NULL,
    contraseña_hash NVARCHAR(255) NOT NULL,
    fecha_creacion DATETIME DEFAULT GETDATE()
);
GO

-- =============================================
-- TABLA DE CATEGORÍA
-- =============================================
CREATE TABLE Categoria (
    categoria_id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50) NOT NULL,
    descripcion NVARCHAR(255) NULL
);
GO

-- =============================================
-- TABLA DE PRODUCTO
-- =============================================
CREATE TABLE Producto (
    producto_id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    descripcion NVARCHAR(255) NULL,
    precio DECIMAL(10,2) NOT NULL,
    disponible BIT NOT NULL DEFAULT 1,
    imagen_url NVARCHAR(500) NOT NULL,
    categoria_id INT NULL,
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (categoria_id)
        REFERENCES Categoria(categoria_id)
);
GO

-- =============================================
-- TABLA DE SABORES
-- =============================================
CREATE TABLE Sabor(
    Sabor_id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(75) NOT NULL,
    Precio_Sabor DECIMAL(10,2) NULL,
    Id_Producto INT NOT NULL,

    CONSTRAINT FK_Sabor_Producto FOREIGN KEY (Id_Producto)
        REFERENCES Producto(producto_id)
        ON DELETE CASCADE   -- <<< AQUI EL CAMBIO IMPORTANTE
);
GO

-- =============================================
-- TABLA DE EXTRAS
-- =============================================
CREATE TABLE Extra(
    Extra_id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(75) NOT NULL,
    Precio_Extra DECIMAL(10,2) NULL,
    Id_Producto INT NOT NULL,

    CONSTRAINT FK_Extra_Producto FOREIGN KEY (Id_Producto)
        REFERENCES Producto(producto_id)
        ON DELETE CASCADE   -- <<< AQUI EL OTRO CAMBIO IMPORTANTE
);
GO

-- =============================================
-- INSERT ADMIN POR DEFECTO
-- =============================================
INSERT INTO Administrador(usuario, contraseña_hash)
VALUES ('admin', '$2a$11$L0My82Ma0WZSAND1VhtZC.qGGb2EzaMM/7BxadJ6j9VLxkPNpA8fa');
GO
