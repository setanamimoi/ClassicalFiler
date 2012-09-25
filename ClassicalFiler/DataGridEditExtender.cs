using System;
using System.Windows.Controls;

namespace ClassicalFiler
{
    /// <summary>
    /// 任意のタイミングのみ DataGrid の編集機能をオンにする機能を実装するクラスです。
    /// </summary>
    /// <remarks>
    /// <![CDATA[
    /// 編集中に DataGrid の IsReadOnly プロパティを利用すると変更がキャンセルされる為、
    /// 編集開始時に DataGridEditManager クラスの IsEdit プロパティを確認し、
    /// DataGrid の IsReadOnly プロパティとは別に編集を実行するか確認する
    /// ]]>
    /// </remarks>
    public class DataGridEditExtender : DataGridExtender
    {
        /// <summary>
        /// デフォルトコンストラクタによる DataGridEditManager クラスのインスタンス初期化は許可していません。
        /// </summary>
        protected DataGridEditExtender() : base()
        {
        }

        /// <summary>
        /// 管理対象となる DataGrid のインスタンスを指定して DataGridEditManager クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">管理対象となる DataGrid のインスタンス</param>
        public DataGridEditExtender(DataGrid source) : base(source)
        {
            this.Source.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(OnPreparingCellForEdit);
        }

        /// <summary>
        /// DataGrid を編集中かどうかを取得します。
        /// </summary>
        public bool IsEditing
        {
            get;
            private set;
        }
        /// <summary>
        /// DataGrid の編集を開始します。
        /// </summary>
        public void BeginEdit()
        {
            this.IsEditing = true;
            this.Source.IsReadOnly = false;
            this.Source.BeginEdit();
        }
        /// <summary>
        /// DataGrid の編集を終了します。
        /// </summary>
        public void EndEdit()
        {
            this.IsEditing = false;
        }

        /// <summary>
        /// 編集中に DataGrid の IsReadOnly プロパティを利用すると変更がキャンセルされる為、編集開始時に DataGridEditManager クラスの IsEdit プロパティを確認し、編集開始を実行するかキャンセルするかを判断します。
        /// </summary>
        /// <param name="sender">編集する DataGrid</param>
        /// <param name="e">PreparingCellForEdit イベント引数</param>
        private void OnPreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (this.IsEditing == false)
            {
                this.Source.IsReadOnly = true;
            }
        }
    }
}
