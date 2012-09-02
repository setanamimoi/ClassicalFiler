using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ClassicalFiler
{
    /// <summary>
    /// ファイルサイズの書式指定サービスを提供するクラスです。
    /// </summary>
    public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// 指定した型の書式指定サービスを提供するオブジェクトを返します。
        /// </summary>
        /// <param name="formatType">返す書式オブジェクトの型を指定するオブジェクト。</param>
        /// <returns>System.IFormatProvider の実装が formatType で指定された型のオブジェクトを提供できる場合は、そのオブジェクトのインスタンス。それ以外の場合は null 。</returns>
        /// <remarks>
        /// <![CDATA[
        /// FileSizeFormatProvider の書式は 3 つのパーツに分かれます。
        /// #,##0(数字書式):FS(固定文字)2(プレースホルダー)
        /// 数字書式は規定の数字書式が適用されます。
        /// :FSは固定となり必ず指定する必要があります。
        /// プレースホルダーは単位を左半角スペース詰めで指定します。
        /// 
        /// arg に null をセットした場合は null が戻り値としてセットされます。
        /// ]]>
        /// </remarks>
        /// <example>
        /// <![CDData[
        /// FileSizeFormatProvider formatProvider = new FileSizeFormatProvider();
        /// Assert.AreEqual("1 KB", string.Format(formatProvider, "{0:#,##0:FS3}", 1024));
        /// Assert.AreEqual("1.00  B", string.Format(formatProvider, "{0:0.00:FS3}", 1000));
        /// ]]>
        /// </example>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            return null;
        }

        /// <summary>
        /// 指定した書式およびカルチャ固有の書式情報を使用して、指定したオブジェクトの値をそれと等価な文字列形式に変換します。
        /// </summary>
        /// <param name="format">書式指定を格納している書式指定文字列。</param>
        /// <param name="arg">書式指定するオブジェクト。</param>
        /// <param name="formatProvider">現在のインスタンスについての書式情報を提供するオブジェクト。</param>
        /// <returns>format と formatProvider の指定に従って書式指定した arg の値の文字列形式。</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string[] splitParts = format.Split(':');
            if (splitParts.Length != 2)
            {
                throw new ArgumentException();
            }

            string numericFormat = splitParts[0];
            string byteFormat = splitParts[1];

            Regex regex = new Regex(@"FS(?<PlaceFolder>\d+)");
            Match match = regex.Match(byteFormat);
            if (match.Success == false)
            {
                throw new ArgumentException();
            }

            string placeFolder = match.Groups["PlaceFolder"].Value;

            int totalWidth;
            if (int.TryParse(placeFolder, NumberStyles.None, null, out totalWidth) == false)
            {
                throw new ArgumentException();
            }

            if (arg == null)
            {
                return null;
            }

            string[] suffix = new string[] { "K", "M", "G", "T" };
            decimal formatedValue = Convert.ToDecimal(arg);
            int suffixIndex;
            for (suffixIndex = 0;
                suffixIndex < suffix.Length && formatedValue >= 1024;
                suffixIndex++, formatedValue /= 1024) ;

            string formatedSuffix = "B";
            if (suffixIndex > 0)
            {
                formatedSuffix = suffix[suffixIndex - 1] + formatedSuffix;
            }

            return string.Format("{0:" + numericFormat + "}{1}", formatedValue, regex.Replace(byteFormat, formatedSuffix).PadLeft(totalWidth));
        }
    }
}
