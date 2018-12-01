using System;
using System.Threading.Tasks;
using RockPaperScissors.Entities;
using RockPaperScissors.Interactors.Repositories;

namespace RockPaperScissors.Interactors
{
	public class FinishGameInteractor
	{
		public class Request
		{
			public string Identifier { get; set; }
			public Shape Shape { get; set; }
		}

		public class Response
		{
			public Outcome Outcome { get; set; }
		}

		private readonly IGameRepository gameRepository;

		public FinishGameInteractor(IGameRepository gameRepository) =>
			this.gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));

		public async Task<Response> HandleRequestAsync(Request request)
		{
			var game = await gameRepository.ReadAsync(request.Identifier);
			if (game == null) return new Response() { Outcome = Outcome.Undecided };
			if (game.Outcome != Outcome.Undecided) return new Response() { Outcome = game.Outcome };
			game.ThrowPlayer2(request.Shape);
			await gameRepository.UpdateAsync(request.Identifier, game);
			return new Response() { Outcome = game.Outcome };
		}
	}
}
