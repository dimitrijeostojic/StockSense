using System.Data;

namespace Domain.Abstractions;

public interface IAuthUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    IDbTransaction BeginTransaction();
}
