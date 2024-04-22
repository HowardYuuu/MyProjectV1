namespace prjTravelPlatform_release.Areas.Customer.LinePay.DTO
{
    public class PaymentRequestDTO
    {
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string OrderId { get; set; }
        public List<PackageDTO> Packages { get; set; }
        public RedirectUrlsDTO RedirectUrls { get; set; }
        public RequestOptionDTO? Options { get; set; }
    }
    public class PackageDTO
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public string Name { get; set; }
        public List<LinePayProductDTO> Products { get; set; }
        public int? UserFee { get; set; }

    }
    public class LinePayProductDTO
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public string? Id { get; set; }
        public string? ImageUrl { get; set; }
        public int? OriginalPrice { get; set; }
    }

    public class RedirectUrlsDTO
    {
        public string ConfirmUrl { get; set; }
        public string CancelUrl { get; set; }
        public string? AppPackageName { get; set; }
        public string? ConfirmUrlType { get; set; }
    }

    public class RequestOptionDTO
    {
        public PaymentOptionDTO? Payment { get; set; }
        public DisplpyOptionDTO? Displpy { get; set; }
        public ShippingOptionDTO? Shipping { get; set; }
        public ExtraOptionsDTO? Extra { get; set; }
    }
    public class PaymentOptionDTO
    {
        public bool? Capture { get; set; }
        public string? PayType { get; set; }
    }
    public class DisplpyOptionDTO
    {
        public string? Local { get; set; }
        public bool? CheckConfirmUrlBrowser { get; set; }
    }
    public class ShippingOptionDTO
    {
        public string? Type { get; set; }
        public int FeeAmount { get; set; }
        public string? FeeInquiryUrl { get; set; }
        public string? FeeInquiryType { get; set; }
        public ShippingAddressDTO? Address { get; set; }
    }

    public class ShippingAddressDTO
    {
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Detail { get; set; }
        public string? Optional { get; set; }
        public ShippingAddressRecipientDTO Recipient { get; set; }
    }

    public class ShippingAddressRecipientDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FirstNameOptional { get; set; }
        public string? LastNameOptional { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? Type { get; set; }
    }

    public class ExtraOptionsDTO
    {
        public string? BranchName { get; set; }
        public string? BranchId { get; set; }
    }
}
