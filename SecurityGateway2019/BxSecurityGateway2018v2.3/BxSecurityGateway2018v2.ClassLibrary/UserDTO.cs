using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BxSecurityGateway2018v2.ClassLibrary
{
    class UserDTO
    {
        public UserDTO()
        {
        }

        private string name;
        private string changePassword;
        private string returnCode;
        private string lastLoginDate;
        private string badLoginCount;
        private string rolesFuncionesDelUsuario;
        private string databaseCount;

        public string toString(){
            return string.Format("name={0} , changePassword={1} , returnCode={2} , lastLoginDate={3} , badLoginCount={4} , rolesFuncionesDelUsuario={5} , databaseCount={6}",name , changePassword , returnCode , lastLoginDate , badLoginCount , rolesFuncionesDelUsuario , databaseCount);
        }

        public string getName() { return name; }
        public void setName(string name) { this.name = name; }

        public string getChangePassword() { return changePassword; }
        public void setChangePassword(string changePassword) { this.changePassword = changePassword; }

        public string getReturnCode() { return returnCode; }
        public void setReturnCode(string returnCode) { this.returnCode = returnCode; }

        public string getLastLoginDate() { return lastLoginDate; }
        public void setLastLoginDate(string lastLoginDate) { this.lastLoginDate = lastLoginDate; }
        public string getBadLoginCount() { return badLoginCount; }
        public void setBadLoginCount(string badLoginCount) { this.badLoginCount = badLoginCount; }
        public string getFuncionesDelUsuario() { return rolesFuncionesDelUsuario; }
        public void setFuncionesDelUsuario(string rolesFuncionesDelUsuario) { this.rolesFuncionesDelUsuario = rolesFuncionesDelUsuario; }

        public string getDatabaseCount() { return databaseCount; }
        public void setDatabaseCount(string databaseCount) { this.databaseCount = databaseCount; }

        public string ReturnCode { get; internal set; }
        public string CsCredUser { get; internal set; }
        public string CSCredUser { get; internal set; }
        public string ChangePassword { get; internal set; }
        public DBInfoDTO[] DbInfo { get; internal set; }
        public string Functions { get; internal set; }
    }
}
