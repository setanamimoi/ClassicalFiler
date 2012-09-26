using System;
using System.Linq;
using System.Windows;

namespace ClassicalFiler
{
    /// <summary>
    /// エントリポイントを定義するクラスです。
    /// </summary>
    public class Program
    {
        /// <summary>
        /// プログラムを開始します。
        /// </summary>
        /// <param name="commandLineArguments">コマンドライン引数</param>
        [STAThread]
        public static void Main(string[] commandLineArguments)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application application = new Application();

            application.Run(new MainWindow(commandLineArguments.FirstOrDefault()));
        }

        /// <summary>
        /// 想定外のエラーが発生した場合例外メッセージを表示し、プログラムを終了します。
        /// </summary>
        /// <param name="sender">AppDomain クラスのインスタンス</param>
        /// <param name="e">UnhandledException イベント引数</param>
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
