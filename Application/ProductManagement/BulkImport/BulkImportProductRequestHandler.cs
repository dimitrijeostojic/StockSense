using Application.Abstractions.Services;
using CsvHelper;
using Domain.Abstractions;
using Domain.Core;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using System.Globalization;

namespace Application.ProductManagement.BulkImport;

internal sealed class BulkImportProductRequestHandler(
    ICurrentUserAccessor currentUserAccessor,
    ICategoryRepository categoryRepository,
    ISupplierRepository supplierRepository,
    IUnitOfWork unitOfWork,
    IProductRepository productRepository)
    : IRequestHandler<BulkImportProductRequest, TResult<BulkImportProductResponse>>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly ISupplierRepository _supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

    public async Task<TResult<BulkImportProductResponse>> Handle(BulkImportProductRequest request, CancellationToken cancellationToken)
    {
        var tenantPublicId = _currentUserAccessor.TenantPublicId;

        var existingCategories = await _categoryRepository.GetAllAsync(tenantPublicId, cancellationToken);

        var categoryCache = existingCategories.ToDictionary(c => c.Name, c => c, StringComparer.OrdinalIgnoreCase);

        var existingSuppliers = (await _supplierRepository.GetAllAsync(tenantPublicId, null, null, true, null, null, 1, int.MaxValue, cancellationToken)).Items;

        var supplierCache = existingSuppliers.ToDictionary(s => s.Name, s => s, StringComparer.OrdinalIgnoreCase);

        var errors = new List<ImportRowError>();
        var successCount = 0;
        var rowNumber = 1; // header is row 1

        using var reader = new StreamReader(new MemoryStream(request.FileContent));
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            try
            {
                var record = csv.GetRecord<ProductImportRowDto>();

                if (!categoryCache.TryGetValue(record.CategoryName, out var category))
                {
                    category = Category.Create(record.CategoryName, null, tenantPublicId);
                    await _categoryRepository.AddAsync(category, cancellationToken);
                    categoryCache[record.CategoryName] = category;
                }

                if (!supplierCache.TryGetValue(record.SupplierName, out var supplier))
                {
                    supplier = Supplier.CreateSupplier(record.SupplierName, null, null, null, tenantPublicId);
                    await _supplierRepository.AddAsync(supplier, cancellationToken);
                    supplierCache[record.SupplierName] = supplier;
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var product = Product.CreateProduct(
                    record.Name,
                    record.Description,
                    record.Price,
                    record.MinimumStockQuantity,
                    category.Id,
                    supplier.Id,
                    tenantPublicId);

                await _productRepository.AddAsync(product, cancellationToken);
                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new ImportRowError(rowNumber, ex.Message));
            }
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return TResult<BulkImportProductResponse>.Success(
      new BulkImportProductResponse(successCount, errors.Count, errors));
    }
}
