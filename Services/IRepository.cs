using HunGivenNames.Data;

namespace HunGivenNames.Services;

public interface IRepository
{
	void Save(Database database);

	Database Load();
}