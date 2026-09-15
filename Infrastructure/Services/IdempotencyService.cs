using Application.Abstractions.Idempotency;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public sealed class IdempotencyService(ApplicationDbContext applicationDbContext) : IIdempotencyService
{
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

    public async Task CreateRequestAsync(Guid requestId, string name, CancellationToken cancellationToken = default)
    {
        var ir = IdempotentRequest.Create(requestId, name, DateTime.UtcNow);
        await _applicationDbContext.IdempotentRequests.AddAsync(ir, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RequestExistsAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _applicationDbContext.IdempotentRequests.AnyAsync(ir => ir.RequestId == requestId, cancellationToken);
    }
}
