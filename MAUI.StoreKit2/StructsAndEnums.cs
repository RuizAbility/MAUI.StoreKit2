using ObjCRuntime;

namespace StoreKit2 {

    /// <summary>
    /// Outcome of a purchase made through PurchaseProductWithResult.
    /// </summary>
    [Native]
    public enum PaymentPurchaseResultCode : long
    {
        Success = 0,
        UserCancelled = 1,
        Pending = 2,
        Unverified = 3,
        ProductNotFound = 4,
        Failed = 5,
    }
}
