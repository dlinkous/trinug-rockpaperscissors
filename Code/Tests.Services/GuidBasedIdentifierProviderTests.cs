using System;
using Xunit;
using RockPaperScissors.Services;

namespace RockPaperScissors.Tests.Services
{
	public class GuidBasedIdentifierProviderTests
	{
		[Fact]
		public void BasicGenerationTest()
		{
			var provider = new GuidBasedIdentifierProvider();
			var identifier = provider.Generate();
			Assert.NotNull(identifier);
			const int identifierLength = 8;
			Assert.Equal(identifierLength, identifier.Length);
		}
	}
}
