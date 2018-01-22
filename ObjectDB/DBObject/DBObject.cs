using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ObjectDB
{
    [Serializable]
    public class DBObject : DBBaseObject //DBBaseObject can't be internal
    {
        #region Fields: Internal

        internal InstnaceInfo Info;
        
        #endregion

        #region Properties: Private

        internal IEnumerable<MemberInfo> MemberInfos => GetType().GetMembers()
            .Where(member => Attribute.IsDefined(member, typeof(ObjectDBAttribute)));

        #endregion

        #region Constructors: Protected

        protected internal DBObject()
        {
            Info = new InstnaceInfo(GetType().Name, Guid.Empty, null, -1);
            FillMembers();
        }

        #endregion

        #region Methods: Private

        private object GetMemberValue(MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(this);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(this);
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Methods: Internal

        internal void FillMembers()
        {
            foreach (var memberInfo in MemberInfos)
            {
                Members[memberInfo.Name] = GetMemberValue(memberInfo);
            }
        }

        internal void SetMemberValue(MemberInfo memberInfo, object value)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(this, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)memberInfo).SetValue(this, value);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Methods: Public

        public Guid DBGetId()
        {
            return Info.Id;
        }

        public string DBGetName()
        {
            return Info.InstanceName;
        }

        public string DBGetObjectName()
        {
            return Info.ObjectName;
        }

        #endregion

    }
}
