using System;

namespace RockPaperScissors.Entities
{
	[Serializable]
	public class Game
	{
		public Shape? Player1Shape { get; private set; }
		public Shape? Player2Shape { get; private set; }
		public Outcome Outcome { get; private set; }

		public void ThrowPlayer1(Shape shape)
		{
			Player1Shape = shape;
			CalculateOutcome();
		}

		public void ThrowPlayer2(Shape shape)
		{
			Player2Shape = shape;
			CalculateOutcome();
		}

		private void CalculateOutcome()
		{
			if (!Player1Shape.HasValue || !Player2Shape.HasValue)
			{
				Outcome = Outcome.Undecided;
				return;
			}
			if (Player1Shape.Value == Player2Shape.Value)
			{
				Outcome = Outcome.Tie;
				return;
			}
			if (LeftBeatsRight(Player1Shape.Value, Player2Shape.Value))
				Outcome = Outcome.Player1Wins;
			else
				Outcome = Outcome.Player2Wins;
		}

		private bool LeftBeatsRight(Shape left, Shape right) =>
			left == Shape.Rock && right == Shape.Scissors ||
			left == Shape.Paper && right == Shape.Rock ||
			left == Shape.Scissors && right == Shape.Paper;
	}
}
