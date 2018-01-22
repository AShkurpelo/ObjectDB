using System;

namespace ObjectDB
{
    [Serializable]
    public sealed class InstnaceInfo
    {
        #region Properties: Public

        public string ObjectName { get; internal set; }

        public Guid Id { get; internal set; }

        public string InstanceName { get; internal set; }

        public long Position { get; internal set; }

        #endregion

        #region Constructors: Internal

        internal InstnaceInfo(string objectName, Guid id, string instanceName, long position)
        {
            ObjectName = objectName;
            Id = id;
            InstanceName = instanceName;
            Position = position;
        }

        #endregion
    }
}
