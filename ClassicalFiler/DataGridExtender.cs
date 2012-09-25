using System.Windows.Controls;

namespace ClassicalFiler
{
    /// <summary>
    /// DataGrid を拡張する抽象クラスです。
    /// </summary>
    public abstract class DataGridExtender
    {
        /// <summary>
        /// デフォルトコンストラクタによる DataGridEditManager クラスのインスタンス初期化は許可していません。
        /// </summary>
        protected DataGridExtender()
        {
        }

        /// <summary>
        /// 管理対象となる DataGrid のインスタンスを指定して DataGridEditManager クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">管理対象となる DataGrid のインスタンス</param>
        public DataGridExtender(DataGrid source)
        {
            this.Source = source;
        }

        /// <summary>
        /// 管理対象となる DataGrid のインスタンスを取得します。
        /// </summary>
        public DataGrid Source
        {
            get;
            private set;
        }
    }
}
