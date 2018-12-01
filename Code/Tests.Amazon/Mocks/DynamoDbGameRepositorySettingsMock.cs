using System;
using RockPaperScissors.Amazon.Repositories;

namespace RockPaperScissors.Tests.Amazon.Mocks
{
	internal class DynamoDbGameRepositorySettingsMock : IDynamoDbGameRepositorySettings
	{
		public string AccessKey => "YOUR_ACCESS_KEY_GOES_HERE";

		public string SecretKey => "YOUR_SECRET_KEY_GOES_HERE";

		public string RegionName => "us-east-1";

		public string TableName => "RockPaperScissorsIntegrationTesting";

		public string KeyName => "Identifier";
	}
}
