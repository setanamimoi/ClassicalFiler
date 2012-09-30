using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._ApplicationExtentions
{
    [TestFixture]
    public class Append
    {
        [Test]
        public void 新しい文字が追加される()
        {
            Assert.AreEqual("exsample12", "exsample".Append("1", "2"));
        }

        [Test]
        public void 空文字の場合新しい文字のみがそのまま戻り値としてセットする()
        {
            Assert.AreEqual("1", "".Append("1"));
        }

        [Test]
        public void nullの場合新しい文字のみがそのまま戻り値としてセットする()
        {
            string actual = null;
            Assert.AreEqual("1", actual.Append("1"));
        }
    }
}
