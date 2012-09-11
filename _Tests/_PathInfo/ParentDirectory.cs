using System.IO;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class ParentDirectory
    {
        [Test]
        public void ディレクトリの親ディレクトリのパスが取得できる()
        {
            Assert.AreEqual(new FileInfo(".").Directory.FullName, new PathInfo(".").ParentDirectory.FullPath);
        }

        [Test]
        public void ファイルの親ディレクトリのパスが取得できる()
        {
            Assert.AreEqual(
                new FileInfo(System.Windows.Forms.Application.ExecutablePath).Directory.FullName,
                new PathInfo(System.Windows.Forms.Application.ExecutablePath).ParentDirectory.FullPath);
        }

        [Test]
        public void ルートドライブの親ディレクトリはnullを取得する()
        {
            Assert.AreEqual(
                null,
                new PathInfo(@"C:\").ParentDirectory);
        }
    }
}
