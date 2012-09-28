
namespace ClassicalFiler
{
    /// <summary>
    /// ディレクトリの選択状態を保持するクラスです。
    /// </summary>
    public class DirectorySelectState
    {
        /// <summary>
        /// <![CDATA[
        /// 開いているディレクトリと選択しているパスを指定して
        /// DirectorySelectState クラスの新しいインスタンスを初期化します。
        /// ]]>
        /// </summary>
        /// <param name="directory">開いているディレクトリ</param>
        /// <param name="selectPathes">選択しているパス</param>
        public DirectorySelectState(PathInfo directory) :this(directory, new PathInfo[]{})
        {
        }
        /// <summary>
        /// <![CDATA[
        /// 開いているディレクトリと選択しているパスを指定して
        /// DirectorySelectState クラスの新しいインスタンスを初期化します。
        /// ]]>
        /// </summary>
        /// <param name="directory">開いているディレクトリ</param>
        /// <param name="selectPathes">選択しているパス</param>
        public DirectorySelectState(PathInfo directory, PathInfo[] selectPathes)
        {
            this.Directory = directory;
            this.SelectPathes = selectPathes;
        }

        /// <summary>
        /// 開いているディレクトリを取得・設定します。
        /// </summary>
        public PathInfo Directory
        {
            get;
            set;
        }

        /// <summary>
        /// 現在選択しているパスを取得・設定します。
        /// </summary>
        public PathInfo[] SelectPathes
        {
            get;
            set;
        }
    }
}
