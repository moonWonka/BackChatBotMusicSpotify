# Endpoints de IA - SpotifyMusicChatBot

Este documento describe los nuevos endpoints para la interacción con modelos de IA (Gemini y Anthropic) implementados en el proyecto SpotifyMusicChatBot.

## Descripción General

Los endpoints de IA permiten procesar preguntas de usuarios sobre música siguiendo un flujo estructurado que incluye:

1. **Contextualización** - Analiza si la pregunta necesita contexto del historial de conversación
2. **Validación** - Verifica que la pregunta sea relevante para el dominio musical
3. **Generación SQL** - Convierte la pregunta a una consulta SQL
4. **Ejecución** - Ejecuta la consulta contra la base de datos de música
5. **Respuesta Natural** - Convierte los resultados en una respuesta conversacional

## Endpoints Disponibles

### 1. Procesar Pregunta Completa
**POST** `/api/AI/process-question`

Procesa una pregunta completa siguiendo todo el flujo de IA.

#### Request Body:
```json
{
  "sessionId": "session_123",
  "question": "¿Cuáles son las canciones más populares de Bad Bunny?",
  "aiModel": "Gemini",
  "includeContext": true,
  "contextLimit": 10
}
```

#### Response:
```json
{
  "isSuccess": true,
  "message": "Pregunta procesada exitosamente",
  "originalQuestion": "¿Cuáles son las canciones más populares de Bad Bunny?",
  "contextualizedQuestion": "¿Cuáles son las canciones más populares de Bad Bunny?",
  "wasContextualized": false,
  "validationStatus": "VALIDA",
  "generatedSQL": "SELECT TOP 50 t.name as track_name, a.name as artist_name, t.popularity FROM tracks t INNER JOIN artists a ON t.artist_id = a.artist_id WHERE a.name LIKE '%Bad Bunny%' ORDER BY t.popularity DESC",
  "databaseResults": "Resultados de canciones de Bad Bunny...",
  "naturalResponse": "Bad Bunny tiene varias canciones muy populares. Entre sus éxitos más destacados se encuentran...",
  "aiModelUsed": "Gemini",
  "processingTimeMs": 1250,
  "steps": {
    "contextualizationTimeMs": 200,
    "validationTimeMs": 300,
    "sqlGenerationTimeMs": 400,
    "sqlExecutionTimeMs": 100,
    "naturalResponseTimeMs": 250
  }
}
```

### 2. Contextualizar Pregunta
**POST** `/api/AI/contextualize-question`

Analiza si una pregunta necesita contexto del historial de conversación.

#### Request Body:
```json
{
  "sessionId": "session_123",
  "question": "¿Y cuáles son sus canciones más populares?",
  "aiModel": "Gemini",
  "includeContext": true,
  "contextLimit": 5
}
```

#### Response:
```json
{
  "isSuccess": true,
  "message": "Pregunta contextualizada exitosamente",
  "originalQuestion": "¿Y cuáles son sus canciones más populares?",
  "contextualizedQuestion": "¿Cuáles son las canciones más populares de Bad Bunny?",
  "wasContextualized": true,
  "analysisType": "CONTEXTUALIZADA",
  "conversationHistory": [
    {
      "turn": 1,
      "messageType": "User",
      "content": "¿Quién es Bad Bunny?",
      "timestamp": "2025-06-27T10:30:00Z"
    },
    {
      "turn": 2,
      "messageType": "Assistant",
      "content": "Bad Bunny es un cantante y rapero puertorriqueño...",
      "timestamp": "2025-06-27T10:30:15Z"
    }
  ],
  "aiModelUsed": "Gemini",
  "processingTimeMs": 200
}
```

### 3. Validar Pregunta
**POST** `/api/AI/validate-question`

Valida si una pregunta es relevante para el asistente musical.

#### Request Body:
```json
{
  "question": "¿Cuáles son las características musicales del reggaetón?",
  "aiModel": "Gemini"
}
```

#### Response:
```json
{
  "isSuccess": true,
  "message": "Pregunta válida para el asistente musical",
  "question": "¿Cuáles son las características musicales del reggaetón?",
  "validationStatus": "VALIDA",
  "validationReason": "La pregunta está relacionada con música",
  "identifiedCategory": "GENEROS",
  "aiModelUsed": "Gemini",
  "processingTimeMs": 300,
  "confidenceLevel": 85
}
```

#### Ejemplo de respuesta para pregunta que requiere aclaración:
```json
{
  "isSuccess": false,
  "message": "La pregunta requiere aclaración",
  "question": "¿Cuánto?",
  "validationStatus": "ACLARAR",
  "validationReason": "La pregunta es demasiado corta o ambigua",
  "suggestions": [
    "Proporciona más detalles en tu pregunta",
    "Especifica sobre qué artista, canción o álbum quieres saber"
  ],
  "confidenceLevel": 75
}
```

### 4. Generar SQL
**POST** `/api/AI/generate-sql`

Genera una consulta SQL a partir de una pregunta en lenguaje natural.

#### Request Body:
```json
{
  "question": "¿Qué canciones tienen más de 80 de popularidad?",
  "aiModel": "Gemini",
  "resultLimit": 25
}
```

