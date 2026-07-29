using System;
using Foundation;
using ObjCRuntime;

namespace StoreKit2
{
    /// <summary>
    /// PaymentManager is the main class for managing in-app purchases.
    /// It provides methods to request products, purchase products, restore purchases, and check purchase status.
    /// It also provides a delegate for handling purchase events.
    /// </summary>
    // @interface PaymentManager : NSObject
    [BaseType(typeof(NSObject), Name = "_TtC18StoreKit2Framework14PaymentManager")]
    [DisableDefaultCtor]
    interface PaymentManager
    {
        /// <summary>
        /// The shared instance of the PaymentManager.
        /// </summary>
        // @property (readonly, nonatomic, strong, class) PaymentManager * _Nonnull shared;
        [Static]
        [Export("shared", ArgumentSemantic.Strong)]
        PaymentManager Shared { get; }

        /// <summary>
        /// The delegate for handling purchase events.
        /// </summary>
        [Wrap("WeakDelegate")]
        [NullAllowed]
        PaymentManagerDelegate Delegate { get; set; }

        /// <summary>
        /// The delegate for handling purchase events.
        /// </summary>
        // @property (nonatomic, weak) id<PaymentManagerDelegate> _Nullable delegate;
        [NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
        NSObject WeakDelegate { get; set; }

        /// <summary>
        /// Requests products from the App Store.
        /// </summary>
        /// <param name="productIds">The product IDs to request.</param>
        /// <param name="completion">The completion handler to call when the request is complete.</param>
        // -(void)requestProductsWithProductIds:(NSArray<NSString *> * _Nonnull)productIds completion:(void (^ _Nonnull)(BOOL, NSString * _Nullable))completion;
        [Export("requestProductsWithProductIds:completion:")]
        void RequestProductsWithProductIds(string[] productIds, Action<bool, NSString> completion);

        /// <summary>
        /// Purchases a product from the App Store.
        /// </summary>
        /// <param name="productId">The product ID to purchase.</param>
        /// <param name="appAccountToken">The app account token to use for the purchase.</param>
        /// <param name="completion">The completion handler to call when the purchase is complete.</param>
        // -(void)purchaseProductWithProductId:(NSString * _Nonnull)productId appAccountToken:(NSUUID * _Nullable)appAccountToken completion:(void (^ _Nonnull)(BOOL, NSString * _Nullable))completion;
        [Export("purchaseProductWithProductId:appAccountToken:completion:")]
        void PurchaseProductWithProductId(string productId, [NullAllowed] NSUuid appAccountToken, Action<bool, NSString> completion);

        /// <summary>
        /// Restores purchases from the App Store.
        /// </summary>
        /// <param name="completion">The completion handler to call when the restore is complete.</param>
        // -(void)restorePurchasesWithCompletion:(void (^ _Nonnull)(BOOL, NSString * _Nullable))completion;
        [Export("restorePurchasesWithCompletion:")]
        void RestorePurchasesWithCompletion(Action<bool, NSString> completion);

        /// <summary>
        /// Gets a product requested.
        /// </summary>
        /// <param name="productId">The product ID to get.</param>
        /// <returns>The product.</returns>
        // -(PaymentProduct * _Nullable)getProductWithProductId:(NSString * _Nonnull)productId __attribute__((warn_unused_result("")));
        [Export("getProductWithProductId:")]
        [return: NullAllowed]
        PaymentProduct GetProductWithProductId(string productId);

        /// <summary>
        /// Gets all products requested.
        /// </summary>
        /// <returns>The products.</returns>
        // -(NSArray<PaymentProduct *> * _Nonnull)getAllProducts __attribute__((warn_unused_result("")));
        [Export("getAllProducts")]
        PaymentProduct[] AllProducts { get; }

        /// <summary>
        /// Checks the purchase status of a product.
        /// </summary>
        /// <param name="productId">The product ID to check.</param>
        /// <param name="completion">The completion handler to call when the check is complete.</param>
        // -(void)checkPurchaseStatusWithProductId:(NSString * _Nonnull)productId completion:(void (^ _Nonnull)(BOOL, PaymentTransaction * _Nullable))completion;
        [Export("checkPurchaseStatusWithProductId:completion:")]
        void CheckPurchaseStatusWithProductId(string productId, Action<bool, PaymentTransaction> completion);

        /// <summary>
        /// Purchases a product and reports a structured result instead of a (bool, message) pair.
        /// </summary>
        /// <param name="productId">The product ID to purchase.</param>
        /// <param name="appAccountToken">The app account token to use for the purchase.</param>
        /// <param name="completion">The completion handler receiving the structured result.</param>
        // -(void)purchaseProductWithResultWithProductId:(NSString * _Nonnull)productId appAccountToken:(NSUUID * _Nullable)appAccountToken completion:(void (^ _Nonnull)(PaymentPurchaseResult * _Nonnull))completion;
        [Export("purchaseProductWithResultWithProductId:appAccountToken:completion:")]
        void PurchaseProductWithResult(string productId, [NullAllowed] NSUuid appAccountToken, Action<PaymentPurchaseResult> completion);

        /// <summary>
        /// Gets the statuses of the subscription group the product belongs to — one entry per
        /// subscription the account holds in the group (family sharing can add more). Empty when
        /// the account never subscribed. The product must have been loaded via RequestProducts first.
        /// </summary>
        /// <param name="productId">A subscription product ID in the group.</param>
        /// <param name="completion">The completion handler receiving the statuses and an error message.</param>
        // -(void)getSubscriptionStatusWithProductId:(NSString * _Nonnull)productId completion:(void (^ _Nonnull)(NSArray<PaymentSubscriptionStatus *> * _Nonnull, NSString * _Nullable))completion;
        [Export("getSubscriptionStatusWithProductId:completion:")]
        void GetSubscriptionStatusWithProductId(string productId, Action<PaymentSubscriptionStatus[], NSString> completion);

        /// <summary>
        /// Checks per-account introductory-offer eligibility for the product's subscription group.
        /// </summary>
        /// <param name="productId">A subscription product ID in the group.</param>
        /// <param name="completion">The completion handler receiving eligibility and an error message.</param>
        // -(void)checkIntroOfferEligibilityWithProductId:(NSString * _Nonnull)productId completion:(void (^ _Nonnull)(BOOL, NSString * _Nullable))completion;
        [Export("checkIntroOfferEligibilityWithProductId:completion:")]
        void CheckIntroOfferEligibilityWithProductId(string productId, Action<bool, NSString> completion);

        /// <summary>
        /// Gets the App Store environment for this install: "Production", "Sandbox", or "Xcode".
        /// Returns "unknown" plus a message below iOS 16 or when the signed AppTransaction fails
        /// verification — fall back to another heuristic rather than trusting an unverified value.
        /// </summary>
        /// <param name="completion">The completion handler receiving the environment and an error message.</param>
        // -(void)getAppEnvironmentWithCompletion:(void (^ _Nonnull)(NSString * _Nonnull, NSString * _Nullable))completion;
        [Export("getAppEnvironmentWithCompletion:")]
        void GetAppEnvironment(Action<NSString, NSString> completion);

        /// <summary>
        /// Gets one atomic snapshot of every verified current entitlement.
        /// </summary>
        /// <param name="completion">The completion handler receiving the transactions.</param>
        // -(void)getCurrentEntitlementsWithCompletion:(void (^ _Nonnull)(NSArray<PaymentTransaction *> * _Nonnull))completion;
        [Export("getCurrentEntitlementsWithCompletion:")]
        void GetCurrentEntitlements(Action<PaymentTransaction[]> completion);
    }

