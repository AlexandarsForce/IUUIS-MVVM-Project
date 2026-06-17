using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
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
            set { imagePath = value; }
        }

        public DERModelType(DERType name, string imagePath)
        {
            this.name = name;
            this.imagePath = imagePath;
        }
    }
}
