using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.API.Application.Mappers;
using Microsoft.Data.SqlClient;
using SpotifyMusicChatBot.Infra.Application.Repository;

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
            var concreteRepo = (ChatIARepository)_chatRepository;
            (SqlConnection connection, SqlTransaction transaction) = await concreteRepo.InitTransactionAsync(cancellationToken);
            await using (connection)
            await using (transaction)
            {
                try
                {
                    // Inicializar sessionId
                    string sessionId = request.SessionId ?? _chatRepository.GenerateSessionId();

                    // Guardar la conversación
                    bool success = await _chatRepository.SaveConversationAsync(request.UserPrompt, request.AiResponse, sessionId, transaction);

                    if (success)
                    {
                        await transaction.CommitAsync(cancellationToken);
                        _logger.LogInformation("✅ Conversación guardada exitosamente para sessionId: {SessionId}", sessionId);
                        return SaveConversationMapper.ToSuccessResponse(sessionId);
                    }
                    else
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        _logger.LogWarning("⚠️ No se pudo guardar la conversación para sessionId: {SessionId}", sessionId);
                        return SaveConversationMapper.ToErrorResponse(400, "Error al guardar la conversación");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error saving conversation: {Message}", ex.Message);
                    try
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        _logger.LogInformation("🔄 Transacción revertida correctamente");
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(rollbackEx, "❌ Error durante rollback de transacción: {Message}", rollbackEx.Message);
                    }

                    return SaveConversationMapper.ToErrorResponse(500, "Error interno del servidor al guardar la conversación");
                }
            }
        }
    }
}
