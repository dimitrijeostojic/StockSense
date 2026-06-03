using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.OrderManagement.DeleteOrder;

internal sealed class DeleteOrderRequestHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserAccessor currentUserAccessor)
    : IRequestHandler<DeleteOrderRequest, Result>
{
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    public async Task<Result> Handle(DeleteOrderRequest request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByPublicIdAsync(request.OrderPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (order == null)
        {
            return Result.Failure(ApplicationErrors.NotFound);
        }
        _orderRepository.Delete(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}