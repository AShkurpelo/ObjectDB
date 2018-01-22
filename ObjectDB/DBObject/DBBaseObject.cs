using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ObjectDB
{
    [Serializable]
    public abstract class DBBaseObject : DynamicObject
    {
        #region Fields: Internal

        internal Dictionary<string, object> Members = new Dictionary<string, object>();

        #endregion

        #region Properties: Public
        #endregion

        #region Operators: Public

        public object this[string key]
        {
            get
            {
                TryGetMember(key, out var res);
                return res;
            }
            set => TrySetMember(key, value);
        }

        #endregion

        #region Methods: Public

        public bool TrySetMember(string name, object value)
        {
            if (!Members.ContainsKey(name))
            {
                LogWriter.Log($"Error: Object dont's contains member {name}");
                return false;
            }
            try
            {
                var castedValue = Convert.ChangeType(value, Members[name].GetType());
                if (castedValue == null && value != null)
                    throw new Exception();

                Members[name] = value;
            }
            catch (Exception e)
            {
                LogWriter.Log($"Error: Can't cast {value.GetType().FullName} to {Members[name].GetType().FullName}");
            }
            return true;
        }

        public bool TryGetMember(string name, out object result)
        {
            result = null;
            if (!Members.ContainsKey(name))
            {
                LogWriter.Log($"Error: Object don't contains member {name}");
                return false;
            }
            result = Members[name];
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.Members.Keys;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            TrySetMember(binder.Name, value);
            return true;
        }
    
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            TryGetMember(binder.Name, out result);
            return true;
        }

        public T GetTypedMemberValue<T>(string memberName)
        {
            TryGetTypedMemberValue(memberName, out T converted);
            return converted;
        }

        public bool TryGetTypedMemberValue<T>(string memberName, out T converted)
        {
            converted = default(T);
            try
            {
                if (!TryGetMember(memberName, out var value))
                    return false;
                converted = (T)Convert.ChangeType(value, typeof(T));
                return true;
            }
            catch (Exception e)
            {
                LogWriter.Log($"Error: can't cast object's field {memberName} with value {Members[memberName]} to {typeof(T).Name}");
                return false;
            }
        }

        #endregion

    }
}
