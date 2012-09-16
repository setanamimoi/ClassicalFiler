using System.IO;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class Attributes
    {
        [Test]
        public void 属性が含まれているか確認できる()
        {
            Assert.IsTrue((new PathInfo(@".\").Attributes & FileAttributes.Directory) == FileAttributes.Directory);
        }

        [Test]
        public void マイコンピュータはDirectoryを取得する()
        {
            Assert.IsTrue((new PathInfo("%MyComputer%").Attributes & FileAttributes.Directory) == FileAttributes.Directory);
        }
    }
}
