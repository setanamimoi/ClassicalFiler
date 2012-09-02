using System.IO;
using System.Windows.Forms;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._IOPath
{
    [TestFixture]
    public class Name
    {
        [Test]
        public void ディレクトリ名を取得する()
        {
            Assert.AreEqual("NUnit",
                new IOPath(Application.StartupPath).Name);
        }
        [Test]
        public void ファイル名を取得する()
        {
            Assert.AreEqual(
                "nunit.exe",
                new IOPath(Application.ExecutablePath).Name);
        }
        [Test]
        public void 存在しないファイルも想定ファイル名を取得する()
        {
            Assert.AreEqual("存在しないファイル.txt",
                new IOPath(Path.Combine(Application.StartupPath, "存在しないファイル.txt")).Name);
        }
    }
}
