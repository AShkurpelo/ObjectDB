using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDB
{
    public sealed class ObjectDBConnection : IDisposable
    {
        #region Fields: Private

        private readonly string _dbDirectoryPath;

        private Dictionary<string, FileStream> _lockedFiles = new Dictionary<string, FileStream>();//full file name with directory path

        private DBDescriptor _descriptor;

        #endregion

        #region Static Fields: Private
        #endregion

        #region Constructors: Public

        public ObjectDBConnection(string objectDBFolderPath, string dbName)
        {
            _dbDirectoryPath = string.Join("\\", objectDBFolderPath, dbName);
            InitDirectory();
            LogWriter.LoggingDirectoryPath = _dbDirectoryPath;
            _descriptor = new DBDescriptor(_dbDirectoryPath);
            InitFiles();
        }

        public ObjectDBConnection(string dbName) : this(string.Join("\\", Directory.GetCurrentDirectory(), "ObjectDB"), dbName)
        {
        }

        #endregion

        #region Destructors

        #endregion

        #region Methods: Private

        private string GetFilePath(string fileName)
        {
            return string.Join("\\", _dbDirectoryPath, fileName);
        }

        private void InitDirectory()
        {
            var directoryInfo = new DirectoryInfo(_dbDirectoryPath);
            directoryInfo.Create();
        }

        private void InitFiles()
        {
            foreach (var dbFile in GetDBFileNames())
            {
                LockFile(GetFilePath(dbFile));
            }
        }

        private IEnumerable<string> GetDBFileNames()
        {
            foreach (var objectName in _descriptor.ObjectDict.Keys)
            {
                yield return $"{objectName}.dat";
                yield return $"{objectName}.descriptor";
            }
        }

        #endregion

        #region Methods: Internal

        internal void LockFile(string filePath)
        {
            _lockedFiles[filePath] = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }

        internal FileStream GetFileStream(string fileName)
        {
            string filePath = GetFilePath(fileName);
            if (!_lockedFiles.ContainsKey(filePath))
            {
                LockFile(filePath);

                string secondFilePath = filePath.EndsWith(".dat")
                    ? String.Concat(filePath.Remove(filePath.Length - 4), ".descriptor")
                    : String.Concat(filePath.Remove(filePath.Length - 11), ".dat");

                LockFile(secondFilePath);
            }

            return _lockedFiles[filePath];
        }

        internal Dictionary<string, long> GetObjectInfo()
        {
            return _descriptor.ObjectDict;
        }

        internal void AddObjectInstance(string objectName)
        {
            _descriptor.AddObjectInstance(objectName);
        }

        internal void SaveDataBaseDescriptor()
        {
            _descriptor.Save();
        }

        #endregion

        #region Methods: Public

        public string GetDBDirectoryPath()
        {
            return _dbDirectoryPath;
        }

        public bool ContainsObject(string objectName)
        {
            return _descriptor.ObjectDict.ContainsKey(objectName);
        }

        #endregion

        #region Static Methods: Public

        public static bool Exists(string objectDBFolderPath, string dbName)
        {
            return File.Exists(string.Join("\\", objectDBFolderPath, dbName, "db.descriptor"));
        }

        public static bool Exists(string dbName)
        {
            return Exists(Directory.GetCurrentDirectory(), dbName);
        }

        #endregion

        public void Dispose()
        {
        }
    }
}
