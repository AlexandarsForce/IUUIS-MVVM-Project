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
        private ObservableCollection<DERModel> ders;
        public List<DERType> FilterTypes { get; set; } = new List<DERType>{ DERType.SOLAR_PANEL, DERType.WIND_TURBINE };
        public MyICommand<string> FilterCommand { get; set; }
        private string filterIDTextBox = string.Empty;
        private DERType? filterTypeComboBox = null;
        private bool? lessThanRadioButton = false;
        private bool? equalToRadioButton = false;
        private bool? greaterThanRadioButton = false;

        public ObservableCollection<DERModel> DERs { get { return ders; } set { ders = value; OnPropertyChanged("DERs"); } }
        public string FilterIDTextBox { get { return filterIDTextBox; } set { filterIDTextBox = value; OnPropertyChanged("FilterIDTextBox"); } }
        public DERType? FilterTypeComboBox { get { return filterTypeComboBox; } set { filterTypeComboBox = value; OnPropertyChanged("FilterTypeComboBox"); } }
        public bool? LessThanRadioButton { get { return lessThanRadioButton; } set { lessThanRadioButton = value; OnPropertyChanged("LessThanRadioButton"); } }
        public bool? EqualToRadioButton { get { return equalToRadioButton; } set { equalToRadioButton = value; OnPropertyChanged("EqualToRadioButton"); } }
        public bool? GreaterThanRadioButton { get { return greaterThanRadioButton; } set { greaterThanRadioButton = value; OnPropertyChanged("GreaterThanRadioButton"); } }

        public NetworkEntitiesViewModel()
        {
            DERs = new ObservableCollection<DERModel>();
            FilterCommand = new MyICommand<string>(OnFilter);
        }

        public NetworkEntitiesViewModel(ObservableCollection<DERModel> ders)
        {
            DERs = ders;
            FilterCommand = new MyICommand<string>(OnFilter);
        }

        private void OnFilter(string filterKind)
        {
            DERs = MainWindowViewModel.DERs;
            switch (filterKind)
            {
                case "SearchFilter":
                    ObservableCollection<DERModel> filteredDERs = new ObservableCollection<DERModel>();
                    if (!string.IsNullOrEmpty(FilterIDTextBox))
                    {
                        if(FilterTypeComboBox != null)
                        {
                            if (LessThanRadioButton == true)
                            {
                                foreach (var der in DERs)
                                {
                                    if (der.Id < int.Parse(FilterIDTextBox) && der.Type.Name == FilterTypeComboBox)
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                            else if (EqualToRadioButton == true)
                            {
                                foreach (var der in DERs)
                                {
                                    if (der.Id == int.Parse(FilterIDTextBox) && der.Type.Name == FilterTypeComboBox)
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                            else if (GreaterThanRadioButton == true)
                            {
                                foreach (var der in DERs)
                                {
                                    if (der.Id > int.Parse(FilterIDTextBox) && der.Type.Name == FilterTypeComboBox)
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if(LessThanRadioButton == true)
                            {
                                foreach(var der in DERs)
                                {
                                    if (der.Id < int.Parse(FilterIDTextBox))
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                            else if(EqualToRadioButton == true)
                            {
                                foreach(var der in DERs)
                                {
                                    if (der.Id == int.Parse(FilterIDTextBox))
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                            else if(GreaterThanRadioButton == true)
                            {
                                foreach(var der in DERs)
                                {
                                    if (der.Id > int.Parse(FilterIDTextBox))
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var der in DERs)
                        {
                            if (FilterTypeComboBox != null && der.Type.Name == FilterTypeComboBox)
                            {
                                filteredDERs.Add(der);
                            }
                        }
                    }
                    DERs = filteredDERs;
                    break;

                case "ResetFilter":
                    FilterIDTextBox = string.Empty;
                    FilterTypeComboBox = null;
                    LessThanRadioButton = false;
                    EqualToRadioButton = false;
                    GreaterThanRadioButton = false;
                    break;
                default:
                    break;
            }
        }
    }
}
