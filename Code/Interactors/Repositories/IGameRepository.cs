using System;
using System.Threading.Tasks;
using RockPaperScissors.Entities;

namespace RockPaperScissors.Interactors.Repositories
{
	public interface IGameRepository
	{
		Task CreateAsync(string identifier, Game game);
		Task<Game> ReadAsync(string identifier);
		Task UpdateAsync(string identifier, Game game);
	}
}
