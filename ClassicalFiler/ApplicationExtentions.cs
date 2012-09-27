using System;
using System.Collections.Generic;
using System.Linq;
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
            self.Focus();
            self.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        #region string
        /// <summary>
        /// 拡張元の String のインスタンスに新しい文字を追加します。
        /// </summary>
        /// <param name="self">拡張元の String のインスタンス</param>
        /// <param name="paramArguments">新しい文字</param>
        /// <returns>追加後の文字列</returns>
        public static string Append(this string self, params string[] paramArguments)
        {
            List<string> ret = new List<string>();
            ret.Add(Convert.ToString(self));
            ret.AddRange(paramArguments);

            return string.Concat(ret.ToArray());
        }
        /// <summary>
        /// 拡張元の String のインスタンスの最後の文字を削除します。
        /// </summary>
        /// <param name="self">拡張元の String のインスタンス</param>
        /// <returns>最後の文字を削除した文字列</returns>
        public static string RemoveLast(this string self)
        {
            if (string.IsNullOrEmpty(self) == true)
            {
                return self;
            }

            return string.Concat(self.Take(self.Length - 1));
        }
        #endregion
    }
}
