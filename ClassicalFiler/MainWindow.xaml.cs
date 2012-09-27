﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private string InitializeDirectory
        {
            get;
            set;
        }

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

        private DataGridWrapperModelExtender<PathInfo> DataGridWrapperModelExtender
        {
            get;
            set;
        }

        private DataGridEditExtender DataGridEditExtender
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
            this.DataGridWrapperModelExtender = new DataGridWrapperModelExtender<PathInfo>(this.dataGrid);
            this.DataGridEditExtender = new DataGridEditExtender(this.dataGrid);

            DirectorySelectState directoryState = 
                new DirectorySelectState(new PathInfo(this.InitializeDirectory));

            this.DirectoryHistory.Add(directoryState);
            this.OpenDirectory();

            this.dataGrid.FocusFirstCell();
        }

        private void dataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.DataGridEditExtender.IsEditing == true)
            {
                return;
            }

            PathInfo selectedItem = this.DataGridWrapperModelExtender.SelectedDataContext;
            
            if (selectedItem != null)
            {
                //Left キー単独で動作するコマンドが有る為 BrowserBack を先行して評価する
                if (Keyboard.IsKeyDown(Key.BrowserBack) == true)
                {
                    this.DirectoryHistory.Current.SelectPath = selectedItem;
                    if (this.DirectoryHistory.MovePrevious() == false)
                    {
                        return;
                    }
                    this.OpenDirectory();
                    return;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Right) == true)
                {
                    this.DirectoryHistory.Current.SelectPath = selectedItem;
                    if (this.DirectoryHistory.MoveNext() == false)
                    {
                        return;
                    }
                    this.OpenDirectory();
                    return;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.L) == true)
                {
                    this.addressBar.Focus();
                    return;
                }
                else if (Keyboard.IsKeyDown(Key.Delete) == true)
                {
                    if (MessageBox.Show("削除していいですか？", "ClassicalFilter", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                    {
                        e.Handled = true;
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

                    e.Handled = true;
                    this.dataGrid.FocusFirstCell();
                    return;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.C) == true)
                {
                    List<PathInfo> list = new List<PathInfo>();
                    foreach (dynamic item in this.dataGrid.SelectedItems)
                    {
                        PathInfo pathInfo = item.Instance as PathInfo;
                        list.Add(pathInfo);
                    }

                    string[] pathInfos = list.Select(path => path.FullPath).ToArray();

                    //コピーするファイルのパスをStringCollectionに追加する
                    System.Collections.Specialized.StringCollection files =
                        new System.Collections.Specialized.StringCollection();
                    foreach (string p in pathInfos)
                    {
                        files.Add(p);
                    }
                    //クリップボードにコピーする
                    Clipboard.SetFileDropList(files);
                    e.Handled = true;
                    return;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.X) == true)
                {
                    List<PathInfo> list = new List<PathInfo>();
                    foreach (dynamic item in this.dataGrid.SelectedItems)
                    {
                        PathInfo pathInfo = item.Instance as PathInfo;
                        list.Add(pathInfo);
                    }

                    string[] pathInfos = list.Select(path => path.FullPath).ToArray();

                    //ファイルドロップ形式のDataObjectを作成する
                    IDataObject data = new DataObject(DataFormats.FileDrop, pathInfos);

                    //DragDropEffects.Moveを設定する（DragDropEffects.Move は 2）
                    byte[] bs = new byte[] { (byte)DragDropEffects.Move, 0, 0, 0 };
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(bs);
                    data.SetData("Preferred DropEffect", ms);

                    //クリップボードに切り取る
                    Clipboard.SetDataObject(data);
                    e.Handled = true;
                    return;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.V) == true)
                {
                    //クリップボードのデータを取得する
                    IDataObject data = Clipboard.GetDataObject();
                    //クリップボードにファイルドロップ形式のデータがあるか確認
                    if (data != null && data.GetDataPresent(DataFormats.FileDrop))
                    {
                        //コピーされたファイルのリストを取得する
                        string[] files = (string[])data.GetData(DataFormats.FileDrop);
                        //DragDropEffectsを取得する
                        DragDropEffects dde = GetPreferredDropEffect(data);

                        PathInfo[] ps = files.Select(m => new PathInfo(m)).ToArray();

                        //ファイルが切り取られていた時
                        foreach (PathInfo p in ps)
                        {
                            PathInfo newPath = this.DirectoryHistory.Current.Directory.Combine(p.Name);
                            if(dde == DragDropEffects.Move)
                            {
                                p.Move(newPath);
                            }
                            else
                            {
                                p.Copy(newPath);
                            }
                            
                            dynamic[] dynamicPath = DataGridWrapperModelExtender<PathInfo>.CreateWrapModel(newPath);
                            foreach(dynamic d in dynamicPath)
                            {
                                List<object> itemlist = this.dataGrid.ItemsSource.Cast<object>().ToList();
                                itemlist.Add(d);
                                this.dataGrid.ItemsSource = itemlist.ToArray();
                            }
                        }
                    }
                    e.Handled = true;
                    this.dataGrid.FocusFirstCell();
                    return;
                }
                else if (Keyboard.IsKeyDown(Key.F2) == true)
                {
                    this.DataGridEditExtender.BeginEdit();
                    return;
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
                    return;
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
                        return;
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
                        return;
                    }
                }
            }

            if (e.Key == Key.Back)
            {
                if (this.IsSearchModeAtAddressBar == true)
                {
                    this.SearchStringAtAddressBar =
                        this.SearchStringAtAddressBar.RemoveLast();
                }
                this.FilterDataGrid();
            }

            if (e.Key == Key.Escape)
            {
                this.SearchStringAtAddressBar = null;
                this.FilterDataGrid();
            }

            if(this.AlphabetDefineString.Contains(e.Key.ToString()) == true)
            {
                this.SearchStringAtAddressBar =
                    this.SearchStringAtAddressBar.Append(e.Key.ToString());

                this.FilterDataGrid();
            }
        }

        /// <summary>
        /// クリップボードの"Preferred DropEffect"を調べる
        /// </summary>
        public static DragDropEffects GetPreferredDropEffect(IDataObject data)
        {
            DragDropEffects dde = DragDropEffects.None;

            if (data != null)
            {
                //Preferred DropEffect形式のデータを取得する
                System.IO.MemoryStream ms =
                    (System.IO.MemoryStream)data.GetData("Preferred DropEffect");
                if (ms != null)
                {
                    //先頭のバイトからDragDropEffectsを取得する
                    dde = (DragDropEffects)ms.ReadByte();

                    if (dde == (DragDropEffects.Copy | DragDropEffects.Link))
                    {
                        Console.WriteLine("コピー");
                    }
                    else if (dde == DragDropEffects.Move)
                    {
                        Console.WriteLine("切り取り");
                    }
                }
            }

            return dde;
        }

        /// <summary>
        /// カレントディレクトリを開き、一番上のパスを選択します。
        /// </summary>
        private void OpenDirectory()
        {
            this.addressBar.Text = this.DirectoryHistory.Current.Directory.FullPath;
            this.Title = this.DirectoryHistory.Current.Directory.FullPath + " - ClassicalFiler";
            PathInfo selectPath = this.DirectoryHistory.Current.SelectPath;

            this.SearchStringAtAddressBar = null;
            this.FilterDataGrid();

            this.DataGridWrapperModelExtender.ItemsSouce = 
                this.DirectoryHistory.Current.Directory.GetChildren();

            this.dataGrid.FocusFirstCell();

            PathInfo firstItem = this.DataGridWrapperModelExtender.ItemsSouce.FirstOrDefault();

            if (firstItem == null)
            {
                return;
            }

            if (selectPath == null)
            {
                this.DataGridWrapperModelExtender.SelectedDataContext = firstItem;
            }
            else
            {
                this.DataGridWrapperModelExtender.SelectedDataContext = selectPath;
            }

            dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.SelectedItem, dataGrid.Columns.First());
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
            else if(elementPath.Type == PathInfo.PathType.File)
            {
                File.Move(elementPath.FullPath, renamedPath.FullPath);
            }

            this.DataGridWrapperModelExtender.SetDataContext(e.Row, renamedPath);
            this.DataGridEditExtender.EndEdit();
        }

        private void addressTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Down))
            {
                this.dataGrid.FocusFirstCell();
            }
            else if (Keyboard.IsKeyDown(Key.Enter))
            {
                PathInfo selectedItem = this.DirectoryHistory.Current.SelectPath;

                if (this.IsSearchModeAtAddressBar == true)
                {
                    this.FilterDataGrid();

                    e.Handled = true;
                    return;
                }

                string inputText = this.addressBar.Text.Trim();
                PathInfo[] pathes = this.DirectoryHistory.Current.Directory.GetChildren();
                this.dataGrid.ItemsSource = DataGridWrapperModelExtender<PathInfo>.CreateWrapModel(pathes);
                e.Handled = true;

                PathInfo nextPath = new PathInfo(inputText);

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataGrid.FocusFirstCell();
        }

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

        #region DataGrid 関連メソッド
        /// <summary>
        /// アドレスバーの検索文字列を元にDataGridに表示する内容を検索し絞り込みます。
        /// </summary>
        private void FilterDataGrid()
        {
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
                this.dataGrid.SelectedItem = this.dataGrid.Items.Cast<object>().FirstOrDefault();
            }
        }
        #endregion
    }
}
