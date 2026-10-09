using System.Configuration;
using System.Data.SqlClient;
namespace Matab
{
    internal static class DatabaseSettings
    {
        internal static string ConnectionString
        {
            get
            {
                var setting = ConfigurationManager.ConnectionStrings["MatabDatabase"];
                if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                    throw new ConfigurationErrorsException("MatabDatabase connection string is missing in app.config.");
                return setting.ConnectionString;
            }
        }
        internal static SqlConnection CreateConnection() { return new SqlConnection(ConnectionString); }
    }
}
