using System;
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
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && Keyboard.IsKeyDown(Key.L) == true)
            {
                this.addressTextBox.Focus();
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
                    if (pathItem.Type == PathInfo.PathType.Directory)
                    {
                        Directory.Delete(pathItem.FullPath, true);
                    }
                    else
                    {
                        File.Delete(pathItem.FullPath);
                    }

                    removeList.Add(item);
                }

                this.dataGrid.ItemsSource = this.DirectoryHistory.Current.Directory.GetChildren();
                
                this.dataGrid.FocusFirstCell();
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

                ////ファイルドロップ形式のDataObjectを作成する
                //IDataObject data = new DataObject(DataFormats.FileDrop, pathInfos);
                ////クリップボードにコピーする
                //Clipboard.SetDataObject(data);

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

                    if (dde == DragDropEffects.Move)
                    {
                        //ファイルが切り取られていた時
                        CopyFilesToDirectory(files, this.DirectoryHistory.Current.Directory.FullPath, true);
                    }
                    else
                    {
                        //ファイルがコピーされていた時
                        CopyFilesToDirectory(files, this.DirectoryHistory.Current.Directory.FullPath, false);
                    }
                }
                e.Handled = true;
            }
            else if (Keyboard.IsKeyDown(Key.F2) == true)
            {
                this.DataGridEditExtender.BeginEdit();
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
        /// 複数のファイルを指定したフォルダにコピーまたは移動する
        /// </summary>
        public void CopyFilesToDirectory(string[] sourceFiles, string destDir, bool move)
        {
            foreach (string sourcePath in sourceFiles)
            {
                //コピー先のパスを決定する
                string destName = System.IO.Path.GetFileName(sourcePath);
                string destPath = System.IO.Path.Combine(destDir, destName);
                if (!move)
                {
                    if (new PathInfo(sourcePath).Type == PathInfo.PathType.Directory)
                    {
                        CopyDirectory(sourcePath, destPath);
                    }
                    else
                    {
                        System.IO.File.Copy(sourcePath, destPath);
                    }
                }
                else
                {
                    //ファイルを移動する
                    if (new PathInfo(sourcePath).Type == PathInfo.PathType.Directory)
                    {
                        System.IO.Directory.Move(sourcePath, destPath);
                    }
                    else if(new PathInfo(sourcePath).Type == PathInfo.PathType.File)
                    {
                        System.IO.File.Move(sourcePath, destPath);
                    }
                }
                PathInfo newPath = new PathInfo(destPath);
                dynamic[] dynamicPath = DataGridWrapperModelExtender<PathInfo>.CreateWrapModel(newPath);
                foreach(dynamic d in dynamicPath)
                {
                    List<object> itemlist = this.dataGrid.ItemsSource.Cast<object>().ToList();
                    itemlist.Add(d);
                    this.dataGrid.ItemsSource = itemlist.ToArray();
                }
            }

            this.dataGrid.FocusFirstCell();
        }
        /// <summary>
        /// ディレクトリをコピーする
        /// </summary>
        /// <param name="sourceDirName">コピーするディレクトリ</param>
        /// <param name="destDirName">コピー先のディレクトリ</param>
        public static void CopyDirectory(
            string sourceDirName, string destDirName)
        {
            //コピー先のディレクトリがないときは作る
            if (!System.IO.Directory.Exists(destDirName))
            {
                System.IO.Directory.CreateDirectory(destDirName);
                //属性もコピー
                System.IO.File.SetAttributes(destDirName,
                    System.IO.File.GetAttributes(sourceDirName));
            }

            //コピー先のディレクトリ名の末尾に"\"をつける
            if (destDirName[destDirName.Length - 1] !=
                    System.IO.Path.DirectorySeparatorChar)
                destDirName = destDirName + System.IO.Path.DirectorySeparatorChar;

            //コピー元のディレクトリにあるファイルをコピー
            string[] files = System.IO.Directory.GetFiles(sourceDirName);
            foreach (string file in files)
                System.IO.File.Copy(file,
                    destDirName + System.IO.Path.GetFileName(file), true);

            //コピー元のディレクトリにあるディレクトリについて、
            //再帰的に呼び出す
            string[] dirs = System.IO.Directory.GetDirectories(sourceDirName);
            foreach (string dir in dirs)
                CopyDirectory(dir, destDirName + System.IO.Path.GetFileName(dir));
        }
        /// <summary>
        /// カレントディレクトリを開き、一番上のパスを選択します。
        /// </summary>
        private void OpenDirectory()
        {
            this.addressTextBox.Text = this.DirectoryHistory.Current.Directory.FullPath;
            this.Title = this.DirectoryHistory.Current.Directory.FullPath + " - ClassicalFiler";
            PathInfo selectPath = this.DirectoryHistory.Current.SelectPath;

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

                string inputText = this.addressTextBox.Text.Trim();
                if (string.IsNullOrEmpty(inputText) == true)
                {
                    PathInfo[] pathes = this.DirectoryHistory.Current.Directory.GetChildren();
                    this.dataGrid.ItemsSource = DataGridWrapperModelExtender<PathInfo>.CreateWrapModel(pathes);
                    e.Handled = true;
                    return;
                }
                else if (inputText.IndexOf(":") == 0)
                {
                    PathInfo[] pathes =  this.DirectoryHistory.Current.Directory.GetChildren();
                    this.dataGrid.ItemsSource = DataGridWrapperModelExtender<PathInfo>.CreateWrapModel( pathes.Where(p => Regex.IsMatch(p.Name, inputText.TrimStart(':')) == true).ToArray());
                    e.Handled = true;
                    return;
                }

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
    }
}
