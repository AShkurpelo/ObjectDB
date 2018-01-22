using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectDB;

namespace ObjectDB_Example
{
    public class Student : DBObject
    {
        [ObjectDB]
        private bool prepared;

        [ObjectDB]
        public string Name { get; set; }

        [ObjectDB]
        public string Department { get; set; }

        [ObjectDB]
        public int Hostel { get; set; }

        [ObjectDB]
        public bool HasFacilities { get; set; }
        
        public Student()
        {
            this.Name = "<undefined>";
            this.Department = "<undefined>";
            this.prepared = false;
        }

        public Student(string name, string department, int hostel, bool hasFacilities = false)
        {
            this.Name = name;
            this.Department = department;
            this.Hostel = hostel;
            this.HasFacilities = hasFacilities;
        }

        public void PrepareForExam(string subject)
        {
            prepared = true;
        }

        public bool TryPassExam(string subject, out string mark)
        {
            if (prepared)
            {
                mark = "C";
                return true;
            }
            mark = "Fx";
            return false;
        }
    }
}
