using Microsoft.AspNetCore.Mvc;
using SpotifyMusicChatBot.API.Application.Command.ExcludedTerms;
using SpotifyMusicChatBot.API.Application.Query.GetExcludedTerms;
using SpotifyMusicChatBot.Domain.Application.Repository;

namespace SpotifyMusicChatBot.API.Application.Controller
{
    /// <summary>
    /// Controlador para gestión de términos excluidos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExcludedTermsController : ControllerBase
    {
        private readonly IChatBotRepository _repository;
        private readonly ILogger<ExcludedTermsController> _logger;

        public ExcludedTermsController(IChatBotRepository repository, ILogger<ExcludedTermsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Crea un nuevo término excluido para un usuario
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateExcludedTerm([FromBody] CreateExcludedTermCommand command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command.FirebaseUserId))
                {
                    return BadRequest("FirebaseUserId es requerido");
                }

                if (string.IsNullOrWhiteSpace(command.Term))
                {
                    return BadRequest("Term es requerido");
                }

                if (string.IsNullOrWhiteSpace(command.Category))
                {
                    return BadRequest("Category es requerido");
                }

                var request = command.ToRequest();
                var result = await _repository.CreateExcludedTermAsync(request);

                if (result)
                {
                    _logger.LogInformation("✅ Término excluido creado exitosamente para usuario {UserId}: {Term} ({Category})", 
                        command.FirebaseUserId, command.Term, command.Category);
                    return Ok(new { Success = true, Message = "Término excluido creado exitosamente" });
                }
                else
                {
                    _logger.LogWarning("⚠️ No se pudo crear el término excluido para usuario {UserId}: {Term} ({Category})", 
                        command.FirebaseUserId, command.Term, command.Category);
                    return Conflict(new { Success = false, Message = "El término ya existe o no se pudo crear" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creando término excluido para usuario {UserId}", command.FirebaseUserId);
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene todos los términos excluidos de un usuario
        /// </summary>
        [HttpGet("{firebaseUserId}")]
        public async Task<IActionResult> GetExcludedTerms(string firebaseUserId, [FromQuery] string? category = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(firebaseUserId))
                {
                    return BadRequest("FirebaseUserId es requerido");
                }

                var terms = string.IsNullOrWhiteSpace(category) 
                    ? await _repository.GetExcludedTermsByUserAsync(firebaseUserId)
                    : await _repository.GetExcludedTermsByCategoryAsync(firebaseUserId, category);

                _logger.LogInformation("✅ Términos excluidos obtenidos para usuario {UserId}: {Count} términos", 
                    firebaseUserId, terms.Count);

                return Ok(new { Success = true, Data = terms, Count = terms.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error obteniendo términos excluidos para usuario {UserId}", firebaseUserId);
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualiza un término excluido
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExcludedTerm(int id, [FromBody] UpdateExcludedTermCommand command)
        {
            try
            {
                if (id != command.Id)
                {
                    return BadRequest("El ID en la URL no coincide con el ID del comando");
                }

                if (string.IsNullOrWhiteSpace(command.FirebaseUserId))
                {
                    return BadRequest("FirebaseUserId es requerido");
                }

                if (string.IsNullOrWhiteSpace(command.Term))
                {
                    return BadRequest("Term es requerido");
                }

                if (string.IsNullOrWhiteSpace(command.Category))
                {
                    return BadRequest("Category es requerido");
                }

                var request = command.ToRequest();
                var result = await _repository.UpdateExcludedTermAsync(request);

                if (result)
                {
                    _logger.LogInformation("✅ Término excluido actualizado exitosamente: ID {Id} para usuario {UserId}", 
                        id, command.FirebaseUserId);
                    return Ok(new { Success = true, Message = "Término excluido actualizado exitosamente" });
                }
                else
                {
                    _logger.LogWarning("⚠️ No se pudo actualizar el término excluido: ID {Id} para usuario {UserId}", 
                        id, command.FirebaseUserId);
                    return NotFound(new { Success = false, Message = "Término excluido no encontrado" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error actualizando término excluido: ID {Id} para usuario {UserId}", 
                    id, command.FirebaseUserId);
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Elimina (desactiva) un término excluido
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExcludedTerm(int id, [FromQuery] string firebaseUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(firebaseUserId))
                {
                    return BadRequest("FirebaseUserId es requerido");
                }

                var result = await _repository.DeleteExcludedTermAsync(id, firebaseUserId);

                if (result)
                {
                    _logger.LogInformation("✅ Término excluido eliminado exitosamente: ID {Id} para usuario {UserId}", 
                        id, firebaseUserId);
                    return Ok(new { Success = true, Message = "Término excluido eliminado exitosamente" });
                }
                else
                {
                    _logger.LogWarning("⚠️ No se pudo eliminar el término excluido: ID {Id} para usuario {UserId}", 
                        id, firebaseUserId);
                    return NotFound(new { Success = false, Message = "Término excluido no encontrado" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error eliminando término excluido: ID {Id} para usuario {UserId}", 
                    id, firebaseUserId);
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Verifica si un término existe para un usuario
        /// </summary>
        [HttpGet("exists")]
        public async Task<IActionResult> CheckTermExists([FromQuery] string firebaseUserId, [FromQuery] string term, [FromQuery] string category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(firebaseUserId) || string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(category))
                {
                    return BadRequest("FirebaseUserId, Term y Category son requeridos");
                }

                var exists = await _repository.ExcludedTermExistsAsync(term, category, firebaseUserId);

                return Ok(new { Success = true, Exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error verificando existencia de término para usuario {UserId}", firebaseUserId);
                return StatusCode(500, new { Success = false, Message = "Error interno del servidor" });
            }
        }
    }
}
