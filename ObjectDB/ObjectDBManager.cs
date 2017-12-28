using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDB
{

    public class ObjectDBManager
    {
        #region Fields: Private

        private string _directory;
        private SerializeType _serializeType;

        #endregion

        #region Delegates: Private

        private Func<string, DBBaseObject> readFync;

        private Func<DBBaseObject, string, bool> writeFync;

        #endregion

        #region Properties: Private

        private SerializeType SerializeType
        {
            get => _serializeType;
            set
            {
                switch (value)
                {
                    case (SerializeType.Binary):
                        readFync = ReadFromBinaryFile;
                        writeFync = WriteToBinaryFile;
                        break;
                }
                this._serializeType = value;
            }
        }

        #endregion

        #region Constructors: Public

        public ObjectDBManager()
        {
            _directory = Directory.GetCurrentDirectory();
            SerializeType = SerializeType.Binary;
        }

        public ObjectDBManager(string directoryPath) : this()
        {
            _directory = directoryPath;
        }

        #endregion

        #region Methods: Private

        private string FilePath(string objectTypeName) => $"{_directory}/{objectTypeName}";

        private IEnumerable<MemberInfo> GetTypeDBMembers(Type objType)
        {
            return objType.GetMembers().Where(member => Attribute.IsDefined(member, typeof(ObjectDBAttribute)));
        }

        private DBBaseObject PrepareObjectData(DBBaseObject obj)
        {
            DBBaseObject objData = new DBBaseObject();
            var objType = obj.GetType();
            foreach (var member in GetTypeDBMembers(objType))
            {
                objData[member.Name] = obj.GetType().GetField(member.Name).GetValue(obj);
            }
            return objData;
        }

        private bool WriteToBinaryFile(DBBaseObject objData, string filePath)
        {
            filePath += ".dat";
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                try
                {
                    formatter.Serialize(fs, objData);
                }
                catch (Exception e)
                {
                    string errorMessage = $"Error: can't write object to path {filePath}.";
                    Logger.Log(errorMessage);
                    return false;
                }
            }
            return true;
        }

        private bool WriteToFile(DBBaseObject obj, string filePath)
        {
            var objData = PrepareObjectData(obj);
            
            return writeFync(objData, filePath);
        }

        private DBBaseObject ReadFromBinaryFile(string filePath)
        {
            filePath += ".dat";
            DBBaseObject obj = new DBBaseObject();

            if (!File.Exists(filePath))
            {
                string errorMessage = $"Error: object not found by path: {filePath}";
                Logger.Log(errorMessage);
                return obj;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                try
                {
                    obj = (DBBaseObject)formatter.Deserialize(fs);
                }
                catch (Exception e)
                {
                    string errorMessage = $"Error: can't read object from path: {filePath}.";
                    Logger.Log(errorMessage);
                    return obj;
                }
            }

            return obj;
        }

        private DBBaseObject ReadFromFile(string filePath)
        {
            return readFync(filePath);
        }

        #endregion

        #region Methods: Public

        public bool SaveToDB(DBBaseObject obj)
        {
            string filePath = this.FilePath(obj.GetType().Name);

            return WriteToFile(obj, filePath);
        }

        public DBBaseObject FetchFromDB(string objectTypeName)
        {
            string filePath = this.FilePath(objectTypeName);

            return ReadFromFile(filePath);
        }

        public bool TryFetchFromDB<T>(out T objectType)
        {
            objectType = default(T);
            string typeName = typeof(T).Name;
            DBBaseObject dbBaseObj;
            try
            {
                dbBaseObj = FetchFromDB(typeName);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error: can't fetch object {typeName}";
                Logger.Log(errorMessage);
                return false;
            }
            dynamic dynamicObj = dbBaseObj;
            foreach (var memberName in dbBaseObj.GetDynamicMemberNames())
            {
                
            }

            return true;
        }

        #endregion
    }
}
