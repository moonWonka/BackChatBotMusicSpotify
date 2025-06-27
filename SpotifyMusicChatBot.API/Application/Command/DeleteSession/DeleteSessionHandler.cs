using MediatR;
using SpotifyMusicChatBot.Domain.Application.Repository;
using SpotifyMusicChatBot.Domain.Application;
using SpotifyMusicChatBot.API.Application.Mappers;
using Microsoft.Data.SqlClient;

namespace SpotifyMusicChatBot.API.Application.Command.DeleteSession
{
    public class DeleteSessionHandler : IRequestHandler<DeleteSessionRequest, DeleteSessionResponse>
    {
        private readonly IChatBotRepository _chatRepository;
        private readonly ILogger<DeleteSessionHandler> _logger;

        public DeleteSessionHandler(IChatBotRepository chatRepository, ILogger<DeleteSessionHandler> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }

        public async Task<DeleteSessionResponse> Handle(DeleteSessionRequest request, CancellationToken cancellationToken)
        {
            IAbstractRepository abstractRepository = (IAbstractRepository)_chatRepository;
            (SqlConnection connection, SqlTransaction transaction) = await abstractRepository.InitTransactionAsync(cancellationToken);
            
            using SqlConnection conn = connection;
            using SqlTransaction trans = transaction;
            try
            {
                bool deleted = await _chatRepository.DeleteSessionAsync(request.SessionId, trans);
                
                if (deleted)
                {
                    await trans.CommitAsync(cancellationToken);
                    _logger.LogInformation("✅ Sesión eliminada exitosamente: {SessionId}", request.SessionId);
                    return DeleteSessionMapper.ToSuccessResponse(request.SessionId);
                }
                else
                {
                    await trans.RollbackAsync(cancellationToken);
                    _logger.LogWarning("⚠️ Sesión no encontrada: {SessionId}", request.SessionId);
                    return DeleteSessionMapper.ToNotFoundResponse(request.SessionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error deleting session ID: {SessionId}, Message: {Message}", request.SessionId, ex.Message);
                try
                {
                    await trans.RollbackAsync(cancellationToken);
                    _logger.LogInformation("🔄 Transacción revertida correctamente");
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "❌ Error durante rollback de transacción: {Message}", rollbackEx.Message);
                }

                return DeleteSessionMapper.ToErrorResponse(500, "Error interno del servidor");
            }
        }
    }
}
