using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClassicalFiler
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 現在のディレクトリを取得・設定する。
        /// </summary>
        private PathInfo CurrentDirectory
        {
            get;
            set;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.OpenDirectory(new PathInfo(@"C:\"));
        }

        private void dataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Left) == true)
            {
                PathInfo selectPath = this.CurrentDirectory;

                PathInfo nextDirectory = this.CurrentDirectory.ParentDirectory;

                if (nextDirectory == null)
                {
                    return;
                }

                this.OpenDirectory(nextDirectory, selectPath);
            }
            else if (Keyboard.IsKeyDown(Key.Enter) || Keyboard.IsKeyDown(Key.Right))
            {
                PathInfo nextPath = this.dataGrid.SelectedItem as PathInfo;

                if (nextPath == null)
                {
                    return;
                }

                if (nextPath.Type != PathInfo.PathType.Directory)
                {
                    e.Handled = true;
                    return;
                }

                if ((nextPath.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                {
                    MessageBox.Show(
                        string.Format("{0}にアクセスできません。{1}{1}アクセスが拒否されました。", nextPath.FullPath, Environment.NewLine), this.Content.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Handled = true;
                    return;
                }

                this.OpenDirectory(nextPath);
            }
        }

        /// <summary>
        /// 指定したディレクトリを開き、一番上のパスを選択します。
        /// </summary>
        /// <param name="openDirectory">参照するディレクトリ</param>
        private void OpenDirectory(PathInfo openDirectory)
        {
            OpenDirectory(openDirectory, null);
        }

        /// <summary>
        /// 指定したディレクトリを開き、指定したパスを選択します。
        /// </summary>
        /// <param name="openDirectory">参照するディレクトリ</param>
        /// <param name="selectPath">ディレクトリ内で選択するパス</param>
        /// <remarks>
        /// ディレクトリ内で選択するパスが null の場合、一番上のパスを選択します。
        /// </remarks>
        private void OpenDirectory(PathInfo openDirectory, PathInfo selectPath)
        {
            this.CurrentDirectory = openDirectory;

            this.dataGrid.ItemsSource = openDirectory.GetChildren();
            this.dataGrid.Focus();

            object firstItem = this.dataGrid.Items.Cast<object>().FirstOrDefault();

            if (firstItem == null)
            {
                return;
            }

            if (selectPath == null)
            {
                this.dataGrid.SelectedItem = firstItem;
            }
            else
            {
                this.dataGrid.SelectedItem = selectPath;
            }

            dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.SelectedItem, dataGrid.Columns.First());
        }
    }
}
