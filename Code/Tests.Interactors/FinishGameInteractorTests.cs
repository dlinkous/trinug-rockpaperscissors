using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using RockPaperScissors.Entities;
using RockPaperScissors.Interactors;
using RockPaperScissors.Tests.Interactors.Mocks;

namespace RockPaperScissors.Tests.Interactors
{
	public class FinishGameInteractorTests
	{
		[Fact]
		public void Constructor_RequiresNonNullGameRepository()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				var interactor = new FinishGameInteractor(null);
			});
		}

		[Fact]
		public async Task WhenIdentifierDoesNotExist_OutcomeIsUndecidedAsync()
		{
			var interactor = new FinishGameInteractor(new GameRepositoryMock());
			var request = new FinishGameInteractor.Request()
			{
				Identifier = "MissingIdentifier",
				Shape = Shape.Paper
			};
			var response = await interactor.HandleRequestAsync(request);
			Assert.Equal(Outcome.Undecided, response.Outcome);
		}

		[Fact]
		public async Task WhenGameAlreadyHasOutcome_GameDoesNotChangeAsync()
		{
			var gameRepositoryMock = new GameRepositoryMock();
			var interactor = new FinishGameInteractor(gameRepositoryMock);
			var finishedGame = new Game();
			finishedGame.ThrowPlayer1(Shape.Paper);
			finishedGame.ThrowPlayer2(Shape.Rock);
			const string identifier = "UnitTestIdentifier";
			gameRepositoryMock.Games.Add(identifier, finishedGame);
			var request = new FinishGameInteractor.Request()
			{
				Identifier = identifier,
				Shape = Shape.Scissors
			};
			var response = await interactor.HandleRequestAsync(request);
			Assert.Equal(Outcome.Player1Wins, response.Outcome);
			var gameItem = gameRepositoryMock.Games.Single();
			Assert.Equal(identifier, gameItem.Key);
			var game = gameItem.Value;
			Assert.Equal(Shape.Paper, game.Player1Shape.Value);
			Assert.Equal(Shape.Rock, game.Player2Shape.Value);
			Assert.Equal(Outcome.Player1Wins, game.Outcome);
		}

		[Fact]
		public async Task FinishesGameCorrectlyAsync()
		{
			var gameRepositoryMock = new GameRepositoryMock();
			var interactor = new FinishGameInteractor(gameRepositoryMock);
			var unfinishedGame = new Game();
			unfinishedGame.ThrowPlayer1(Shape.Paper);
			const string identifier = "UnitTestIdentifier";
			gameRepositoryMock.Games.Add(identifier, unfinishedGame);
			var request = new FinishGameInteractor.Request()
			{
				Identifier = identifier,
				Shape = Shape.Scissors
			};
			var response = await interactor.HandleRequestAsync(request);
			Assert.Equal(Outcome.Player2Wins, response.Outcome);
			var gameItem = gameRepositoryMock.Games.Single();
			Assert.Equal(identifier, gameItem.Key);
			var game = gameItem.Value;
			Assert.Equal(Shape.Paper, game.Player1Shape.Value);
			Assert.Equal(Shape.Scissors, game.Player2Shape.Value);
			Assert.Equal(Outcome.Player2Wins, game.Outcome);
		}
	}
}
