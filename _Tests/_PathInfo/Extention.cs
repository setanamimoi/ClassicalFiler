using System.IO;
using System.Windows.Forms;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class Extention
    {
        [Test]
        public void 存在するファイルは拡張子を取得する()
        {
            Assert.AreEqual(".exe", new PathInfo(Application.ExecutablePath).Extention);
        }

        [Test]
        public void マイコンピュータはnullを取得する()
        {
            Assert.AreEqual(null, new PathInfo("%MyComputer%").Extention);
        }
        [Test]
        public void ディレクトリはnullを取得する()
        {
            Assert.AreEqual(null, new PathInfo(Application.StartupPath).Extention);
        }
        [Test]
        public void 存在しないファイルはnullを取得する()
        {
            Assert.AreEqual(null, new PathInfo(Path.Combine(Application.StartupPath, "存在しないファイル.txt")).Extention);
        }
    }
}
