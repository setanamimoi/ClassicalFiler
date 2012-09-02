using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClassicalFiler
{
    public class IOPath
    {
        protected IOPath()
        {
        }

        public IOPath[] Children
        {
            get
            {
                List<IOPath> ret = new List<IOPath>();

                if (this.Type != IOPathType.Directory)
                {
                    return ret.ToArray();
                }

                foreach (string s in Directory.GetDirectories(this.FullPath))
                {
                    ret.Add(new IOPath(s));
                }
                foreach (string s in Directory.GetFiles(this.FullPath))
                {
                    ret.Add(new IOPath(s));
                }

                return ret.ToArray();
            }
        }
        public IOPath(string path)
        {
            string tmppath = System.IO.Path.GetFullPath(path).TrimEnd('\\');
            if (tmppath.Last() == ':')
            {
                this.FullPath = path;
                return;
            }

            this.FullPath = tmppath;
        }

        public string FullPath
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return System.IO.Path.GetFileName(this.FullPath);
            }
        }

        public DateTime? LastWriteTime
        {
            get
            {
                if (this.Type == IOPathType.UnExists)
                {
                    return null;
                }
                return new FileInfo(this.FullPath).LastWriteTime;
            }
        }
        public string Extention
        {
            get
            {
                if (this.Type == IOPathType.File)
                {
                    return System.IO.Path.GetExtension(this.FullPath);
                }
                return null;
            }
        }
        public long? Length
        {
            get
            {
                if (this.Type == IOPathType.File)
                {
                    return new FileInfo(this.FullPath).Length;
                }

                return null;
            }
        }

        public IOPathType Type
        {
            get
            {
                if (new DirectoryInfo(this.FullPath).Exists == true)
                {
                    return IOPathType.Directory;
                }

                if (new FileInfo(this.FullPath).Exists == true)
                {
                    return IOPathType.File;
                }

                return IOPathType.UnExists;
            }
        }

        public enum IOPathType
        {
            File = 0,
            Directory = 1,
            UnExists = 2,
        }
    }
}
