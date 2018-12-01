using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using lambdajson = Amazon.Lambda.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RockPaperScissors.Interactors;

namespace RockPaperScissors.Amazon.Invocation
{
	public class GatewayInvoker
	{
		private const int httpOk = 200;
		private const int httpNotFound = 404;

		[LambdaSerializer(typeof(lambdajson.JsonSerializer))]
		public async Task<APIGatewayProxyResponse> HandleAsync(APIGatewayProxyRequest request)
		{
			var name = request.Path.Substring(1);
			var body = request.Body;
			var invoker = new LambdaInvoker();
			switch (name)
			{
				case "Begin":
					return await InvokeAsync<BeginGameInteractor.Request, BeginGameInteractor.Response>(invoker.BeginGameAsync, body);
				case "Finish":
					return await InvokeAsync<FinishGameInteractor.Request, FinishGameInteractor.Response>(invoker.FinishGameAsync, body);
				case "Check":
					return await InvokeAsync<CheckGameInteractor.Request, CheckGameInteractor.Response>(invoker.CheckGameAsync, body);
				default:
					return new APIGatewayProxyResponse() { StatusCode = httpNotFound };
			}
		}

		private async Task<APIGatewayProxyResponse> InvokeAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> funcAsync, string body)
		{
			var settings = new JsonSerializerSettings();
			settings.Converters.Add(new StringEnumConverter());
			var request = JsonConvert.DeserializeObject<TRequest>(body, settings);
			var response = await funcAsync(request);
			return new APIGatewayProxyResponse()
			{
				StatusCode = httpOk,
				Body = JsonConvert.SerializeObject(response, settings),
				Headers = new Dictionary<string, string>()
				{
					{ "Access-Control-Allow-Headers", "Content-Type" },
					{ "Access-Control-Allow-Methods", "OPTIONS,POST" },
					{ "Access-Control-Allow-Origin", "*" },
					{ "Access-Control-Max-Age", "300" },
					{ "Content-Type", "application/json" }
				}
			};
		}
	}
}
