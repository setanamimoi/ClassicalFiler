using System.IO;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._IOPath
{
    [TestFixture]
    public class FullPath
    {
        [Test]
        public void フルパスが取得できる()
        {
            Assert.AreEqual(Directory.GetCurrentDirectory(), new IOPath(@".\").FullPath);
        }
        [Test]
        public void 存在しないファイルでも想定されるフルパスが取得できる()
        {
            Assert.AreEqual(
                Path.Combine(Directory.GetCurrentDirectory(), "存在しないファイル.txt"),
                new IOPath(@".\存在しないファイル.txt").FullPath);
        }
    }
}
