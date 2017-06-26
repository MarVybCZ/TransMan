using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransMan
{
    public class EnumChild
    {
        public string LocKey { get; set; }
        public string Value { get; set; }

        [DisplayName("Sort order")]
        public int SortOrder { get; set; }


        public bool IsDefault { get; set; }
    }

    public class EnumItem
    {
        public string EnumType { get; set; }
        public ObservableCollection<EnumChild> EnumChilds { get; set; }

        public EnumItem()
        {
            EnumChilds = new ObservableCollection<EnumChild>();
        }        
    }
}
