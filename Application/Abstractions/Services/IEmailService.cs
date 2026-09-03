
using Domain.Dtos;

namespace Application.Abstractions.Services;

public interface IEmailService
{
    Task SendAsync(EmailMessageDto emailMessageDto, CancellationToken cancellationToken);

}
