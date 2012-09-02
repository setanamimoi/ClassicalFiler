using System.IO;
using System.Windows.Forms;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class LastWriteTime
    {
        [Test]
        public void ディレクトリの最終更新日を取得する()
        {
            Assert.AreEqual(
                new FileInfo(Application.StartupPath).LastWriteTime,
                new PathInfo(Application.StartupPath).LastWriteTime);
        }
        [Test]
        public void ファイルの最終更新日を取得する()
        {
            Assert.AreEqual(
                new FileInfo(Application.ExecutablePath).LastWriteTime,
                new PathInfo(Application.ExecutablePath).LastWriteTime);
        }
        [Test]
        public void 存在しないファイルはnullを取得する()
        {
            Assert.AreEqual(null, new PathInfo(Path.Combine(Application.StartupPath, "存在しないファイル.txt")).LastWriteTime);
        }
    }
}
