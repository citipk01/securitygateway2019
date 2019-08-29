using System;
using BxSecurityGateway2018v2.ClassLibrary;

namespace BxSecurityGateway2018v2.AppConsola
{
    class Program
    {
        const string configXmlFilePath = @"C:\Windows\SysWOW64\config.xml";
        const string configXmlFilePathUAT = @"C:\Temp\ConfigXML\UAT\config.xml";
        const string configXmlFilePathPRO = @"C:\Temp\ConfigXML\PROD\config.xml";
        const int systemAppId = 20322020;
        const string ssoLogin = "as54421";
        const string ssoPassword = "prograK678";
        const string serverSG = "https://sgsit.nam.nsroot.net:7209/SGAdmin/LegacyController";
        const string serverSGnew = "https://sgsit.nam.nsroot.net:7211/SGAdmin/LegacyController";
        const string serverSGUAT = "https://sguat.nam.nsroot.net:7211/SGAdmin/LegacyController";
        const string serverSGPRO = "https://sgway.nam.nsroot.net:7211/SGAdmin/LegacyController";
        // El servidor SG :7211 lo obtiene del archivo xml
        const string newPass1 = "prograK111";
        const string newPass2 = "prograK111";
        const string appName = "SGDemoNET";
        static void Main(string[] args)
        {
            SGLogin sg = new SGLogin();
            try
            {

                sg.SecurityGateWayServer = serverSGUAT;
                sg.ApplicationID = systemAppId;
                // sg.setApplicationName(appName);
                sg.setConfigXmlFilePath(configXmlFilePath);

                #region Input User
                // ***********************INPUT USER******************** //
                // ***************************************************** //
                Console.WriteLine("Chose: 0=PROD        1=UAT    *=input path      (any)=SIT\n");
                switch (Console.ReadLine())
                {
                    case "0":
                        sg.setConfigXmlFilePath(configXmlFilePathUAT);
                        break;
                    case "1":
                        sg.setConfigXmlFilePath(configXmlFilePathPRO);
                        break;
                    case "*":
                        Console.WriteLine("Input XML filepath: ");
                        string inputPathXML = Console.ReadLine();
                        sg.setConfigXmlFilePath(inputPathXML);
                        break;
                    default:
                        sg.setConfigXmlFilePath(configXmlFilePath);
                        break;

                }
                Console.WriteLine("CREDENCIALES DE USUARIO\n");
                string inputUser = Console.ReadLine();
                string inputPass = Console.ReadLine();
                sg.Login(inputUser, inputPass);

                // *******************END INPUT USER******************** //
                // ***************************************************** //
                #endregion 
                //sg.Login(ssoLogin, ssoPassword);
                sg.Login(inputUser, inputPass);
                string funcionesUsuario = sg.GetFunctionsStr(ssoLogin, ssoPassword);
                string nombreUsuario = sg.Name;

                LOG.grabarLog(string.Format("Hola {0}! \nTus funciones son: {1}", nombreUsuario, funcionesUsuario));

                sg.Logout(ssoLogin);
            }
            catch (Exception e)
            {
                if (sg.HasToChangePassword)
                {
                    LOG.grabarLog(string.Format("El usuario {0} debe cambiar la contraseña", ssoLogin));
                    try
                    {
                        sg.ChangePassword(ssoLogin, ssoPassword, newPass1, newPass2);
                        return;
                    }
                    catch (Exception e2)
                    {
                        LOG.grabarLog("BxSG - ERROR - " + e2.Message);
                        //LOG.grabarLogException(e2.Message);
                        return;
                    }
                }
                //LOG.grabarLogException(string.Format("[ReturnCode: {0}]: {1}", sg.ReturnCode, e.Message));
                LOG.grabarLog(string.Format("BxSG - ERROR - [ReturnCode: {0}]: {1}", sg.ReturnCode, e.Message));
            }
            finally
            {
                //Console.ReadLine();
                System.Threading.Thread.Sleep(5000);
                Environment.Exit(1);
            }
        }
    }
}
