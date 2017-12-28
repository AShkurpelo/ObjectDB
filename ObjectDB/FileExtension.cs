using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDB
{
    public sealed class FileExtension
    {

        private readonly String name;
        private readonly int value;

        public static readonly FileExtension TXT = new FileExtension(1, "txt");
        public static readonly FileExtension LOG = new FileExtension(2, "log");

        private FileExtension(int value, String name)
        {
            this.name = name;
            this.value = value;
        }

        public override String ToString()
        {
            return name;
        }

    }
}
