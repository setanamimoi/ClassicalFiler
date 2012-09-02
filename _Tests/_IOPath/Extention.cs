using System.IO;
using System.Windows.Forms;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._IOPath
{
    [TestFixture]
    public class Extention
    {
        [Test]
        public void 存在するファイルは拡張子を取得する()
        {
            Assert.AreEqual(".exe", new IOPath(Application.ExecutablePath).Extention);
        }

        [Test]
        public void ディレクトリはnullを取得する()
        {
            Assert.AreEqual(null, new IOPath(Application.StartupPath).Extention);
        }
        [Test]
        public void 存在しないファイルはnullを取得する()
        {
            Assert.AreEqual(null, new IOPath(Path.Combine(Application.StartupPath, "存在しないファイル.txt")).Extention);
        }
    }
}
