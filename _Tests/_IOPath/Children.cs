using System.IO;
using System.Windows.Forms;
using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._IOPath
{
    [TestFixture]
    public class Children
    {
        [Test]
        public void ファイルは要素０の配列を取得する()
        {
            Assert.AreEqual(new IOPath[]{},
                new IOPath(Application.ExecutablePath).Children);
        }

        [Test]
        public void ディレクトリの中の要素を取得する()
        {
            IOPath[] actual = new IOPath(Application.StartupPath).Children;

            Assert.AreEqual("framework", actual[0].Name);
            Assert.AreEqual("lib", actual[1].Name);
            Assert.AreEqual("tests", actual[2].Name);

            Assert.AreEqual("agent.conf", actual[3].Name);
            Assert.AreEqual("agent.log.conf", actual[4].Name);
            Assert.AreEqual("launcher.log.conf", actual[5].Name);
            Assert.AreEqual("nunit-agent-x86.exe", actual[6].Name);
            Assert.AreEqual("nunit-agent-x86.exe.config", actual[7].Name);
            Assert.AreEqual("nunit-agent.exe", actual[8].Name);
            Assert.AreEqual("nunit-agent.exe.config", actual[9].Name);
            Assert.AreEqual("nunit-console-x86.exe", actual[10].Name);
            Assert.AreEqual("nunit-console-x86.exe.config", actual[11].Name);
            Assert.AreEqual("nunit-console.exe", actual[12].Name);
            Assert.AreEqual("nunit-console.exe.config", actual[13].Name);
            Assert.AreEqual("nunit-editor.exe", actual[14].Name);
            Assert.AreEqual("nunit-x86.exe", actual[15].Name);
            Assert.AreEqual("nunit-x86.exe.config", actual[16].Name);
            Assert.AreEqual("nunit.exe", actual[17].Name);
            Assert.AreEqual("nunit.exe.config", actual[18].Name);
            Assert.AreEqual("nunit.framework.dll", actual[19].Name);
            Assert.AreEqual("NUnitFitTests.html", actual[20].Name);
            Assert.AreEqual("NUnitTests.config", actual[21].Name);
            Assert.AreEqual("NUnitTests.nunit", actual[22].Name);
            Assert.AreEqual("NUnitTests.VisualState.xml", actual[23].Name);
            Assert.AreEqual("pnunit-agent.exe", actual[24].Name);
            Assert.AreEqual("pnunit-agent.exe.config", actual[25].Name);
            Assert.AreEqual("pnunit-launcher.exe", actual[26].Name);
            Assert.AreEqual("pnunit-launcher.exe.config", actual[27].Name);
            Assert.AreEqual("pnunit.framework.dll", actual[28].Name);
            Assert.AreEqual("pnunit.tests.dll", actual[29].Name);
            Assert.AreEqual("runpnunit.bat", actual[30].Name);
            Assert.AreEqual("test.conf", actual[31].Name);
        }
        [Test]
        public void 存在しないファイルは要素０の配列を取得する()
        {
            Assert.AreEqual(new IOPath[]{},
                new IOPath(Path.Combine(Application.StartupPath, "存在しないファイル.txt")).Children);
        }
    }
}
