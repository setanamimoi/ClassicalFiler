using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class Equals
    {
        [Test]
        public void Equalsメソッドで比較できる()
        {
            Assert.IsTrue(new PathInfo(@"C:\").Equals(new PathInfo(@"C:\")));
        }

        [Test]
        public void ファイルパスの大文字小文字関係なく比較できる()
        {
            Assert.IsTrue(new PathInfo(@"C:\").Equals(new PathInfo(@"c:\")));
        }

        [Test]
        public void ファイルセパレータの数が異なっていても論理的なファイルパスで比較できる()
        {
            Assert.IsTrue(new PathInfo(@"C:\").Equals(new PathInfo(@"c:\\\\")));
        }

        [Test]
        public void 存在しないパスでもファイルパスとして比較できる()
        {
            Assert.IsTrue(new PathInfo(@"C:\存在しないパス.txt").Equals(new PathInfo(@"c:\存在しないパス.txt")));
        }

        [Test]
        public void objectにボクシングしても比較できる()
        {
            object boxing = new PathInfo(@"c:\");
            Assert.IsTrue(new PathInfo(@"C:\").Equals(boxing));
        }

        [Test]
        public void 関係のない型は比較は常にfalseとなる()
        {
            Assert.IsFalse(new PathInfo(@"C:\").Equals(@"C:\"));
        }
    }
}
