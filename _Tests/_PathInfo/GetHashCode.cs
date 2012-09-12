using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class GetHashCode
    {
        [Test]
        public void 同じハッシュ値が取得できる()
        {
            Assert.IsTrue(new PathInfo(@"C:\").GetHashCode() == new PathInfo(@"C:\").GetHashCode());
        }

        [Test]
        public void ファイルパスの大文字小文字関係なく同じハッシュ値が取得できる()
        {
            Assert.IsTrue(new PathInfo(@"C:\").GetHashCode() == new PathInfo(@"c:\").GetHashCode());
        }

        [Test]
        public void ファイルセパレータの数が同じハッシュ値が取得できる()
        {
            Assert.IsTrue(new PathInfo(@"C:\").GetHashCode() == new PathInfo(@"c:\\\\").GetHashCode());
        }

        [Test]
        public void 存在しないパスでも同じハッシュ値が取得できる()
        {
            Assert.IsTrue(new PathInfo(@"C:\存在しないパス.txt").GetHashCode() == new PathInfo(@"c:\存在しないパス.txt").GetHashCode());
        }

        [Test]
        public void objectにボクシングしても同じハッシュ値が取得できる()
        {
            object boxing = new PathInfo(@"c:\");
            Assert.IsTrue(new PathInfo(@"C:\").GetHashCode() == boxing.GetHashCode());
        }
    }
}
