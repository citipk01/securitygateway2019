using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Collections;
using System.Reflection;
using System.Text;
//2019
//[assembly: AssemblyKeyFileAttribute("BxSNK.snk")]
//[assembly: AssemblyDelaySignAttribute(true)]
namespace BxSecurityGateway2018v2.ClassLibrary//BxSGWAY//.ClassLibrary
{
    /// <summary>
    /// Se encarga de realizar el login y obtener los roles de un usuario
    /// </summary>
    //[ClassInterface(ClassInterfaceType.AutoDispatch)]

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SGLogin
    {
        SGInterface sgInterface = FactorySGInstance.getInstanceInterface();

        public SGLogin()
        {
            LOG.grabarLogSeparador();
            SGLogin.LastLoginFunctionIDs = "";
            string item = "";
            item = ConfigurationManager.AppSettings["SecurityGateway.Url"];
            if (!string.IsNullOrEmpty(item))
            {
                this.SecurityGateWayServer = ConfigurationManager.AppSettings["SecurityGateway.Url"];
            }
            item = ConfigurationManager.AppSettings["SecurityGateway.ApplicationID"];
            if (!string.IsNullOrEmpty(item))
            {
                this.ApplicationID = int.Parse(item);
            }

            LOG.grabarLog("BxSG - Se inició la clase SGLogin");
            //LOG.grabarLog("BxSG - SecurityGateWayServer: " + SecurityGateWayServer);
            //LOG.grabarLog("BxSG - ApplicationID        : " + ApplicationID);
        }
        #region Desarrollo 2018 - Config.xml parametrizable
        [Obsolete("loginHardcoded is deprecated, please use login instead.")]
        public void loginHardcoded()
        {
            string configFile = @"C:\Windows\SysWOW64\config.xml";
            string systemAppId = "20322020";
            string ssoLogin = "as54421";
            string ssoPassword = "prograK01";
            string response = "";
            int returnValue = 0;
            returnValue = sgInterface.noGuiLogin(systemAppId, "SGDemoNET", configFile, ssoLogin, ssoPassword, out response);

            if (returnValue == 1)
            {
                LOG.grabarLogException("Buxis PK: Estás Autenticado como: " + ssoLogin);
            }
            else
            {
                LOG.grabarLogException("Buxis PK: Error en la Autenticación");
            }

        }
        #endregion

        #region BxSG 2019 - 0) Atributos - GETTERS y SETTERS
        public string SecurityGateWayServer;
        public int ApplicationID;
        private string _Name = "";
        public string Name
        {
            get
            {
                return this._Name;
            }
        }
        public bool HasToChangePassword;
        public int ReturnCode = -1;
        private string ipAddress;
        private string configXmlFilePath;
        public string getConfigXmlFilePath()
        {
            return this.configXmlFilePath;
        }
        public void setConfigXmlFilePath(string configXml)
        {
            configXmlFilePath = configXml;
            LOG.grabarLog("setConfigXmlFilePath: " + configXmlFilePath);
        }
        #endregion

        #region BxSG 2019 - 0) Métodos Globales - Request, GetIPAddress *LoginUser
        private string Request(string request, string data)
        {
            string end;
            try
            {
                HttpWebRequest version10 = (HttpWebRequest)WebRequest.Create(new Uri(request));
                version10.Proxy = null;
                version10.KeepAlive = false;
                version10.Method = "POST";
                version10.ProtocolVersion = HttpVersion.Version10;
                byte[] bytes = (new ASCIIEncoding()).GetBytes(data);
                version10.ContentType = "application/x-www-form-urlencoded";
                version10.ContentLength = (long)((int)bytes.Length);
                version10.GetRequestStream().Write(bytes, 0, (int)bytes.Length);
                using (Stream responseStream = ((HttpWebResponse)version10.GetResponse()).GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("iso-8859-15")))
                    {
                        end = streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception exception1)
            {
                LOG.grabarLog("Error al conectarse con SG. " + exception1.Message);
                return null;
            }
            return end;
        }

        private string GetIPAddress()
        {
            //string ipAddress = "";
            if (this.ipAddress == null || this.ipAddress.Trim().Length == 0)
            {
                IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                if (addressList == null && (int)addressList.Length <= 0)
                {
                    LOG.grabarLog("BxSG - No se pudo determinar la IP del cliente");
                }
                this.ipAddress = addressList[0].ToString();
            }
            return this.ipAddress;
        }

        private string LoginUser(string user, string password, int appId)
        {
            LOG.grabarLog(String.Format("BxSG - Logueando usuario: {0} , AppID: {1}", user, appId));
            string securityGateWayServer = this.SecurityGateWayServer;
            object[] objArray = new object[] { user, password, appId, this.GetIPAddress(), "SGLibraryNET_5.5" };
            string str = string.Format("aMethodName=authenticateCredentials&aLoginID={0}&aPassword={1}&aSystemID={2}&userIP={3}&aUseSessionDBUser=false&aAgentVersion={4}", objArray);
            if (string.IsNullOrEmpty(str))
            {
                LOG.grabarLogException("Error leyendo la información de SG al efectuar 'authenticateCredentials'");
            }
            return this.Request(securityGateWayServer, str);
        }


        #endregion

        #region BxSG 2019 - 1) Login
        /// <summary>
        /// Recibe las credenciales de usuario para el login SSO
        /// Previamente Debe estar seteado el ApplicationID y el ConfigXmlFilePath
        /// </summary>
        /// <param name="ApplicationID">systemAppId</param>
        /// <param name="ConfigXmlFilePath">configXmlFilePath</param>
        /// <param name="ssoLoginUsr">Usuario SSO</param>
        /// <param name="ssoLoginPass">Clave SSO</param>
        public void Login(string ssoLoginUsr, string ssoLoginPass)
        {
            LOG.grabarLog("BxSG - Se inicia proceso Login: sgInterface.noGuiLogin");
            string applicationName = "SGDemoNET";
            string response = "";
            int returnValue = 0;

            string systemAppId = ApplicationID.ToString();
            returnValue = sgInterface.noGuiLogin(systemAppId, applicationName, getConfigXmlFilePath(), ssoLoginUsr, ssoLoginPass, out response);
            LOG.grabarLog(String.Format("BxSG - PARÁMETROS - systemAppId: {0} applicationName: {1} getConfigXmlFilePath: {2}  ssoLoginUsr: {3}", systemAppId, applicationName, getConfigXmlFilePath(), ssoLoginUsr));
            // LOG.grabarLog("SecurityGateWayServer: " + this.SecurityGateWayServer);
            /// TODO: LEER el parámetro de server del ConfigXmlFilePath
            //LOG.grabarLog("BxSG - response : " + response);
            //sgInterface.RsmsLogin(systemAppId, applicationName, configXmlFilePath, out response); // Este es el login form
            //LOG.grabarLog("BxSG - returnValue : " + returnValue);
            //LOG.grabarLog("BxSG - response : " + response);

            LOG.grabarLog("BxSG - Login: Comienza proceso Validaciones");
            if (returnValue != 1 && returnValue != 2)
            {
                LOG.grabarLog("BxSG - Error en la Autenticación - Return value no fue el esperado, es distinto de 1");
                if (string.IsNullOrWhiteSpace(systemAppId) || systemAppId == "0")
                    LOG.grabarLogException("Campo con valor nulo: 'systemAppId'");
                if (string.IsNullOrWhiteSpace(applicationName))
                    LOG.grabarLogException("Campo con valor nulo: 'applicationName'");
                if (string.IsNullOrWhiteSpace(configXmlFilePath))
                    LOG.grabarLogException("Campo con valor nulo: 'configXmlFilePath'");
                if (string.IsNullOrWhiteSpace(ssoLoginUsr))
                    LOG.grabarLogException("Campo con valor nulo: 'ssoLoginUsr'");
                if (string.IsNullOrWhiteSpace(ssoLoginPass))
                    LOG.grabarLogException("Campo con valor nulo: 'ssoLoginPass'");
                LOG.grabarLogException(response);
            }
            else
            {
                LOG.grabarLog("BxSG - Éxito: Estás autenticado como: " + ssoLoginUsr);
                UserDTO objUsuario = requestObtenerInfoUsuarioLogueado(ssoLoginUsr, ssoLoginPass);

                LOG.grabarLog(String.Format("BxSG - objUsuario.getChangePassword()). {0}", objUsuario.getChangePassword()));
                LOG.grabarLog(String.Format("BxSG - Convert.ToBoolean(objUsuario.getChangePassword()). {0}", Convert.ToBoolean(objUsuario.getChangePassword())));
                if (Convert.ToBoolean(objUsuario.getChangePassword()))
                    LOG.grabarLogException("BxSG - Cambiar contraseña del usuario. returnValue : " + returnValue);
            }
            LOG.grabarLog("BxSG - Login: Finaliza proceso Validaciones");
            LOG.grabarLog("BxSG - Finaliza proceso Login: sgInterface.noGuiLogin");
        }

        private UserDTO requestObtenerInfoUsuarioLogueado(string ssoLoginUsr, string ssoLoginPass)
        {
            LOG.grabarLog(String.Format("BxSG - Obteniendo información del usuario: {0}", ssoLoginUsr));

            LOG.grabarLog(String.Format("BxSG - Comienza request de información del usuario: {0}", ssoLoginUsr));
            // Obtener info del usuario
            string strRequest = this.LoginUser(ssoLoginUsr, ssoLoginPass, this.ApplicationID);
            LOG.grabarLog(String.Format("BxSG - Se finalizó el request de información. Se obtuvo: {0}", strRequest));

            //ReturnCode=0**DatabaseCount=0**ChangePassword=false**LastLoginDate=09252002**BadLoginCount=0**Name=Andrea,Salomon
            //ReturnCode=0**DatabaseCount=0**ChangePassword=false**LastLoginDate=09252002**BadLoginCount=0**Name=Andrea,Salomon
            UserDTO objUsuario = new UserDTO();

            string[] requestParam = strRequest.Split('*');

            foreach (String itemRequest in requestParam)
            {
                LOG.grabarLog(String.Format("BxSG - charSeparator * : " + itemRequest));
                if (!string.IsNullOrWhiteSpace(itemRequest))
                {

                    string[] ArrayItemRequest = itemRequest.Split('=');

                    //LOG.grabarLog(String.Format("BxSG - ArrayItemRequest:{0}  - valor:{1}", ArrayItemRequest));
                    switch (ArrayItemRequest[0])
                    {
                        case "ReturnCode":
                            objUsuario.setReturnCode(ArrayItemRequest[1]);
                            break;
                        case "DatabaseCount":
                            objUsuario.setDatabaseCount(ArrayItemRequest[1]);
                            break;
                        case "ChangePassword":
                            objUsuario.setChangePassword(ArrayItemRequest[1]);
                            break;
                        case "LastLoginDate":
                            objUsuario.setLastLoginDate(ArrayItemRequest[1]);
                            break;
                        case "BadLoginCount":
                            objUsuario.setBadLoginCount(ArrayItemRequest[1]);
                            break;
                        case "Name":
                            objUsuario.setName(ArrayItemRequest[1]);
                            break;
                    }
                }

            }

            LOG.grabarLog(String.Format("BxSG - DATOS USUARIO - {0}", objUsuario.toString()));


            LOG.grabarLog("BxSG - Se obtuvo la información del usuario con éxito");
            /// TODO: Elimnar this.user : this.user = objUsuario;
            this._Name = objUsuario.getName();
            this.HasToChangePassword = Convert.ToBoolean(objUsuario.getChangePassword());
            this.ReturnCode = Convert.ToInt16(objUsuario.getReturnCode());
            return objUsuario;
        }

        #endregion

        #region BxSG 2019 - 2) Obtener funciones/ Roles
        public string GetFunctionsStr(string ssoLoginUsr, string ssoLoginPass)
        {
            LOG.grabarLog(String.Format("BxSG - Comienza obtención de Funciones/Roles del usuario {0}", ssoLoginUsr));
            string strRequest = getBxFunctionsByRequest(ssoLoginUsr, ssoLoginPass);

            //ReturnCode=0||Functions=VAC_SOL.VIEW*VAC_SOL.UPD*
            string rolesFuncionesDelUsuario = "";
            string[] requestParam = strRequest.Split('|');
            foreach (String itemRequest in requestParam)
            {
                LOG.grabarLog(String.Format("BxSG - charSeparator * : " + itemRequest));
                if (!string.IsNullOrWhiteSpace(itemRequest))
                {

                    string[] ArrayItemRequest = itemRequest.Split('=');
                    switch (ArrayItemRequest[0])
                    {
                        case "ReturnCode":
                            this.ReturnCode = Convert.ToInt16(ArrayItemRequest[1]);
                            break;
                        case "Functions":
                            if (ArrayItemRequest[1].Length == 0)
                                LOG.grabarLog(string.Format("BxSG - No se encontraron funciones: {0}", ArrayItemRequest[1].Length));
                            else
                            {
                                rolesFuncionesDelUsuario = ArrayItemRequest[1].Replace('*', ',');
                                //if (rolesFuncionesDelUsuario[rolesFuncionesDelUsuario.Length - 1].Equals(","))
                                //    rolesFuncionesDelUsuario = rolesFuncionesDelUsuario.Remove(rolesFuncionesDelUsuario.Length -1);
                            }
                            break;
                    }
                }
            }

            return rolesFuncionesDelUsuario;

        }

        private string getBxFunctionsByRequest(string ssoLoginUsr, string ssoLoginPass)
        {
            LOG.grabarLog("BxSG - Comienza el request de funciones");
            string securityGateWayServer = this.SecurityGateWayServer;
            object[] objArray = new object[] { ssoLoginUsr, ssoLoginPass, this.ApplicationID, this.GetIPAddress() };
            LOG.grabarLog(String.Format("BxSG - PARÁMETROS - ssoLoginUsr: {0} appId: {1} sgServer: {2}  IP: {3}", ssoLoginUsr, ApplicationID, SecurityGateWayServer, GetIPAddress()));
            string str = string.Format("aMethodName=getSystemFunctionNames&aLoginID={0}&aPassword={1}&aSystemID={2}&userIP={3}", objArray);
            string strRequest = this.Request(securityGateWayServer, str);
            if ((strRequest == null || strRequest.Trim().Length <= 0) && string.IsNullOrEmpty(str))
            {
                throw new Exception("Error leyendo la información de SG al efectuar 'getSystemFunctionNames'.");
            }
            LOG.grabarLog(String.Format("BxSG - Finaliza el request de funciones. Se obtuvo: {0}", strRequest));
            return strRequest;
        }

        #endregion

        #region BxSG 2019 - 3) ChangePassword

        public void ChangePassword(string ssoLoginUsr, string oldPassword, string newPassword, string newPassword2)
        {
            //ReturnCode=0
            LOG.grabarLog("BxSG - Inicio de change Password");
            if (newPassword != newPassword2)
            {
                LOG.grabarLogException("Las contraseñas nuevas no coinciden");
            }

            this.ReturnCode = requestStateOfChangePassword(ssoLoginUsr, oldPassword, newPassword, newPassword2);
            if (this.ReturnCode != 0)
            {
                LOG.grabarLogException("Error al cambiar la contraseña");
            }
            else
                LOG.grabarLog("BxSG - Se cambiaron las claves");

            LOG.grabarLog("BxSG - Fin de change Password");
        }

        private int requestStateOfChangePassword(string ssoLoginUsr, string oldPassword, string newPassword, string newPassword2)
        {
            LOG.grabarLog(String.Format("BxSG - Comienza request de cambio de claves del usuario: {0}", ssoLoginUsr));

            string securityGateWayServer = this.SecurityGateWayServer;
            object[] objArray = new object[] { ssoLoginUsr, oldPassword, this.ApplicationID, this.GetIPAddress(), newPassword, newPassword2 };
            string str = string.Format("aMethodName=changeSMPwd&aLoginID={0}&oldPassword={1}&aSystemID={2}&userIP={3}&newPassword={4}&reconfirmPassword={5}&bChangeAndLogin=false", objArray);
            string strRequest = this.Request(securityGateWayServer, str);

            LOG.grabarLog(String.Format("BxSG - el request de información obtuvo: {0}", strRequest));
            //LOG.grabarLog(String.Format("BxSG - lectura de la información obtenida: {0}", strRequest));
            int returnCode = 100;
            string[] requestParam = strRequest.Split('*');

            foreach (String itemRequest in requestParam)
            {
                LOG.grabarLog(String.Format("BxSG - charSeparator * : " + itemRequest));
                if (!string.IsNullOrWhiteSpace(itemRequest))
                {
                    string[] ArrayItemRequest = itemRequest.Split('=');
                    switch (ArrayItemRequest[0])
                    {
                        case "ReturnCode":
                            returnCode = Convert.ToInt16(ArrayItemRequest[1]);
                            break;
                    }
                }
            }
            LOG.grabarLog("BxSG - Se finalizó el request de información");
            return returnCode;
        }
        #endregion

        #region BxSG 2019 - 4) Logout
        public void Logout(string ssoLoginUsr)
        {
            ssoLoginUsr = ssoLoginUsr.Trim();
            //string securityGateWayServer = this.SecurityGateWayServer;
            //object[] objArray = new object[] { "aMethodName=getDataBaseServerAssociations&aLoginID=", user, "&aSystemID=", this.ApplicationID, "&userIP=", this.GetIPAddress(), "&aUseSessionDBUser=true&aAgentVersion=SGLibraryNET_5.5" };
            //string str = this.Request(securityGateWayServer, string.Concat(objArray));
            //if (string.IsNullOrEmpty(str))
            //{
            //    throw new Exception("Error leyendo la información de SG al efectuar 'getDataBaseServerAssociations'.");
            //}
            UserDTO userDTO = requestLogoutUser(ssoLoginUsr);
            this.ReturnCode = int.Parse(userDTO.ReturnCode);
            LOG.grabarLog(String.Format("BxSG - ReturnCode: {0}", ReturnCode));
            LOG.grabarLog("BxSG - Comienza proceso de cierre de sesión por DB");
            if (userDTO.DbInfo != null)
            {
                for (int i = 0; i < (int)userDTO.DbInfo.Length; i++)
                {
                    DatabaseBase databaseBase = DatabaseFactory.Create(userDTO.DbInfo[i]);
                    if (databaseBase != null)
                    {
                        if (userDTO.CSCredUser == null || userDTO.CSCredUser.Length <= 0)
                        {
                            databaseBase.RemoveUser(userDTO.DbInfo[i], ssoLoginUsr, false);
                        }
                        else
                        {
                            databaseBase.RemoveUser(userDTO.DbInfo[i], userDTO.CSCredUser, true);
                        }
                    }
                }
                for (int j = 0; j < (int)userDTO.DbInfo.Length; j++)
                {
                    DatabaseBase databaseBase1 = DatabaseFactory.Create(userDTO.DbInfo[j]);
                    if (databaseBase1 != null)
                    {
                        if (userDTO.CSCredUser == null || userDTO.CSCredUser.Length <= 0)
                        {
                            databaseBase1.RemoveLogin(userDTO.DbInfo[j], ssoLoginUsr);
                        }
                        else
                        {
                            databaseBase1.RemoveLogin(userDTO.DbInfo[j], userDTO.CSCredUser);
                        }
                    }
                }
                LOG.grabarLog("BxSG - ÉXITO: Finaliza el proceso de cierre de sesión por DB");
            }
            else
                LOG.grabarLog("BxSG - Error userDTO.DbInfo is null");
        }
        private UserDTO requestLogoutUser(string ssoLoginUsr)
        {
            LOG.grabarLog(String.Format("BxSG - Comienza request de logout del usuario: {0}", ssoLoginUsr));

            string securityGateWayServer = this.SecurityGateWayServer;
            //object[] objArray = new object[] { ssoLoginUsr, this.ApplicationID, this.GetIPAddress() };
            //string str = string.Format("aMethodName=getDataBaseServerAssociations&aLoginID={0}&aSystemID={1}&userIP={2}&aUseSessionDBUser=true&aAgentVersion=SGLibraryNET_5.5", objArray);
            //string strRequest = this.Request(securityGateWayServer, str);

            object[] objArray = new object[] { "aMethodName=getDataBaseServerAssociations&aLoginID=", ssoLoginUsr, "&aSystemID=", this.ApplicationID, "&userIP=", this.GetIPAddress(), "&aUseSessionDBUser=true&aAgentVersion=SGLibraryNET_5.5" };
            string strRequest = this.Request(securityGateWayServer, string.Concat(objArray));

            LOG.grabarLog(String.Format("BxSG - el request de información obtuvo: {0}", strRequest));

            UserDTO objUsuario = new UserDTO();

            string[] requestParam = strRequest.Split('*');

            foreach (String itemRequest in requestParam)
            {
                LOG.grabarLog(String.Format("BxSG - charSeparator * : " + itemRequest));
                if (!string.IsNullOrWhiteSpace(itemRequest))
                {

                    string[] ArrayItemRequest = itemRequest.Split('=');

                    //LOG.grabarLog(String.Format("BxSG - ArrayItemRequest:{0}  - valor:{1}", ArrayItemRequest));
                    switch (ArrayItemRequest[0])
                    {
                        case "ReturnCode":
                            objUsuario.setReturnCode(ArrayItemRequest[1]);
                            break;
                    }
                }

            }


            //int returnCode = 100;
            //returnCode = Convert.ToInt16(objUsuario.getReturnCode());
            LOG.grabarLog("BxSG - Se finalizó el request de información");
            return objUsuario;

        }

        #endregion

        #region Anteriores desarrollos - Atributos del Portal - Login
        //ApplicationID y SecurityGateWayServer: Son seteados por el sql = "SELECT CODIGO, DESCRI_COD FROM QSCODIGOS WHERE GRUPO = 30018"

        public string RawFunctionData
        { get; set; }
        public string RawData { get; private set; }
        public static string LastLoginFunctionIDs { get; private set; }


        //private UserDTO user;
        private string User = "";
        private string Password = "";
        #endregion
        #region Anteriores desarrollos - funcionalidades
        private void Logout()
        {
            this.LogoutBx(this.User);
        }

        public void LogoutBx(string user)
        {
            user = user.Trim();
            string securityGateWayServer = this.SecurityGateWayServer;
            object[] objArray = new object[] { "aMethodName=getDataBaseServerAssociations&aLoginID=", user, "&aSystemID=", this.ApplicationID, "&userIP=", this.GetIPAddress(), "&aUseSessionDBUser=true&aAgentVersion=SGLibraryNET_5.5" };
            string str = this.Request(securityGateWayServer, string.Concat(objArray));
            if (string.IsNullOrEmpty(str))
            {
                throw new Exception("Error leyendo la información de SG al efectuar 'getDataBaseServerAssociations'.");
            }
            UserDTO userDTO = new UserDTO();
            this.ReturnCode = int.Parse(userDTO.ReturnCode);
            if (userDTO.DbInfo != null)
            {
                for (int i = 0; i < (int)userDTO.DbInfo.Length; i++)
                {
                    DatabaseBase databaseBase = DatabaseFactory.Create(userDTO.DbInfo[i]);
                    if (databaseBase != null)
                    {
                        if (userDTO.CSCredUser == null || userDTO.CSCredUser.Length <= 0)
                        {
                            databaseBase.RemoveUser(userDTO.DbInfo[i], user, false);
                        }
                        else
                        {
                            databaseBase.RemoveUser(userDTO.DbInfo[i], userDTO.CSCredUser, true);
                        }
                    }
                }
                for (int j = 0; j < (int)userDTO.DbInfo.Length; j++)
                {
                    DatabaseBase databaseBase1 = DatabaseFactory.Create(userDTO.DbInfo[j]);
                    if (databaseBase1 != null)
                    {
                        if (userDTO.CSCredUser == null || userDTO.CSCredUser.Length <= 0)
                        {
                            databaseBase1.RemoveLogin(userDTO.DbInfo[j], user);
                        }
                        else
                        {
                            databaseBase1.RemoveLogin(userDTO.DbInfo[j], userDTO.CSCredUser);
                        }
                    }
                }
            }
        }

        private void AddUser(UserDTO user)
        {
            DBInfoDTO[] dbInfo = user.DbInfo;
            for (int i = 0; i < (int)dbInfo.Length; i++)
            {
                DBInfoDTO dBInfoDTO = dbInfo[i];
                try
                {
                    //this._SGAdminPassword = dBInfoDTO.DatabasePassword;
                    DatabaseBase databaseBase = DatabaseFactory.Create(dBInfoDTO);
                    if (databaseBase != null)
                    {
                        databaseBase.AddUser(dBInfoDTO, user.CSCredUser);
                    }
                }
                catch (Exception exception1)
                {
                    Exception exception = exception1;
                    object[] server = new object[] { dBInfoDTO.Server, dBInfoDTO.ServerPortNo, dBInfoDTO.DatabaseInstance, exception.Message };
                    throw new ApplicationException(string.Format("Error al crear usuario en {0}:{1}/{2}. {3}", server), exception);
                }
            }
        }


        private static Hashtable convertStringToHash(string message, string separator)
        {
            Hashtable hashtables = new Hashtable();
            string str = "=";
            string[] strArrays = message.Split(separator.ToCharArray());
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string[] strArrays1 = strArrays[i].Split(str.ToCharArray());
                if (strArrays1 != null && (int)strArrays1.Length == 2)
                {
                    hashtables.Add(strArrays1[0].Replace("\n", ""), strArrays1[1].Replace("\n", ""));
                }
            }
            return hashtables;
        }
        private static UserDTO GetInfoParser(string message, UserDTO user, string separator)
        {
            LOG.grabarLog("BxSG - Se inicia proceso GetInfoParser");
            if (user == null)
            {
                throw new Exception("UserDTO no seteado");
            }
            Hashtable hash = convertStringToHash(message, separator);
            PropertyInfo[] properties = user.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (hash.ContainsKey(propertyInfo.Name))
                {
                    string item = (string)hash[propertyInfo.Name];
                    item = item.Replace("\\n", "");
                    MethodInfo setMethod = propertyInfo.GetSetMethod();
                    object[] objArray = new object[] { item };
                    setMethod.Invoke(user, objArray);
                }
            }
            int num = 0;
            //int num = int.Parse(user.DatabaseCount);
            if (num > 0)
            {
                DBInfoDTO[] dBInfoDTOArray = new DBInfoDTO[num];
                for (int j = 0; j < num; j++)
                {
                    DBInfoDTO dBInfoDTO = new DBInfoDTO();
                    PropertyInfo[] propertyInfoArray = dBInfoDTO.GetType().GetProperties();
                    for (int k = 0; k < (int)propertyInfoArray.Length; k++)
                    {
                        PropertyInfo propertyInfo1 = propertyInfoArray[k];
                        string str = string.Concat(propertyInfo1.Name, j + 1);
                        if (hash.ContainsKey(str))
                        {
                            string item1 = (string)hash[str];
                            item1 = item1.Replace("\\n", "");
                            MethodInfo methodInfo = propertyInfo1.GetSetMethod();
                            object[] objArray1 = new object[] { item1 };
                            methodInfo.Invoke(dBInfoDTO, objArray1);
                        }
                    }
                    dBInfoDTOArray[j] = dBInfoDTO;
                }
                user.DbInfo = dBInfoDTOArray;
            }
            return user;
        }
        public string[] GetFunctions(string user, string password)
        {
            string securityGateWayServer = this.SecurityGateWayServer;
            object[] objArray = new object[] { user, password, this.ApplicationID, this.GetIPAddress() };
            string str = string.Format("aMethodName=getSystemFunctionNames&aLoginID={0}&aPassword={1}&aSystemID={2}&userIP={3}", objArray);
            string str1 = this.Request(securityGateWayServer, str);
            this.RawFunctionData = str1;
            if ((str1 == null || str1.Trim().Length <= 0) && string.IsNullOrEmpty(str))
            {
                throw new Exception("Error leyendo la información de SG al efectuar 'getSystemFunctionNames'.");
            }
            UserDTO userDTO = new UserDTO();
            //userDTO = GetInfoParser(str1, userDTO, "||");
            //this.ReturnCode = int.Parse(userDTO.ReturnCode);
            //if (this.ReturnCode != 0)
            //{
            //    throw new Exception("Error al leer las funciones.");
            //}
            string[] strArrays = userDTO.Functions.Split(new char[] { '*' });
            List<string> strs = new List<string>();
            string[] strArrays1 = strArrays;
            for (int i = 0; i < (int)strArrays1.Length; i++)
            {
                string str2 = strArrays1[i];
                if (str2 != "")
                {
                    strs.Add(str2);
                }
            }
            string[] strArrays2 = new string[strs.Count];
            strs.CopyTo(strArrays2);
            string item = ConfigurationManager.AppSettings["SecurityGateway.ReadFunctionIds"];
            if (!string.IsNullOrEmpty(item) && string.Compare(item, "true", true) == 0)
            {
                SGLogin.LastLoginFunctionIDs = GetFunctionsIDs(user, password);
            }
            return strArrays2;
        }


        public string GetFunctionsIDs(string user, string password)
        {
            string securityGateWayServer = this.SecurityGateWayServer;
            object[] objArray = new object[] { user, password, this.ApplicationID, this.GetIPAddress() };
            string str = string.Format("aMethodName=getHierarchyModulesAndFunctions&aLoginID={0}&aPassword={1}&aSystemID={2}&userIP={3}", objArray);
            string str1 = this.Request(securityGateWayServer, str);
            int num = str1.IndexOf("Entitlements=");
            if (num >= 0)
            {
                str1 = str1.Substring(num + "Entitlements=".Length);
            }
            return str1.Replace("-", "*").Replace(";", "**").Trim();
        }
        public void LoginBX(string userName, string password)
        {
            userName = userName.Trim();
            if (this.ApplicationID == 0)
            {
                throw new Exception("La propiedad ApplicationID no fue seteada.");
            }
            if (string.IsNullOrEmpty(this.SecurityGateWayServer))
            {
                throw new Exception("La propiedad SecurityGateWayServer no fue seteada.");
            }
            this.User = userName;
            this.Password = password;
            string str = LoginUser(userName, password, ApplicationID);
            this.RawData = str;
            UserDTO user = new UserDTO();
            user = GetInfoParser(str, user, "**");
            //this._Name = user.Name;
            if (string.IsNullOrEmpty(user.CSCredUser))
            {
                user.CSCredUser = userName;
            }
            int num = 0;
            if (!int.TryParse(user.ReturnCode, out num))
            {
                throw new ApplicationException(str);
            }
            this.ReturnCode = num;
            if (user.ChangePassword != null)
            {
                this.HasToChangePassword = bool.Parse(user.ChangePassword);
            }
            if (this.HasToChangePassword)
            {
                this.ReturnCode = 360;
            }
            if (this.ReturnCode != 0)
            {
                throw new Exception("Error al validar al usuario.");
            }
            //this._DBUser = user.CSCredUser;
            if (user.DbInfo != null && (int)user.DbInfo.Length > 0)
            {
                //this._DBPassword = user.DbInfo[0].CSCredPwd;
            }
            if (user.DbInfo != null)
            {
                this.AddUser(user);
            }
        }

        #endregion
    }
}