    /// <summary>
    /// PaymentManagerDelegate is the delegate for handling purchase events.
    /// It provides methods to handle purchase events.
    /// </summary>
    // @protocol PaymentManagerDelegate
    [Protocol(Name = "_TtP18StoreKit2Framework22PaymentManagerDelegate_"), Model]
    [BaseType(typeof(NSObject))]
    interface PaymentManagerDelegate
    {
        /// <summary>
        /// Called when a purchase is finished.
        /// </summary>
        /// <param name="productId">The product ID of the purchased product.</param>
        /// <param name="transaction">The transaction of the purchased product.</param>
        // @optional -(void)paymentManagerDidFinishPurchase:(NSString * _Nonnull)productId transaction:(PaymentTransaction * _Nonnull)transaction;
        [Export("paymentManagerDidFinishPurchase:transaction:")]
        void PaymentManagerDidFinishPurchase(string productId, PaymentTransaction transaction);

        /// <summary>
        /// Called when a purchase fails.
        /// </summary>
        /// <param name="productId">The product ID of the failed product.</param>
        /// <param name="error">The error message.</param>
        // @optional -(void)paymentManagerDidFailPurchase:(NSString * _Nonnull)productId error:(NSString * _Nonnull)error;
        [Export("paymentManagerDidFailPurchase:error:")]
        void PaymentManagerDidFailPurchase(string productId, string error);

