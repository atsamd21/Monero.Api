using System.Text.Json.Serialization;

namespace Monero.Api.BigCommerce.Responses;

public class BillingAddress
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

public class GetOrderResponse
{
    [JsonPropertyName("customer_id")]
    public int CustomerId { get; set; }
    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; set; } = string.Empty;
    [JsonPropertyName("subtotal_inc_tax")]
    public decimal SubTotal {  get; set; }
    [JsonPropertyName("shipping_cost_inc_tax")]
    public decimal ShippingCost {  get; set; }
    [JsonPropertyName("handling_cost_inc_tax")]
    public decimal HandlingCost { get; set; }
    [JsonPropertyName("wrapping_cost_inc_tax")]
    public decimal WrappingCost { get; set; }
    [JsonPropertyName("discount_amount")]
    public decimal DiscountAmount { get; set; }
    [JsonPropertyName("coupon_discount")]
    public decimal CouponDiscount { get; set; }
    [JsonPropertyName("billing_address")]
    public BillingAddress BillingAddress { get; set; } = new();
}
