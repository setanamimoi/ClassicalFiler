using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ClassicalFiler;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class Delete
    {
        [Test]
        public void ファイルが削除される()
        {
            PathInfo startupPath = new PathInfo(System.Windows.Forms.Application.StartupPath);
            PathInfo copyPath = startupPath.Combine("testForCopy.conf");

            startupPath.Combine("test.conf").Copy(copyPath);
            Assert.AreEqual(PathInfo.PathType.File, copyPath.Type);

            copyPath.Delete();
            Assert.AreEqual(PathInfo.PathType.UnExists, copyPath.Type);
        }

        [Test]
        public void ディレクトリが削除される()
        {
            PathInfo startupPath = new PathInfo(System.Windows.Forms.Application.StartupPath);
            PathInfo copyPath = startupPath.Combine("frameworkForCopy");

            startupPath.Combine("framework").Copy(copyPath);
            Assert.AreEqual(PathInfo.PathType.Directory, copyPath.Type);

            copyPath.Delete();
            Assert.AreEqual(PathInfo.PathType.UnExists, copyPath.Type);
        }
    }
}
