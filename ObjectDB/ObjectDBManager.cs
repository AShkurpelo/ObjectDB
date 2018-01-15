using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ObjectDB
{

    public sealed class ObjectDBManager : IObjectDBManager
    {
        #region Fields: Private

        private readonly string _directory;

        private SerializeType _serializeType;

        private readonly ObjectDBConnection _connection;

        //private ObjectDescriptor _descriptor;

        #endregion

        #region Delegates: Private

        private Func<string, Guid, DBObject> _readFync;

        private Func<DBObject, string, bool> _writeFync;

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
                        _readFync = ReadFromBinaryFile;
                        _writeFync = WriteToBinaryFile;
                        break;
                }
                this._serializeType = value;
            }
        }

        #endregion

        #region Constructors: Public

        public ObjectDBManager(ObjectDBConnection connection)
        {
            _connection = connection;
            _directory = connection.GetDBDirectoryPath();
            SerializeType = SerializeType.Binary;
        }

        #endregion

        #region Methods: Private

        private bool WriteToBinaryFile(DBObject objData, string objectName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            var fileName = objectName + ".dat";
            bool isFirst = _connection.ContainsObject(objectName);
            var fs = _connection.GetFileStream(fileName);

            try
            {
                var objectDescriptor = new ObjectDescriptor(_directory, objectName, _connection.GetFileStream(objectName + ".descriptor"));
                if (!objectDescriptor.Exists(objData.GetId()))
                {
                    objData.Info = objectDescriptor.AddInstance(objData.GetType().Name);
                    _connection.AddObjectInstance(objData.GetType().Name);
                }
                fs.Position = objData.Info.Position;
                formatter.Serialize(fs, objData.Members);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error: can't write instance {objectName} to database.";
                LogWriter.Log(errorMessage);
                return false;
            }
            return true;
        }

        private DBObject ReadFromBinaryFile(string objectName, Guid instanceId)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            var fullFileName = objectName + ".dat";
            if (!Exists(objectName))
            {
                string errorMessage = $"Error: database don't contain object {objectName}";
                LogWriter.Log(errorMessage);
                return null;
            }
            var fs = _connection.GetFileStream(fullFileName);
            DBObject result = new DBObject();

            var objectDescriptor = new ObjectDescriptor(_directory, objectName, _connection.GetFileStream(objectName + ".descriptor"));
            if (!objectDescriptor.Exists(instanceId))
            {
                string errorMessage = $"Error: can't read not existing instance of object {objectName}";
                LogWriter.Log(errorMessage);
                return null;
            }

            var instanceInfo = objectDescriptor.MemberDescriptions.First(member => member.Id == instanceId);
            result.Info = instanceInfo;
            fs.Position = instanceInfo.Position;

            result.Members = (Dictionary<string, object>)formatter.Deserialize(fs);

            return result;
        }

        //private DBObject ReadFromFile(string filePath)
        //{
        //    return _readFync(filePath);
        //}

        #endregion

        #region Methods: Public

        public Dictionary<string, long> GetObjectInfos()
        {
            return _connection.GetObjectInfo();
        }

        public Dictionary<string, Guid> GetInstanceInfos(string objectName)
        {
            if (!Exists(objectName))
            {
                string errorMessage = $"Error: can't get instance infos, database don't contain object {objectName}.";
                LogWriter.Log(errorMessage);
                return null;
            }
            var tempDescriptor = new ObjectDescriptor(_directory, objectName, _connection.GetFileStream(objectName + ".descriptor"));
            return tempDescriptor.MemberDescriptions.ToDictionary(member => member.InstanceName, member => member.Id);
        }

        public bool Exists(string objectName)
        {
            return _connection.ContainsObject(objectName);
        }

        public bool SaveToDB(DBObject obj)
        {
            obj.FillMembers();
            return _writeFync(obj, obj.GetType().Name);
        }

        public DBObject FetchFromDB(string objectName, string instanceName)
        {
            if (!Exists(objectName))
            {
                string errorMessage = $"Error: database don't contain object {objectName}";
                LogWriter.Log(errorMessage);
                return null;
            }
            var descriptor = new ObjectDescriptor(_directory, objectName, _connection.GetFileStream(instanceName + ".descriptor"));
            if (!descriptor.MemberDescriptions.Exists(member => member.InstanceName == instanceName))
            {
                string errorMessage = $"Error: can't read not existing instance of object {objectName}";
                LogWriter.Log(errorMessage);
                return null;
            }

            var instanceId = descriptor.MemberDescriptions.First(member => member.InstanceName == instanceName).Id;
            return _readFync(objectName, instanceId);
        }

        public DBObject FetchFromDB(string objectName, Guid instanceId)
        {
            return _readFync(objectName, instanceId);
        }

        public bool TryFetchFromDB<T>(out T saveTo, string instanceName)
        {
            saveTo = default(T);
            return false;
        }

        public bool TryFetchFromDB<T>(out T saveTo, Guid instanceId)
        {
            saveTo = default(T);
            return false;
        }

        //TODO: delete this shit
        public bool TryFetchFromDB<T>(out T objectType)
        {
            objectType = default(T);
            //string typeName = typeof(T).Name;
            //DBObject dbBaseObj;
            //try
            //{
            //    dbBaseObj = FetchFromDB(typeName);
            //}
            //catch (Exception e)
            //{
            //    string errorMessage = $"Error: can't fetch object {typeName}";
            //    LogWriter.Log(errorMessage);
            //    return false;
            //}
            //dynamic dynamicObj = dbBaseObj;
            //foreach (var memberName in dbBaseObj.GetDynamicMemberNames())
            //{
                
            //}

            return true;
        }

        #endregion
    }

}
