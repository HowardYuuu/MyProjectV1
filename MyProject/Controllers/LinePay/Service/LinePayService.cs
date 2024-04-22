using prjTravelPlatform_release.Areas.Customer.LinePay.DTO;
using prjTravelPlatform_release.Areas.Customer.LinePay.Providers;
using System.Text;


namespace prjTravelPlatform_release.Areas.Customer.LinePay.Service
{
	public class LinePayService
	{
		private static HttpClient? client;
		private readonly JsonProvider _jsonProvider;

		public LinePayService()
		{
			client = new HttpClient();
			_jsonProvider = new JsonProvider();
		}

		//金鑰端點放置區
		//private readonly string channelId = "20044845650";
		//private readonly string channelSecretKey = "02b88ff93c73397a9162d09c0414317e0";

		//private readonly string linePayBaseApiUrl = "https://sandbox-api-pay.line.me";
		//金鑰端點放置區

		public async Task<PaymentResponseDTO> SendPaymentRequest(PaymentRequestDTO DTO)
		{
			var json = _jsonProvider.Serialize(DTO);
			// 產生 GUID Nonce
			var nonce = Guid.NewGuid().ToString();
			// 要放入 signature 中的 requestUrl
			var requestUrl = "/v3/payments/request";

			//使用 channelSecretKey & requestUrl & jsonBody & nonce 做簽章
			var signature = SignatureProvider.HMACSHA256(channelSecretKey, channelSecretKey + requestUrl + json + nonce);

			var request = new HttpRequestMessage(HttpMethod.Post, linePayBaseApiUrl + requestUrl)
			{
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};
			// 帶入 Headers
			client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
			client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
			client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

			var response = await client.SendAsync(request);
			var linePayResponse = _jsonProvider.Deserialize<PaymentResponseDTO>(await response.Content.ReadAsStringAsync());

			Console.WriteLine(nonce);
			Console.WriteLine(signature);

			return linePayResponse;
		}

		public async Task<PaymentConfirmResponseDTO> ConfirmPayment(string transactionId, string orderId, PaymentConfirmDTO DTO) //加上 OrderId 去找資料
		{
			var json = _jsonProvider.Serialize(DTO);

			var nonce = Guid.NewGuid().ToString();
			var requestUrl = string.Format("/v3/payments/{0}/confirm", transactionId);
			var signature = SignatureProvider.HMACSHA256(channelSecretKey, channelSecretKey + requestUrl + json + nonce);

			var request = new HttpRequestMessage(HttpMethod.Post, String.Format(linePayBaseApiUrl + requestUrl, transactionId))
			{
				Content = new StringContent(json, Encoding.UTF8, "application/json")
			};

			client.DefaultRequestHeaders.Add("X-LINE-ChannelId", channelId);
			client.DefaultRequestHeaders.Add("X-LINE-Authorization-Nonce", nonce);
			client.DefaultRequestHeaders.Add("X-LINE-Authorization", signature);

			var response = await client.SendAsync(request);
			var responseDTO = _jsonProvider.Deserialize<PaymentConfirmResponseDTO>(await response.Content.ReadAsStringAsync());
			return responseDTO;
		}

		public async void TransactionCancel(string transactionId)
		{
			//使用者取消交易則會到這裏。
			Console.WriteLine($"訂單 {transactionId} 已取消");
		}
	}


}
