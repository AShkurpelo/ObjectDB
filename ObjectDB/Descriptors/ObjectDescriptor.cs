using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDB
{
    internal sealed class ObjectDescriptor
    {
        #region Fields: Private

        private string objectName;

        private string path;

        private FileStream fs;

        #endregion

        #region Properties: Public

        public List<InstnaceInfo> MemberDescriptions { get; private set; }

        #endregion

        #region Constructors: Public

        public ObjectDescriptor(FileStream fs, bool isNew = false)
        {
            MemberDescriptions = new List<InstnaceInfo>();
            path = fs.Name;
            objectName = path.Split('\\').Last().Split('.').First();
            this.fs = fs;

            if (new FileInfo(path).Length != 0)
                Read();
            else
                Write();
        }

        #endregion

        #region Methods: Internal

        internal bool Exists(Guid Id)
        {
            return MemberDescriptions.Exists(member => member.Id == Id);
        }

        internal bool Exists(string instanceName)
        {
            return MemberDescriptions.Exists(member => member.InstanceName == instanceName);
        }

        internal InstnaceInfo AddInstance(string instanceName, long position)
        {
            var newInstanceInfo = new InstnaceInfo(objectName, Guid.NewGuid(), instanceName, position);
            MemberDescriptions.Add(newInstanceInfo);
            return newInstanceInfo;
        }

        internal InstnaceInfo GetInstanceInfo(string instanceName)
        {
            return MemberDescriptions.Find(member => member.InstanceName == instanceName);
        }

        internal void Save()
        {
            Write();
        }

        #endregion

        #region Methods: Private



        private void Read()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemberDescriptions = (List<InstnaceInfo>)formatter.Deserialize(fs);
            fs.Position = 0;
        }

        private void Write()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, MemberDescriptions);
            fs.Position = 0;
        }

        #endregion
    }
}
