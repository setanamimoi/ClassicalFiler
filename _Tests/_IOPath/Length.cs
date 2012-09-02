using System.IO;
using System.Windows.Forms;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._IOPath
{
    [TestFixture]
    public class Length
    {
        [Test]
        public void ディレクトリはnullを取得する()
        {
            Assert.AreEqual(null,
                new IOPath(Application.StartupPath).Length);
        }
        [Test]
        public void ファイルはファイルサイズを取得する()
        {
            Assert.AreEqual(
                new FileInfo(Application.ExecutablePath).Length,
                new IOPath(Application.ExecutablePath).Length);
        }
        [Test]
        public void 存在しないファイルはnullを取得する()
        {
            Assert.AreEqual(null, new IOPath(Path.Combine(Application.StartupPath, "存在しないファイル.txt")).Length);
        }
    }
}
