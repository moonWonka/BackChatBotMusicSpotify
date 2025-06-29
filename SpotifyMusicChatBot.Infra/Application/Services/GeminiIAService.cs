using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SpotifyMusicChatBot.Domain.Application.Services;

namespace SpotifyMusicChatBot.Infra.Application.Services
{
    public class GeminiIAService : IGeminiIAService
    {
        public string ModelName => "Gemini";

        public string Provider => "Google";

        private static readonly string? APIKEY = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        public GeminiIAService()
        {
        }

        public async Task<AIModelResponse> ExecuteModelAsync(string prompt, float temperature = 0.7F, int maxTokens = 1000, CancellationToken cancellationToken = default, object? generationConfig = null)
        {
            if (string.IsNullOrWhiteSpace(APIKEY))
                throw new InvalidOperationException("La variable de entorno GEMINI_API_KEY no está configurada.");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-goog-api-key", APIKEY);
                var requestBody = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                requestBody["contents"] = new[]
                {
                    new {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                };
                if (generationConfig != null)
                {
                    requestBody["generationConfig"] = generationConfig;
                }
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(
                    "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent",
                    content,
                    cancellationToken);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                // Deserializar usando los modelos GeminiResponse, GeminiCandidate, etc.
                string extractedText = responseBody;
                try
                {
                    var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    extractedText = geminiResponse?.CANDIDATES?
                        .FirstOrDefault()?.CONTENT?
                        .PARTS?.FirstOrDefault()?.TEXT ?? string.Empty;
                }
                catch { /* Si falla el parseo, deja el body completo */ }
                return new AIModelResponse
                {
                    Content = extractedText
                };
            }
        }

        // Sobrecarga para compatibilidad con la interfaz
        public async Task<AIModelResponse> ExecuteModelAsync(string prompt, float temperature = 0.7F, int maxTokens = 1000, CancellationToken cancellationToken = default)
        {
            return await ExecuteModelAsync(prompt, temperature, maxTokens, cancellationToken, null);
        }
    }

    // Modelos para deserializar la respuesta de Gemini
    public class GeminiResponse
    {
        public List<GeminiCandidate> CANDIDATES { get; set; }
    }

    public class GeminiCandidate
    {
        public GeminiContent CONTENT { get; set; }
    }

    public class GeminiContent
    {
        public List<GeminiPart> PARTS { get; set; }
        public string ROLE { get; set; }
    }

    public class GeminiPart
    {
        public string TEXT { get; set; }
    }
}