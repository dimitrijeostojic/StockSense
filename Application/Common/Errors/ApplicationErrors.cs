using Domain.Core;

namespace Application.Common.Errors;

public static class ApplicationErrors
{
    public static readonly Error NotFound = new("ApplicationErrors.NotFound", "The requested resource was not found.");
    public static readonly Error InvalidCredentials = new("ApplicationErrors.InvalidCredentials", "Invalid email or password.");
    public static readonly Error EmailAlreadyExists = new("ApplicationErrors.EmailAlreadyExists", "A user with this email already exists.");
    public static readonly Error RegistrationFailed = new("ApplicationErrors.RegistrationFailed", "User registration failed.");
    public static readonly Error InvalidRefreshToken = new("ApplicationErrors.InvalidRefreshToken", "Refresh token is invalid or has expired.");
    public static readonly Error InvalidTwoFactorCode = new("ApplicationErrors.InvalidTwoFactorCode", "The two-factor authentication code is invalid.");
    public static readonly Error TwoFactorNotEnabled = new("ApplicationErrors.TwoFactorNotEnabled", "Two-factor authentication is not enabled for this user.");
    public static readonly Error UnauthorizedRole = new("ApplicationErrors.UnauthorizedRole", "Access is restricted to administrators and managers only.");
    public static readonly Error DeleteFailure = new("ApplicationErrors.DeleteFailure", "Failed to delete user.");
    public static readonly Error PIBAlreadyExists = new("ApplicationErrors.PIBAlreadyExists", "A tenant with this PIB already exists.");
    public static readonly Error InvalidOrderStatusTransition = new("ApplicationErrors.InvalidOrderStatusTransition", "The requested order status transition is invalid.");
    public static readonly Error UserLockedOut = new("ApplicationErrors.UserLockedOut", "This account has been locked out.");
    public static readonly Error CannotDeleteAdminUser = new("ApplicationErrors.CannotDeleteAdminUser", "Cannot delete an admin user.");
    public static readonly Error InvalidStatus = new("ApplicationErrors.InvalidStatus", "Only pending orders can be edited.");
    public static readonly Error RequestAlreadyProccessed = new("ApplicationErrors.RequestAlreadyProccessed", "Request has already processed");
    public static readonly Error DuplicateSku = new Error("ApplicationErrors.DuplicateSku", "Product with this SKU already exists.");
    public static readonly Error CategoryHasProducts = new Error("ApplicationErrors.CategoryHasProducts", "Cannot delete category because it has associated products.");
    public static readonly Error SupplierHasProducts = new Error("ApplicationErrors.SupplierHasProducts", "Cannot delete supplier because it has associated products.");
    public static readonly Error SupplierHasOrders = new Error("ApplicationErrors.SupplierHasOrders", "Cannot delete supplier because it has associated orders.");
    public static readonly Error CannotDeleteNonCancelledOrder = new Error("ApplicationErrors.CannotDeleteNonCancelledOrder", "Only cancelled orders can be deleted.");
    public static readonly Error InvalidPageName = new Error("ApplicationErrors.InvalidPageName", "The specified page name is not valid.");
    public static readonly Error OrderNotConfirmed = new Error("ApplicationErrors.OrderNotConfirmed", "Goods receipt can only be created for confirmed orders.");
    public static readonly Error GoodsReceiptAlreadyExists = new Error("ApplicationErrors.GoodsReceiptAlreadyExists", "A goods receipt already exists for this order.");
    public static readonly Error GoodsReceiptRequired = new Error("ApplicationErrors.GoodsReceiptRequired", "A goods receipt must be created before marking the order as received.");
    public static readonly Error ReceivedQuantityExceedsOrdered = new Error("ApplicationErrors.ReceivedQuantityExceedsOrdered", "Received quantity cannot exceed the ordered quantity.");
}
