using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleParcer
{
    public class Record
    {
        public decimal ProjectID { get; set; }
        public decimal IdInXml { get; set; }
        public string BlockName { get; set; }
        public string BlockVariableName { get; set; }
        public string ItemName { get; set; }
        public string ItemVariableName { get; set; }
        public string ItemValue { get; set; }
        public decimal Num { get; set; }
    }
}
