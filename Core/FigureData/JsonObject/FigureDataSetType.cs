using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WibboEmulator.Core.FigureData.JsonObject
{
    public class FigureDataSetType
    {
        public string Type { get; set; }
        public int PaletteId { get; set; }
        public bool Mandatory_m_0 { get; set; }
        public bool Mandatory_f_0 { get; set; }
        public bool Mandatory_m_1 { get; set; }
        public bool Mandatory_f_1 { get; set; }
        public List<FigureDataSet> Sets { get; set; }
    }
}
