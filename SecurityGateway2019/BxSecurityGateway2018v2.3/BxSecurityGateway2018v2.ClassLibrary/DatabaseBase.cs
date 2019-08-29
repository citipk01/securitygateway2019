using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace BxSecurityGateway2018v2.ClassLibrary
{
    internal abstract class DatabaseBase
    {
        private List<string> ConnectionStrings = new List<string>();

        internal abstract string ProviderInvariantName
        {
            get;
        }

        protected DatabaseBase()
        {
        }

        internal void AddUser(DBInfoDTO db, string user)
        {
            string cSCredPwd = db.CSCredPwd;
            DbProviderFactory providerFactory = this.GetProviderFactory();
            using (DbConnection connectionString = providerFactory.CreateConnection())
            {
                connectionString.ConnectionString = this.GetConnectionString(db);
                connectionString.Open();
                this.RemoveUser(db, user, true);
                this.DeleteRoles(user, providerFactory, connectionString);
                this.AddUserInDB(user, db.CSCredPwd, db.Roles, providerFactory, connectionString);
            }
        }

        internal abstract void AddUserInDB(string user, string password, string[] roles, DbProviderFactory factory, DbConnection connection);

        private void AddUserInDB(string dbUserName, DBInfoDTO dbInfo, string connectionString)
        {
        }

        internal abstract void DeleteLogin(string user, DBInfoDTO dmInfoDto, DbProviderFactory factory, DbCommand cmd);

        internal abstract void DeleteRoles(string user, DbProviderFactory factory, DbConnection connection);

        internal abstract void DeleteUser(string user, DBInfoDTO dmInfoDto, DbProviderFactory factory, DbCommand cmd);

        protected void ExecuteNoneQuery(string sql, DbCommand cmd)
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        protected DbDataReader ExecuteReader(string sql, DbCommand cmd)
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            return cmd.ExecuteReader();
        }

        public static DbCommand GetCommand(DbConnection connection)
        {
            DbCommand dbCommand = connection.CreateCommand();
            dbCommand.CommandTimeout = 90;
            return dbCommand;
        }

        internal abstract string GetConnectionString(DBInfoDTO dBInfoDTO);

        private DbProviderFactory GetProviderFactory()
        {
            return DbProviderFactories.GetFactory(this.ProviderInvariantName);
        }

        private void Kill(string user, DbConnection connection)
        {
            DbCommand dbCommand = this.GetProviderFactory().CreateCommand();
            dbCommand.Connection = connection;
            dbCommand.CommandText = string.Concat("sp_who '", user, "'");
            List<string> strs = new List<string>();
            using (DbDataReader dbDataReaders = dbCommand.ExecuteReader())
            {
                while (dbDataReaders.Read())
                {
                    strs.Add(dbDataReaders["spid"].ToString());
                }
            }
            foreach (string str in strs)
            {
                try
                {
                    dbCommand.CommandText = string.Concat("kill ", str);
                    dbCommand.ExecuteNonQuery();
                }
                catch
                {
                }
            }
        }

        internal void RemoveLogin(DBInfoDTO dmInfoDto, string user)
        {
            try
            {
                DbProviderFactory providerFactory = this.GetProviderFactory();
                using (DbConnection connectionString = providerFactory.CreateConnection())
                {
                    connectionString.ConnectionString = this.GetConnectionString(dmInfoDto);
                    connectionString.Open();
                    this.Kill(user, connectionString);
                    this.DeleteLogin(user, dmInfoDto, providerFactory, DatabaseBase.GetCommand(connectionString));
                }
            }
            catch
            {
            }
        }

        internal void RemoveUser(DBInfoDTO dmInfoDto, string user, bool all)
        {
            DbProviderFactory providerFactory = this.GetProviderFactory();
            using (DbConnection connectionString = providerFactory.CreateConnection())
            {
                connectionString.ConnectionString = this.GetConnectionString(dmInfoDto);
                connectionString.Open();
                this.DeleteUser(user, dmInfoDto, providerFactory, DatabaseBase.GetCommand(connectionString));
            }
        }
    }
}