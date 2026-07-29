//
//  StoreKit2Framework.swift
//  StoreKit2Framework
//
//  Created by Yuting Li on 2025/5/28.
//

import Foundation
import StoreKit

@available(iOS 15.0, *)
@objc public protocol PaymentManagerDelegate: AnyObject {
    @objc optional func paymentManagerDidFinishPurchase(_ productId: String, transaction: PaymentTransaction)
    @objc optional func paymentManagerDidFailPurchase(_ productId: String, error: String)
    @objc optional func paymentManagerDidUpdateProducts(_ products: [PaymentProduct])
    @objc optional func paymentManagerDidRestorePurchases(_ transactions: [PaymentTransaction])

    // Fired for every Transaction.updates event — renewals, Ask to Buy
    // approvals, plan changes landing, refunds/revocations — the store's push
    // channel, independent of any purchase call this process made.
    @objc optional func paymentManagerDidReceiveTransactionUpdate(_ transaction: PaymentTransaction)
}

@available(iOS 15.0, *)
@objc public class PaymentProduct: NSObject {
    @objc public let productId: String
    @objc public let displayName: String
    @objc public let productDescription: String
    @objc public let price: NSDecimalNumber
    @objc public let displayPrice: String
    @objc public let productType: String

    // Subscription metadata; nil / 0 for non-subscription products.
    @objc public let subscriptionGroupId: String?
    @objc public let subscriptionPeriodUnit: String?
    @objc public let subscriptionPeriodValue: Int

    // Introductory offer as configured in App Store Connect; nil / 0 when the
    // product has none. Eligibility is per-account — see checkIntroOfferEligibility.
    @objc public let introOfferPaymentMode: String?
    @objc public let introOfferPrice: NSDecimalNumber?
    @objc public let introOfferDisplayPrice: String?
    @objc public let introOfferPeriodUnit: String?
    @objc public let introOfferPeriodValue: Int
    @objc public let introOfferPeriodCount: Int

    public init(product: Product) {
        self.productId = product.id
        self.displayName = product.displayName
        self.productDescription = product.description
        self.price = NSDecimalNumber(decimal: product.price)
        self.displayPrice = product.displayPrice

        switch product.type {
        case .consumable:
            self.productType = "consumable"
        case .nonConsumable:
            self.productType = "nonConsumable"
        case .autoRenewable:
            self.productType = "autoRenewable"
        case .nonRenewable:
            self.productType = "nonRenewable"
        default:
            self.productType = "unknown"
        }

        if let subscription = product.subscription {
            self.subscriptionGroupId = subscription.subscriptionGroupID
            self.subscriptionPeriodUnit = PaymentProduct.periodUnitString(subscription.subscriptionPeriod.unit)
            self.subscriptionPeriodValue = subscription.subscriptionPeriod.value

            if let intro = subscription.introductoryOffer {
                switch intro.paymentMode {
                case .freeTrial:
                    self.introOfferPaymentMode = "freeTrial"
                case .payAsYouGo:
                    self.introOfferPaymentMode = "payAsYouGo"
                case .payUpFront:
                    self.introOfferPaymentMode = "payUpFront"
                default:
                    self.introOfferPaymentMode = "unknown"
                }
                self.introOfferPrice = NSDecimalNumber(decimal: intro.price)
                self.introOfferDisplayPrice = intro.displayPrice
                self.introOfferPeriodUnit = PaymentProduct.periodUnitString(intro.period.unit)
                self.introOfferPeriodValue = intro.period.value
                self.introOfferPeriodCount = intro.periodCount
            } else {
                self.introOfferPaymentMode = nil
                self.introOfferPrice = nil
                self.introOfferDisplayPrice = nil
                self.introOfferPeriodUnit = nil
                self.introOfferPeriodValue = 0
                self.introOfferPeriodCount = 0
            }
        } else {
            self.subscriptionGroupId = nil
            self.subscriptionPeriodUnit = nil
            self.subscriptionPeriodValue = 0
            self.introOfferPaymentMode = nil
            self.introOfferPrice = nil
            self.introOfferDisplayPrice = nil
            self.introOfferPeriodUnit = nil
            self.introOfferPeriodValue = 0
            self.introOfferPeriodCount = 0
        }

        super.init()
    }

