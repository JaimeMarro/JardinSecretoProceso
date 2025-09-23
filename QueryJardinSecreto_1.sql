-- =============================================
-- CREACIÓN DE BASE DE DATOS
-- =============================================
CREATE DATABASE JardinSecreto;
GO

USE JardinSecreto;
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
-- TABLA DE PRODUCTO
-- =============================================
CREATE TABLE Producto (
    producto_id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(100) NOT NULL,
    descripcion NVARCHAR(255) NULL,
    precio DECIMAL(10,2) NOT NULL,
    disponible BIT NOT NULL DEFAULT 1,
    imagen_url NVARCHAR(500) NOT NULL
);
GO

-- =============================================
-- OPCIONAL: TABLA DE CATEGORÍA
-- =============================================
CREATE TABLE Categoria (
    categoria_id INT IDENTITY(1,1) PRIMARY KEY,
    nombre NVARCHAR(50) NOT NULL,
    descripcion NVARCHAR(255) NULL
);
GO

-- Agregar FK en Producto si quieres usar categorías
ALTER TABLE Producto
ADD categoria_id INT NULL
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (categoria_id)
    REFERENCES Categoria(categoria_id);

GO

INSERT INTO Administrador(usuario, contraseña_hash) VALUES ('admin', '$2a$11$L0My82Ma0WZSAND1VhtZC.qGGb2EzaMM/7BxadJ6j9VLxkPNpA8fa');
