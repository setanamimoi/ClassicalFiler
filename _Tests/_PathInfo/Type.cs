using System.IO;
using System.Windows.Forms;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class Type
    {
        [Test]
        public void ディレクトリはDirectoryを取得する()
        {
            Assert.AreEqual(PathInfo.PathType.Directory,
                new PathInfo(Application.StartupPath).Type);
        }
        [Test]
        public void ファイルはFileを取得する()
        {
            Assert.AreEqual(
                PathInfo.PathType.File,
                new PathInfo(Application.ExecutablePath).Type);
        }
        [Test]
        public void 存在しないファイルはUnExistsを取得する()
        {
            Assert.AreEqual(PathInfo.PathType.UnExists,
                new PathInfo(Path.Combine(Application.StartupPath, "存在しないファイル.txt")).Type);
        }
    }
}
