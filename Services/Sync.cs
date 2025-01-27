using System.Text.Json;
using HunGivenNames.Data;

namespace HunGivenNames.Services;

public class Sync(IHttpClientFactory httpClientFactory, IConfiguration configuration, IRepository repository, ILogger<Sync> logger) : ISync
{
	private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
	
	private readonly string _databaseFile = configuration.GetValue<string>("DatabaseFile")
		?? throw new Exception("Unable to load configuration \"DatabaseFile\" from appsettings.json.");

	private readonly string _femaleNamesUri = configuration.GetValue<string>("FemaleNamesUri")
		?? throw new Exception("Unable to load configuration \"FemaleNamesUri\" from appsettings.json.");

	private readonly string _maleNamesUri = configuration.GetValue<string>("MaleNamesUri")
		?? throw new Exception("Unable to load configuration \"MaleNamesUri\" from appsettings.json.");

    public void Fetch()
    {
        if (!File.Exists(_databaseFile))
        {
			logger.LogInformation("Database file missing.");

            Download();
			return;
        }
		
		DateTime createdAt = File.GetCreationTime(_databaseFile);

		if (createdAt < DateTime.Now && createdAt.Month != DateTime.Now.Month)
		{
			logger.LogInformation("Database file out of date.");
			
			Download();
			return;
		}

		logger.LogInformation("Database file in sync.");
    }

	private void Download()
	{
		logger.LogInformation("Redownloading.");

		string femaleNames = _httpClient.Send(new(HttpMethod.Get, _femaleNamesUri)).Content.ReadAsStringAsync().Result;
		string maleNames = _httpClient.Send(new(HttpMethod.Get, _maleNamesUri)).Content.ReadAsStringAsync().Result;

		Database database = new(
			[.. femaleNames.Split("\n", StringSplitOptions.TrimEntries).Skip(1)],
			[.. maleNames.Split("\n", StringSplitOptions.TrimEntries).Skip(1)]
		);

		logger.LogInformation("Synced {} names.", database.Female.Count + database.Male.Count);

		repository.Save(database);
	}
}