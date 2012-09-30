using ClassicalFiler;
using NUnit.Framework;

namespace _Tests._PathInfo
{
    [TestFixture]
    public class Move
    {
        [Test]
        public void ファイルを移動する()
        {
            PathInfo startupPath = new PathInfo(System.Windows.Forms.Application.StartupPath);

            PathInfo path = startupPath.Combine("test.conf");

            PathInfo copyPath = startupPath.Combine("copytest.conf");
            PathInfo movePath = startupPath.Combine("movetest.conf");

            try
            {
                path.Copy(copyPath);

                copyPath.Move(movePath);

                Assert.AreEqual(PathInfo.PathType.UnExists, copyPath.Type);
                Assert.AreEqual(PathInfo.PathType.File, movePath.Type);
            }
            finally
            {
                if (copyPath.Type != PathInfo.PathType.UnExists)
                {
                    copyPath.Delete();
                }

                if (movePath.Type != PathInfo.PathType.UnExists)
                {
                    movePath.Delete();
                }
            }
        }

        [Test]
        public void ディレクトリを移動する()
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

                newDirectory1.Move(newDirectory2);

                Assert.AreEqual(PathInfo.PathType.UnExists, newDirectory1.Type);
                Assert.AreEqual(PathInfo.PathType.UnExists, newDirectory1InFile.Type);

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
    }
}
