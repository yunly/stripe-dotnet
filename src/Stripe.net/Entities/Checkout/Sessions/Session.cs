namespace Stripe.Checkout
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Stripe.Infrastructure;

    public class Session : StripeEntity, IHasId, IHasObject
    {
        /// <summary>
        /// Unique identifier for the object.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// String representing the object’s type. Objects of the same type share the same value.
        /// </summary>
        [JsonProperty("object")]
        public string Object { get; set; }

        /// <summary>
        /// Specify whether Checkout should collect the customer’s billing address. If set to
        /// <c>required</c>, Checkout will always collect the customer’s billing address. If left
        /// blank or set to <c>auto</c> Checkout will only collect the billing address when
        /// necessary.
        /// </summary>
        [JsonProperty("billing_address_collection")]
        public string BillingAddressCollection { get; set; }

        /// <summary>
        /// The URL the customer will be directed to if they decide to go back to your website.
        /// </summary>
        [JsonProperty("cancel_url")]
        public string CancelUrl { get; set; }

        /// <summary>
        /// A unique string to reference the Checkout Session. This can be a customer ID, a cart
        /// ID, or similar. It is included in the <c>checkout.session.completed</c> webhook and can
        /// be used to fulfill the purchase.
        /// </summary>
        [JsonProperty("client_reference_id")]
        public string ClientReferenceId { get; set; }

        #region Expandable Customer

        /// <summary>
        /// ID of the customer this Session is for if one exists.
        /// </summary>
        [JsonIgnore]
        public string CustomerId { get; set; }

        [JsonIgnore]
        public Customer Customer { get; set; }

        [JsonProperty("customer")]
        internal object InternalCustomer
        {
            get
            {
                return this.Customer ?? (object)this.CustomerId;
            }

            set
            {
                StringOrObject<Customer>.Map(value, s => this.CustomerId = s, o => this.Customer = o);
            }
        }
        #endregion

        /// <summary>
        /// The email address used to create the customer object.
        /// </summary>
        [JsonProperty("customer_email")]
        public string CustomerEmail { get; set; }

        /// <summary>
        /// The line items, plans, or SKUs that were purchased by the customer.
        /// </summary>
        [JsonProperty("display_items")]
        public List<SessionDisplayItem> DisplayItems { get; set; }

        /// <summary>
        /// Has the value <c>true</c> if the object exists in live mode or the value
        /// <c>false</c> if the object exists in test mode.
        /// </summary>
        [JsonProperty("livemode")]
        public bool Livemode { get; set; }

        /// <summary>
        /// The IETF language tag of the locale Checkout is displayed in. If blank or <c>auto</c>,
        /// the browser’s locale is used.
        /// </summary>
        [JsonProperty("locale")]
        public string Locale { get; set; }

        #region Expandable PaymentIntent

        /// <summary>
        /// The ID of the PaymentIntent created if SKUs or line items were provided.
        /// </summary>
        [JsonIgnore]
        public string PaymentIntentId { get; set; }

        [JsonIgnore]
        public PaymentIntent PaymentIntent { get; set; }

        [JsonProperty("payment_intent")]
        internal object InternalPaymentIntent
        {
            get
            {
                return this.PaymentIntent ?? (object)this.PaymentIntentId;
            }

            set
            {
                StringOrObject<PaymentIntent>.Map(value, s => this.PaymentIntentId = s, o => this.PaymentIntent = o);
            }
        }
        #endregion

        /// <summary>
        /// The list of payment method types (e.g. card) that this Checkout Session is allowed to
        /// use.
        /// </summary>
        [JsonProperty("payment_method_types")]
        public List<string> PaymentMethodTypes { get; set; }

        #region Expandable Subscription

        /// <summary>
        /// The ID of the subscription created if one or more plans were provided.
        /// </summary>
        [JsonIgnore]
        public string SubscriptionId { get; set; }

        [JsonIgnore]
        public Subscription Subscription { get; set; }

        [JsonProperty("subscription")]
        internal object InternalSubscription
        {
            get
            {
                return this.Subscription ?? (object)this.SubscriptionId;
            }

            set
            {
                StringOrObject<Subscription>.Map(value, s => this.SubscriptionId = s, o => this.Subscription = o);
            }
        }
        #endregion

        /// <summary>
        /// The URL the customer will be directed to after a successful payment.
        /// </summary>
        [JsonProperty("success_url")]
        public string SuccessUrl { get; set; }
    }
}
