using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

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
            DirectoryInfo source = new DirectoryInfo(@"C:\");

            List<FileSystemInfo> children = new List<FileSystemInfo>();
            children.AddRange(source.GetDirectories());
            children.AddRange(source.GetFiles());

            //this.listView.Columns.Add("Name", "名前");
            //this.listView.Columns.Add("UpdateTime", "更新日時");
            //this.listView.Columns.Add("Extentions", "種類");
            //this.listView.Columns.Add("Size", "サイズ");
            //this.dataGrid.ItemsSource = children;

            List<object> lst = new List<object>();
            foreach (FileSystemInfo child in children)
            {
                string extentions = System.IO.Path.GetExtension(child.FullName);
                if (child is DirectoryInfo)
                {
                    extentions = "dir";
                }

                string size = "";
                FileInfo file = child as FileInfo;
                if (file != null)
                {
                    size = file.Length.ToString();
                }

                
                lst.Add(
                    new  { 
                        Name = child.Name, 
                        LastWriteTime = string.Format("{0:yyyy/MM/dd HH:mm:ss}", child.LastWriteTime), 
                        Extention = extentions,
                        Length = size,
                    });

                //this.listView.Items.Add(item);
            }

            this.dataGrid.ItemsSource = lst;
        }
    }
}
