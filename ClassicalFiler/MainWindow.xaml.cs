using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Dynamic;

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

            dynamic selectedDynamicItem = this.dataGrid.SelectedItem;
            
            if (selectedDynamicItem == null)
            {
                return;
            }


            //Left キー単独で動作するコマンドが有る為 BrowserBack を先行して評価する
            if (Keyboard.IsKeyDown(Key.BrowserBack) == true)
            {
                this.DirectoryHistory.Current.SelectPath = selectedDynamicItem.Instance as PathInfo;
                if (this.DirectoryHistory.MovePrevious() == false)
                {
                    return;
                }
                this.OpenDirectory();
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Right) == true)
            {
                this.DirectoryHistory.Current.SelectPath = selectedDynamicItem.Instance as PathInfo;
                if (this.DirectoryHistory.MoveNext() == false)
                {
                    return;
                }
                this.OpenDirectory();
            }
            else if (Keyboard.IsKeyDown(Key.F2) == true)
            {
                PathInfo selectPath = selectedDynamicItem.Instance as PathInfo;
                this.dataGrid.Columns.First().IsReadOnly = false;
                this.dataGrid.BeginEdit();
            }
            else if (Keyboard.IsKeyDown(Key.Left) == true)
            {
                PathInfo selectPath = selectedDynamicItem.Instance as PathInfo;

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
                PathInfo nextPath = selectedDynamicItem.Instance as PathInfo;

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

                    this.DirectoryHistory.Current.SelectPath = selectedDynamicItem.Instance as PathInfo;

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

            dynamic[] bindModels = BindingModel.CreateBindingModel(
                this.DirectoryHistory.Current.Directory.GetChildren());

            this.dataGrid.ItemsSource = bindModels;

            this.dataGrid.Focus();

            dynamic firstDynamicItem = this.dataGrid.Items.Cast<dynamic>().FirstOrDefault();

            if (firstDynamicItem == null)
            {
                return;
            }

            object firstItem = firstDynamicItem.Instance;

            if (firstItem == null)
            {
                return;
            }

            if (selectPath == null)
            {
                foreach (dynamic i in this.dataGrid.Items)
                {
                    if(i.Instance == firstItem)
                    {
                        this.dataGrid.SelectedItem = i;
                        break;
                    }
                }
            }
            else
            {
                foreach (dynamic i in this.dataGrid.Items)
                {
                    if (i.Instance.Equals(selectPath) == true)
                    {
                        this.dataGrid.SelectedItem = i;
                        break;
                    }
                }
            }

            dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.SelectedItem, dataGrid.Columns.First());

            dataGrid.Columns.First().IsReadOnly = false;
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            dynamic element = e.EditingElement.DataContext as ExpandoObject;
            if (element == null)
            {
                return;
            }

            PathInfo elementPath = element.Instance as PathInfo;

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
            

            element.Instance = renamedPath;
            this.IsEdit = false;
        }

        private bool IsEdit = false;

        private void dataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            this.IsEdit = true;
        }
    }

    public class BindingModel
    {
        public static dynamic[] CreateBindingModel(params PathInfo[] instance)
        {
            List<object> ret = new List<object>();

            foreach (PathInfo i in instance)
            {
                dynamic x = new System.Dynamic.ExpandoObject();

                System.Reflection.PropertyInfo[] ps = i.GetType().GetProperties();

                foreach (System.Reflection.PropertyInfo p in ps)
                {
                    object o = p.GetValue(i, null);

                    ((System.Collections.Generic.IDictionary<string, object>)x).Add(p.Name, o);

                    
                }

                x.Instance = i;
                ret.Add(x);

            }

            
            return ret.ToArray();
        }
    }
}
