using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClassicalFiler
{
    /// <summary>
    /// ファイルパスに関しての機能を持つクラスです。
    /// </summary>
    public class PathInfo
    {
        /// <summary>
        /// デフォルトインスタンスでの初期化は許可していません。
        /// </summary>
        private PathInfo()
        {
        }
        /// <summary>
        /// ファイルパスを指定して IOPath クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public PathInfo(string path)
        {
            string trimedPath = Path.GetFullPath(path).TrimEnd('\\');
            if (trimedPath.Last() == ':')
            {
                this.FullPath = path;
                return;
            }

            this.FullPath = trimedPath;
        }

        /// <summary>
        /// 子要素を取得します。
        /// </summary>
        /// <returns>パスに関連する子要素の配列。</returns>
        /// <remarks>インスタンスのパスがディレクトリ以外の場合は空の配列を返します。</remarks>
        public PathInfo[] GetChildren()
        {
            List<PathInfo> ret = new List<PathInfo>();

            if (this.Type != PathType.Directory)
            {
                return ret.ToArray();
            }

            foreach (string s in Directory.GetDirectories(this.FullPath))
            {
                ret.Add(new PathInfo(s));
            }
            foreach (string s in Directory.GetFiles(this.FullPath))
            {
                ret.Add(new PathInfo(s));
            }

            return ret.ToArray();
        }

        /// <summary>
        /// ファイルパスのフルパスを取得します。
        /// </summary>
        public string FullPath
        {
            get;
            private set;
        }

        /// <summary>
        /// ファイル名、ディレクトリ名を取得します。
        /// </summary>
        public string Name
        {
            get
            {
                return Path.GetFileName(this.FullPath);
            }
        }

        /// <summary>
        /// 最終更新日時を取得します。
        /// </summary>
        /// <remarks>存在しないファイル、ディレクトリの場合 null を取得します。</remarks>
        public DateTime? LastWriteTime
        {
            get
            {
                if (this.Type == PathType.UnExists)
                {
                    return null;
                }
                return new FileInfo(this.FullPath).LastWriteTime;
            }
        }
        /// <summary>
        /// ファイルの拡張子を取得します。
        /// </summary>
        /// <remarks>ファイル以外は null を取得します。</remarks>
        public string Extention
        {
            get
            {
                if (this.Type == PathType.File)
                {
                    return Path.GetExtension(this.FullPath);
                }
                return null;
            }
        }

        /// <summary>
        /// ファイルサイズを取得します。
        /// </summary>
        /// <remarks>ファイル以外は null を取得します。</remarks>
        public long? Size
        {
            get
            {
                if (this.Type == PathType.File)
                {
                    return new FileInfo(this.FullPath).Length;
                }

                return null;
            }
        }

        /// <summary>
        /// パスの種別を取得します。
        /// </summary>
        public PathType Type
        {
            get
            {
                if (new DirectoryInfo(this.FullPath).Exists == true)
                {
                    return PathType.Directory;
                }

                if (new FileInfo(this.FullPath).Exists == true)
                {
                    return PathType.File;
                }

                return PathType.UnExists;
            }
        }

        /// <summary>
        /// パスの種別を定義した列挙体です。
        /// </summary>
        public enum PathType
        {
            /// <summary>
            /// ファイル
            /// </summary>
            File = 0,
            /// <summary>
            /// ディレクトリ
            /// </summary>
            Directory = 1,
            /// <summary>
            /// 存在しないパス
            /// </summary>
            UnExists = 2,
        }
    }
}
