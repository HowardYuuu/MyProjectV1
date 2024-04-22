namespace prjTravelPlatform_release.Areas.Customer.LinePay.DTO
{
    public class PaymentConfirmResponseDTO
    {
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public ConfirmResponseInfoDTO Info { get; set; }
    }
    public class ConfirmResponseInfoDTO
    {
        public string OrderId { get; set; }
        public long TransactionId { get; set; }
        public string AuthorizationExpireDate { get; set; }
        public string RegKey { get; set; }
        public ConfirmResponsePayInfoDTO[] PayInfo { get; set; }
    }

    public class ConfirmResponsePayInfoDTO
    {
        public string Method { get; set; }
        public int Amount { get; set; }
        public string CreditCardNickname { get; set; }
        public string CreditCardBrand { get; set; }
        public string MaskedCreditCardNumber { get; set; }
        public ConfirmResponsePackageDTO[] Packages { get; set; }
        public ConfirmResponseShippingOptionsDTO Shipping { get; set; }
    }
    public class ConfirmResponsePackageDTO
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public int UserFeeAmount { get; set; }
    }
    public class ConfirmResponseShippingOptionsDTO
    {
        public string MethodId { get; set; }
        public int FeeAmount { get; set; }
        public ShippingAddressDTO Address { get; set; }
    }
}