#### Response:
```json
{
  "isSuccess": true,
  "message": "SQL generado exitosamente",
  "question": "¿Qué canciones tienen más de 80 de popularidad?",
  "generatedSQL": "SELECT TOP 25 t.name as track_name, a.name as artist_name, t.popularity FROM tracks t INNER JOIN artists a ON t.artist_id = a.artist_id WHERE t.popularity > 80 ORDER BY t.popularity DESC",
  "sqlExplanation": "Consulta SQL generada para buscar información musical",
  "tablesUsed": ["tracks", "artists"],
  "fieldsSelected": ["t.name as track_name", "a.name as artist_name", "t.popularity"],
  "whereConditions": ["t.popularity > 80"],
  "operationType": "SELECT",
  "complexityLevel": "SIMPLE",
  "aiModelUsed": "Gemini",
  "processingTimeMs": 400,
  "confidenceLevel": 80
}
```

### 5. Generar Respuesta Natural
**POST** `/api/AI/generate-natural-response`

Convierte resultados de base de datos en una respuesta conversacional.

#### Request Body:
```json
{
  "question": "¿Qué canciones tienen más de 80 de popularidad?",
  "databaseResults": "track_name: Tití Me Preguntó, artist_name: Bad Bunny, popularity: 95\ntrack_name: As It Was, artist_name: Harry Styles, popularity: 92\ntrack_name: Me Porto Bonito, artist_name: Bad Bunny, popularity: 88",
  "aiModel": "Gemini",
  "responseTone": "casual",
  "responseLength": "medium"
}
```

#### Response:
```json
{
  "isSuccess": true,
  "message": "Respuesta generada exitosamente",
  "question": "¿Qué canciones tienen más de 80 de popularidad?",
  "naturalResponse": "¡Hola! Te cuento que encontré 3 resultados que coinciden con tu búsqueda. Los datos muestran información relevante para tu consulta. ¿Te gustaría saber algo más específico?",
  "dataSummary": "Se encontraron 3 resultados con información sobre: track_name, artist_name, popularity",
  "statistics": {
    "totalItems": 3,
    "mainCategories": ["track_name", "artist_name", "popularity"]
  },
  "relatedQuestions": [
    "¿Cuáles son los géneros musicales más populares?",
    "¿Qué artistas están en tendencia actualmente?",
    "¿Cuáles son los éxitos más recientes?"
  ],
  "highlights": [
    "3 resultados encontrados",
    "Categorías: track_name, artist_name"
  ],
  "responseTone": "casual",
  "responseLength": "medium",
  "aiModelUsed": "Gemini",
  "processingTimeMs": 350,
  "confidenceLevel": 85,
  "processedItemsCount": 3
}
```

## Modelos de IA Soportados

- **Gemini** (Google) - Modelo por defecto
- **Anthropic** (Claude) - Modelo alternativo

## Códigos de Estado HTTP

- **200** - Operación exitosa
- **400** - Datos de entrada inválidos
- **422** - Pregunta fuera de contexto o requiere aclaración
- **500** - Error interno del servidor

## Esquema de Base de Datos

Los endpoints generan SQL basándose en el siguiente esquema:

```sql
-- Artistas
CREATE TABLE artists (
    artist_id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE
);

-- Canciones
CREATE TABLE tracks (
    track_id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    artist_id INT NOT NULL,
    duration_ms INT NOT NULL,
    popularity INT CHECK (popularity >= 0 AND popularity <= 100),
    release_date DATE,
    explicit BIT DEFAULT 0,
    FOREIGN KEY (artist_id) REFERENCES artists(artist_id)
);

-- Características de Audio
CREATE TABLE audio_features (
    feature_id INT IDENTITY(1,1) PRIMARY KEY,
    track_id INT NOT NULL,
    danceability FLOAT CHECK (danceability >= 0 AND danceability <= 1),
    energy FLOAT CHECK (energy >= 0 AND energy <= 1),
    loudness FLOAT,
    speechiness FLOAT CHECK (speechiness >= 0 AND speechiness <= 1),
    acousticness FLOAT CHECK (acousticness >= 0 AND acousticness <= 1),
    instrumentalness FLOAT CHECK (instrumentalness >= 0 AND instrumentalness <= 1),
    liveness FLOAT CHECK (liveness >= 0 AND liveness <= 1),
    valence FLOAT CHECK (valence >= 0 AND valence <= 1),
    tempo FLOAT CHECK (tempo > 0),
    time_signature INT CHECK (time_signature >= 1 AND time_signature <= 7),
    FOREIGN KEY (track_id) REFERENCES tracks(track_id)
);
```

## Ejemplos de Preguntas Válidas

- "¿Cuáles son las canciones más populares de Bad Bunny?"
- "¿Qué artistas tienen canciones con alta energía?"
- "Muéstrame canciones de reggaetón con alta bailabilidad"
- "¿Cuáles son los álbumes más recientes?"
- "¿Qué colaboraciones ha hecho este artista?"

## Flujo Recomendado de Uso

1. **Para preguntas simples**: Usar directamente `/process-question`
2. **Para depuración**: Usar endpoints individuales para analizar cada paso
3. **Para aplicaciones conversacionales**: Usar `/contextualize-question` seguido del flujo completo
4. **Para validación previa**: Usar `/validate-question` antes de procesar
