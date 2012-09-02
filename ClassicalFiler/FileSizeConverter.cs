using System;
using System.Globalization;
using System.Windows.Data;

namespace ClassicalFiler
{
    /// <summary>
    /// ファイルサイズを変換するクラスです。
    /// ConverterParameter 書式に従ってファイルサイズを変換するクラスです。
    /// </summary>
    /// <remarks>
    /// ファイルサイズから文字列に変換する一方通行のみです。
    /// 文字列からファイルサイズに変換する機能は未実装です。
    /// </remarks>
    public class FileSizeConverter : IValueConverter
    {
        /// <summary>
        /// 変換に使用する ConverterParameter 書式。
        /// </summary>
        private static readonly FileSizeFormatProvider FormatProvider = new FileSizeFormatProvider();

        /// <summary>
        /// 値を FileSizeFormatProvider に従って変換します。
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値。</param>
        /// <param name="targetType">バインディング ターゲット プロパティの型。</param>
        /// <param name="parameter">使用するコンバーター パラメーター。</param>
        /// <param name="culture">コンバーターで使用するカルチャ。</param>
        /// <returns>変換された値。メソッドが null を返す場合は、有効な null 値が使用されています。</returns>
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            string format = "{0:" + parameter + "}";
            return string.Format(FileSizeConverter.FormatProvider, format, value);
        }

        /// <summary>
        /// 値を FileSizeFormatProvider に従って変換します。
        /// </summary>
        /// <param name="value">バインディング ターゲットによって生成される値。</param>
        /// <param name="targetType">変換後の型。</param>
        /// <param name="parameter">使用するコンバーター パラメーター。</param>
        /// <param name="culture">コンバーターで使用するカルチャ。</param>
        /// <returns>変換された値。メソッドが null を返す場合は、有効な null 値が使用されています。</returns>
        /// <remarks>
        /// <![CDATA[
        /// このメソッドは未実装です。
        /// また、今後このメソッドを実装する予定はありません。
        /// ]]>
        /// </remarks>
        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
