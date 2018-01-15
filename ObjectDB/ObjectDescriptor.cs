using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDB
{
    internal sealed class ObjectDescriptor
    {
        #region Fields: Private

        private string _path;

        private FileStream _fs;

        #endregion

        #region Properties: Public

        public List<InstnaceInfo> MemberDescriptions { get; private set; }

        #endregion

        #region Constructors: Public

        public ObjectDescriptor(string dbDirictoryPath, string objectName, FileStream fs, bool isNew = false)
        {
            _path = string.Join("\\", dbDirictoryPath, objectName + ".descriptor");
            _fs = fs;

            if (File.ReadAllBytes(_path).Length != 0)
                Read();
            else
                Write();
        }

        #endregion

        #region Destructors

        ~ObjectDescriptor()
        {
            Write();
        }

        #endregion

        #region Methods: Internal

        internal bool Exists(Guid Id)
        {
            return MemberDescriptions.Exists(member => member.Id == Id);
        }

        internal InstnaceInfo AddInstance(string instanceName)
        {
            var newInstanceInfo = new InstnaceInfo(Guid.NewGuid(), instanceName, _fs.Length);
            MemberDescriptions.Add(newInstanceInfo);
            return newInstanceInfo;
        }

        #endregion

        #region Methods: Private

        private void Read()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemberDescriptions = (List<InstnaceInfo>)formatter.Deserialize(_fs);
            _fs.Position = 0;
        }

        private void Write()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_fs, MemberDescriptions);
            _fs.Position = 0;
        }

        #endregion
    }
}
