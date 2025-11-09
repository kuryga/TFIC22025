using System.Data.SqlClient;

namespace Utilities
{
    public class ConnectionSingleton
    {
        private static SqlConnection connection;

        private static SqlConnection constructor()
        {
            return new SqlConnection(RegReader.read("conection"));
        }

        public static SqlConnection getConnection()
        {
            if (connection == null)
            {
                connection = constructor();
            }
            return connection;
        }
    }
}
