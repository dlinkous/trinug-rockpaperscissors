using System;

namespace RockPaperScissors.Amazon.Repositories
{
	public class EnvironmentDynamoDbGameRepositorySettings : IDynamoDbGameRepositorySettings
	{
		public string AccessKey =>
			Environment.GetEnvironmentVariable("AccessKey");

		public string SecretKey =>
			Environment.GetEnvironmentVariable("SecretKey");

		public string RegionName =>
			Environment.GetEnvironmentVariable("RegionName");

		public string TableName =>
			Environment.GetEnvironmentVariable("TableName");

		public string KeyName =>
			Environment.GetEnvironmentVariable("KeyName");
	}
}
