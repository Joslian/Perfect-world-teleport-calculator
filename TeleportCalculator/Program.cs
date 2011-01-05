using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace TeleportCalculator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }

        private static void SubscribeUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException +=
                  new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            Console.WriteLine("Unhandled exception: {0}", ex.Message);

            StreamWriter writer = new StreamWriter(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "TeleportCalculator.log.txt"));
            writer.WriteLine("Message: " + ex.Message);
            writer.WriteLine("StackTrace: " + ex.StackTrace);
            if (ex.InnerException != null)
                writer.WriteLine("InnerException: " + ex.InnerException.Message);
            writer.WriteLine("Source: " + ex.Source);
            writer.WriteLine();
            writer.Close();
        }
    }
}
