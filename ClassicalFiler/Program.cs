using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ClassicalFiler
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] arguments)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application application = new Application();

            application.Run(new MainWindow(arguments.FirstOrDefault()));
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;

            MessageBox.Show(
                string.Join(
                    Environment.NewLine, "想定外のエラーが発生しました。", exception.Message),
                    System.Windows.Forms.Application.ProductName,
                    MessageBoxButton.OK, MessageBoxImage.Error);

            Environment.Exit(-1);
        }
    }
}
