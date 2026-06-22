using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public enum DERState
    {
        LOW,
        NORMAL,
        HIGH
    }
    public enum DERType
    {
        SOLAR_PANEL,
        WIND_TURBINE
    }
    public class DERModelType
    {
        private DERType name;
        private string imagePath;
        
        public DERType Name
        {
            get { return name; }
            set { name = value; }
        }
        public string ImagePath
        {
            get { return imagePath; }
            private set { imagePath = value; }
        }

        public DERModelType(DERType name)
        {
            Name = name;
            if(Name == DERType.SOLAR_PANEL)
            {
                ImagePath = "..\\..\\Assets\\Images\\solar_panel.jpg";
            }
            else if(Name == DERType.WIND_TURBINE)
            {
                ImagePath = "..\\..\\Assets\\Images\\wind_turbine.jpg";
            }
        }
    }
}
