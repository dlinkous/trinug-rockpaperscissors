using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using SimpleInjector;
using RockPaperScissors.Amazon.Repositories;
using RockPaperScissors.Interactors;
using RockPaperScissors.Interactors.Providers;
using RockPaperScissors.Interactors.Repositories;
using RockPaperScissors.Services;

namespace RockPaperScissors.Amazon.Invocation
{
	public class LambdaInvoker
	{
		private readonly Container container;

		public LambdaInvoker()
		{
			container = new Container();
			container.Register<IDynamoDbGameRepositorySettings, EnvironmentDynamoDbGameRepositorySettings>();
			container.Register<IIdentifierProvider, GuidBasedIdentifierProvider>();
			container.Register<IGameRepository, DynamoDbGameRepository>();
			container.Register<BeginGameInteractor>();
			container.Register<FinishGameInteractor>();
			container.Register<CheckGameInteractor>();
			container.Verify();
		}

		[LambdaSerializer(typeof(JsonSerializer))]
		public async Task<BeginGameInteractor.Response> BeginGameAsync(BeginGameInteractor.Request request) =>
			await container.GetInstance<BeginGameInteractor>().HandleRequestAsync(request);

		[LambdaSerializer(typeof(JsonSerializer))]
		public async Task<FinishGameInteractor.Response> FinishGameAsync(FinishGameInteractor.Request request) =>
			await container.GetInstance<FinishGameInteractor>().HandleRequestAsync(request);

		[LambdaSerializer(typeof(JsonSerializer))]
		public async Task<CheckGameInteractor.Response> CheckGameAsync(CheckGameInteractor.Request request) =>
			await container.GetInstance<CheckGameInteractor>().HandleRequestAsync(request);
	}
}