    private static func periodUnitString(_ unit: Product.SubscriptionPeriod.Unit) -> String {
        switch unit {
        case .day:
            return "day"
        case .week:
            return "week"
        case .month:
            return "month"
        case .year:
            return "year"
        default:
            return "unknown"
        }
    }
}

@available(iOS 15.0, *)
@objc public class PaymentTransaction: NSObject {
    @objc public let transactionId: String
    @objc public let productId: String
    @objc public let purchaseDate: Date
    @objc public let isUpgraded: Bool
    @objc public let revocationDate: Date?
    @objc public let revocationReason: String?

    // End of the current subscription period; nil for non-subscriptions.
    @objc public let expirationDate: Date?

    // "Production" / "Sandbox" / "Xcode"; "unknown" below iOS 16 where
    // Transaction.environment is unavailable.
    @objc public let environment: String

    // Which offer bought the current period: "introductory" / "promotional" /
    // "code"; nil when it was bought at the regular price. Note this names the
    // offer, not its payment mode — whether an introductory offer is a free
    // trial depends on how it's configured in App Store Connect.
    @objc public let offerType: String?

    public init(transaction: Transaction) {
        self.transactionId = String(transaction.id)
        self.productId = transaction.productID
        self.purchaseDate = transaction.purchaseDate
        self.isUpgraded = transaction.isUpgraded
        self.revocationDate = transaction.revocationDate
        self.expirationDate = transaction.expirationDate

        if #available(iOS 16.0, *) {
            self.environment = transaction.environment.rawValue
        } else {
            self.environment = "unknown"
        }

        let rawOfferType: Transaction.OfferType?
        if #available(iOS 17.2, *) {
            rawOfferType = transaction.offer?.type
        } else {
            rawOfferType = transaction.offerType
        }
        switch rawOfferType {
        case .introductory:
            self.offerType = "introductory"
        case .promotional:
            self.offerType = "promotional"
        case .code:
            self.offerType = "code"
        case .some:
            self.offerType = "unknown"
        case nil:
            self.offerType = nil
        }
        
        if let reason = transaction.revocationReason {
            switch reason {
            case .developerIssue:
                self.revocationReason = "developerIssue"
            case .other:
                self.revocationReason = "other"
            default:
                self.revocationReason = "unknown"
            }
        } else {
            self.revocationReason = nil
        }
        
        super.init()
    }
}

// One auto-renewable subscription's status within its group: the renewal state
// plus the signed renewal info (what renews next, when, and for how much the
// store will bill), and the latest transaction for the period.
@available(iOS 15.0, *)
@objc public class PaymentSubscriptionStatus: NSObject {
    // "subscribed" / "expired" / "inBillingRetryPeriod" / "inGracePeriod" / "revoked"
    @objc public let state: String

    // Product currently entitling this subscription group.
    @objc public let currentProductId: String?

    @objc public let willAutoRenew: Bool

    // Product that renews at the next billing cycle when the user has queued a
    // plan change; equals currentProductId otherwise. nil if renewal info was
    // unverified or auto-renew is off.
    @objc public let autoRenewPreference: String?

    @objc public let renewalDate: Date?

    // "autoRenewDisabled" / "billingError" / "didNotConsentToPriceIncrease" /
    // "productUnavailable" — only set once the subscription has expired.
    @objc public let expirationReason: String?

    @objc public let gracePeriodExpirationDate: Date?

    // Latest verified transaction for this subscription; carries expirationDate.
    @objc public let transaction: PaymentTransaction?

