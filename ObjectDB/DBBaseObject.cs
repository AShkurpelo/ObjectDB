using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDB
{
    [Serializable]
    public class DBBaseObject : DynamicObject
    {
        Dictionary<string, object> _properties = new Dictionary<string, object>();

        public IEnumerable<string> ObjectDbMemberNames
        {
            get => this.GetType().GetMembers().Where(member => Attribute.IsDefined(member, typeof(ObjectDBAttribute))).Select(member => member.Name);
        }

        public object this[string key]
        {
            get => _properties.ContainsKey(key) ? _properties[key] : null;
            set => _properties[key] = value;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this._properties.Keys;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!_properties.ContainsKey(binder.Name))
            {
                Logger.Log($"Error: Object dont's contains member {binder.Name}");
                return true;
            }
            _properties[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if (!_properties.ContainsKey(binder.Name))
            {
                var objectMemberName = ObjectDbMemberNames.Where(name => name == binder.Name);
                if (objectMemberName.Any())
                {
                    result = this.GetType().GetField(objectMemberName.First()).GetValue(this);
                    return true;
                }
                Logger.Log($"Error: Object don't contains member {binder.Name}");                
                return true;
            }
            result = _properties[binder.Name];
            var isOldMember = ObjectDbMemberNames.Any(name => name == binder.Name);
            return true;
        }

        public T GetTypedMemberValue<T>(string memberName)
        {
            if (!_properties.ContainsKey(memberName))
            {
                Logger.Log($"Error: Object dont's contains member {memberName}");
                return default(T);
            }
            try
            {
                return (T) Convert.ChangeType(_properties[memberName], typeof(T));
            }
            catch (Exception e)
            {
                Logger.Log($"Error: can't cast object's field value {_properties[memberName]} to {typeof(T)}");
                return default(T);
            }
        }

        public bool TryGetTypedMemberValue<T>(string memberName, out T converted)
        {
            converted = default(T);
            if (!_properties.ContainsKey(memberName))
            {
                Logger.Log($"Error: Object don't contains member {memberName}");
                return false;
            }
            try
            {
                converted = (T)Convert.ChangeType(_properties[memberName], typeof(T));
                return true;
            }
            catch (Exception e)
            {
                Logger.Log($"Error: can't cast object's field value {_properties[memberName]} to {typeof(T)}");
                return false;
            }
        }
    }
}
