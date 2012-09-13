using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._ChainList
{
    [TestFixture]
    public class Push
    {
        [Test]
        public void 何もプッシュしていないカレントオブジェクトはnullである()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();

            Assert.AreEqual(null, list.Current);
        }

        [Test]
        public void プッシュした値が取得できる()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();
            list.Push(new PathInfo(@"C:\"));

            Assert.AreEqual(new PathInfo(@"C:\"), list.Current);

            list.Push(new PathInfo(@"C:\AA"));

            Assert.AreEqual(new PathInfo(@"C:\AA"), list.Current);
        }
    }
}
