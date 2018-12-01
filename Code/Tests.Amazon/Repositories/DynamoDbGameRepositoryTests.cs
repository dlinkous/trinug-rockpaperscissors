using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Xunit;
using RockPaperScissors.Amazon.Repositories;
using RockPaperScissors.Entities;
using RockPaperScissors.Tests.Amazon.Mocks;

namespace RockPaperScissors.Tests.Amazon.Repositories
{
	public class DynamoDbGameRepositoryTests
	{
		[Fact]
		public void Constructor_RequiresNonNullSettings()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var repository = new DynamoDbGameRepository(null);
			});
		}

		[Fact]
		public async Task FullCycleDynamoDbGameRepositoryTestAsync()
		{
			var settings = new DynamoDbGameRepositorySettingsMock();
			var repository = new DynamoDbGameRepository(settings);
			using (var client = new AmazonDynamoDBClient(settings.AccessKey, settings.SecretKey, RegionEndpoint.GetBySystemName(settings.RegionName)))
			{
				const string identifier = nameof(FullCycleDynamoDbGameRepositoryTestAsync);
				const string player1ShapeName = "Player1Shape";
				const string player2ShapeName = "Player2Shape";
				const string outcomeName = "Outcome";
				await client.DeleteItemAsync(new DeleteItemRequest()
				{
					TableName = settings.TableName,
					Key = GetKey(settings, identifier)
				});
				var originalGame = new Game();
				originalGame.ThrowPlayer1(Shape.Paper);
				await repository.CreateAsync(identifier, originalGame);
				var initialItemResponse = await client.GetItemAsync(new GetItemRequest()
				{
					TableName = settings.TableName,
					Key = GetKey(settings, identifier),
					ConsistentRead = true
				});
				var initialItem = initialItemResponse.Item;
				Assert.Equal(3, initialItem.Count);
				Assert.Equal(identifier, initialItem[settings.KeyName].S);
				Assert.Equal(Shape.Paper.ToString(), initialItem[player1ShapeName].S);
				Assert.Equal(Outcome.Undecided.ToString(), initialItem[outcomeName].S);
				var readGame = await repository.ReadAsync(identifier);
				Assert.Equal(Shape.Paper, readGame.Player1Shape.Value);
				Assert.Null(readGame.Player2Shape);
				Assert.Equal(Outcome.Undecided, readGame.Outcome);
				readGame.ThrowPlayer2(Shape.Scissors);
				await repository.UpdateAsync(identifier, readGame);
				var updatedItemResponse = await client.GetItemAsync(new GetItemRequest()
				{
					TableName = settings.TableName,
					Key = GetKey(settings, identifier),
					ConsistentRead = true
				});
				var updatedItem = updatedItemResponse.Item;
				Assert.Equal(4, updatedItem.Count);
				Assert.Equal(identifier, updatedItem[settings.KeyName].S);
				Assert.Equal(Shape.Paper.ToString(), updatedItem[player1ShapeName].S);
				Assert.Equal(Shape.Scissors.ToString(), updatedItem[player2ShapeName].S);
				Assert.Equal(Outcome.Player2Wins.ToString(), updatedItem[outcomeName].S);
				var finalGame = await repository.ReadAsync(identifier);
				Assert.Equal(Shape.Paper, finalGame.Player1Shape.Value);
				Assert.Equal(Shape.Scissors, finalGame.Player2Shape.Value);
				Assert.Equal(Outcome.Player2Wins, finalGame.Outcome);
				await client.DeleteItemAsync(new DeleteItemRequest()
				{
					TableName = settings.TableName,
					Key = GetKey(settings, identifier)
				});
				var missingGame = await repository.ReadAsync("MissingIdentifier");
				Assert.Null(missingGame);
			}
		}

		private Dictionary<string, AttributeValue> GetKey(DynamoDbGameRepositorySettingsMock settings, string identifier) =>
			new Dictionary<string, AttributeValue>() { { settings.KeyName, new AttributeValue() { S = identifier } } };
	}
}
