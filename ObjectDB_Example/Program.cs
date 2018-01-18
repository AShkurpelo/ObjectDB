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
        static void test(IEnumerable<object> objects)
        {
            var t = objects;
        }

        static void Main(string[] args)
        {
            Student student = new Student("Alexander Shkurpelo", "FICT", 3);
           

            using (ObjectDBConnection connection = new ObjectDBConnection("UniversityDB"))
            {
                ObjectDBManager manager = new ObjectDBManager(connection);
                manager.SaveToDB(student, nameof(student));

                var fetched = manager.FetchFromDB("Student", "student");
                Type a = fetched.GetType();
                Type b;
                var fetchedDepartment = fetched["Department"];
                var fetchedMembers = fetched.GetDynamicMemberNames();
                var success = fetched.TryGetTypedMemberValue("HasFacilities", out bool facilities);
                var s = fetched.GetDynamicMemberNames();
            }
            var stname = nameof(student);

            bool breakPoint = true;
        }
    }
}
