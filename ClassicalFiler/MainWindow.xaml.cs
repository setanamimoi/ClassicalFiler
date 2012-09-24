using System;
using System.Diagnostics;
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
            this.DirectoryHistory = new ChainList<DirectorySelectState>();
            
            this.InitializeComponent();
        }

        private DataGridDynamicModelBinder<PathInfo> DataGridModelBinder
        {
            get;
            set;
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
            this.DataGridModelBinder = new DataGridDynamicModelBinder<PathInfo>(this.dataGrid);

            DirectorySelectState directoryState = 
                new DirectorySelectState(new PathInfo(@"C:\"));

            this.DirectoryHistory.Add(directoryState);
            this.OpenDirectory();
        }

        private void dataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.IsEdit == true)
            {
                return;
            }

            PathInfo selectedItem = this.DataGridModelBinder.SelectedDataContext;
            
            if (selectedItem == null)
            {
                return;
            }

            //Left キー単独で動作するコマンドが有る為 BrowserBack を先行して評価する
            if (Keyboard.IsKeyDown(Key.BrowserBack) == true)
            {
                this.DirectoryHistory.Current.SelectPath = selectedItem;
                if (this.DirectoryHistory.MovePrevious() == false)
                {
                    return;
                }
                this.OpenDirectory();
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Right) == true)
            {
                this.DirectoryHistory.Current.SelectPath = selectedItem;
                if (this.DirectoryHistory.MoveNext() == false)
                {
                    return;
                }
                this.OpenDirectory();
            }
            else if (Keyboard.IsKeyDown(Key.F2) == true)
            {
                PathInfo selectPath = selectedItem;
                this.dataGrid.Columns.First().IsReadOnly = false;
                this.dataGrid.BeginEdit();
            }
            else if (Keyboard.IsKeyDown(Key.Left) == true)
            {
                PathInfo selectPath = selectedItem;

                PathInfo selectDirectory = this.DirectoryHistory.Current.Directory;
                PathInfo nextDirectory = selectDirectory.ParentDirectory;

                if (nextDirectory == null)
                {
                    return;
                }

                this.DirectoryHistory.Current.SelectPath = selectPath;

                DirectorySelectState directoryState =
                    new DirectorySelectState(nextDirectory);

                this.DirectoryHistory.Add(directoryState);

                this.DirectoryHistory.Current.SelectPath = selectDirectory;
                this.OpenDirectory();
            }
            else if (Keyboard.IsKeyDown(Key.Enter) || Keyboard.IsKeyDown(Key.Right))
            {
                PathInfo nextPath = selectedItem;

                if (nextPath == null)
                {
                    return;
                }

                if (nextPath.Type == PathInfo.PathType.File)
                {
                    using (Process.Start(nextPath.FullPath)) { }
                    e.Handled = true;
                }
                else if (nextPath.Type == PathInfo.PathType.Directory)
                {
                    if ((nextPath.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    {
                        MessageBox.Show(
                            string.Format("{0}にアクセスできません。{1}{1}アクセスが拒否されました。", nextPath.FullPath, Environment.NewLine), this.Content.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        e.Handled = true;
                        return;
                    }

                    this.DirectoryHistory.Current.SelectPath = selectedItem;

                    DirectorySelectState directoryState =
                        new DirectorySelectState(nextPath);

                    this.DirectoryHistory.Add(directoryState);
                    this.OpenDirectory();
                    e.Handled = true;

                }
            }
        }

        /// <summary>
        /// カレントディレクトリを開き、一番上のパスを選択します。
        /// </summary>
        private void OpenDirectory()
        {
            PathInfo selectPath = this.DirectoryHistory.Current.SelectPath;

            this.DataGridModelBinder.ItemsSouce = 
                this.DirectoryHistory.Current.Directory.GetChildren();

            this.dataGrid.Focus();

            PathInfo firstItem = this.DataGridModelBinder.ItemsSouce.FirstOrDefault();

            if (firstItem == null)
            {
                return;
            }

            if (selectPath == null)
            {
                this.DataGridModelBinder.SelectedDataContext = firstItem;
            }
            else
            {
                this.DataGridModelBinder.SelectedDataContext = selectPath;
            }

            dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.SelectedItem, dataGrid.Columns.First());

            dataGrid.Columns.First().IsReadOnly = false;
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            PathInfo elementPath = this.DataGridModelBinder.GetDataContext(e.Row);

            if (elementPath == null)
            {
                return;
            }

            TextBox renamedTextBox = e.EditingElement as TextBox;

            PathInfo renamedPath = new PathInfo(Path.Combine(elementPath.ParentDirectory.FullPath, renamedTextBox.Text));
            if (elementPath.Type == PathInfo.PathType.Directory)
            {
                Directory.Move(elementPath.FullPath, renamedPath.FullPath);
            }
            else if(elementPath.Type == PathInfo.PathType.File)
            {
                File.Move(elementPath.FullPath, renamedPath.FullPath);
            }

            this.DataGridModelBinder.SetDataContext(e.Row, renamedPath);
            this.IsEdit = false;
        }

        private bool IsEdit = false;

        private void dataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            this.IsEdit = true;
        }
    }
}
