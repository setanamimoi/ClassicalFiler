using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._ChainList
{
    [TestFixture]
    public class Add
    {
        [Test]
        public void 何も追加していないカレントオブジェクトはnullである()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();

            Assert.AreEqual(null, list.Current);
        }

        [Test]
        public void 追加した値が取得できる()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();
            list.Add(new PathInfo(@"C:\"));

            Assert.AreEqual(new PathInfo(@"C:\"), list.Current);

            list.Add(new PathInfo(@"C:\AA"));

            Assert.AreEqual(new PathInfo(@"C:\AA"), list.Current);
        }
    }
}
