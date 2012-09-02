using System.IO;
using System.Windows.Forms;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._IOPath
{
    [TestFixture]
    public class Type
    {
        [Test]
        public void ディレクトリはDirectoryを取得する()
        {
            Assert.AreEqual(IOPath.IOPathType.Directory,
                new IOPath(Application.StartupPath).Type);
        }
        [Test]
        public void ファイルはFileを取得する()
        {
            Assert.AreEqual(
                IOPath.IOPathType.File,
                new IOPath(Application.ExecutablePath).Type);
        }
        [Test]
        public void 存在しないファイルはUnExistsを取得する()
        {
            Assert.AreEqual(IOPath.IOPathType.UnExists,
                new IOPath(Path.Combine(Application.StartupPath, "存在しないファイル.txt")).Type);
        }
    }
}