    public init(status: Product.SubscriptionInfo.Status) {
        switch status.state {
        case .subscribed:
            self.state = "subscribed"
        case .expired:
            self.state = "expired"
        case .inBillingRetryPeriod:
            self.state = "inBillingRetryPeriod"
        case .inGracePeriod:
            self.state = "inGracePeriod"
        case .revoked:
            self.state = "revoked"
        default:
            self.state = "unknown"
        }

        switch status.renewalInfo {
        case .verified(let renewalInfo):
            self.currentProductId = renewalInfo.currentProductID
            self.willAutoRenew = renewalInfo.willAutoRenew
            self.autoRenewPreference = renewalInfo.autoRenewPreference
            self.renewalDate = renewalInfo.renewalDate
            self.gracePeriodExpirationDate = renewalInfo.gracePeriodExpirationDate

            if let reason = renewalInfo.expirationReason {
                switch reason {
                case .autoRenewDisabled:
                    self.expirationReason = "autoRenewDisabled"
                case .billingError:
                    self.expirationReason = "billingError"
                case .didNotConsentToPriceIncrease:
                    self.expirationReason = "didNotConsentToPriceIncrease"
                case .productUnavailable:
                    self.expirationReason = "productUnavailable"
                default:
                    self.expirationReason = "unknown"
                }
            } else {
                self.expirationReason = nil
            }
        case .unverified(_, _):
            self.currentProductId = nil
            self.willAutoRenew = false
            self.autoRenewPreference = nil
            self.renewalDate = nil
            self.gracePeriodExpirationDate = nil
            self.expirationReason = nil
        }

        switch status.transaction {
        case .verified(let transaction):
            self.transaction = PaymentTransaction(transaction: transaction)
        case .unverified(_, _):
            self.transaction = nil
        }

        super.init()
    }
}

@objc public enum PaymentPurchaseResultCode: Int {
    case success = 0
    case userCancelled = 1
    case pending = 2
    case unverified = 3
    case productNotFound = 4
    case failed = 5
}

@available(iOS 15.0, *)
@objc public class PaymentPurchaseResult: NSObject {
    @objc public let code: PaymentPurchaseResultCode
    @objc public let transaction: PaymentTransaction?
    @objc public let errorMessage: String?

    public init(code: PaymentPurchaseResultCode, transaction: PaymentTransaction? = nil, errorMessage: String? = nil) {
        self.code = code
        self.transaction = transaction
        self.errorMessage = errorMessage
        super.init()
    }
}

@available(iOS 15.0, *)
@objc public class PaymentManager: NSObject {
    @objc public static let shared: PaymentManager = PaymentManager()
    @objc public weak var delegate: PaymentManagerDelegate?
    
    private var products: [String: Product] = [:]
    private var transactionListener: Task<Void, Error>?
    
    private override init() {
        super.init()
        startTransactionListener()
    }
    
    deinit {
        transactionListener?.cancel()
    }
    
    // MARK: - Public Methods
    
    @objc public func requestProducts(productIds: [String], completion: @escaping (Bool, String?) -> Void) {
        Task {
            do {
                let storeProducts: [Product] = try await Product.products(for: Set(productIds))
                
                await MainActor.run {
                    for product: Product in storeProducts {
                        self.products[product.id] = product
                    }
                    
                    let paymentProducts: [PaymentProduct] = storeProducts.map { PaymentProduct(product: $0) }
                    self.delegate?.paymentManagerDidUpdateProducts?(paymentProducts)
                    completion(true, nil)
                }
            } catch {
                await MainActor.run {
                    completion(false, error.localizedDescription)
                }
            }
        }
    }
    
    @objc public func purchaseProduct(productId: String, appAccountToken: UUID? = nil, completion: @escaping (Bool, String?) -> Void) {
        guard let product = products[productId] else {
            completion(false, "Product not found: \(productId)")
            return
        }
        
        Task {
            do {
                var options: Set<Product.PurchaseOption> = []
                if let token = appAccountToken {
                    options.insert(.appAccountToken(token))
                }
                let result: Product.PurchaseResult = try await product.purchase(options: options)
                
                switch result {
                case .success(let verification):
                    switch verification {
                    case .verified(let transaction):
                        let paymentTransaction = PaymentTransaction(transaction: transaction)
                        await MainActor.run {
                            self.delegate?.paymentManagerDidFinishPurchase?(productId, transaction: paymentTransaction)
                            completion(true, nil)
                        }
                        await transaction.finish()
                    case .unverified(_, let error):
                        await MainActor.run {
                            self.delegate?.paymentManagerDidFailPurchase?(productId, error: "Unverified transaction: \(error)")
                            completion(false, "Unverified transaction")
                        }
                    }
                case .userCancelled:
                    await MainActor.run {
                        self.delegate?.paymentManagerDidFailPurchase?(productId, error: "User cancelled")
                        completion(false, "User cancelled")
                    }
                case .pending:
                    await MainActor.run {
                        completion(false, "Purchase pending")
                    }
                @unknown default:
                    await MainActor.run {
                        completion(false, "Unknown result")
                    }
                }
            } catch {
                await MainActor.run {
                    self.delegate?.paymentManagerDidFailPurchase?(productId, error: error.localizedDescription)
                    completion(false, error.localizedDescription)
                }
            }
        }
    }
    
