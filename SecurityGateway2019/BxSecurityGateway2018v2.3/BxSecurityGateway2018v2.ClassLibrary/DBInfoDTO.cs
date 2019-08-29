using System;

namespace BxSecurityGateway2018v2.ClassLibrary
{
	internal class DBInfoDTO
	{
		private string cSCredPwd;

		private string databaseInstance;

		private string databasePassword;

		private string databaseRole;

		private string databaseType;

		private string databaseUserID;

		private string[] roles;

		private string server;

		private string serverPortNo;

		private string tableSpace;

		public string CSCredPwd
		{
			get
			{
				return this.cSCredPwd;
			}
			set
			{
				this.cSCredPwd = value;
			}
		}

		public string DatabaseInstance
		{
			get
			{
				return this.databaseInstance;
			}
			set
			{
				this.databaseInstance = value;
			}
		}

		public string DatabasePassword
		{
			get
			{
				return this.databasePassword;
			}
			set
			{
				this.databasePassword = value;
			}
		}

		public string DatabaseRole
		{
			get
			{
				return this.databaseRole;
			}
			set
			{
				this.databaseRole = value;
				this.roles = this.databaseRole.Split(",".ToCharArray());
			}
		}

		public string DatabaseType
		{
			get
			{
				return this.databaseType;
			}
			set
			{
				this.databaseType = value;
			}
		}

		public string DatabaseUserID
		{
			get
			{
				return this.databaseUserID;
			}
			set
			{
				this.databaseUserID = value;
			}
		}

		public string[] Roles
		{
			get
			{
				return this.roles;
			}
			set
			{
				this.roles = value;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
			set
			{
				this.server = value;
			}
		}

		public string ServerPortNo
		{
			get
			{
				return this.serverPortNo;
			}
			set
			{
				this.serverPortNo = value;
			}
		}

		public string TableSpace
		{
			get
			{
				return this.tableSpace;
			}
			set
			{
				this.tableSpace = value;
			}
		}

		public DBInfoDTO()
		{
		}
	}
}