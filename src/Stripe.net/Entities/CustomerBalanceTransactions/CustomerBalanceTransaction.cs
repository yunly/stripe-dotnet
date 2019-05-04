namespace Stripe
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Stripe.Infrastructure;

    public class CustomerBalanceTransaction : StripeEntity, IHasId, IHasMetadata, IHasObject
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
        /// The amount of the transaction. A negative value is a credit for the customer’s balance,
        /// and a positive value is a debit to the customer’s balance.
        /// </summary>
        [JsonProperty("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// Time at which the object was created. Measured in seconds since the Unix epoch.
        /// </summary>
        [JsonProperty("created")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Created { get; set; }

        /// <summary>
        /// Three-letter ISO currency code, in lowercase. Must be a supported currency.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        #region Expandable Customer

        /// <summary>
        /// ID of the customer this tax ID is for if one exists.
        /// </summary>
        [JsonIgnore]
        public string CustomerId => this.InternalCustomer.Id;

        [JsonIgnore]
        public Customer Customer => this.InternalCustomer.ExpandedObject;

        [JsonProperty("customer")]
        [JsonConverter(typeof(ExpandableFieldConverter<Customer>))]
        internal ExpandableField<Customer> InternalCustomer { get; set; }
        #endregion

        /// <summary>
        /// An arbitrary string attached to the object. Often useful for displaying to users.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// The customer’s balance after the transaction was applied. A negative value decrease the
        /// amount due on the customer’s next invoice. A positive value increase the amount due on
        /// the customer’s next invoice.
        /// </summary>
        [JsonProperty("ending_balance")]
        public long EndingBalance { get; set; }

        #region Expandable Invoice

        /// <summary>
        /// ID of the invoice this charge is for if one exists.
        /// </summary>
        [JsonIgnore]
        public string InvoiceId => this.InternalInvoice.Id;

        [JsonIgnore]
        public Invoice Invoice => this.InternalInvoice.ExpandedObject;

        [JsonProperty("invoice")]
        [JsonConverter(typeof(ExpandableFieldConverter<Invoice>))]
        internal ExpandableField<Invoice> InternalInvoice { get; set; }
        #endregion

        /// <summary>
        /// Has the value <c>true</c> if the object exists in live mode or the value
        /// <c>false</c> if the object exists in test mode.
        /// </summary>
        [JsonProperty("livemode")]
        public bool Livemode { get; set; }

        /// <summary>
        /// Set of key-value pairs that you can attach to an object. This can be useful for storing
        /// additional information about the object in a structured format.
        /// </summary>
        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Transaction type: <c>adjustment</c>, <c> applied_to_invoice</c>, <c> credit_note</c>,
        /// <c> initial</c>, <c> invoice_too_large</c>, <c> invoice_too_small</c>,
        /// <c> migration</c>, <c> receiver_source_mispayment</c>, or <c> reversal</c>.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
