using HomeCalc.Model.DbConnectionWrappers;

namespace HomeCalc.Model.DbService
{
    public interface IDatabaseManager
    {
        StorageConnection GetConnection();
    }
}
