using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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

            using (ObjectDBConnection connection = new ObjectDBConnection("D:\\ObjectDBTest", "UniversityDB"))
            {
                ObjectDBManager manager = new ObjectDBManager(connection);
                PrintInfos(manager.GetInstanceInfos("Student"));
                bool objectExist = manager.Exists("Student");
                bool instanceExist = manager.Exists("Student", "student");

                manager.SaveToDB(student, "student");

                var fetched = manager.FetchFromDB("Student", "student");
                Print(fetched, "Fetched by name");
                if (fetched.TryGetTypedMemberValue("HasFacilities", out bool facilities))
                {
                    Console.WriteLine($"Typed member");
                    Console.WriteLine($"\tHasFacilities: {facilities}\n\n\n");
                }

                fetched["Department"] = "IASS";
                fetched["Hostel"] = 10;
                manager.SaveToDB(fetched, fetched.DBGetName());

                instanceExist = manager.Exists("Student", "student");
                var fetchedById = manager.FetchFromDB("Student", fetched.DBGetId());
                Print(fetchedById, "Fetched by Id");

                for (int i = 0; i < 10; ++i)
                {
                    manager.SaveToDB(new Student($"test {i}", "FICT", 20), $"student{i}");
                }

                var list = manager.GetInstanceInfos("Student");
                PrintInfos(list);

                var student8 = manager.FetchFromDB("Student", "student8");
                Print(student8);

                Student castedStudent = new Student();

                bool casted = manager.TryFetchFromDB(ref castedStudent, "student5");
                Console.WriteLine($"{castedStudent.Department}\n");

                dynamic dynStudent = manager.FetchFromDB("Student", "student7");
                dynStudent.Department = "Changed";
                Console.WriteLine(dynStudent.Department);
                manager.SaveToDB(dynStudent, dynStudent.DBGetName());

                var dynamicSaved = manager.FetchFromDB("Student", "student7");
                Print(dynamicSaved);

            }

            Console.ReadKey();

        }

        static void Print(DBObject obj, string header = "")
        {
            Console.WriteLine($"{header} ({obj.DBGetObjectName()}  {obj.DBGetId()}  '{obj.DBGetName()}')");
            foreach (var memberName in obj.GetDynamicMemberNames())
            {
                Console.WriteLine($"\t{memberName}: {obj[memberName]}");
            }
            Console.WriteLine($"\n\n");
        }

        static void PrintInfos(Dictionary<string, Guid> infos)
        {
            Console.WriteLine("Instance infos");
            foreach (var info in infos)
            {
                Console.WriteLine($"\tId = {info.Value}\tName = {info.Key}");
            }
            Console.WriteLine($"\n\n");
        }
    }
}
