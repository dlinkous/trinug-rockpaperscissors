using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Xunit;
using RockPaperScissors.Amazon.Invocation;
using RockPaperScissors.Entities;
using RockPaperScissors.Interactors;
using RockPaperScissors.Tests.Amazon.Mocks;


namespace RockPaperScissors.Tests.Amazon.Invocation
{
	public class GatewayInvokerTests
	{
		private const int httpOk = 200;
		private const int httpNotFound = 404;

		[Fact]
		public async Task FullCycleGatewayInvokerTestAsync()
		{
			var settings = new DynamoDbGameRepositorySettingsMock();
			Environment.SetEnvironmentVariable(nameof(settings.AccessKey), settings.AccessKey);
			Environment.SetEnvironmentVariable(nameof(settings.SecretKey), settings.SecretKey);
			Environment.SetEnvironmentVariable(nameof(settings.RegionName), settings.RegionName);
			Environment.SetEnvironmentVariable(nameof(settings.TableName), settings.TableName);
			Environment.SetEnvironmentVariable(nameof(settings.KeyName), settings.KeyName);
			var invoker = new GatewayInvoker();
			var beginGameProxyResponse = await invoker.HandleAsync(GetProxyRequest("Begin", new BeginGameInteractor.Request() { Shape = Shape.Scissors }));
			var beginGameResponse = GetResponse<BeginGameInteractor.Response>(beginGameProxyResponse, httpOk);
			var identifier = beginGameResponse.Identifier;
			var finishGameProxyResponse = await invoker.HandleAsync(GetProxyRequest("Finish", new FinishGameInteractor.Request()
			{
				Identifier = identifier,
				Shape = Shape.Paper
			}));
			var finishGameResponse = GetResponse<FinishGameInteractor.Response>(finishGameProxyResponse, httpOk);
			var finishOutcome = finishGameResponse.Outcome;
			var checkGameProxyResponse = await invoker.HandleAsync(GetProxyRequest("Check", new CheckGameInteractor.Request() { Identifier = identifier }));
			var checkGameResponse = GetResponse<CheckGameInteractor.Response>(checkGameProxyResponse, httpOk);
			var checkOutcome = checkGameResponse.Outcome;
			Assert.Equal(checkOutcome, finishOutcome);
			Assert.Equal(Outcome.Player1Wins, checkOutcome);
			using (var client = new AmazonDynamoDBClient(settings.AccessKey, settings.SecretKey, RegionEndpoint.GetBySystemName(settings.RegionName)))
			{
				await client.DeleteItemAsync(new DeleteItemRequest()
				{
					TableName = settings.TableName,
					Key = GetKey(settings, identifier)
				});
			}
			var badProxyResponse = await invoker.HandleAsync(GetProxyRequest("BadName", new BeginGameInteractor.Request() { Shape = Shape.Rock }));
			Assert.Equal(httpNotFound, badProxyResponse.StatusCode);
		}

		private APIGatewayProxyRequest GetProxyRequest<T>(string name, T request) =>
			new APIGatewayProxyRequest()
			{
				Path = $"/{name}",
				Body = JsonConvert.SerializeObject(request, GetJsonSettings())
			};

		private T GetResponse<T>(APIGatewayProxyResponse proxyResponse, int expectedStatusCode)
		{
			Assert.Equal(expectedStatusCode, proxyResponse.StatusCode);
			return JsonConvert.DeserializeObject<T>(proxyResponse.Body, GetJsonSettings());
		}

		private Dictionary<string, AttributeValue> GetKey(DynamoDbGameRepositorySettingsMock settings, string identifier) =>
			new Dictionary<string, AttributeValue>() { { settings.KeyName, new AttributeValue() { S = identifier } } };

		private JsonSerializerSettings GetJsonSettings()
		{
			var settings = new JsonSerializerSettings();
			settings.Converters.Add(new StringEnumConverter());
			return settings;
		}
	}
}
