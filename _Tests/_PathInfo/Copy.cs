using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class Copy
    {
        [Test]
        public void ファイルをコピーする()
        {
            PathInfo startupPath = new PathInfo(System.Windows.Forms.Application.StartupPath);

            PathInfo path = startupPath.Combine("test.conf");

            PathInfo copyPath = startupPath.Combine("copytest.conf");

            try
            {
                path.Copy(copyPath);

                Assert.AreEqual(PathInfo.PathType.File, copyPath.Type);
            }
            finally
            {
                if (copyPath.Type != PathInfo.PathType.UnExists)
                {
                    copyPath.Delete();
                }
            }
        }

        [Test]
        public void ディレクトリをコピーする()
        {
            PathInfo startupPath = new PathInfo(System.Windows.Forms.Application.StartupPath);

            PathInfo newDirectory1 = startupPath.Combine("NewDirectory1");
            PathInfo newDirectory1InFile = newDirectory1.Combine("NewDirectory1File.txt");
            PathInfo newDirectory2 = startupPath.Combine("NewDirectory2");
            PathInfo newDirectory2InFile = newDirectory2.Combine("NewDirectory1File.txt");

            try
            {
                System.IO.Directory.CreateDirectory(newDirectory1.FullPath);
                using (System.IO.File.Create(newDirectory1InFile.FullPath))
                {
                }

                newDirectory1.Copy(newDirectory2);

                Assert.AreEqual(PathInfo.PathType.Directory, newDirectory2.Type);
                Assert.AreEqual(PathInfo.PathType.File, newDirectory2InFile.Type);
            }
            finally
            {
                if (newDirectory1.Type != PathInfo.PathType.UnExists)
                {
                    newDirectory1.Delete();
                }
                if (newDirectory2.Type != PathInfo.PathType.UnExists)
                {
                    newDirectory2.Delete();
                }
            }
        }

        [Test]
        public void ディレクトリの属性もコピーする()
        {
            PathInfo startupPath = new PathInfo(System.Windows.Forms.Application.StartupPath);
            PathInfo newDirectory1 = startupPath.Combine("NewDirectory1");
            PathInfo newDirectory1InFile = newDirectory1.Combine("NewDirectory1File.txt");
            PathInfo newDirectory2 = startupPath.Combine("NewDirectory2");
            PathInfo newDirectory2InFile = newDirectory2.Combine("NewDirectory1File.txt");

            try
            {
                System.IO.Directory.CreateDirectory(newDirectory1.FullPath);
                new System.IO.DirectoryInfo(newDirectory1.FullPath).Attributes |= System.IO.FileAttributes.Hidden;
                using (System.IO.File.Create(newDirectory1InFile.FullPath))
                {
                }

                newDirectory1.Copy(newDirectory2);

                Assert.AreEqual(PathInfo.PathType.Directory, newDirectory2.Type);
                Assert.IsTrue((System.IO.FileAttributes.Hidden & newDirectory2.Attributes) == System.IO.FileAttributes.Hidden);
                Assert.AreEqual(PathInfo.PathType.File, newDirectory2InFile.Type);
            }
            finally
            {
                if (newDirectory1.Type != PathInfo.PathType.UnExists)
                {
                    newDirectory1.Delete();
                }
                if (newDirectory2.Type != PathInfo.PathType.UnExists)
                {
                    newDirectory2.Delete();
                }
            }
        }

        [Test]
        public void 再起的にディレクトリをコピーする()
        {
            PathInfo startupPath = new PathInfo(System.Windows.Forms.Application.StartupPath);
            PathInfo newDirectory1 = startupPath.Combine("NewDirectory1");
            PathInfo newDirectory1InFile = newDirectory1.Combine("NewDirectory1File.txt");
            PathInfo newDirectory1In1 = newDirectory1.Combine("NewDirectory1In1");
            PathInfo newDirectory1In1File = newDirectory1In1.Combine("NewDirectory1In1File.txt");

            PathInfo newDirectory2 = startupPath.Combine("NewDirectory2");
            PathInfo newDirectory2InFile = newDirectory2.Combine("NewDirectory1File.txt");
            PathInfo newDirectory2In1 = newDirectory2.Combine("NewDirectory1In1");
            PathInfo newDirectory2In1InFile = newDirectory2In1.Combine("NewDirectory1In1File.txt");

            try
            {
                System.IO.Directory.CreateDirectory(newDirectory1.FullPath);
                using (System.IO.File.Create(newDirectory1InFile.FullPath))
                {
                }
                System.IO.Directory.CreateDirectory(newDirectory1In1.FullPath);
                using (System.IO.File.Create(newDirectory1In1File.FullPath))
                {
                }

                newDirectory1.Copy(newDirectory2);

                Assert.AreEqual(PathInfo.PathType.Directory, newDirectory2.Type);
                Assert.AreEqual(PathInfo.PathType.File, newDirectory2InFile.Type);
                Assert.AreEqual(PathInfo.PathType.Directory, newDirectory2In1.Type);
                Assert.AreEqual(PathInfo.PathType.File, newDirectory2In1InFile.Type);
            }
            finally
            {
                if (newDirectory1.Type != PathInfo.PathType.UnExists)
                {
                    newDirectory1.Delete();
                }

                if (newDirectory2.Type != PathInfo.PathType.UnExists)
                {
                    newDirectory2.Delete();
                }
            }
        }
    }
}
