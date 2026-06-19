using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        public ObservableCollection<DERModel> DERs;

        public NetworkEntitiesViewModel()
        {
            DERs = new ObservableCollection<DERModel>();
        }

        public NetworkEntitiesViewModel(ObservableCollection<DERModel> ders)
        {
            DERs = ders;
        }
    }
}
