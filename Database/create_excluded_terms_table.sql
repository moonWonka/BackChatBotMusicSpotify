-- =======================================
-- SCRIPT DE CREACIÓN DE TABLA PARA TÉRMINOS EXCLUIDOS
-- =======================================

-- Tabla para almacenar términos excluidos por usuario
CREATE TABLE excluded_terms (
    id INT IDENTITY(1,1) PRIMARY KEY,
    firebase_user_id NVARCHAR(255) NOT NULL,
    term NVARCHAR(255) NOT NULL,
    category NVARCHAR(100) NOT NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETDATE(),
    updated_at DATETIME2 NULL,
    is_active BIT NOT NULL DEFAULT 1,
    
    -- Índices para mejorar el rendimiento
    INDEX IX_excluded_terms_user_active (firebase_user_id, is_active),
    INDEX IX_excluded_terms_category (category),
    INDEX IX_excluded_terms_term (term),
    
    -- Restricción única para evitar duplicados
    CONSTRAINT UQ_excluded_terms_user_term_category 
        UNIQUE (firebase_user_id, term, category)
);

-- Comentarios para documentar la tabla
EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla que almacena los términos excluidos por cada usuario para filtrar respuestas del chatbot',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'excluded_terms';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID único del término excluido',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'excluded_terms',
    @level2type = N'COLUMN', @level2name = N'id';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID del usuario de Firebase que excluye el término',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'excluded_terms',
    @level2type = N'COLUMN', @level2name = N'firebase_user_id';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Término a excluir (artista, género, palabra clave, etc.)',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'excluded_terms',
    @level2type = N'COLUMN', @level2name = N'term';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Categoría del término (Artista, Género, Palabra clave, etc.)',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'excluded_terms',
    @level2type = N'COLUMN', @level2name = N'category';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha de creación del término excluido',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'excluded_terms',
    @level2type = N'COLUMN', @level2name = N'created_at';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha de última actualización del término excluido',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'excluded_terms',
    @level2type = N'COLUMN', @level2name = N'updated_at';

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si el término excluido está activo (1) o inactivo (0)',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'excluded_terms',
    @level2type = N'COLUMN', @level2name = N'is_active';

-- Datos de ejemplo para testing (opcional)
/*
INSERT INTO excluded_terms (firebase_user_id, term, category) VALUES
('user123', 'Lady Gaga', 'Artista'),
('user123', 'Reggaeton', 'Género'),
('user123', 'no me gusta', 'Palabra clave'),
('user456', 'Justin Bieber', 'Artista'),
('user456', 'Pop', 'Género');
*/

PRINT 'Tabla excluded_terms creada exitosamente con índices y restricciones.';
