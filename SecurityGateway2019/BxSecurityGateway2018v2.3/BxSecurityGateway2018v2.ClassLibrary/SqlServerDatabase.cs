using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace BxSecurityGateway2018v2.ClassLibrary
{
    internal class SqlServerDatabase : DatabaseBase
    {
        internal override string ProviderInvariantName
        {
            get
            {
                return "System.Data.SqlClient";
            }
        }

        public SqlServerDatabase()
        {
        }

        internal override void AddUserInDB(string user, string password, string[] roles, DbProviderFactory factory, DbConnection connection)
        {
            DbCommand command = DatabaseBase.GetCommand(connection);
            try
            {
                base.ExecuteNoneQuery(string.Format("sp_addlogin '{0}', '{1}'", user, password), command);
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number != 15025)
                {
                    throw;
                }
            }
            string serverVersion = connection.ServerVersion;
            try
            {
                serverVersion.CompareTo("09.00.00.0");
            }
            catch (SqlException sqlException1)
            {
                throw sqlException1;
            }
            try
            {
                base.ExecuteNoneQuery(string.Format("sp_adduser '{0}'", user), command);
            }
            catch (SqlException sqlException2)
            {
                if (sqlException2.Number != 15023)
                {
                    throw;
                }
            }
            if (roles != null)
            {
                string[] strArrays = roles;
                for (int i = 0; i < (int)strArrays.Length; i++)
                {
                    string str = strArrays[i];
                    if (string.Compare(str, "public", true) != 0)
                    {
                        base.ExecuteNoneQuery(string.Format("sp_addrolemember '{0}', '{1}'", str, user), command);
                    }
                }
            }
            base.ExecuteNoneQuery(string.Format("sp_password null,'{0}','{1}'", password, user), command);
        }

        internal override void DeleteLogin(string user, DBInfoDTO dmInfoDto, DbProviderFactory factory, DbCommand cmd)
        {
            try
            {
                base.ExecuteNoneQuery(string.Format("sp_droplogin '{0}'", user), cmd);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        internal override void DeleteRoles(string user, DbProviderFactory factory, DbConnection connection)
        {
            DbCommand command = DatabaseBase.GetCommand(connection);
            List<string> strs = new List<string>();
            string[] database = new string[] { "SELECT sysusers1.name AS [role] FROM dbo.sysmembers sysmembers INNER JOIN dbo.sysusers sysusers1 ON sysmembers.groupuid = sysusers1.uid inner join ", connection.Database, "..sysusers sysusers2 ON sysmembers.memberuid = sysusers2.uid WHERE sysusers2.name = '", user, "'" };
            using (DbDataReader dbDataReaders = base.ExecuteReader(string.Concat(database), command))
            {
                while (dbDataReaders.Read())
                {
                    strs.Add(dbDataReaders.GetString(0));
                }
            }
            foreach (string str in strs)
            {
                try
                {
                    string[] strArrays = new string[] { "sp_droprolemember '", str, "','", user, "'" };
                    base.ExecuteNoneQuery(string.Concat(strArrays), command);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }

        internal override void DeleteUser(string user, DBInfoDTO dmInfoDto, DbProviderFactory factory, DbCommand cmd)
        {
            try
            {
                base.ExecuteNoneQuery(string.Format("sp_dropuser '{0}'", user), cmd);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        internal override string GetConnectionString(DBInfoDTO db)
        {
            object[] server = new object[] { db.Server, db.DatabaseInstance, db.DatabaseUserID, db.DatabasePassword, db.ServerPortNo };
            return string.Format("Server={0},{4}; Database={1}; User={2}; Password={3};", server);
        }
    }
}