        /// <summary>
        /// Called when the products are updated.
        /// </summary>
        /// <param name="products">The products.</param>
        // @optional -(void)paymentManagerDidUpdateProducts:(NSArray<PaymentProduct *> * _Nonnull)products;
        [Export("paymentManagerDidUpdateProducts:")]
        void PaymentManagerDidUpdateProducts(PaymentProduct[] products);

        /// <summary>
        /// Called when the purchases are restored.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        // @optional -(void)paymentManagerDidRestorePurchases:(NSArray<PaymentTransaction *> * _Nonnull)transactions;
        [Export("paymentManagerDidRestorePurchases:")]
        void PaymentManagerDidRestorePurchases(PaymentTransaction[] transactions);

        /// <summary>
        /// Called for every Transaction.updates event — renewals, Ask to Buy approvals, plan
        /// changes landing, refunds/revocations — the store's push channel, independent of any
        /// purchase call this process made.
        /// </summary>
        /// <param name="transaction">The updated transaction.</param>
        // @optional -(void)paymentManagerDidReceiveTransactionUpdate:(PaymentTransaction * _Nonnull)transaction;
        [Export("paymentManagerDidReceiveTransactionUpdate:")]
        void PaymentManagerDidReceiveTransactionUpdate(PaymentTransaction transaction);
    }

    /// <summary>
    /// PaymentProduct is the class for representing a product.
    /// It provides information about the product.
    /// </summary>
    // @interface PaymentProduct : NSObject
    [BaseType(typeof(NSObject), Name = "_TtC18StoreKit2Framework14PaymentProduct")]
    [DisableDefaultCtor]
    interface PaymentProduct
    {
        /// <summary>
        /// The product ID.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nonnull productId;
        [Export("productId")]
        string ProductId { get; }

        /// <summary>
        /// The display name of the product.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nonnull displayName;
        [Export("displayName")]
        string DisplayName { get; }

        /// <summary>
        /// The description of the product.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nonnull productDescription;
        [Export("productDescription")]
        string ProductDescription { get; }

        /// <summary>
        /// The price of the product.
        /// </summary>
        // @property (readonly, nonatomic, strong) NSDecimalNumber * _Nonnull price;
        [Export("price", ArgumentSemantic.Strong)]
        NSDecimalNumber Price { get; }

        /// <summary>
        /// The display price of the product.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nonnull displayPrice;
        [Export("displayPrice")]
        string DisplayPrice { get; }

        /// <summary>
        /// The type of the product.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nonnull productType;
        [Export("productType")]
        string ProductType { get; }

        /// <summary>
        /// The subscription group ID; null for non-subscription products.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nullable subscriptionGroupId;
        [NullAllowed, Export("subscriptionGroupId")]
        string SubscriptionGroupId { get; }

        /// <summary>
        /// The subscription period unit ("day", "week", "month", "year"); null for non-subscriptions.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nullable subscriptionPeriodUnit;
        [NullAllowed, Export("subscriptionPeriodUnit")]
        string SubscriptionPeriodUnit { get; }