    @objc public func restorePurchases(completion: @escaping (Bool, String?) -> Void) {
        Task {
            do {
                try await AppStore.sync()
                
                let restoredTransactions = await collectCurrentEntitlements()
                
                await MainActor.run {
                    self.delegate?.paymentManagerDidRestorePurchases?(restoredTransactions)
                    completion(true, nil)
                }
            } catch {
                await MainActor.run {
                    completion(false, error.localizedDescription)
                }
            }
        }
    }
    
    @objc public func getProduct(productId: String) -> PaymentProduct? {
        guard let product = products[productId] else { return nil }
        return PaymentProduct(product: product)
    }
    
    @objc public func getAllProducts() -> [PaymentProduct] {
        return products.values.map { PaymentProduct(product: $0) }
    }
    
    @objc public func purchaseProductWithResult(productId: String, appAccountToken: UUID? = nil, completion: @escaping (PaymentPurchaseResult) -> Void) {
        guard let product = products[productId] else {
            completion(PaymentPurchaseResult(code: .productNotFound, errorMessage: "Product not found: \(productId)"))
            return
        }

        Task {
            do {
                var options: Set<Product.PurchaseOption> = []
                if let token = appAccountToken {
                    options.insert(.appAccountToken(token))
                }
                let result: Product.PurchaseResult = try await product.purchase(options: options)

                switch result {
                case .success(let verification):
                    switch verification {
                    case .verified(let transaction):
                        let paymentTransaction = PaymentTransaction(transaction: transaction)
                        await MainActor.run {
                            self.delegate?.paymentManagerDidFinishPurchase?(productId, transaction: paymentTransaction)
                            completion(PaymentPurchaseResult(code: .success, transaction: paymentTransaction))
                        }
                        await transaction.finish()
                    case .unverified(_, let error):
                        await MainActor.run {
                            self.delegate?.paymentManagerDidFailPurchase?(productId, error: "Unverified transaction: \(error)")
                            completion(PaymentPurchaseResult(code: .unverified, errorMessage: "\(error)"))
                        }
                    }
                case .userCancelled:
                    await MainActor.run {
                        self.delegate?.paymentManagerDidFailPurchase?(productId, error: "User cancelled")
                        completion(PaymentPurchaseResult(code: .userCancelled))
                    }
                case .pending:
                    await MainActor.run {
                        completion(PaymentPurchaseResult(code: .pending))
                    }
                @unknown default:
                    await MainActor.run {
                        completion(PaymentPurchaseResult(code: .failed, errorMessage: "Unknown result"))
                    }
                }
            } catch {
                await MainActor.run {
                    self.delegate?.paymentManagerDidFailPurchase?(productId, error: error.localizedDescription)
                    completion(PaymentPurchaseResult(code: .failed, errorMessage: error.localizedDescription))
                }
            }
        }
    }

    // Statuses for the subscription group the product belongs to — one entry
    // per subscription the account holds in the group (family sharing can add
    // more). Empty when the account has never subscribed to the group. Requires
    // the product to have been loaded via requestProducts first.
    @objc public func getSubscriptionStatus(productId: String, completion: @escaping ([PaymentSubscriptionStatus], String?) -> Void) {
        guard let product = products[productId] else {
            completion([], "Product not found: \(productId)")
            return
        }
        guard let subscription = product.subscription else {
            completion([], "Not an auto-renewable subscription: \(productId)")
            return
        }

        Task {
            do {
                let statuses = try await subscription.status
                let paymentStatuses = statuses.map { PaymentSubscriptionStatus(status: $0) }
                await MainActor.run {
                    completion(paymentStatuses, nil)
                }
            } catch {
                await MainActor.run {
                    completion([], error.localizedDescription)
                }
            }
        }
    }

