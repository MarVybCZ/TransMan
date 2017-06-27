using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TransMan
{
    public class EnumerationWrapper
    {
        public List<EnumerationType> EnumTypes;

        public EnumerationWrapper(PMSEntities ent)
        {
            EnumTypes = new List<EnumerationType>();

            var xxx = ent.Enumerations.ToList().Distinct(new EnumEquity());

            foreach (Enumeration _enum in xxx)
            {
                EnumTypes.Add(new EnumerationType(_enum.enumerationType, ent.Enumerations.Where(x => x.enumerationType == _enum.enumerationType).ToList()));
            }

            EnumTypes = EnumTypes.OrderBy(x => x.Name).ToList();
        }
    }

    public class EnumEquity : IEqualityComparer<Enumeration>
    {
        public bool Equals(Enumeration x, Enumeration y)
        {
            return (x.enumerationType == y.enumerationType);
        }

        public int GetHashCode(Enumeration obj)
        {
            return obj.enumerationType == null ? 0 : obj.enumerationType.GetHashCode();
        }
    }

    public class EnumerationType
    {
        public EnumerationType(string name, List<Enumeration> enums)
        {
            Name = name;
            Enumerations = enums;
        }

        public string Name { get; set; }
        public List<Enumeration> Enumerations { get; set; }
    }
}
