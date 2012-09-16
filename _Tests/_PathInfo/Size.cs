using System.IO;
using System.Windows.Forms;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class Size
    {
        [Test]
        public void ディレクトリはnullを取得する()
        {
            Assert.AreEqual(null,
                new PathInfo(Application.StartupPath).Size);
        }

        [Test]
        public void マイコンピュータはnullを取得する()
        {
            Assert.AreEqual(null,
                new PathInfo("%MyComputer%").Size);
        }
        [Test]
        public void ファイルはファイルサイズを取得する()
        {
            Assert.AreEqual(
                new FileInfo(Application.ExecutablePath).Length,
                new PathInfo(Application.ExecutablePath).Size);
        }
        [Test]
        public void 存在しないファイルはnullを取得する()
        {
            Assert.AreEqual(null, new PathInfo(Path.Combine(Application.StartupPath, "存在しないファイル.txt")).Size);
        }
    }
}
