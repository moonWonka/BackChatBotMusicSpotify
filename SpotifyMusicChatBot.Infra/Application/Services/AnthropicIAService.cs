using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SpotifyMusicChatBot.Domain.Application.Services;

namespace SpotifyMusicChatBot.Infra.Application.Services
{
    public class AnthropicIAService : IAnthropicIAService
    {
        private readonly ILogger<AnthropicIAService>? _logger;
        
        // Configuraciones por defecto (mejoradas)
        private static readonly string DefaultModel = "claude-3-5-sonnet-20241022";
        private static readonly string DefaultApiVersion = "2023-06-01";
        private static readonly string ApiBaseUrl = "https://api.anthropic.com/v1/messages";
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

        public string ModelName => "Claude";
        public string Provider => "Anthropic";

        public AnthropicIAService(ILogger<AnthropicIAService>? logger = null)
        {
            _logger = logger;
        }

        public async Task<AIModelResponse> ExecuteModelAsync(string prompt, float temperature = 0.7F, int maxTokens = 1000, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            
            try
            {
                // Validaciones de entrada
                if (string.IsNullOrWhiteSpace(prompt))
                {
                    var errorResponse = new AIModelResponse
                    {
                        Content = string.Empty,
                        IsSuccess = false,
                        Message = "El prompt no puede estar vacío",
                        ModelUsed = ModelName,
                        ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds
                    };
                    _logger?.LogWarning("Prompt vacío enviado al servicio de Anthropic");
                    return errorResponse;
                }

                if (temperature < 0 || temperature > 2)
                {
                    var errorResponse = new AIModelResponse
                    {
                        Content = string.Empty,
                        IsSuccess = false,
                        Message = "La temperatura debe estar entre 0 y 2",
                        ModelUsed = ModelName,
                        ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds
                    };
                    _logger?.LogWarning("Temperatura inválida: {Temperature}", temperature);
                    return errorResponse;
                }

                if (maxTokens <= 0 || maxTokens > 4096)
                {
                    var errorResponse = new AIModelResponse
                    {
                        Content = string.Empty,
                        IsSuccess = false,
                        Message = "El número máximo de tokens debe estar entre 1 y 4096",
                        ModelUsed = ModelName,
                        ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds
                    };
                    _logger?.LogWarning("MaxTokens inválido: {MaxTokens}", maxTokens);
                    return errorResponse;
                }

                // Obtener API Key
                var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                {
                    var errorResponse = new AIModelResponse
                    {
                        Content = string.Empty,
                        IsSuccess = false,
                        Message = "ANTHROPIC_API_KEY environment variable is not set.",
                        ModelUsed = ModelName,
                        ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds
                    };
                    _logger?.LogError("ANTHROPIC_API_KEY no está configurada");
                    return errorResponse;
                }

                // Configurar HttpClient con timeout y headers mejorados
                using var httpClient = new HttpClient();
                httpClient.Timeout = DefaultTimeout;
                
                // Headers requeridos por Anthropic
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                httpClient.DefaultRequestHeaders.Add("anthropic-version", DefaultApiVersion);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "SpotifyMusicChatBot/1.0");

                // Preparar el cuerpo de la solicitud con modelo actualizado
                var requestBody = new
                {
                    model = DefaultModel, // Usando claude-3-5-sonnet-20241022
                    max_tokens = maxTokens,
                    temperature,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });
                
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger?.LogInformation("Enviando solicitud a Anthropic API. Modelo: {Model}, Temperatura: {Temperature}, MaxTokens: {MaxTokens}", 
                    DefaultModel, temperature, maxTokens);

                // Realizar la solicitud HTTP
                var response = await httpClient.PostAsync(ApiBaseUrl, content, cancellationToken);
                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                // Manejar errores HTTP
                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = $"Anthropic API error: {response.StatusCode} - {responseString}";
                    _logger?.LogError("Error en API de Anthropic: {StatusCode} - {Response}", 
                        response.StatusCode, responseString);
                    
                    return new AIModelResponse
                    {
                        Content = string.Empty,
                        IsSuccess = false,
                        Message = errorMessage,
                        ModelUsed = ModelName,
                        ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds
                    };
                }

                // Parsear la respuesta JSON con manejo de errores mejorado
                try
                {
                    using var doc = JsonDocument.Parse(responseString);
                    var root = doc.RootElement;
                    
                    string messageContent = string.Empty;
                    int tokensUsed = 0;
                    
                    // Extraer contenido de la respuesta
                    if (root.TryGetProperty("content", out var contentElement) && 
                        contentElement.ValueKind == JsonValueKind.Array && 
                        contentElement.GetArrayLength() > 0)
                    {
                        var first = contentElement[0];
                        if (first.TryGetProperty("text", out var textProp) && 
                            textProp.ValueKind == JsonValueKind.String)
                        {
                            messageContent = textProp.GetString() ?? string.Empty;
                        }
                    }

                    // Extraer información de uso de tokens
                    if (root.TryGetProperty("usage", out var usageElement))
                    {
                        if (usageElement.TryGetProperty("output_tokens", out var outputTokensProp))
                        {
                            tokensUsed = outputTokensProp.GetInt32();
                        }
                    }

                    var processingTime = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
                    
                    _logger?.LogInformation("Respuesta exitosa de Anthropic API. Tokens utilizados: {TokensUsed}, Tiempo: {ProcessingTime}ms", 
                        tokensUsed, processingTime);

                    return new AIModelResponse
                    {
                        Content = messageContent,
                        IsSuccess = true,
                        Message = "Response generated successfully.",
                        ModelUsed = ModelName,
                        ProcessingTimeMs = processingTime,
                        TokensUsed = tokensUsed,
                        ConfidenceLevel = 85 // Valor por defecto, podría mejorarse con análisis de la respuesta
                    };
                }
                catch (JsonException jsonEx)
                {
                    var errorMessage = $"Error parsing Anthropic API response: {jsonEx.Message}";
                    _logger?.LogError(jsonEx, "Error al parsear respuesta JSON de Anthropic: {Response}", responseString);
                    
                    return new AIModelResponse
                    {
                        Content = string.Empty,
                        IsSuccess = false,
                        Message = errorMessage,
                        ModelUsed = ModelName,
                        ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds
                    };
                }
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                var errorMessage = "Timeout al conectar con Anthropic API";
                _logger?.LogError(ex, "Timeout en solicitud a Anthropic API");
                
                return new AIModelResponse
                {
                    Content = string.Empty,
                    IsSuccess = false,
                    Message = errorMessage,
                    ModelUsed = ModelName,
                    ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds
                };
            }
            catch (HttpRequestException httpEx)
            {
                var errorMessage = $"Error de red al conectar con Anthropic API: {httpEx.Message}";
                _logger?.LogError(httpEx, "Error de red en solicitud a Anthropic API");
                
                return new AIModelResponse
                {
                    Content = string.Empty,
                    IsSuccess = false,
                    Message = errorMessage,
                    ModelUsed = ModelName,
                    ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds
                };
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error inesperado: {ex.Message}";
                _logger?.LogError(ex, "Error inesperado en AnthropicIAService");
                
                return new AIModelResponse
                {
                    Content = string.Empty,
                    IsSuccess = false,
                    Message = errorMessage,
                    ModelUsed = ModelName,
                    ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds
                };
            }
        }
    }
}