        /// <summary>
        /// The subscription period length in units; 0 for non-subscriptions.
        /// </summary>
        // @property (readonly, nonatomic) NSInteger subscriptionPeriodValue;
        [Export("subscriptionPeriodValue")]
        nint SubscriptionPeriodValue { get; }

        /// <summary>
        /// The introductory offer payment mode ("freeTrial", "payAsYouGo", "payUpFront");
        /// null when the product has no intro offer. Eligibility is per-account — see
        /// CheckIntroOfferEligibilityWithProductId.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nullable introOfferPaymentMode;
        [NullAllowed, Export("introOfferPaymentMode")]
        string IntroOfferPaymentMode { get; }

        /// <summary>
        /// The introductory offer price; null when the product has no intro offer.
        /// </summary>
        // @property (readonly, nonatomic, strong) NSDecimalNumber * _Nullable introOfferPrice;
        [NullAllowed, Export("introOfferPrice", ArgumentSemantic.Strong)]
        NSDecimalNumber IntroOfferPrice { get; }

        /// <summary>
        /// The localized introductory offer price; null when the product has no intro offer.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nullable introOfferDisplayPrice;
        [NullAllowed, Export("introOfferDisplayPrice")]
        string IntroOfferDisplayPrice { get; }

        /// <summary>
        /// The introductory offer period unit ("day", "week", "month", "year"); null when none.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nullable introOfferPeriodUnit;
        [NullAllowed, Export("introOfferPeriodUnit")]
        string IntroOfferPeriodUnit { get; }

        /// <summary>
        /// The introductory offer period length in units; 0 when none.
        /// </summary>
        // @property (readonly, nonatomic) NSInteger introOfferPeriodValue;
        [Export("introOfferPeriodValue")]
        nint IntroOfferPeriodValue { get; }

        /// <summary>
        /// How many periods the introductory offer repeats for; 0 when none.
        /// </summary>
        // @property (readonly, nonatomic) NSInteger introOfferPeriodCount;
        [Export("introOfferPeriodCount")]
        nint IntroOfferPeriodCount { get; }
    }

    /// <summary>
    /// PaymentTransaction is the class for representing a transaction.
    /// It provides information about the transaction.
    /// </summary>
    // @interface PaymentTransaction : NSObject
    [BaseType(typeof(NSObject), Name = "_TtC18StoreKit2Framework18PaymentTransaction")]
    [DisableDefaultCtor]
    interface PaymentTransaction
    {
        /// <summary>
        /// The transaction ID.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nonnull transactionId;
        [Export("transactionId")]
        string TransactionId { get; }

        /// <summary>
        /// The product ID.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nonnull productId;
        [Export("productId")]
        string ProductId { get; }

        /// <summary>
        /// The purchase date.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSDate * _Nonnull purchaseDate;
        [Export("purchaseDate", ArgumentSemantic.Copy)]
        NSDate PurchaseDate { get; }

        /// <summary>
        /// Whether the transaction is upgraded.
        /// </summary>
        // @property (readonly, nonatomic) BOOL isUpgraded;
        [Export("isUpgraded")]
        bool IsUpgraded { get; }

        /// <summary>
        /// The revocation date.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSDate * _Nullable revocationDate;
        [NullAllowed, Export("revocationDate", ArgumentSemantic.Copy)]
        NSDate RevocationDate { get; }

        /// <summary>
        /// The revocation reason.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nullable revocationReason;
        [NullAllowed, Export("revocationReason")]
        string RevocationReason { get; }

        /// <summary>
        /// The end of the current subscription period; null for non-subscriptions.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSDate * _Nullable expirationDate;
        [NullAllowed, Export("expirationDate", ArgumentSemantic.Copy)]
        NSDate ExpirationDate { get; }

        /// <summary>
        /// The transaction's App Store environment: "Production", "Sandbox", or "Xcode".
        /// "unknown" below iOS 16 where Transaction.environment is unavailable.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nonnull environment;
        [Export("environment")]
        string Environment { get; }
    }

