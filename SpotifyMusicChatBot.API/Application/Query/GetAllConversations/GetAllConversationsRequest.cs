using MediatR;
using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.Query.GetAllConversations
{
    public class GetAllConversationsRequest : IRequest<GetAllConversationsResponse>
    {
        /// <summary>
        /// ID del usuario de Firebase para filtrar las conversaciones
        /// </summary>
        [Required(ErrorMessage = "El ID de usuario de Firebase es requerido")]
        [StringLength(100, ErrorMessage = "El ID de usuario de Firebase no puede exceder 100 caracteres")]
        public string FirebaseUserId { get; set; } = string.Empty;
    }
}
