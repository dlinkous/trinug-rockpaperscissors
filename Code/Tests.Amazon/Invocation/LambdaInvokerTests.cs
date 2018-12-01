using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Xunit;
using RockPaperScissors.Amazon.Invocation;
using RockPaperScissors.Entities;
using RockPaperScissors.Interactors;
using RockPaperScissors.Tests.Amazon.Mocks;

namespace RockPaperScissors.Tests.Amazon.Invocation
{
	public class LambdaInvokerTests
	{
		[Fact]
		public async Task FullCycleLambdaInvokerTestAsync()
		{
			var settings = new DynamoDbGameRepositorySettingsMock();
			Environment.SetEnvironmentVariable(nameof(settings.AccessKey), settings.AccessKey);
			Environment.SetEnvironmentVariable(nameof(settings.SecretKey), settings.SecretKey);
			Environment.SetEnvironmentVariable(nameof(settings.RegionName), settings.RegionName);
			Environment.SetEnvironmentVariable(nameof(settings.TableName), settings.TableName);
			Environment.SetEnvironmentVariable(nameof(settings.KeyName), settings.KeyName);
			var invoker = new LambdaInvoker();
			var beginGameResponse = await invoker.BeginGameAsync(new BeginGameInteractor.Request() { Shape = Shape.Scissors });
			var identifier = beginGameResponse.Identifier;
			var finishGameResponse = await invoker.FinishGameAsync(new FinishGameInteractor.Request()
			{
				Identifier = identifier,
				Shape = Shape.Paper
			});
			var finishOutcome = finishGameResponse.Outcome;
			var checkGameResponse = await invoker.CheckGameAsync(new CheckGameInteractor.Request() { Identifier = identifier });
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
		}

		private Dictionary<string, AttributeValue> GetKey(DynamoDbGameRepositorySettingsMock settings, string identifier) =>
			new Dictionary<string, AttributeValue>() { { settings.KeyName, new AttributeValue() { S = identifier } } };
	}
}
