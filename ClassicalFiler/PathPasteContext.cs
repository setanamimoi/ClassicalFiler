
namespace ClassicalFiler
{
    /// <summary>
    /// パス情報をクリップボードから貼り付ける時に使用するデータです。
    /// </summary>
    public class PathPasteContext
    {
        /// <summary>
        /// 貼り付けタイプとパス情報を指定して PathPasteContext クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">貼り付けタイプ</param>
        /// <param name="values">貼り付けるパス情報</param>
        internal PathPasteContext(PasteType type, PathInfo[] values)
        {
            this.Type = type;
            this.Values = values;
        }

        /// <summary>
        /// 貼り付けタイプを取得します。
        /// </summary>
        public PasteType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// 貼り付けるパスを取得します。
        /// </summary>
        public PathInfo[] Values
        {
            get;
            private set;
        }
    }
}
