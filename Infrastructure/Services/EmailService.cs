using Application.Abstractions.Services;
using Domain.Abstractions;
using Domain.Dtos;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Services;

public sealed class EmailService(
    ApplicationDbContext applicationDbContext,
    IUnitOfWork unitOfWork) : IEmailService
{
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task SendAsync(EmailMessageDto emailMessageDto, CancellationToken cancellationToken)
    {
        var message = OutboxEmailMessage.Create(emailMessageDto.To, emailMessageDto.Subject, emailMessageDto.Body);
        await _applicationDbContext.OutboxEmailMessages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
    }
}
