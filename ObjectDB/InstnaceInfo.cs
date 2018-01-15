using System;

namespace ObjectDB
{
    [Serializable]
    public sealed class InstnaceInfo
    {
        #region Properties: Public

        public Guid Id { get; internal set; }

        public string InstanceName { get; internal set; }

        public long Position { get; internal set; }

        #endregion

        #region Constructors: Internal

        internal InstnaceInfo(Guid id, string instanceName, long position)
        {
            Id = id;
            InstanceName = instanceName;
            Position = position;
        }

        #endregion
    }
}
