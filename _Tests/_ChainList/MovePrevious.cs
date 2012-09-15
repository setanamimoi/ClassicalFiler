using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._ChainList
{
    [TestFixture]
    public class MovePrevious
    {
        [Test]
        public void 何もプッシュされてていない時はfalseを取得する()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();

            Assert.IsFalse(list.MovePrevious());
        }

        [Test]
        public void 前回プッシュされた値が取得できる()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();
            list.Add(new PathInfo(@"C:\"));
            list.Add(new PathInfo(@"C:\AA"));

            list.MovePrevious();

            Assert.AreEqual(new PathInfo(@"C:\"), list.Current);
        }

        [Test]
        public void 戻るべきオブジェクトがある場合trueを取得する()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();
            list.Add(new PathInfo(@"C:\"));
            list.Add(new PathInfo(@"C:\AA"));

            Assert.IsTrue(list.MovePrevious());
        }

        [Test]
        public void 戻るべきオブジェクトがない場合falseを取得する()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();
            list.Add(new PathInfo(@"C:\"));
            list.Add(new PathInfo(@"C:\AA"));

            list.MovePrevious();
            Assert.IsFalse(list.MovePrevious());
        }

        [Test]
        public void 一度戻った後にプッシュした場合戻る前のオブジェクトの参照は削除される()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();
            list.Add(new PathInfo(@"C:\"));
            list.Add(new PathInfo(@"C:\AA"));

            list.MovePrevious();
            list.Add(new PathInfo(@"C:\BB"));

            list.MovePrevious();

            Assert.AreEqual(new PathInfo(@"C:\"), list.Current);
        }
    }
}
