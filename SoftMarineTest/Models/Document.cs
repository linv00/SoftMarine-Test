using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SoftMarineTest.Models
{
    public class Document : INotifyPropertyChanged
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

        private string docCode { get; set; }
        private string sectionCode { get; set; }
        private string docName { get; set; }
        private string lastChangeDate { get; set; }
        private int countLinkedFile { get; set; }
    }
}
