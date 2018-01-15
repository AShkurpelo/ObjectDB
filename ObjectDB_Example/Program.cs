using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ObjectDB;

namespace ObjectDB_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Student student = new Student("Alexander Shkurpelo", "FICT", 3);
            

            using (ObjectDBConnection connection = new ObjectDBConnection("UniversityDB"))
            {
                ObjectDBManager manager = new ObjectDBManager(connection);
                manager.SaveToDB(student);

                
            }
            
            bool breakPoint = true;
        }
    }
}
