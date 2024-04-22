using Microsoft.AspNetCore.Mvc;
using prjTravelPlatform_release.Areas.Customer.LinePay.DTO;
using prjTravelPlatform_release.Areas.Customer.LinePay.Service;

namespace MyProject.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LinePayController : ControllerBase
	{
		private readonly LinePayService _linePayService;
		public LinePayController()
		{
			_linePayService = new LinePayService();
		}

		[HttpPost]
		[Route("Create")]
		public async Task<PaymentResponseDTO> CreatePayment(PaymentRequestDTO dto)
		{
			return await _linePayService.SendPaymentRequest(dto);
		}

		[HttpPost("Confirm")]
		public async Task<PaymentConfirmResponseDTO> ConfirmPayment([FromQuery] string transactionId, [FromQuery] string orderId, PaymentConfirmDTO dto)
		{
			return await _linePayService.ConfirmPayment(transactionId, orderId, dto);
		}

		[HttpGet("Cancel")]
		public async void CancelTransaction([FromQuery] string transactionId)
		{
			_linePayService.TransactionCancel(transactionId);
		}

	}
}
