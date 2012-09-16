using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;

namespace ClassicalFiler
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DirectoryHistory = new ChainList<DirectorySelectState>();
            InitializeComponent();
        }

        /// <summary>
        /// ディレクトリの履歴を取得・設定します。
        /// </summary>
        private ChainList<DirectorySelectState> DirectoryHistory
        {
            get;
            set;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            DirectorySelectState directoryState = 
                new DirectorySelectState(new PathInfo(@"%MyComputer%"));

            this.DirectoryHistory.Add(directoryState);
            this.OpenDirectory();
        }

        private void dataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Left キー単独で動作するコマンドが有る為 BrowserBack を先行して評価する
            if (Keyboard.IsKeyDown(Key.BrowserBack) == true)
            {
                this.DirectoryHistory.Current.SelectPath  = this.dataGrid.SelectedItem as PathInfo;
                if (this.DirectoryHistory.MovePrevious() == false)
                {
                    return;
                }
                this.OpenDirectory();
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Right) == true)
            {
                this.DirectoryHistory.Current.SelectPath = this.dataGrid.SelectedItem as PathInfo;
                if (this.DirectoryHistory.MoveNext() == false)
                {
                    return;
                }
                this.OpenDirectory();
            }
            else if (Keyboard.IsKeyDown(Key.Left) == true)
            {
                PathInfo selectPath = this.dataGrid.SelectedItem as PathInfo;

                PathInfo nextDirectory = this.DirectoryHistory.Current.Directory.ParentDirectory;

                if (nextDirectory == null)
                {
                    return;
                }

                this.DirectoryHistory.Current.SelectPath = selectPath;

                DirectorySelectState directoryState = 
                    new DirectorySelectState(nextDirectory);

                this.DirectoryHistory.Add(directoryState);
                this.OpenDirectory();
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

                this.DirectoryHistory.Current.SelectPath = this.dataGrid.SelectedItem as PathInfo;

                DirectorySelectState directoryState = 
                    new DirectorySelectState(nextPath);

                this.DirectoryHistory.Add(directoryState);
                this.OpenDirectory();
                e.Handled = true;
            }
        }

        /// <summary>
        /// カレントディレクトリを開き、一番上のパスを選択します。
        /// </summary>
        private void OpenDirectory()
        {
            PathInfo selectPath = this.DirectoryHistory.Current.SelectPath;

            this.dataGrid.ItemsSource = this.DirectoryHistory.Current.Directory.GetChildren();
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
