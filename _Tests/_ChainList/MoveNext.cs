using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._ChainList
{
    [TestFixture]
    public class MoveNext
    {
        [Test]
        public void 何も追加されてていない時はfalseを取得する()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();

            Assert.IsFalse(list.MoveNext());
        }

        [Test]
        public void 前回追加された値が取得できる()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();
            list.Add(new PathInfo(@"C:\"));
            list.Add(new PathInfo(@"C:\AA"));

            list.MovePrevious();
            list.MoveNext();

            Assert.AreEqual(new PathInfo(@"C:\AA"), list.Current);
        }

        [Test]
        public void 進むべきオブジェクトがある場合trueを取得する()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();
            list.Add(new PathInfo(@"C:\"));
            list.Add(new PathInfo(@"C:\AA"));
            list.MovePrevious();

            Assert.IsTrue(list.MoveNext());
        }

        [Test]
        public void 進むべきオブジェクトがない場合falseを取得する()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();
            list.Add(new PathInfo(@"C:\"));
            list.Add(new PathInfo(@"C:\AA"));

            Assert.IsFalse(list.MoveNext());
        }

        [Test]
        public void 一度戻った後に追加した場合戻る前のオブジェクトの参照は削除される()
        {
            ChainList<PathInfo> list = new ChainList<PathInfo>();
            list.Add(new PathInfo(@"C:\"));
            list.Add(new PathInfo(@"C:\AA"));

            list.MovePrevious();
            list.Add(new PathInfo(@"C:\BB"));

            list.MovePrevious();
            list.MoveNext();

            Assert.AreEqual(new PathInfo(@"C:\BB"), list.Current);
        }
    }
}
