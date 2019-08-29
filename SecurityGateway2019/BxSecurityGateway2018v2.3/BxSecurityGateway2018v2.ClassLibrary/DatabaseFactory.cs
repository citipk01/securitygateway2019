namespace BxSecurityGateway2018v2.ClassLibrary
{
    internal class DatabaseFactory
    {
        public DatabaseFactory()
        {
        }

        internal static DatabaseBase Create(DBInfoDTO db)
        {
            if (db.DatabaseType == "1")
            {
                return new SqlServerDatabase();
            }
            if (db.DatabaseType == "3")
            {
                return new SqlServerDatabase();
            }
            return null;
        }
    }
}
