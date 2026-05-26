using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

internal sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork
{

}
