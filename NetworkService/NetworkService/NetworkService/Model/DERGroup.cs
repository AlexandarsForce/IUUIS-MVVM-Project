using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class DERGroup
    {
        public DERType Name { get; set; }
        public ObservableCollection<DERModel> DERModels { get; set; }

        public DERGroup(DERType name)
        {
            Name = name;
            DERModels = new ObservableCollection<DERModel>();
        }
    }
}
