using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]public class Combine
    {
        [Test]
        public void 連結したパスが取得できる()
        {
            PathInfo target = new PathInfo(System.Windows.Forms.Application.StartupPath);

            PathInfo actual = target.Combine("A", "test.conf");
            string path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "A","test.conf");

            Assert.AreEqual(path, actual.FullPath);
        }
    }
}
