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
            ObjectDBManager dbManager = new ObjectDBManager();
            Student student = new Student("Alexander Shkurpelo", "FICT", 3, false);
            string name = student.Name;
            dbManager.SaveToDB(student);

            var dbStudent = student as DBObject;
            var sn = dbStudent["Name"];

            dynamic obj = dbManager.FetchFromDB("Student");
            var dbObj = obj as DBBaseObject;
            

            foreach (var memberName in dbObj.GetDynamicMemberNames())
            {
                Console.WriteLine($"{memberName}={dbObj[memberName]}");
            }

            var objName = obj.Name;


            int hostelNum;
            ((DBBaseObject)obj).TryGetTypedMemberValue("Hostel", out hostelNum);

            //Guid id2 = obj.__Id__;

            Type curType = obj.GetType();
        }
    }
}
