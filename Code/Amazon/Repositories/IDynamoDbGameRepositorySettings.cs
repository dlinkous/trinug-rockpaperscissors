using System;

namespace RockPaperScissors.Amazon.Repositories
{
	public interface IDynamoDbGameRepositorySettings
	{
		string AccessKey { get; }
		string SecretKey { get; }
		string RegionName { get; }
		string TableName { get; }
		string KeyName { get; }
	}
}
