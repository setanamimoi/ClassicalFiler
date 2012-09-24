using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class OperatorEqual
    {
        [Test]
        public void 等号演算子で比較できる()
        {
            Assert.IsTrue(new PathInfo(@"C:\") == new PathInfo(@"C:\"));
        }
        

        [Test]
        public void null同士を等号演算子で比較できる()
        {
            PathInfo basePathInfo = null;
            PathInfo compareToPathInfo = null;

            Assert.IsTrue(basePathInfo == compareToPathInfo);
        }

        [Test]
        public void 左辺がnullでも等号演算子で比較できる()
        {
            PathInfo basePathInfo = null;
            PathInfo compareToPathInfo = new PathInfo(@"C:\");

            Assert.IsFalse(basePathInfo == compareToPathInfo);
        }

        [Test]
        public void 右辺がnullでも等号演算子で比較できる()
        {
            PathInfo basePathInfo = new PathInfo(@"C:\");
            PathInfo compareToPathInfo = null;

            Assert.IsFalse(basePathInfo == compareToPathInfo);
        }

        [Test]
        public void 不等号演算子で比較できる()
        {
            Assert.IsTrue(new PathInfo(@"C:\1") != new PathInfo(@"C:\"));
        }

        [Test]
        public void null同士を不等号演算子で比較できる()
        {
            PathInfo basePathInfo = null;
            PathInfo compareToPathInfo = null;

            Assert.IsFalse(basePathInfo != compareToPathInfo);
        }

        [Test]
        public void 左辺がnullでも不等号演算子で比較できる()
        {
            PathInfo basePathInfo = null;
            PathInfo compareToPathInfo = new PathInfo(@"C:\");

            Assert.IsTrue(basePathInfo != compareToPathInfo);
        }

        [Test]
        public void 右辺がnullでも不等号演算子で比較できる()
        {
            PathInfo basePathInfo = new PathInfo(@"C:\");
            PathInfo compareToPathInfo = null;

            Assert.IsTrue(basePathInfo != compareToPathInfo);
        }
    }
}
