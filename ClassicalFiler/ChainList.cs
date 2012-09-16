
namespace ClassicalFiler
{
    /// <summary>
    /// 前の項目と後の項目の参照を保持する連鎖リストです。
    /// </summary>
    /// <typeparam name="T">管理する項目の型。</typeparam>
    public class ChainList<T>
    {
        /// <summary>
        /// 現在の連鎖項目を取得・設定します。
        /// </summary>
        private ChainItem<T> CurrentChainItem
        {
            get;
            set;
        }

        /// <summary>
        /// 現在の項目を取得します。
        /// </summary>
        public T Current
        {
            get
            {
                if (this.CurrentChainItem == null)
                {
                    return default(T);
                }

                return this.CurrentChainItem.CurrentItem;
            }
        }

        /// <summary>
        /// 項目を新しく追加します。
        /// </summary>
        /// <param name="addItem">連鎖リストに追加する項目。</param>
        public void Add(T addItem)
        {
            ChainItem<T> next = new ChainItem<T>(addItem);

            if (this.CurrentChainItem != null)
            {
                this.CurrentChainItem.Next = next;
            }
            next.Previous = this.CurrentChainItem;

            this.CurrentChainItem = next;
        }

        /// <summary>
        /// 連鎖リストの前の項目に移動します。
        /// </summary>
        /// <returns>移動できた場合 true 、そうでなければ false 。</returns>
        public bool MovePrevious()
        {
            if (this.CurrentChainItem == null)
            {
                return false;
            }
            if (this.CurrentChainItem.Previous == null)
            {
                return false;
            }

            this.CurrentChainItem = this.CurrentChainItem.Previous;

            return true;
        }

        /// <summary>
        /// 連鎖リストの次の項目に移動します。
        /// </summary>
        /// <returns>移動できた場合 true 、そうでなければ false 。</returns>
        public bool MoveNext()
        {
            if (this.CurrentChainItem == null)
            {
                return false;
            }
            if (this.CurrentChainItem.Next == null)
            {
                return false;
            }

            this.CurrentChainItem = this.CurrentChainItem.Next;

            return true;
        }

        /// <summary>
        /// ChainList クラスで使用する 連鎖項目です。
        /// </summary>
        /// <typeparam name="ChainT">連鎖項目の型</typeparam>
        internal class ChainItem<ChainT>
        {
            /// <summary>
            /// ChainItem クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="current"></param>
            internal ChainItem(ChainT current)
            {
                this.CurrentItem = current;
            }

            /// <summary>
            /// 現在の連鎖項目を取得・設定します。
            /// </summary>
            internal ChainT CurrentItem
            {
                get;
                set;
            }

            /// <summary>
            /// 次の連鎖項目を取得・設定します。
            /// </summary>
            internal ChainItem<ChainT> Next
            {
                get;
                set;
            }

            /// <summary>
            /// 前の連鎖項目を取得・設定します。
            /// </summary>
            internal ChainItem<ChainT> Previous
            {
                get;
                set;
            }
        }
    }
}
