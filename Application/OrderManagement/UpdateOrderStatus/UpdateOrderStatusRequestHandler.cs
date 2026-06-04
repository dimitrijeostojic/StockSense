using Application.Common.Errors;
using Application.Common.Interfaces;
using Domain.Abstractions;
using Domain.Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.OrderManagement.UpdateOrderStatus;

internal sealed class UpdateOrderStatusRequestHandler(
    IOrderRepository orderRepository,
    ICurrentUserAccessor currentUserAccessor,
    IUnitOfWork unitOfWork,
    IMediator mediator)
    : IRequestHandler<UpdateOrderStatusRequest, TResult<UpdateOrderStatusResponse>>
{
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task<TResult<UpdateOrderStatusResponse>> Handle(UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByPublicIdAsync(request.OrderPublicId, _currentUserAccessor.TenantPublicId, cancellationToken);
        if (order is null)
        {
            return TResult<UpdateOrderStatusResponse>.Failure(ApplicationErrors.NotFound);
        }

        var orderResult = order.WithOrderStatus(request.Status);
        if (!orderResult.IsSuccess)
        {
            return TResult<UpdateOrderStatusResponse>.Failure(ApplicationErrors.InvalidOrderStatusTransition);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in order.DomainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        order.ClearDomainEvents();

        return TResult<UpdateOrderStatusResponse>.Success(new UpdateOrderStatusResponse(order.PublicId, order.OrderStatus));
    }
}
