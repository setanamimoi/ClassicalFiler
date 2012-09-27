using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._ApplicationExtentions
{
    [TestFixture]
    public class RemoveLast
    {
        [Test]
        public void 最後の文字が削除される()
        {
            Assert.AreEqual("exsampl", "exsample".RemoveLast());
        }

        [Test]
        public void 空文字の場合空文字がそのまま戻り値としてセットする()
        {
            Assert.AreEqual("", "".RemoveLast());
        }

        [Test]
        public void nullの場合nullがそのまま戻り値としてセットする()
        {
            string actual = null;
            Assert.AreEqual(null, actual.RemoveLast());
        }
    }
}
