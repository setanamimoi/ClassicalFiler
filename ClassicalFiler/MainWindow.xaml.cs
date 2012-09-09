using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Linq;
using System.Windows.Controls;

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

        private void Window_Initialized(object sender, EventArgs e)
        {
            PathInfo pathInfo = new PathInfo(@"C:\");

            this.dataGrid_OpenDirectory(pathInfo);
        }

        private void dataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter) || Keyboard.IsKeyDown(Key.Right))
            {
                PathInfo selectedPath = this.dataGrid.SelectedItem as PathInfo;

                if (selectedPath == null)
                {
                    return;
                }

                this.dataGrid_OpenDirectory(selectedPath);
            }
        }

        private void dataGrid_OpenDirectory(PathInfo pathInfo)
        {
            this.dataGrid.ItemsSource = pathInfo.GetChildren();
            this.dataGrid.Focus();
            
            object firstItem = this.dataGrid.Items.Cast<object>().First();
            this.dataGrid.CurrentCell = new DataGridCellInfo(firstItem, this.dataGrid.Columns.First());
        }
    }
}
