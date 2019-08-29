using System;
using System.Diagnostics;
using System.IO;

namespace BxSecurityGateway2018v2.ClassLibrary
{
    public class LOG
    {
        public static void grabarLog(string mensaje)
        {
            //StreamWriter log;
            //const string directorio = @"C:\Temp\SGBuxisLOGS2019\";
            //const string nombreArchivo = @"log_BxSecurityGateway2018v3.txt";
            //System.IO.Directory.CreateDirectory(directorio);
            //string rutaArchivo = @"C:\Temp\SGBuxisLOGS2019\log_BxSecurityGateway2018v3.txt";
            //rutaArchivo = directorio + nombreArchivo;
            //log = new StreamWriter(rutaArchivo, true);
            ////MessageBox.Show(mensaje);
            //log.WriteLine(mensaje);
            //Debug.WriteLine(mensaje);
            //Console.WriteLine(mensaje);
            //log.Close();
            //log.Dispose();
        }

        public static void grabarLogException(string mensaje)
        {
            //mensaje = "BxSG - ERROR - Exception: " + mensaje;
            grabarLog(mensaje);
            Console.Error.WriteLine(mensaje);
            throw new Exception(mensaje);
        }

        public static void grabarLogSeparador()
        {
            string barras = "*----------------------------------------*";
            string mensaje = string.Format("\n \n {0}  {1}  {2} \n", barras, DateTime.Now.ToString("yyyy/MM/dd  HH:mm:ss"), barras);
            grabarLog(mensaje);
        }

    }
}
