using System;
using System.Collections.Generic;
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
        public MainWindow(string initializeDirectory = null)
        {
            this.InitializeDirectory = initializeDirectory;
            if (this.InitializeDirectory == null)
            {
                this.InitializeDirectory = "%MyComputer%";
            }
            
            this.DirectoryHistory = new ChainList<DirectorySelectState>();

            this.InitializeComponent();
        }

        #region InitializeDirectory プロパティ
        private string InitializeDirectory
        {
            get;
            set;
        }
        #endregion

        #region AlphabetDefineString プロパティ
        /// <summary>
        /// アルファベット文字列の定義を取得・設定します。
        /// </summary>
        private string AlphabetDefineString
        {
            get
            {
                if (this._alphabetDefineString == null)
                {
                    List<string> alphabetList = new List<string>();
                    for (int i = 'A'; i <= 'Z'; i++)
                    {
                        char c = (char)i;

                        alphabetList.Add(c.ToString());
                    }

                    this._alphabetDefineString = string.Concat(alphabetList.ToArray());
                }

                return this._alphabetDefineString;
            }
        }
        private string _alphabetDefineString = null;
        #endregion

        #region DataGridWrapperModelExtender プロパティ
        private DataGridWrapperModelExtender<PathInfo> DataGridWrapperModelExtender
        {
            get;
            set;
        }
        #endregion

        #region DataGridEditExtender プロパティ
        private DataGridEditExtender DataGridEditExtender
        {
            get;
            set;
        }
        #endregion

        #region DirectoryHistory プロパティ
        /// <summary>
        /// ディレクトリの履歴を取得・設定します。
        /// </summary>
        private ChainList<DirectorySelectState> DirectoryHistory
        {
            get;
            set;
        }
        #endregion

        #region Window_Initialized 関連イベントハンドラ
        private void Window_Initialized(object sender, EventArgs e)
        {
            this.DataGridWrapperModelExtender = new DataGridWrapperModelExtender<PathInfo>(this.dataGrid);
            this.DataGridEditExtender = new DataGridEditExtender(this.dataGrid);

            DirectorySelectState directoryState = 
                new DirectorySelectState(new PathInfo(this.InitializeDirectory));

            this.DirectoryHistory.Add(directoryState);
            this.OpenDirectoryAtDataGrid();

            this.dataGrid.FocusFirstCell();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataGrid.FocusFirstCell();
        }
        #endregion

        #region AddressBar 関連プロパティ
        /// <summary>
        /// アドレスバーの内容を元に抽出文字列を取得・設定します。
        /// </summary>
        private string SearchStringAtAddressBar
        {
            get
            {
                if (this.IsSearchModeAtAddressBar == false)
                {
                    return null;
                }
                return string.Concat(this.addressBar.Text.Skip(1));
            }
            set
            {
                if (value == null)
                {
                    this.addressBar.Text = this.DirectoryHistory.Current.Directory.FullPath;
                    return;
                }
                this.addressBar.Text = ":" + value;
            }
        }

        /// <summary>
        /// アドレスバーの内容から検索モード中かどうかを確認します。
        /// </summary>
        private bool IsSearchModeAtAddressBar
        {
            get
            {
                if (this.addressBar.Text.FirstOrDefault() == ':')
                {
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region AddressBar 関連イベントハンドラ
        private void addressBar_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Down))
            {
                this.dataGrid.FocusFirstCell();
            }
            else if (Keyboard.IsKeyDown(Key.Enter))
            {
                PathInfo selectedItem = this.DirectoryHistory.Current.SelectPathes.First();

                if (this.IsSearchModeAtAddressBar == true)
                {
                    this.FilterDataGrid();

                    e.Handled = true;
                    return;
                }

                string inputText = this.addressBar.Text.Trim();

                PathInfo nextPath = new PathInfo(inputText);

                if (nextPath == null)
                {
                    return;
                }

                if (nextPath.Type == PathInfo.PathType.UnExists)
                {
                    return;
                }

                DirectorySelectState openDirectory = new DirectorySelectState(nextPath);
                if (openDirectory.Directory.Type == PathInfo.PathType.File)
                {
                    openDirectory.SelectPathes = new PathInfo[] { openDirectory.Directory };
                    openDirectory.Directory = openDirectory.Directory.ParentDirectory;
                }

                this.DirectoryHistory.Add(openDirectory);

                this.OpenDirectoryAtDataGrid();

                if (nextPath.Type == PathInfo.PathType.File)
                {
                    using (Process.Start(nextPath.FullPath)) { }
                }
                e.Handled = true;
            }
        }
        #endregion

        #region DataGrid 関連メソッド
        /// <summary>
        /// カレントディレクトリを開き、一番上のパスを選択します。
        /// </summary>
        private void OpenDirectoryAtDataGrid()
        {
            this.addressBar.Text = this.DirectoryHistory.Current.Directory.FullPath;
            this.Title = string.Format("{0} - {1}",
                this.DirectoryHistory.Current.Directory.FullPath,
                System.Windows.Forms.Application.ProductName);

            this.FilterDataGrid();

            PathInfo[] selectPathes = this.DirectoryHistory.Current.SelectPathes;

            this.DataGridWrapperModelExtender.ItemsSouce =
                this.DirectoryHistory.Current.Directory.GetChildren();

            this.DataGridWrapperModelExtender.SelectedDataContexts = selectPathes;

            if (selectPathes.Any() == false)
            {
                PathInfo firstItem = this.DataGridWrapperModelExtender.ItemsSouce.FirstOrDefault();

                if (firstItem != null)
                {
                    this.DataGridWrapperModelExtender.SelectedDataContexts =
                        new PathInfo[] { firstItem };
                }
            }

            this.dataGrid.FocusFirstCell();

            this.dataGrid.CurrentCell =
                new DataGridCellInfo(dataGrid.SelectedItem, dataGrid.Columns.First());

            this.dataGrid.ScrollIntoView(
                this.dataGrid.SelectedItems.Cast<object>().FirstOrDefault(),
                this.dataGrid.Columns.First());
        }
        /// <summary>
        /// アドレスバーの検索文字列を元にDataGridに表示する内容を検索し絞り込みます。
        /// </summary>
        private void FilterDataGrid()
        {
            PathInfo[] selectedPathes = this.DataGridWrapperModelExtender.SelectedDataContexts;

            try
            {
                string searchString = this.SearchStringAtAddressBar;
                if (searchString == null)
                {
                    this.dataGrid.Items.Filter = item => true;
                    return;
                }

                this.dataGrid.Items.Filter =
                    item =>
                    {
                        dynamic wrapModelItem = item as dynamic;

                        string modelName = wrapModelItem.Name as string;

                        return modelName.ToUpper().Contains(searchString.ToUpper());
                    };
            }
            finally
            {
                this.dataGrid.FocusFirstCell();
                this.DataGridWrapperModelExtender.SelectedDataContexts = selectedPathes;
            }
        }
        #endregion

        #region DataGrid 関連イベントハンドラ
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.dataGrid.ScrollIntoView(
                this.dataGrid.SelectedItems.Cast<object>().FirstOrDefault(),
                this.dataGrid.Columns.First());

            this.DirectoryHistory.Current.SelectPathes =
                this.DataGridWrapperModelExtender.SelectedDataContexts;
        }
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                return;
            }
            PathInfo elementPath = this.DataGridWrapperModelExtender.GetDataContext(e.Row);

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
            else if (elementPath.Type == PathInfo.PathType.File)
            {
                File.Move(elementPath.FullPath, renamedPath.FullPath);
            }

            this.DataGridWrapperModelExtender.SetDataContext(e.Row, renamedPath);
            this.DataGridEditExtender.EndEdit();
        }
        private void dataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.DataGridEditExtender.IsEditing == true)
            {
                return;
            }

            bool isHandled = true;

            try
            {
                PathInfo[] selectedPathes = this.DataGridWrapperModelExtender.SelectedDataContexts;

                if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Left) == true)
                {
                    if (this.DirectoryHistory.MovePrevious() == false)
                    {
                        return;
                    }
                    this.OpenDirectoryAtDataGrid();
                    return;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Right) == true)
                {
                    if (this.DirectoryHistory.MoveNext() == false)
                    {
                        return;
                    }
                    this.OpenDirectoryAtDataGrid();
                    return;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.L) == true)
                {
                    this.addressBar.Focus();
                    return;
                }
                else if (Keyboard.IsKeyDown(Key.Delete) == true)
                {
                    if (selectedPathes.Any() == false)
                    {
                        return;
                    }

                    if (MessageBox.Show("削除していいですか？", "ClassicalFilter", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                    {
                        return;
                    }

                    List<object> removeList = new List<object>();

                    foreach (dynamic item in this.dataGrid.SelectedItems)
                    {
                        PathInfo pathItem = item.Instance as PathInfo;
                        pathItem.Delete();

                        removeList.Add(item);
                    }

                    this.dataGrid.ItemsSource = DataGridWrapperModelExtender<PathInfo>.CreateWrapModel(this.DirectoryHistory.Current.Directory.GetChildren());
                    this.dataGrid.SelectedItem = this.dataGrid.Items.Cast<object>().First();

                    this.dataGrid.FocusFirstCell();
                    return;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.C) == true)
                {
                    if (selectedPathes.Any() == false)
                    {
                        return;
                    }
                    List<PathInfo> list = new List<PathInfo>();
                    foreach (dynamic item in this.dataGrid.SelectedItems)
                    {
                        PathInfo pathInfo = item.Instance as PathInfo;
                        list.Add(pathInfo);
                    }

                    PathClipboard.Copy(list.ToArray());
                    e.Handled = true;
                    return;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.X) == true)
                {
                    if (selectedPathes.Any() == false)
                    {
                        return;
                    }

                    List<PathInfo> list = new List<PathInfo>();
                    foreach (dynamic item in this.dataGrid.SelectedItems)
                    {
                        PathInfo pathInfo = item.Instance as PathInfo;
                        list.Add(pathInfo);
                    }

                    PathClipboard.Cut(list.ToArray());
                    return;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.V) == true)
                {
                    if (selectedPathes.Any() == false)
                    {
                        return;
                    }

                    PathPasteContext context = PathClipboard.Context;

                    PasteType pasteType = context.Type;

                    foreach (PathInfo path in context.Values)
                    {
                        PathInfo newPath = this.DirectoryHistory.Current.Directory.Combine(path.Name);
                        if (pasteType == PasteType.Copy)
                        {
                            path.Copy(newPath);
                        }
                        else
                        {
                            path.Move(newPath);
                        }

                        dynamic[] dynamicPath = DataGridWrapperModelExtender<PathInfo>.CreateWrapModel(newPath);
                        foreach (dynamic d in dynamicPath)
                        {
                            List<object> itemlist = this.dataGrid.ItemsSource.Cast<object>().ToList();
                            itemlist.Add(d);
                            this.dataGrid.ItemsSource = itemlist.ToArray();
                        }
                    }
                    this.dataGrid.FocusFirstCell();
                    return;
                }
                else if (Keyboard.IsKeyDown(Key.F2) == true)
                {
                    if (selectedPathes.Any() == false)
                    {
                        return;
                    }

                    this.DataGridEditExtender.BeginEdit();
                    return;
                }
                else if (Keyboard.IsKeyDown(Key.Left) == true)
                {
                    if (selectedPathes.Any() == false)
                    {
                        return;
                    }

                    PathInfo selectPath = selectedPathes.First();

                    PathInfo selectDirectory = this.DirectoryHistory.Current.Directory;
                    PathInfo nextDirectory = selectDirectory.ParentDirectory;

                    if (nextDirectory == null)
                    {
                        return;
                    }

                    DirectorySelectState directoryState =
                        new DirectorySelectState(nextDirectory);

                    this.DirectoryHistory.Add(directoryState);

                    this.DirectoryHistory.Current.SelectPathes = new PathInfo[] { selectDirectory };
                    this.OpenDirectoryAtDataGrid();
                    return;
                }
                else if (Keyboard.IsKeyDown(Key.Enter) || Keyboard.IsKeyDown(Key.Right))
                {
                    if (selectedPathes.Any() == false)
                    {
                        return;
                    }

                    PathInfo nextPath = selectedPathes.First();

                    if (nextPath == null)
                    {
                        return;
                    }

                    if (nextPath.Type == PathInfo.PathType.File)
                    {
                        using (Process.Start(nextPath.FullPath)) { }
                        return;
                    }
                    else if (nextPath.Type == PathInfo.PathType.Directory)
                    {
                        if ((nextPath.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                        {
                            MessageBox.Show(
                                string.Format("{0}にアクセスできません。{1}{1}アクセスが拒否されました。", nextPath.FullPath, Environment.NewLine), this.Content.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        DirectorySelectState directoryState =
                            new DirectorySelectState(nextPath);

                        this.DirectoryHistory.Add(directoryState);
                        this.OpenDirectoryAtDataGrid();
                        return;
                    }
                }
                else if (e.Key == Key.Back)
                {
                    if (this.IsSearchModeAtAddressBar == true)
                    {
                        this.SearchStringAtAddressBar =
                            this.SearchStringAtAddressBar.RemoveLast();
                    }
                    this.FilterDataGrid();
                    return;
                }
                else if (e.Key == Key.Escape)
                {
                    this.SearchStringAtAddressBar = null;
                    this.FilterDataGrid();
                    return;
                }
                else if (this.AlphabetDefineString.Contains(e.Key.ToString()) == true)
                {
                    this.SearchStringAtAddressBar =
                        this.SearchStringAtAddressBar.Append(e.Key.ToString());

                    this.FilterDataGrid();
                    return;
                }
                isHandled = false;
            }
            finally
            {
                e.Handled = isHandled;
            }
        }
        #endregion
    }
}
