using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ObjectDB
{

    public sealed class ObjectDBManager : IManager
    {
        #region Fields: Private
        
        private SerializeType _serializeType;

        private readonly ObjectDBConnection _connection;

        //private ObjectDescriptor _descriptor;

        #endregion

        #region Delegates: Private

        private Func<string, Guid, DBObject> _readFync;

        private Func<DBObject, string, bool, bool> _writeFync;

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
            SerializeType = SerializeType.Binary;
        }

        #endregion

        #region Methods: Private

        private bool WriteToBinaryFile(DBObject objData, string instanceName, bool overwrite)
        {
            var formatter = new BinaryFormatter();
            string objectName = objData.Info.ObjectName;
            var fileName = objectName + ".dat";
            var fs = _connection.GetFileStream(fileName);
            var objectDescriptor = new ObjectDescriptor(_connection.GetFileStream(objectName + ".descriptor"));

            try
            {
                if (objectDescriptor.Exists(instanceName))
                {
                    if (!overwrite)
                        return false;
                    objData.Info = objectDescriptor.GetInstanceInfo(instanceName);
                }
                else
                {
                    objData.Info = objectDescriptor.AddInstance(instanceName, fs.Position);
                    _connection.AddObjectInstance(objectName);
                }

                fs.Position = objData.Info.Position;
                formatter.Serialize(fs, objData.Members);

                objectDescriptor.Save();
                _connection.SaveDataBaseDescriptor();
            }
            catch (Exception e)
            {
                var errorMessage = $"Error: can't write instance {objectName} to database.";
                LogWriter.Log(errorMessage);
                return false;
            }
            return true;
        }

        private DBObject ReadFromBinaryFile(string objectName, Guid instanceId)
        {
            var formatter = new BinaryFormatter();
            var fullFileName = objectName + ".dat";
            if (!Exists(objectName))
            {
                var errorMessage = $"Error: database don't contain object {objectName}";
                LogWriter.Log(errorMessage);
                return null;
            }
            var fs = _connection.GetFileStream(fullFileName);
            var result = new DBObject();

            var objectDescriptor = new ObjectDescriptor(_connection.GetFileStream(objectName + ".descriptor"));
            if (!objectDescriptor.Exists(instanceId))
            {
                var errorMessage = $"Error: can't read not existing instance of object {objectName}";
                LogWriter.Log(errorMessage);
                return null;
            }

            var instanceInfo = objectDescriptor.MemberDescriptions.First(member => member.Id == instanceId);
            result.Info = instanceInfo;
            fs.Position = instanceInfo.Position;

            result.Members = (Dictionary<string, object>)formatter.Deserialize(fs);

            return result;
        }

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
                var errorMessage = $"Error: can't get instance infos, database don't contain object {objectName}.";
                LogWriter.Log(errorMessage);
                return new Dictionary<string, Guid>();
            }
            var tempDescriptor = new ObjectDescriptor(_connection.GetFileStream(objectName + ".descriptor"));
            return tempDescriptor.MemberDescriptions.ToDictionary(member => member.InstanceName, member => member.Id);
        }

        public bool Exists(string objectName)
        {
            return _connection.ContainsObject(objectName);
        }

        public bool Exists(string objectName, string instanceName)
        {
            var tempDescriptor = new ObjectDescriptor(_connection.GetFileStream(objectName + ".descriptor"));
            return tempDescriptor.Exists(instanceName);
        }

        public bool SaveToDB(DBObject obj, string instanceName, bool overwrite = true)            
        {
            obj.FillMembers();
            return _writeFync(obj, instanceName, overwrite);
        }

        public DBObject FetchFromDB(string objectName, string instanceName)
        {
            if (!Exists(objectName))
            {
                var errorMessage = $"Error: database don't contain object {objectName}";
                LogWriter.Log(errorMessage);
                return null;
            }
            var descriptor = new ObjectDescriptor(_connection.GetFileStream(objectName + ".descriptor"));
            if (!descriptor.MemberDescriptions.Exists(member => member.InstanceName == instanceName))
            {
                var errorMessage = $"Error: can't read not existing instance of object {objectName}";
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

        public bool TryFetchFromDB<T>(ref T target, string instanceName) where T : DBObject
        {
            var objectName = typeof(T).Name;
            var obj = FetchFromDB(objectName, instanceName);
            if (target == null)
            {
                return false;
            }

            foreach (var member in target.MemberInfos)
            {
                if (!obj.TryGetMember(member.Name, out var memberValue))
                    return false;

                target.SetMemberValue(member, memberValue);
            }

            return true;
        }

        public bool TryFetchFromDB<T>(ref T target, Guid instanceId) where T : DBObject
        {
            var objectName = typeof(T).Name;
            var obj = FetchFromDB(objectName, instanceId);
            if (target == null)
            {
                return false;
            }

            foreach (var member in target.MemberInfos)
            {
                if (obj[member.Name] == null)
                    return false;

                target.SetMemberValue(member, obj[member.Name]);
            }

            return true;
        }

        #endregion
    }

}
