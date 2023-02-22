using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace SoftMarineTest.Models
{
    public class Section : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Section()
        {

        }

        public Section(string code, string parentCode, string name)
        {
            this.sectionCode = code;
            this.parentSectionCode = parentCode;
            this.sectionName = name;
            this.hasChildren = false;
            this.arrowPath = "/Assets/downarrow.png";
        }

        public string sectionCode { get; set; }
        public string parentSectionCode { get; set; }
        public string sectionName { get; set; }
        public bool hasChildren { get; set; }
        public string arrowPath { get; set; }
    }
}
