-- Script para crear la tabla de términos excluidos
-- Ejecutar este script en la base de datos del ChatBot

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='excluded_terms' AND xtype='U')
BEGIN
    CREATE TABLE excluded_terms (
        id INT IDENTITY(1,1) PRIMARY KEY,
        firebase_user_id NVARCHAR(255) NOT NULL,
        term NVARCHAR(500) NOT NULL,
        category NVARCHAR(100) NOT NULL,
        created_at DATETIME2 NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME2 NULL,
        is_active BIT NOT NULL DEFAULT 1,
        
        -- Índices para mejorar el rendimiento
        INDEX IX_excluded_terms_user_active (firebase_user_id, is_active),
        INDEX IX_excluded_terms_category (category),
        INDEX IX_excluded_terms_term (term),
        
        -- Índice único para evitar duplicados
        UNIQUE INDEX UX_excluded_terms_user_term_category (firebase_user_id, term, category, is_active)
    );
    
    PRINT '✅ Tabla excluded_terms creada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠️ La tabla excluded_terms ya existe';
END

-- Verificar si existen datos de ejemplo
IF NOT EXISTS (SELECT TOP 1 * FROM excluded_terms)
BEGIN
    -- Insertar algunos datos de ejemplo (opcional)
    INSERT INTO excluded_terms (firebase_user_id, term, category, created_at, is_active) VALUES
    ('demo_user_1', 'lady gaga', 'Artista', GETDATE(), 1),
    ('demo_user_1', 'no me gusta', 'Palabra clave', GETDATE(), 1),
    ('demo_user_1', 'reggaeton', 'Género', GETDATE(), 1),
    ('demo_user_2', 'bad bunny', 'Artista', GETDATE(), 1);
    
    PRINT '✅ Datos de ejemplo insertados';
END
ELSE
BEGIN
    PRINT 'ℹ️ La tabla ya contiene datos';
END

-- Mostrar estructura de la tabla
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'excluded_terms'
ORDER BY ORDINAL_POSITION;

PRINT '📋 Estructura de la tabla excluded_terms mostrada';
