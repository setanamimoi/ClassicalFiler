using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ClassicalFiler;

namespace _Tests._FileSizeFormatProvider
{
    [TestFixture]
    public class Format
    {
        private static FileSizeFormatProvider FormatProvider = new FileSizeFormatProvider();

        private string ToFormat(string format, object actual)
        {
            return string.Format(Format.FormatProvider, format, actual);
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void 数値書式とバイト書式がセットでない場合例外をスローする()
        {
            Assert.AreEqual("1,025.23", ToFormat("{0:#,##0.##}", 1025.23));
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void バイト表記の位置にFSが無かった場合例外をスローする()
        {
            Assert.AreEqual(" B1000.99", ToFormat("{0:FS2:#,##0.##}", 1000.99));
        }
        [Test]
        public void nullの場合空文字を表記する()
        {
            Assert.AreEqual("", ToFormat("{0:#,##0.##:FS2}", null));
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void 桁数プレースホルダーは正の数値以外はプレースホルダーとして認識されず例外をスローする()
        {
            Assert.AreEqual("1,000-2", ToFormat("{0:#,##0.##:FS-2}", 1000));
        }
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void 桁数プレースホルダーはInt32の最大値以上はプレースホルダーとして認識されず例外をスローする()
        {
            Assert.AreEqual("1,000-2", ToFormat("{0:#,##0.##:FS" + (Convert.ToInt64(Int32.MaxValue) + 1).ToString() +"}", 1000));
        }

        [Test]
        public void バイト表記する()
        {
            Assert.AreEqual("1,000 B", ToFormat("{0:#,##0.##:FS2}", 1000));
        }
        [Test]
        public void 数値書式に従ってバイト表記する()
        {
            Assert.AreEqual("1,000.12 B", ToFormat("{0:#,##0.##:FS2}", 1000.12));
        }

        [Test]
        public void キロバイト表記する()
        {
            Assert.AreEqual("1KB", ToFormat("{0:#,##0.##:FS2}", 1024));
        }
        [Test]
        public void メガバイト表記する()
        {
            Assert.AreEqual("1MB", ToFormat("{0:#,##0.##:FS2}", 1048576));
        }
        [Test]
        public void ギガバイト表記する()
        {
            Assert.AreEqual("1GB", ToFormat("{0:#,##0.##:FS2}", 1073741824));
        }
        [Test]
        public void テラバイト表記する()
        {
            Assert.AreEqual("1TB", ToFormat("{0:#,##0.##:FS2}", 1099511627776));
        }
    }
}
