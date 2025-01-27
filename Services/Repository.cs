using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using HunGivenNames.Data;

namespace HunGivenNames.Services;

public class Repository(IConfiguration configuration) : IRepository
{
	private readonly string _databaseFile = configuration.GetValue<string>("DatabaseFile")
		?? throw new Exception("Unable to load configuration \"DatabaseFile\" from appsettings.json.");

    public Database Load() => JsonSerializer.Deserialize<Database>(File.ReadAllText(_databaseFile))
		?? throw new Exception("Could not deserialize the database file.");

    public void Save(Database database) => File.WriteAllText(
		_databaseFile,
		JsonSerializer.Serialize(database)
	);
}