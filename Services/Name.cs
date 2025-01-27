using HunGivenNames.Data;

namespace HunGivenNames.Services;

public class Name(ISync sync, IConfiguration configuration, IRepository repository, ILogger<Name> logger) : IName
{
	private readonly string _databaseFile = configuration.GetValue<string>("DatabaseFile")
		?? throw new Exception("Unable to load configuration \"DatabaseFile\" from appsettings.json.");

    public string Gender(string name)
    {
		sync.Fetch();

		Database database = repository.Load();

		bool isFemale = database.Female.Select((x) => x.ToLowerInvariant()).Contains(name.ToLowerInvariant());
		bool isMale = database.Male.Select((x) => x.ToLowerInvariant()).Contains(name.ToLowerInvariant());

		logger.LogInformation(
			"Searching name: {} -> female: {}, male: {}.",
			name,
			isFemale,
			isMale
		);

		if (isFemale && isMale) return "unisex";
		
		if (isFemale) return "female";
		if (isMale) return "male";

		return "unknown";
    }
}