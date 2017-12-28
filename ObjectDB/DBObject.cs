using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDB
{
    [Serializable]
    public class DBObject : DBBaseObject
    {
        [ObjectDB]
        internal Guid __id__;

        IEnumerable<string> MemberNames => this.GetType().GetMembers()
                    .Where(member => Attribute.IsDefined(member, typeof(ObjectDBAttribute)))
                    .Select(member => member.Name);
    }
}
