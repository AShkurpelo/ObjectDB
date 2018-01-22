using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ObjectDB
{
    internal sealed class DBDescriptor
    {
        #region Fields: Private

        private string _path;

        private FileStream _fs;

        #endregion

        #region Properties: Public

        public Dictionary<string, long> ObjectDict { get; private set; }

        #endregion

        #region Constructors: Public

        public DBDescriptor(string dbDirictoryPath)
        {
            ObjectDict = new Dictionary<string, long>();
            _path = string.Join("\\", dbDirictoryPath, "db.descriptor");

            if (File.Exists(_path))
                Read();
            else
                Write();

        }

        #endregion

        #region Methods: Internal

        internal void AddObjectInstance(string objectName, long count = 1)
        {
            ObjectDict[objectName] = ObjectDict.ContainsKey(objectName) ? ObjectDict[objectName] + count : 1;
        }

        internal void Save()
        {
            Write();
        }

        #endregion

        #region Methods: Private

        private void Read()
        {
            if (_fs == null)
                _fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            BinaryFormatter formatter = new BinaryFormatter();
            ObjectDict = (Dictionary<string, long>) formatter.Deserialize(_fs);
            _fs.Position = 0;
        }

        private void Write()
        {
            if (_fs == null)
                _fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_fs, ObjectDict);
            _fs.Position = 0;
        }

        #endregion
    }
}