    /// <summary>
    /// PaymentSubscriptionStatus is one auto-renewable subscription's status within its group:
    /// the renewal state plus the signed renewal info (what renews next, when, and for how much
    /// the store will bill), and the latest transaction for the period.
    /// </summary>
    // @interface PaymentSubscriptionStatus : NSObject
    [BaseType(typeof(NSObject), Name = "_TtC18StoreKit2Framework25PaymentSubscriptionStatus")]
    [DisableDefaultCtor]
    interface PaymentSubscriptionStatus
    {
        /// <summary>
        /// The renewal state: "subscribed", "expired", "inBillingRetryPeriod", "inGracePeriod",
        /// or "revoked".
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nonnull state;
        [Export("state")]
        string State { get; }

        /// <summary>
        /// The product currently entitling this subscription group; null when renewal info was
        /// unverified.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nullable currentProductId;
        [NullAllowed, Export("currentProductId")]
        string CurrentProductId { get; }

        /// <summary>
        /// Whether the subscription will auto-renew at the end of the period.
        /// </summary>
        // @property (readonly, nonatomic) BOOL willAutoRenew;
        [Export("willAutoRenew")]
        bool WillAutoRenew { get; }

        /// <summary>
        /// The product that renews at the next billing cycle when the user has queued a plan
        /// change; equals CurrentProductId otherwise. Null if renewal info was unverified or
        /// auto-renew is off.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nullable autoRenewPreference;
        [NullAllowed, Export("autoRenewPreference")]
        string AutoRenewPreference { get; }

        /// <summary>
        /// The next renewal date.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSDate * _Nullable renewalDate;
        [NullAllowed, Export("renewalDate", ArgumentSemantic.Copy)]
        NSDate RenewalDate { get; }

        /// <summary>
        /// Why the subscription expired ("autoRenewDisabled", "billingError",
        /// "didNotConsentToPriceIncrease", "productUnavailable"); only set once expired.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nullable expirationReason;
        [NullAllowed, Export("expirationReason")]
        string ExpirationReason { get; }

        /// <summary>
        /// When grace-period access ends; only set while in the grace period.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSDate * _Nullable gracePeriodExpirationDate;
        [NullAllowed, Export("gracePeriodExpirationDate", ArgumentSemantic.Copy)]
        NSDate GracePeriodExpirationDate { get; }

        /// <summary>
        /// The latest verified transaction for this subscription; carries ExpirationDate.
        /// </summary>
        // @property (readonly, nonatomic, strong) PaymentTransaction * _Nullable transaction;
        [NullAllowed, Export("transaction", ArgumentSemantic.Strong)]
        PaymentTransaction Transaction { get; }
    }

    /// <summary>
    /// PaymentPurchaseResult is the structured outcome of a purchase made through
    /// PurchaseProductWithResult.
    /// </summary>
    // @interface PaymentPurchaseResult : NSObject
    [BaseType(typeof(NSObject), Name = "_TtC18StoreKit2Framework21PaymentPurchaseResult")]
    [DisableDefaultCtor]
    interface PaymentPurchaseResult
    {
        /// <summary>
        /// The result code.
        /// </summary>
        // @property (readonly, nonatomic) enum PaymentPurchaseResultCode code;
        [Export("code")]
        PaymentPurchaseResultCode Code { get; }

        /// <summary>
        /// The verified transaction on success; null otherwise.
        /// </summary>
        // @property (readonly, nonatomic, strong) PaymentTransaction * _Nullable transaction;
        [NullAllowed, Export("transaction", ArgumentSemantic.Strong)]
        PaymentTransaction Transaction { get; }

        /// <summary>
        /// The error message for unverified or failed outcomes; null otherwise.
        /// </summary>
        // @property (readonly, copy, nonatomic) NSString * _Nullable errorMessage;
        [NullAllowed, Export("errorMessage")]
        string ErrorMessage { get; }
    }
}