    // Per-account intro-offer eligibility for the product's subscription group.
    @objc public func checkIntroOfferEligibility(productId: String, completion: @escaping (Bool, String?) -> Void) {
        guard let product = products[productId] else {
            completion(false, "Product not found: \(productId)")
            return
        }
        guard let subscription = product.subscription else {
            completion(false, "Not an auto-renewable subscription: \(productId)")
            return
        }

        Task {
            let eligible = await subscription.isEligibleForIntroOffer
            await MainActor.run {
                completion(eligible, nil)
            }
        }
    }

    // App Store environment for this app install: "Production" / "Sandbox" /
    // "Xcode". Returns "unknown" plus a message below iOS 16 or when the signed
    // AppTransaction fails verification — callers should fall back to their own
    // heuristic rather than trust an unverified value.
    @objc public func getAppEnvironment(completion: @escaping (String, String?) -> Void) {
        guard #available(iOS 16.0, *) else {
            completion("unknown", "AppTransaction requires iOS 16")
            return
        }

        Task {
            do {
                let shared = try await AppTransaction.shared
                switch shared {
                case .verified(let appTransaction):
                    await MainActor.run {
                        completion(appTransaction.environment.rawValue, nil)
                    }
                case .unverified(_, let error):
                    await MainActor.run {
                        completion("unknown", "Unverified AppTransaction: \(error)")
                    }
                }
            } catch {
                await MainActor.run {
                    completion("unknown", error.localizedDescription)
                }
            }
        }
    }

    // One atomic snapshot of every verified current entitlement, in place of
    // per-product checkPurchaseStatus round-trips.
    @objc public func getCurrentEntitlements(completion: @escaping ([PaymentTransaction]) -> Void) {
        Task {
            let transactions = await collectCurrentEntitlements()
            await MainActor.run {
                completion(transactions)
            }
        }
    }

    @objc public func checkPurchaseStatus(productId: String, completion: @escaping (Bool, PaymentTransaction?) -> Void) {
        Task {
            let foundTransaction = await findCurrentEntitlement(for: productId)
            
            await MainActor.run {
                if let transaction = foundTransaction {
                    completion(true, transaction)
                } else {
                    completion(false, nil)
                }
            }
        }
    }
    
    // MARK: - Private Methods
    
    private func collectCurrentEntitlements() async -> [PaymentTransaction] {
        var transactions: [PaymentTransaction] = []
        
        for await result in Transaction.currentEntitlements {
            switch result {
            case .verified(let transaction):
                transactions.append(PaymentTransaction(transaction: transaction))
            case .unverified(_, _):
                continue
            }
        }
        
        return transactions
    }
    
    private func findCurrentEntitlement(for productId: String) async -> PaymentTransaction? {
        for await result in Transaction.currentEntitlements {
            switch result {
            case .verified(let transaction):
                if transaction.productID == productId {
                    return PaymentTransaction(transaction: transaction)
                }
            case .unverified(_, _):
                continue
            }
        }
        return nil
    }
    
    private func startTransactionListener() {
        transactionListener = Task(priority: .background) {
            for await result in Transaction.updates {
                switch result {
                case .verified(let transaction):
                    let paymentTransaction = PaymentTransaction(transaction: transaction)
                    await MainActor.run {
                        self.delegate?.paymentManagerDidReceiveTransactionUpdate?(paymentTransaction)
                        self.delegate?.paymentManagerDidFinishPurchase?(transaction.productID, transaction: paymentTransaction)
                    }
                    await transaction.finish()
                case .unverified(_, let error):
                    await MainActor.run {
                        self.delegate?.paymentManagerDidFailPurchase?("unknown", error: "Unverified transaction: \(error)")
                    }
                }
            }
        }
    }
}
