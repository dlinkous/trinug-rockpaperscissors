using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RockPaperScissors.Entities;
using RockPaperScissors.Interactors.Repositories;

namespace RockPaperScissors.Amazon.Repositories
{
	public class DynamoDbGameRepository : IGameRepository
	{
		private readonly IDynamoDbGameRepositorySettings settings;

		private const string player1ShapeName = "Player1Shape";
		private const string player2ShapeName = "Player2Shape";
		private const string outcomeName = "Outcome";

		public DynamoDbGameRepository(IDynamoDbGameRepositorySettings settings) =>
			this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

		public async Task CreateAsync(string identifier, Game game) =>
			await PutAsync(identifier, game);

		public async Task<Game> ReadAsync(string identifier)
		{
			var request = new GetItemRequest()
			{
				TableName = settings.TableName,
				Key = GetItemWithKey(identifier)
			};
			using (var client = CreateClient())
			{
				var response = await client.GetItemAsync(request);
				if (response.IsItemSet)
					return GetGame(response.Item);
				else
					return null;
			}
		}

		public async Task UpdateAsync(string identifier, Game game) =>
			await PutAsync(identifier, game);

		private async Task PutAsync(string identifier, Game game)
		{
			var request = new PutItemRequest()
			{
				TableName = settings.TableName,
				Item = GetItem(identifier, game)
			};
			using (var client = CreateClient())
				await client.PutItemAsync(request);
		}

		private Dictionary<string, AttributeValue> GetItem(string identifier, Game game)
		{
			var item = GetItemWithKey(identifier);
			if (game.Player1Shape.HasValue) item.Add(player1ShapeName, new AttributeValue() { S = game.Player1Shape.Value.ToString() });
			if (game.Player2Shape.HasValue) item.Add(player2ShapeName, new AttributeValue() { S = game.Player2Shape.Value.ToString() });
			item.Add(outcomeName, new AttributeValue() { S = game.Outcome.ToString() });
			return item;
		}

		private Dictionary<string, AttributeValue> GetItemWithKey(string identifier) =>
			new Dictionary<string, AttributeValue>() { { settings.KeyName, new AttributeValue() { S = identifier } } };

		private Game GetGame(Dictionary<string, AttributeValue> item)
		{
			var game = new Game();
			if (item.ContainsKey(player1ShapeName)) game.ThrowPlayer1(Enum.Parse<Shape>(item[player1ShapeName].S));
			if (item.ContainsKey(player2ShapeName)) game.ThrowPlayer2(Enum.Parse<Shape>(item[player2ShapeName].S));
			return game;
		}

		private AmazonDynamoDBClient CreateClient()
		{
			var useKeys = !string.IsNullOrWhiteSpace(settings.AccessKey) && !string.IsNullOrWhiteSpace(settings.SecretKey);
			if (useKeys)
				return new AmazonDynamoDBClient(settings.AccessKey, settings.SecretKey, RegionEndpoint.GetBySystemName(settings.RegionName));
			else
				return new AmazonDynamoDBClient(RegionEndpoint.GetBySystemName(settings.RegionName));
		}
	}
}
