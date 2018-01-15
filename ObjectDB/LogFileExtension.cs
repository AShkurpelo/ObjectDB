using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDB
{
    public sealed class LogFileExtension
    {

        private readonly String name;
        private readonly int value;

        public static readonly LogFileExtension TXT = new LogFileExtension(1, "txt");
        public static readonly LogFileExtension LOG = new LogFileExtension(2, "log");

        private LogFileExtension(int value, String name)
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
