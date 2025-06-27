using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application;
using Microsoft.Data.SqlClient;

namespace SpotifyMusicChatBot.API.Application.Command.SaveConversation
{
    public class SaveConversationHandler : IRequestHandler<SaveConversationRequest, SaveConversationResponse>
    {
        private readonly IChatBotRepository _chatRepository;
        private readonly ILogger<SaveConversationHandler> _logger;

        public SaveConversationHandler(IChatBotRepository chatRepository, ILogger<SaveConversationHandler> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }
        
        public async Task<SaveConversationResponse> Handle(SaveConversationRequest request, CancellationToken cancellationToken)
        {
            IAbstractRepository abstractRepository = (IAbstractRepository)_chatRepository;
            (SqlConnection connection, SqlTransaction transaction) = await abstractRepository.InitTransactionAsync(cancellationToken);
            
            using SqlConnection conn = connection;
            using SqlTransaction trans = transaction;
            
            try
            {
                // Inicializar sessionId
                string sessionId = request.SessionId ?? _chatRepository.GenerateSessionId();

                // Guardar la conversación
                bool success = await _chatRepository.SaveConversationAsync(request.UserPrompt, request.AiResponse, sessionId, trans);
                
                if (success)
                {
                    await trans.CommitAsync(cancellationToken);
                    _logger.LogInformation("✅ Conversación guardada exitosamente para sessionId: {SessionId}", sessionId);
                    return new SaveConversationResponse 
                    { 
                        StatusCode = 200,
                        Message = "Conversación guardada exitosamente",
                        SessionId = sessionId
                    };
                }
                else
                {
                    await trans.RollbackAsync(cancellationToken);
                    _logger.LogWarning("⚠️ No se pudo guardar la conversación para sessionId: {SessionId}", sessionId);
                    return new SaveConversationResponse
                    {
                        StatusCode = 400,
                        Message = "Error al guardar la conversación"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error saving conversation: {Message}", ex.Message);
                try
                {
                    await trans.RollbackAsync(cancellationToken);
                    _logger.LogInformation("🔄 Transacción revertida correctamente");
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "❌ Error durante rollback de transacción: {Message}", rollbackEx.Message);
                }

                return new SaveConversationResponse
                {
                    StatusCode = 500,
                    Message = "Error interno del servidor al guardar la conversación"
                };
            }
        }
    }
}
