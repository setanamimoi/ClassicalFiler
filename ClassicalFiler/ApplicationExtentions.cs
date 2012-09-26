using System.Windows.Controls;
using System.Windows.Input;

namespace ClassicalFiler
{
    /// <summary>
    /// アプリケーションで使用する拡張メソッドを定義したクラスです。
    /// </summary>
    public static class ApplicationExtentions
    {
        /// <summary>
        /// 拡張元の DataGrid の最初のセルにフォーカスします。
        /// </summary>
        /// <param name="self">拡張元の DataGrid</param>
        public static void FocusFirstCell(this DataGrid self)
        {
            self.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }
    }
}
