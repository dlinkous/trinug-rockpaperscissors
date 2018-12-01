using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using RockPaperScissors.Entities;
using RockPaperScissors.Interactors;
using RockPaperScissors.Tests.Interactors.Mocks;

namespace RockPaperScissors.Tests.Interactors
{
	public class BeginGameInteractorTests
	{
		[Fact]
		public void Constructor_RequiresNonNullGameRepository()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var interactor = new BeginGameInteractor(null, new IdentifierProviderMock());
			});
		}

		[Fact]
		public void Constructor_RequiresNonNullIdentifierProvider()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var interactor = new BeginGameInteractor(new GameRepositoryMock(), null);
			});
		}

		[Fact]
		public async Task BeginsGameCorrectlyAsync()
		{
			var gameRepositoryMock = new GameRepositoryMock();
			var identifierProviderMock = new IdentifierProviderMock();
			var interactor = new BeginGameInteractor(gameRepositoryMock, identifierProviderMock);
			const string identifier = "UnitTestIdentifier";
			identifierProviderMock.Identifiers.Enqueue(identifier);
			var request = new BeginGameInteractor.Request() { Shape = Shape.Paper };
			var response = await interactor.HandleRequestAsync(request);
			Assert.Equal(identifier, response.Identifier);
			var gameItem = gameRepositoryMock.Games.Single();
			Assert.Equal(identifier, gameItem.Key);
			var game = gameItem.Value;
			Assert.Equal(Shape.Paper, game.Player1Shape.Value);
			Assert.Null(game.Player2Shape);
			Assert.Equal(Outcome.Undecided, game.Outcome);
		}
	}
}
