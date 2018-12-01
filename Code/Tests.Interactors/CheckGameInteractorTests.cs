using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using RockPaperScissors.Entities;
using RockPaperScissors.Interactors;
using RockPaperScissors.Tests.Interactors.Mocks;

namespace RockPaperScissors.Tests.Interactors
{
	public class CheckGameInteractorTests
	{
		[Fact]
		public void Constructor_RequiresNonNullGameRepository()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var interactor = new CheckGameInteractor(null);
			});
		}

		[Fact]
		public async Task WhenIdentifierDoesNotExist_OutcomeIsUndecidedAsync()
		{
			var interactor = new CheckGameInteractor(new GameRepositoryMock());
			var request = new CheckGameInteractor.Request() { Identifier = "MissingIdentifier" };
			var response = await interactor.HandleRequestAsync(request);
			Assert.Equal(Outcome.Undecided, response.Outcome);
		}

		[Fact]
		public async Task ChecksGameCorrectlyAsync()
		{
			var gameRepositoryMock = new GameRepositoryMock();
			var interactor = new CheckGameInteractor(gameRepositoryMock);
			var finishedGame = new Game();
			finishedGame.ThrowPlayer1(Shape.Scissors);
			finishedGame.ThrowPlayer2(Shape.Rock);
			const string identifier = "UnitTestIdentifier";
			gameRepositoryMock.Games.Add(identifier, finishedGame);
			var request = new CheckGameInteractor.Request() { Identifier = identifier };
			var response = await interactor.HandleRequestAsync(request);
			Assert.Equal(Outcome.Player2Wins, response.Outcome);
			var gameItem = gameRepositoryMock.Games.Single();
			Assert.Equal(identifier, gameItem.Key);
			var game = gameItem.Value;
			Assert.Equal(Shape.Scissors, game.Player1Shape.Value);
			Assert.Equal(Shape.Rock, game.Player2Shape.Value);
			Assert.Equal(Outcome.Player2Wins, game.Outcome);
		}
	}
}
