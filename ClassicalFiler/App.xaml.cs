using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ClassicalFiler
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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
