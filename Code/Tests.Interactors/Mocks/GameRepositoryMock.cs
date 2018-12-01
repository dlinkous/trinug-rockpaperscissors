using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using RockPaperScissors.Entities;
using RockPaperScissors.Interactors.Repositories;

namespace RockPaperScissors.Tests.Interactors.Mocks
{
	internal class GameRepositoryMock : IGameRepository
	{
		internal Dictionary<string, Game> Games { get; } = new Dictionary<string, Game>();

		public Task CreateAsync(string identifier, Game game)
		{
			Games.Add(identifier, Copy(game));
			return Task.CompletedTask;
		}

		public Task<Game> ReadAsync(string identifier)
		{
			return Task.FromResult(Games.TryGetValue(identifier, out Game game) ? game : null);
		}

		public Task UpdateAsync(string identifier, Game game)
		{
			Games[identifier] = Copy(game);
			return Task.CompletedTask;
		}

		private Game Copy(Game original)
		{
			var formatter = new BinaryFormatter();
			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, original);
				stream.Position = 0;
				return (Game)formatter.Deserialize(stream);
			}
		}
	}
}
