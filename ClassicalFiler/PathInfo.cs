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
        private static string MyComputerPath = string.Format("%{0}%", Environment.SpecialFolder.MyComputer);

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
            if (PathInfo.MyComputerPath.ToUpper() == path.ToUpper())
            {
                this.FullPath = path;
                return;
            }
            FileInfo fileInfo = new FileInfo(path);

            string fullPath = fileInfo.FullName;
            if (IsRootDrive(fullPath) == true)
            {
                this.FullPath = fullPath;
                return;
            }

            this.FullPath = fullPath.TrimEnd('\\');
        }

        /// <summary>
        /// 指定したフルパスがルートドライブかどうかを取得します。
        /// </summary>
        /// <param name="fileInfo">ファイルのフルパス</param>
        /// <returns>ルートドライブであれば true 、そうでなければ false 。</returns>
        private static bool IsRootDrive(string fullpath)
        {
            if (fullpath.TrimEnd('\\').Last() == ':')
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 子要素を取得します。
        /// </summary>
        /// <returns>パスに関連する子要素の配列。</returns>
        /// <remarks>インスタンスのパスがディレクトリ以外の場合は空の配列を返します。</remarks>
        public PathInfo[] GetChildren()
        {
            List<PathInfo> ret = new List<PathInfo>();

            if (this.GetHashCode() == PathInfo.MyComputerPath.ToUpper().GetHashCode())
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    ret.Add(new PathInfo(drive.Name));
                }

                return ret.ToArray();
            }

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
        /// 指定した PathInfo が、現在の PathInfo と等しいかどうかを判断します。
        /// </summary>
        /// <param name="obj">現在の PathInfo と比較する PathInfo 。</param>
        /// <returns>指定した PathInfo が現在の PathInfo と等しい場合は true。それ以外の場合は false。</returns>
        public override bool Equals(object obj)
        {
            PathInfo compare = obj as PathInfo;

            if ((object)compare == null)
            {
                return false;
            }

            if (this == compare)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// PathInfo のインスタンス同士で オブジェクトが等しいかどうかを判断します。
        /// </summary>
        /// <param name="compareBase">比較元 PathInfo</param>
        /// <param name="compareTo">比較先 PathInfo</param>
        /// <returns>オブジェクトが等しい場合 true 、そうでない場合 false 。</returns>
        public static bool operator ==(PathInfo compareBase, PathInfo compareTo)
        {
            object compareBaseObject = compareBase as object;
            object compareToObject = compareTo as object;
            
            if (compareBaseObject == null && compareToObject == null)
            {
                return true;
            }
            if (compareBaseObject == null)
            {
                return false;
            }
            if (compareToObject == null)
            {
                return false;
            }

            if (compareBaseObject.GetHashCode() == compareToObject.GetHashCode())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// PathInfo のインスタンス同士で オブジェクトが等しくないかどうかを判断します。
        /// </summary>
        /// <param name="compareBase">比較元 PathInfo</param>
        /// <param name="compareTo">比較先 PathInfo</param>
        /// <returns>オブジェクトが等しくない場合 true 、そうでない場合 false 。</returns>
        public static bool operator !=(PathInfo compareBase, PathInfo compareTo)
        {
            object compareBaseObject = compareBase as object;
            object compareToObject = compareTo as object;

            if (compareBaseObject == null && compareToObject == null)
            {
                return false;
            }
            if (compareBaseObject == null)
            {
                return true;
            }
            if (compareToObject == null)
            {
                return true;
            }

            if (compareBaseObject.GetHashCode() != compareToObject.GetHashCode())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ハッシュ コード を取得します。
        /// </summary>
        /// <returns>現在の PathInfo のハッシュ コード。</returns>
        /// <remarks>同じファイルパスであれば 同じハッシュコードを取得します。</remarks>
        public override int GetHashCode()
        {
            return this.FullPath.ToUpper().GetHashCode();
        }

        /// <summary>
        /// 親ディレクトリを取得します。
        /// </summary>
        /// <remarks>現在ディレクトリがルートドライブの場合 親ディレクトリのインスタンスは null となります。</remarks>
        public PathInfo ParentDirectory
        {
            get
            {
                if (this.GetHashCode() == PathInfo.MyComputerPath.ToUpper().GetHashCode())
                {
                    return null;
                }
                FileInfo fileInfo = new FileInfo(this.FullPath);

                if (IsRootDrive(fileInfo.FullName) == true)
                {
                    return new PathInfo(PathInfo.MyComputerPath);
                }
                return new PathInfo(fileInfo.Directory.FullName);
            }
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
                if (PathInfo.MyComputerPath.ToUpper().GetHashCode() == this.GetHashCode())
                {
                    return this.FullPath.Trim('%');
                }

                FileInfo fileInfo = new FileInfo(this.FullPath);
                if (IsRootDrive(this.FullPath) == true)
                {
                    return this.FullPath;
                }
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
                if (this.GetHashCode() == PathInfo.MyComputerPath.ToUpper().GetHashCode())
                {
                    return PathType.Directory;
                }

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
        /// 属性情報を取得します。
        /// </summary>
        public FileAttributes Attributes
        {
            get
            {
                return new FileInfo(this.FullPath).Attributes;
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
