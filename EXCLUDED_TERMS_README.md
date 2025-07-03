# 🎵 Funcionalidad de Términos Excluidos - ChatBot Musical

Esta funcionalidad permite a los usuarios personalizar sus resultados de búsqueda musical excluyendo artistas, géneros o palabras clave específicas.

## 📋 Características

- **Gestión de términos excluidos por usuario**: Cada usuario puede tener su propia lista personalizada
- **Categorización**: Los términos se organizan en categorías (Artista, Género, Palabra clave)
- **Filtrado automático**: Las respuestas se filtran automáticamente antes de mostrarse al usuario
- **Persistencia**: Los términos se almacenan en base de datos y se recuperan en futuras sesiones

## 🗄️ Base de Datos

### Tabla: `excluded_terms`

```sql
CREATE TABLE excluded_terms (
    id INT IDENTITY(1,1) PRIMARY KEY,
    firebase_user_id NVARCHAR(255) NOT NULL,
    term NVARCHAR(255) NOT NULL,
    category NVARCHAR(100) NOT NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETDATE(),
    updated_at DATETIME2 NULL,
    is_active BIT NOT NULL DEFAULT 1
);
```

Para crear la tabla, ejecuta el script: `Database/create_excluded_terms_table.sql`

## 🚀 API Endpoints

### 1. Crear Término Excluido
```http
POST /api/ExcludedTerms
Content-Type: application/json

{
  "firebaseUserId": "user123",
  "term": "Lady Gaga",
  "category": "Artista"
}
```

### 2. Obtener Términos Excluidos
```http
GET /api/ExcludedTerms/{firebaseUserId}
GET /api/ExcludedTerms/{firebaseUserId}?category=Artista
```

### 3. Actualizar Término Excluido
```http
PUT /api/ExcludedTerms/{id}
Content-Type: application/json

{
  "id": 1,
  "firebaseUserId": "user123",
  "term": "Lady Gaga",
  "category": "Artista",
  "isActive": true
}
```

### 4. Eliminar Término Excluido
```http
DELETE /api/ExcludedTerms/{id}?firebaseUserId=user123
```

### 5. Verificar si Existe un Término
```http
GET /api/ExcludedTerms/exists?firebaseUserId=user123&term=Lady%20Gaga&category=Artista
```

### 6. Chat con Filtros (NUEVO)
```http
POST /api/Chat/ask-filtered
Content-Type: application/json

{
  "question": "¿Cuáles son las mejores canciones de pop?",
  "firebaseUserId": "user123",
  "sessionId": "session456",
  "tone": "casual",
  "resultLimit": 20,
  "modelName": "Gemini"
}
```

## 💻 Ejemplos de Uso

### Escenario 1: Usuario excluye un artista
1. Usuario agrega "Justin Bieber" a términos excluidos (categoría: Artista)
2. Usuario pregunta: "¿Cuáles son las mejores canciones de pop?"
3. El sistema filtra automáticamente cualquier referencia a Justin Bieber
4. La respuesta incluye otros artistas de pop pero no menciona a Justin Bieber

### Escenario 2: Usuario excluye un género
1. Usuario agrega "Reggaeton" a términos excluidos (categoría: Género)
2. Usuario pregunta: "Recomiéndame música para bailar"
3. El sistema filtra automáticamente canciones de reggaeton
4. La respuesta incluye otros géneros bailables

### Escenario 3: Usuario excluye palabras clave
1. Usuario agrega "explícito" a términos excluidos (categoría: Palabra clave)
2. Usuario pregunta: "¿Qué canciones están populares?"
3. El sistema filtra contenido que mencione explícitamente esa palabra
4. La respuesta se enfoca en contenido más apropiado

## 🔧 Configuración Técnica

### Servicios Requeridos

1. **IChatBotRepository**: Para operaciones de base de datos
2. **IExcludedTermsFilterService**: Para filtrado de respuestas
3. **IAIService**: Para procesamiento de lenguaje natural

### Inyección de Dependencias

```csharp
// En Program.cs o Startup.cs
services.AddScoped<IExcludedTermsFilterService, ExcludedTermsFilterService>();
```

### Variables de Entorno

Asegúrate de que la variable `CHATDB` esté configurada con la cadena de conexión a la base de datos.

## 🔄 Flujo de Funcionamiento

1. **Usuario hace una pregunta** → `/api/Chat/ask-filtered`
2. **Validación de pregunta** → IA verifica si es relevante
3. **Generación de SQL** → IA crea consulta a base de datos
4. **Ejecución de consulta** → Se obtienen resultados
5. **Generación de respuesta** → IA crea respuesta natural
6. **⭐ Aplicación de filtros** → Se eliminan términos excluidos
7. **Respuesta final** → Usuario recibe respuesta filtrada
8. **Guardado de conversación** → Se almacena en historial

## 📝 Categorías Soportadas

- **Artista**: Nombres de cantantes, bandas, etc.
- **Género**: Estilos musicales (Pop, Rock, Reggaeton, etc.)
- **Palabra clave**: Términos generales que el usuario no quiere ver

## 🛡️ Seguridad

- Todos los términos están asociados a un `firebase_user_id`
- Un usuario solo puede ver/modificar sus propios términos
- Validación de datos en todos los endpoints
- Logging de operaciones para auditoría

## 🧪 Testing

### Datos de Prueba
```sql
INSERT INTO excluded_terms (firebase_user_id, term, category) VALUES
('testuser1', 'Lady Gaga', 'Artista'),
('testuser1', 'Reggaeton', 'Género'),
('testuser1', 'explícito', 'Palabra clave');
```

### Prueba Manual
1. Crear algunos términos excluidos para un usuario
2. Hacer una pregunta que normalmente incluiría esos términos
3. Verificar que la respuesta esté filtrada correctamente

## 🔮 Próximas Mejoras

- [ ] Filtrado por similitud semántica (no solo coincidencias exactas)
- [ ] Sugerencias automáticas de términos a excluir
- [ ] Importar/exportar listas de términos excluidos
- [ ] Términos excluidos temporales (con fecha de expiración)
- [ ] Dashboard para gestión visual de términos

## 📞 Soporte

Si encuentras problemas con la funcionalidad de términos excluidos, revisa:

1. **Logs del servidor**: Busca mensajes con emojis 🔍, ✅, ❌
2. **Base de datos**: Verifica que la tabla `excluded_terms` exista
3. **Variables de entorno**: Asegúrate de que `CHATDB` esté configurada
4. **Dependencias**: Verifica que todos los servicios estén registrados

## 🎯 Conclusión

La funcionalidad de términos excluidos mejora significativamente la experiencia del usuario al permitir personalizaciones granulares en las respuestas del chatbot musical. Los usuarios pueden mantener sus preferencias entre sesiones y obtener resultados más relevantes a sus gustos.
