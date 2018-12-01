using System;
using Xunit;
using RockPaperScissors.Entities;

namespace RockPaperScissors.Tests.Entities
{
	public class GameTests
	{
		[Fact]
		public void WhenGameIsNew_GameIsUndecided()
		{
			var game = new Game();
			Assert.Equal(Outcome.Undecided, game.Outcome);
			Assert.Null(game.Player1Shape);
			Assert.Null(game.Player2Shape);
		}

		[Fact]
		public void WhenOnlyPlayer1HasThrown_GameIsUndecided()
		{
			var game = new Game();
			game.ThrowPlayer1(Shape.Paper);
			Assert.Equal(Outcome.Undecided, game.Outcome);
		}

		[Fact]
		public void WhenOnlyPlayer2HasThrown_GameIsUndecided()
		{
			var game = new Game();
			game.ThrowPlayer2(Shape.Paper);
			Assert.Equal(Outcome.Undecided, game.Outcome);
		}

		[Fact]
		public void WhenBothPlayersThrow_ThrowsAreSavedCorrectly()
		{
			var game = new Game();
			game.ThrowPlayer1(Shape.Paper);
			game.ThrowPlayer2(Shape.Rock);
			Assert.Equal(Shape.Paper, game.Player1Shape.Value);
			Assert.Equal(Shape.Rock, game.Player2Shape.Value);
		}

		[Fact]
		public void WhenThrowsAreTheSame_OutcomeIsTie()
		{
			void ThrowSameAndAssertTie(Shape shape)
			{
				var game = new Game();
				game.ThrowPlayer1(shape);
				game.ThrowPlayer2(shape);
				Assert.Equal(Outcome.Tie, game.Outcome);
			}
			ThrowSameAndAssertTie(Shape.Paper);
			ThrowSameAndAssertTie(Shape.Rock);
			ThrowSameAndAssertTie(Shape.Scissors);
		}

		[Fact]
		public void WhenPlayerWins_OutcomeIsCorrect()
		{
			void ThrowBothAndAssertOutcome(Shape shape1, Shape shape2, Outcome outcome)
			{
				var game = new Game();
				game.ThrowPlayer1(shape1);
				game.ThrowPlayer2(shape2);
				Assert.Equal(outcome, game.Outcome);
			}
			ThrowBothAndAssertOutcome(Shape.Rock, Shape.Scissors, Outcome.Player1Wins);
			ThrowBothAndAssertOutcome(Shape.Paper, Shape.Rock, Outcome.Player1Wins);
			ThrowBothAndAssertOutcome(Shape.Scissors, Shape.Paper, Outcome.Player1Wins);
			ThrowBothAndAssertOutcome(Shape.Rock, Shape.Paper, Outcome.Player2Wins);
			ThrowBothAndAssertOutcome(Shape.Paper, Shape.Scissors, Outcome.Player2Wins);
			ThrowBothAndAssertOutcome(Shape.Scissors, Shape.Rock, Outcome.Player2Wins);
		}
	}
}
