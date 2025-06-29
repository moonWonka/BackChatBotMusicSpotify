namespace SpotifyMusicChatBot.Domain.Application.Services.Prompts
{
    /// <summary>
    /// Constantes que contienen todos los prompts utilizados para la interacción con modelos de IA
    /// </summary>
    public static class AIPrompts
    {
        /// <summary>
        /// Prompt para contextualizar preguntas basándose en el historial de conversación
        /// </summary>
        public const string PROMPT_CONTEXTO_CONVERSACIONAL = @"
            Eres un asistente musical experto que analiza el contexto de una conversación para mejorar la comprensión de las preguntas del usuario.

            Historial de la conversación actual:
            {historial_conversacion}

            Nueva pregunta del usuario:
            ""{pregunta_actual}""

            Tu tarea es analizar si la nueva pregunta necesita contexto del historial previo para ser completamente comprendida.

            Busca referencias implícitas como:
            - Pronombres (él, ella, esto, eso, sus, aquello)
            - Referencias temporales (después, antes, luego)
            - Continuaciones de temas anteriores
            - Preguntas que asumen conocimiento previo

            REGLAS DE RESPUESTA:
            1. Si la pregunta es completamente independiente y no necesita contexto, responde:
            ""INDEPENDIENTE: [pregunta original]""

            2. Si la pregunta necesita ser reformulada con información del contexto, responde:
            ""CONTEXTUALIZADA: [pregunta reformulada incluyendo la información necesaria del contexto]""

            3. Solo usa información explícita del historial proporcionado
            4. Mantén el sentido original de la pregunta
            5. Sé conciso y directo

            Responde ÚNICAMENTE con el formato especificado.";

        /// <summary>
        /// Prompt para validar si una pregunta es relevante para el asistente musical
        /// </summary>
        public const string PROMPT_VALIDAR_PREGUNTA = @"
            Analiza la siguiente pregunta del usuario y determina si es válida para un asistente musical especializado en música y canciones.

            CONTEXTO DISPONIBLE:
            El asistente puede responder preguntas sobre:
            - Artistas y cantantes (nombres, biografías, estilos)
            - Canciones y álbumes (títulos, fechas, características)
            - Géneros musicales y estilos
            - Colaboraciones y características técnicas de canciones
            - Popularidad, rankings y estadísticas musicales
            - Letras, duración y información de tracks
            - Características de audio (energía, bailabilidad, tempo, etc.)
            - Discografías y repertorios

            PREGUNTA DEL USUARIO: ""{pregunta}""

            CRITERIOS DE VALIDACIÓN:
            1. VÁLIDA: La pregunta está claramente relacionada con música y puede ser respondida
            2. ACLARAR: La pregunta es muy ambigua, demasiado general o necesita más información específica
            3. FUERA_CONTEXTO: La pregunta no está relacionada con música, artistas o canciones

            INSTRUCCIONES:
            - Analiza cuidadosamente el contenido y la intención de la pregunta
            - Considera si se puede responder con información musical
            - Si detectas términos no musicales dominantes, marca como FUERA_CONTEXTO
            - Si la pregunta es muy vaga o general, marca como ACLARAR

            Responde únicamente con una de estas opciones:
            - ""VALIDA""
            - ""ACLARAR: [explicación específica de qué necesita aclarar]""
            - ""FUERA_CONTEXTO""";

        /// <summary>
        /// Prompt para generar consultas SQL a partir de preguntas en lenguaje natural
        /// </summary>
        public const string PROMPT_GENERAR_SQL = @"
            Dada la siguiente estructura de tabla:

            -- =======================================
            -- TABLA DE ARTISTAS
            -- =======================================
            CREATE TABLE artists (
                artist_id INT IDENTITY(1,1) PRIMARY KEY,
                name VARCHAR(255) NOT NULL UNIQUE
            );

            -- =======================================
            -- TABLA DE TRACKS
            -- =======================================
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

            -- =======================================
            -- TABLA DE CARACTERÍSTICAS DE AUDIO
            -- =======================================
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

            Y la siguiente consulta en lenguaje natural:
            ""{pregunta}""

            Genera una consulta SQL para el motor SQL Server que responda la pregunta del usuario.

            REGLAS CRÍTICAS:
            0. Devuelve **solo** la sentencia SQL en texto plano, sin explicaciones adicionales
            1. Usa INNER JOIN para relacionar tablas cuando sea necesario
            2. Aplica filtros relevantes en la cláusula WHERE
            3. Usa ORDER BY para ordenar resultados de manera lógica
            4. Limita resultados con TOP {limite_resultados} al inicio del SELECT
            5. Usa nombres de columnas exactos según el esquema proporcionado
            6. Para búsquedas de texto usa LIKE con % (ej: LIKE '%texto%')
            7. Para rangos numéricos usa operadores apropiados (>, <, BETWEEN)
            8. Usa alias descriptivos para las columnas (AS column_name)
            9. Para agregaciones usa GROUP BY apropiadamente
            10. Evita subconsultas innecesarias
            11. Usa DISTINCT solo cuando sea realmente necesario
            12. Para fechas usa formato ISO (YYYY-MM-DD)
            13. Para porcentajes y decimales usa FLOAT
            14. Valida que los JOINs tengan sentido según las relaciones de FK
            15. Si necesitas calcular duraciones, convierte millisegundos apropiadamente

            VALIDACIÓN DE CONTEXTO:
            16. Si la consulta se sale del contexto de las tablas disponibles, responde exactamente:
                ""No es posible responder a esta consulta con las tablas musicales disponibles""

            17. Si la pregunta es ambigua sobre qué datos específicos buscar, genera la consulta más probable

            EJEMPLOS DE RESPUESTA CORRECTA:
            - ""SELECT TOP 10 name FROM artists ORDER BY name""
            - ""SELECT TOP 20 t.name, a.name AS artist_name FROM tracks t INNER JOIN artists a ON t.artist_id = a.artist_id WHERE t.popularity > 80 ORDER BY t.popularity DESC""

            Responde ÚNICAMENTE con la sentencia SQL o el mensaje de validación especificado.";

        /// <summary>
        /// Prompt para generar respuestas en lenguaje natural a partir de resultados de base de datos
        /// </summary>
        public const string PROMPT_RESPUESTA_NATURAL = @"
            Basándote en la pregunta del usuario y los resultados obtenidos de la base de datos, genera una respuesta clara, concisa y útil en lenguaje natural.

            Pregunta del usuario:
            ""{pregunta}""

            Resultados de la base de datos:
            {resultados_db}

            INSTRUCCIONES PARA LA RESPUESTA:
            1. Proporciona una respuesta en español, clara y fácil de entender
            2. Organiza la información de manera lógica y estructurada
            3. Si hay múltiples resultados, preséntelos de forma ordenada (listas, números)
            4. Incluye datos específicos relevantes (nombres, cifras, fechas)
            5. Usa un tono conversacional y amigable, como un experto musical hablando con un amigo
            6. No menciones aspectos técnicos como SQL, tablas, bases de datos o consultas
            7. Si los resultados están vacíos, indica que no se encontró información
            8. Para listas largas, presenta los más relevantes y menciona si hay más
            9. Incluye contexto adicional cuando sea útil (ej: ""Este artista es conocido por..."")
            10. Si hay datos numéricos, interprételos de manera comprensible
            11. Para fechas, usa formatos naturales (""en el año 2020"" en lugar de ""2020-01-01"")
            12. Si encuentras datos interesantes o sorprendentes, destacalos apropiadamente

            FORMATO DE RESPUESTA:
            - Empieza respondiendo directamente a la pregunta
            - Organiza la información en párrafos cortos o listas cuando sea apropiado
            - Termina con información adicional relevante si la hay
            - Mantén un equilibrio entre ser informativo y conciso

            TONO Y ESTILO:
            - Profesional pero accesible
            - Entusiasta sobre la música
            - Informativo sin ser abrumador
            - Natural y conversacional

            Si no hay resultados o están vacíos, responde:
            ""No encontré información específica sobre lo que me preguntas. Podrías reformular tu pregunta o ser más específico sobre qué artista, canción o información musical te interesa.""

            Genera una respuesta natural y útil basada en estos lineamientos.";

        /// <summary>
        /// Prompt para analizar y ajustar respuestas del modelo
        /// </summary>
        public const string PROMPT_ANALIZAR_RESPUESTA = @"
            Dada la siguiente pregunta del usuario:
            ""{pregunta}""

            Y la siguiente respuesta generada por el modelo:
            {respuesta_modelo}

            Ajusta la respuesta para cumplir con las siguientes reglas específicas del asistente musical:

            REGLAS DE PRESENTACIÓN:
            1. Responde directamente sin hacer mención a SQL, bases de datos u otros términos técnicos
            2. Usa un lenguaje natural, claro y amigable
            3. Organiza la información de manera lógica (listas, párrafos cortos)
            4. Para múltiples resultados, presenta los más relevantes primero
            5. Incluye datos específicos y concretos cuando estén disponibles
            6. Usa un tono conversacional como un experto musical
            7. Elimina cualquier jerga técnica o referencias a implementación
            8. Si hay números o estadísticas, interprételos de manera comprensible
            9. Para fechas, usa formatos naturales y comprensibles
            10. Añade contexto musical relevante cuando sea apropiado
            11. Destaca información interesante o sorprendente
            12. Mantén la respuesta concisa pero informativa
            13. IMPORTANTE: Nunca menciones aspectos técnicos de la implementación

            ESTRUCTURA PREFERIDA:
            - Respuesta directa a la pregunta
            - Información específica y relevante
            - Contexto adicional si es útil
            - Cierre natural de la respuesta

            EJEMPLO DE TRANSFORMACIÓN:
            ❌ ""La consulta SQL retornó 5 filas con los siguientes artistas de la tabla artists...""
            ✅ ""Encontré 5 artistas que coinciden con tu búsqueda...""

            ❌ ""Según los datos de la base de datos de audio_features...""
            ✅ ""Basándome en las características musicales de estas canciones...""

            Genera una versión mejorada de la respuesta que cumpla con estas reglas y mantenga toda la información relevante.";

        /// <summary>
        /// Esquema de base de datos para referencia en prompts
        /// </summary>
        public const string DATABASE_SCHEMA = @"
            -- =======================================
            -- ESQUEMA DE BASE DE DATOS MUSICAL
            -- =======================================

            -- Tabla de artistas
            CREATE TABLE artists (
                artist_id INT IDENTITY(1,1) PRIMARY KEY,
                name VARCHAR(255) NOT NULL UNIQUE
            );

            -- Tabla de canciones/tracks
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

            -- Tabla de características de audio
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
            );";
    }
}
