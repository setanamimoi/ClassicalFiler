using System.IO;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class FullPath
    {
        [Test]
        public void ルートドライブ以外の場合サフィックスにセパレータが削除される()
        {
            Assert.AreEqual(Directory.GetCurrentDirectory().TrimEnd('\\'), new PathInfo(new FileInfo(@".\").FullName + @"\\").FullPath);
        }
        [Test]
        public void マイコンピュータの場合パーセンテージ付きのMyComputerを取得する()
        {
            Assert.AreEqual(@"%MyComputer%", new PathInfo(@"%MyComputer%").FullPath);
        }
        [Test]
        public void ルートドライブの場合サフィックスにセパレータが追加される()
        {
            Assert.AreEqual(@"C:\", new PathInfo(@"C:\").FullPath);
        }
        [Test]
        public void 存在しないファイルでも想定されるフルパスが取得できる()
        {
            Assert.AreEqual(
                Path.Combine(Directory.GetCurrentDirectory(), "存在しないファイル.txt"),
                new PathInfo(@".\存在しないファイル.txt").FullPath);
        }
    }
}
