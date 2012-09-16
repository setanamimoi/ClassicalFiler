using System.IO;
using System.Windows.Forms;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class Name
    {
        [Test]
        public void ディレクトリ名を取得する()
        {
            Assert.AreEqual("NUnit",
                new PathInfo(Application.StartupPath).Name);
        }
        [Test]
        public void マイコンピュータはMyComputerを取得する()
        {
            Assert.AreEqual("MyComputer",
                new PathInfo("%MyComputer%").Name);
        }
        [Test]
        public void ルートドライブはフルパスを取得する()
        {
            Assert.AreEqual(@"C:\",
                new PathInfo(@"C:\").Name);
        }
        [Test]
        public void ファイル名を取得する()
        {
            Assert.AreEqual(
                "nunit.exe",
                new PathInfo(Application.ExecutablePath).Name);
        }
        [Test]
        public void 存在しないファイルも想定ファイル名を取得する()
        {
            Assert.AreEqual("存在しないファイル.txt",
                new PathInfo(Path.Combine(Application.StartupPath, "存在しないファイル.txt")).Name);
        }
    }
}
