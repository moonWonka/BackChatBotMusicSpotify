using MediatR;
using SpotifyMusicChatBot.Domain.Application.Services;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace SpotifyMusicChatBot.API.Application.Command.AI.GenerateSQL
{
    /// <summary>
    /// Handler para generar consultas SQL a partir de preguntas en lenguaje natural
    /// </summary>
    public class GenerateSQLHandler : IRequestHandler<GenerateSQLRequest, GenerateSQLResponse>
    {
        private readonly IAIServiceFactory _aiServiceFactory;
        private readonly ILogger<GenerateSQLHandler> _logger;

        public GenerateSQLHandler(
            IAIServiceFactory aiServiceFactory,
            ILogger<GenerateSQLHandler> logger)
        {
            _aiServiceFactory = aiServiceFactory;
            _logger = logger;
        }

        public async Task<GenerateSQLResponse> Handle(GenerateSQLRequest request, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new GenerateSQLResponse
            {
                Question = request.Question,
                AIModelUsed = request.AIModel
            };

            try
            {
                // Generar SQL con IA
                var aiService = _aiServiceFactory.CreateAIService(request.AIModel);
                var sqlResult = await aiService.GenerateSQLAsync(request.Question, request.ResultLimit, cancellationToken);
                
                if (!sqlResult.IsSuccess || string.IsNullOrEmpty(sqlResult.GeneratedSQL))
                {
                    response.IsSuccess = false;
                    response.Message = sqlResult.Message ?? "No es posible generar SQL para esta consulta";
                    return response;
                }

                response.GeneratedSQL = sqlResult.GeneratedSQL;
                response.SQLExplanation = sqlResult.SQLExplanation;
                response.ConfidenceLevel = sqlResult.ConfidenceLevel;
                response.TablesUsed = sqlResult.TablesUsed;
                response.ComplexityLevel = sqlResult.ComplexityLevel;
                response.Warnings = sqlResult.Warnings;

                // Analizar la consulta generada (si no viene ya analizada)
                if (!response.TablesUsed.Any())
                {
                    var analysis = AnalyzeGeneratedSQL(response.GeneratedSQL);
                    response.TablesUsed = analysis.Tables;
                    response.FieldsSelected = analysis.Fields;
                    response.WhereConditions = analysis.WhereConditions;
                    response.OperationType = analysis.OperationType;
                    response.ComplexityLevel = analysis.ComplexityLevel;
                }

                // Validar y optimizar si es necesario (si no viene ya validado)
                if (!response.Warnings.Any())
                {
                    var validation = ValidateSQL(response.GeneratedSQL);
                    response.Warnings = validation.Warnings;

                    if (validation.CanOptimize)
                    {
                        response.OptimizedSQL = OptimizeSQL(response.GeneratedSQL);
                    }
                }

                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = true;
                response.Message = "SQL generado exitosamente";

                _logger.LogInformation("SQL generado exitosamente. Complejidad: {ComplexityLevel}, Confianza: {Confidence}%, Tiempo: {ElapsedMs}ms", 
                    response.ComplexityLevel, response.ConfidenceLevel, response.ProcessingTimeMs);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                response.IsSuccess = false;
                response.Message = "Error al generar la consulta SQL";

                _logger.LogError(ex, "Error al generar SQL para pregunta: {Question}", request.Question);
                return response;
            }
        }

        private SQLAnalysisResult AnalyzeGeneratedSQL(string sql)
        {
            var result = new SQLAnalysisResult();

            // Determinar tipo de operación
            result.OperationType = sql.TrimStart().ToUpper().StartsWith("SELECT") ? "SELECT" : "OTHER";

            // Extraer tablas
            var tableMatches = Regex.Matches(sql, @"FROM\s+(\w+)|JOIN\s+(\w+)", RegexOptions.IgnoreCase);
            foreach (Match match in tableMatches)
            {
                var tableName = match.Groups[1].Value ?? match.Groups[2].Value;
                if (!string.IsNullOrEmpty(tableName) && !result.Tables.Contains(tableName))
                {
                    result.Tables.Add(tableName);
                }
            }

            // Extraer campos (simplificado)
            var selectMatch = Regex.Match(sql, @"SELECT\s+(.+?)\s+FROM", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (selectMatch.Success)
            {
                var fieldsString = selectMatch.Groups[1].Value;
                var fields = fieldsString.Split(',');
                result.Fields = fields.Select(f => f.Trim()).ToList();
            }

            // Extraer condiciones WHERE (simplificado)
            var whereMatch = Regex.Match(sql, @"WHERE\s+(.+?)(?:\s+ORDER\s+BY|\s+GROUP\s+BY|$)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (whereMatch.Success)
            {
                result.WhereConditions.Add(whereMatch.Groups[1].Value.Trim());
            }

            // Determinar complejidad
            var joinCount = Regex.Matches(sql, @"JOIN", RegexOptions.IgnoreCase).Count;
            var hasSubquery = sql.Contains("(") && sql.Contains("SELECT");
            var hasGroupBy = sql.ToUpper().Contains("GROUP BY");

            if (hasSubquery || joinCount > 2 || hasGroupBy)
                result.ComplexityLevel = "COMPLEX";
            else if (joinCount > 0)
                result.ComplexityLevel = "MEDIUM";
            else
                result.ComplexityLevel = "SIMPLE";

            return result;
        }

        private SQLValidationResult ValidateSQL(string sql)
        {
            var result = new SQLValidationResult();

            // Validaciones básicas
            if (sql.ToUpper().Contains("DELETE") || sql.ToUpper().Contains("DROP") || sql.ToUpper().Contains("TRUNCATE"))
            {
                result.Warnings.Add("Consulta contiene operaciones potencialmente peligrosas");
            }

            if (!sql.ToUpper().Contains("TOP") && sql.ToUpper().Contains("SELECT"))
            {
                result.Warnings.Add("Consulta sin límite de resultados - puede retornar muchos datos");
                result.CanOptimize = true;
            }

            if (Regex.Matches(sql, @"JOIN", RegexOptions.IgnoreCase).Count > 3)
            {
                result.Warnings.Add("Consulta compleja con múltiples JOINs - podría ser lenta");
            }

            return result;
        }

        private string OptimizeSQL(string sql)
        {
            // Optimizaciones básicas
            if (!sql.ToUpper().Contains("TOP") && sql.ToUpper().Contains("SELECT"))
            {
                sql = sql.Replace("SELECT", "SELECT TOP 100");
            }

            return sql;
        }

        private class SQLAnalysisResult
        {
            public List<string> Tables { get; set; } = new List<string>();
            public List<string> Fields { get; set; } = new List<string>();
            public List<string> WhereConditions { get; set; } = new List<string>();
            public string OperationType { get; set; } = "SELECT";
            public string ComplexityLevel { get; set; } = "SIMPLE";
        }

        private class SQLValidationResult
        {
            public List<string> Warnings { get; set; } = new List<string>();
            public bool CanOptimize { get; set; }
        }
    }
}
