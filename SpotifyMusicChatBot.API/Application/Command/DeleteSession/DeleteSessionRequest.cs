using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SpotifyMusicChatBot.API.Application.Command.DeleteSession
{
    public class DeleteSessionRequest : IRequest<DeleteSessionResponse>
    {
        [FromRoute]
        [Required(ErrorMessage = "El ID de sesión es requerido")]
        [StringLength(50, ErrorMessage = "El ID de sesión no puede exceder 50 caracteres")]
        public string SessionId { get; set; } = string.Empty;
    }
}
