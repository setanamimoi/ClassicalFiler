using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;

namespace ClassicalFiler
{
    /// <summary>
    /// ファイルパスをクリップボードで使用するクラスです。
    /// </summary>
    public static class PathClipboard
    {
        /// <summary>
        /// クリップボードに切り取る場合の定数を取得します。
        /// </summary>
        private static readonly string PreferredDropEffect = "Preferred DropEffect";

        /// <summary>
        /// 指定したファイルパスをクリップボードにコピーします。
        /// </summary>
        /// <param name="copies">コピーするファイルパス</param>
        public static void Copy(PathInfo[] copies)
        {
            StringCollection list = new StringCollection();
            foreach (PathInfo path in copies)
            {
                list.Add(path.FullPath);
            }
            //クリップボードにコピーする
            Clipboard.SetFileDropList(list);
        }

        /// <summary>
        /// 指定したファイルパスをクリップボードに切り取ります。
        /// </summary>
        /// <param name="cuts">切り取るファイルパス</param>
        public static void Cut(PathInfo[] cuts)
        {
            string[] pathes = cuts.Select(path => path.FullPath).ToArray();

            //ファイルドロップ形式のDataObjectを作成する
            IDataObject data = new DataObject(DataFormats.FileDrop, pathes);

            //DragDropEffects.Moveを設定する（DragDropEffects.Move は 2）
            byte[] bs = new byte[] { (byte)DragDropEffects.Move, 0, 0, 0 };
            MemoryStream ms = new MemoryStream(bs);
            data.SetData(PreferredDropEffect, ms);

            //クリップボードに切り取る
            Clipboard.SetDataObject(data);
        }

        /// <summary>
        /// パス情報を貼り付ける時のデータを取得します。
        /// </summary>
        public static PathPasteContext Context
        {
            get
            {
                //クリップボードのデータを取得する
                IDataObject data = Clipboard.GetDataObject();

                if (data == null)
                {
                    return null;
                }

                if(data.GetDataPresent(DataFormats.FileDrop) == false)
                {
                    return null;
                }

                //コピーされたファイルのリストを取得する
                string[] files = data.GetData(DataFormats.FileDrop) as string[];

                if (files == null)
                {
                    return null;
                }

                PasteType pasteType = PasteType.Copy;

                MemoryStream memory = data.GetData(PreferredDropEffect) as MemoryStream;

                if (memory != null)
                {
                    if ((DragDropEffects)memory.ReadByte() == DragDropEffects.Move)
                    {
                        pasteType = PasteType.Cut;
                    }
                }

                PathInfo[] pathes = files.Select(m => new PathInfo(m)).ToArray();

                return new PathPasteContext(pasteType, pathes);
            }
        }
    }
}
