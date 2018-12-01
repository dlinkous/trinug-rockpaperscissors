using System;
using System.Collections.Generic;
using RockPaperScissors.Interactors.Providers;

namespace RockPaperScissors.Tests.Interactors.Mocks
{
	internal class IdentifierProviderMock : IIdentifierProvider
	{
		internal Queue<string> Identifiers { get; } = new Queue<string>();

		public string Generate() => Identifiers.Dequeue();
	}
}
