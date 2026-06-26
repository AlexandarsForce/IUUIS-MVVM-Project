using NetworkService.Helpers;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        private ObservableCollection<DERModel> derModels;
        public List<DERType> FilterTypes { get; set; } = new List<DERType> { DERType.SOLAR_PANEL, DERType.WIND_TURBINE };
        public MyICommand<string> FilterCommand { get; set; }
        public MyICommand<string> EntityCommand { get; set; }
        public MyICommand<object> TextBoxGotFocusCommand { get; set; }
        public MyICommand<object> TextBoxLostFocusCommand { get; set; }


        private string filterIDTextBox = string.Empty;
        private DERType? filterTypeComboBox = null;
        private bool? lessThanRadioButton = false;
        private bool? equalToRadioButton = true;
        private bool? greaterThanRadioButton = false;

        public ObservableCollection<DERModel> DERModels { get { return derModels; } set { derModels = value; OnPropertyChanged("DERModels"); } }
        public string FilterIDTextBox { get { return filterIDTextBox; } set { filterIDTextBox = value; OnPropertyChanged("FilterIDTextBox"); } }
        public DERType? FilterTypeComboBox { get { return filterTypeComboBox; } set { filterTypeComboBox = value; OnPropertyChanged("FilterTypeComboBox"); } }
        public bool? LessThanRadioButton { get { return lessThanRadioButton; } set { lessThanRadioButton = value; OnPropertyChanged("LessThanRadioButton"); } }
        public bool? EqualToRadioButton { get { return equalToRadioButton; } set { equalToRadioButton = value; OnPropertyChanged("EqualToRadioButton"); } }
        public bool? GreaterThanRadioButton { get { return greaterThanRadioButton; } set { greaterThanRadioButton = value; OnPropertyChanged("GreaterThanRadioButton"); } }

        public NetworkEntitiesViewModel()
        {
            DERModels = new ObservableCollection<DERModel>();
            FilterCommand = new MyICommand<string>(OnFilter);
            EntityCommand = new MyICommand<string>(OnEntity);
            TextBoxGotFocusCommand = new MyICommand<object>(TextBoxGotFocus);
            TextBoxLostFocusCommand = new MyICommand<object>(TextBoxLostFocus);
        }

        public NetworkEntitiesViewModel(ObservableCollection<DERModel> newDers)
        {
            DERModels = newDers;
            FilterCommand = new MyICommand<string>(OnFilter);
            EntityCommand = new MyICommand<string>(OnEntity);
            TextBoxGotFocusCommand = new MyICommand<object>(TextBoxGotFocus);
            TextBoxLostFocusCommand = new MyICommand<object>(TextBoxLostFocus);
        }

        public void ResetFilter()
        {
            FilterIDTextBox = string.Empty;
            FilterTypeComboBox = null;
            LessThanRadioButton = false;
            EqualToRadioButton = true;
            GreaterThanRadioButton = false;

        }

        public void ResetSelection()
        {
            DERModels = MainWindowViewModel.DERs;
            foreach (DERModel d in DERModels)
            {
                d.IsSelected = false;
            }
        }

        private void OnFilter(string filterOption)
        {
            DERModels = MainWindowViewModel.DERs;
            int tempId;
            switch (filterOption)
            {
                case "SearchFilter":
                    ObservableCollection<DERModel> filteredDERs = new ObservableCollection<DERModel>();
                    bool idFiltered = int.TryParse(FilterIDTextBox, out tempId);
                    if (!string.IsNullOrEmpty(FilterIDTextBox) && idFiltered)
                    {
                        if (FilterTypeComboBox != null)
                        {
                            if (LessThanRadioButton == true)
                            {
                                foreach (var der in DERModels)
                                {
                                    if (der.Id < tempId && der.Type.Name == FilterTypeComboBox)
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                            else if (EqualToRadioButton == true)
                            {
                                foreach (var der in DERModels)
                                {
                                    if (der.Id == tempId && der.Type.Name == FilterTypeComboBox)
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                            else if (GreaterThanRadioButton == true)
                            {
                                foreach (var der in DERModels)
                                {
                                    if (der.Id > tempId && der.Type.Name == FilterTypeComboBox)
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                            else
                            {
                                filteredDERs = DERModels;
                            }
                        }
                        else
                        {
                            if (LessThanRadioButton == true)
                            {
                                foreach (var der in DERModels)
                                {
                                    if (der.Id < tempId)
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                            else if (EqualToRadioButton == true)
                            {
                                foreach (var der in DERModels)
                                {
                                    if (der.Id == tempId)
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                            else if (GreaterThanRadioButton == true)
                            {
                                foreach (var der in DERModels)
                                {
                                    if (der.Id > tempId)
                                    {
                                        filteredDERs.Add(der);
                                    }
                                }
                            }
                            else
                            {
                                filteredDERs = DERModels;
                            }
                        }
                    }
                    else
                    {
                        if (FilterTypeComboBox != null)
                        {
                            foreach (var der in DERModels)
                            {
                                if (der.Type.Name == FilterTypeComboBox)
                                {
                                    filteredDERs.Add(der);
                                }
                            }
                        }
                        else
                        {
                            filteredDERs = DERModels;
                        }
                    }
                    DERModels = filteredDERs;
                    break;

                case "ResetFilter":
                    ResetFilter();
                    break;
                default:
                    break;
            }
        }

        private void OnEntity(string entityOption)
        {
            switch (entityOption)
            {
                case "AddEntity":
                    SaveAddStatus();
                    if (MainWindowViewModel.Instance != null)
                    {
                        MainWindowViewModel.Instance.addEntityViewModel.ResetFields();
                        MainWindowViewModel.Instance.CurrentViewModel = MainWindowViewModel.Instance.addEntityViewModel;
                    }
                    break;
                case "DeleteEntity":
                    ObservableCollection<DERModel> tempDER = MainWindowViewModel.DERs;
                    List<DERModel> selectedDERs = new List<DERModel>();
                    SaveDeleteStatus();
                    foreach (var der in tempDER)
                    {
                        if(der.IsSelected)
                        {
                            selectedDERs.Add(der);
                        }
                    }
                    foreach (var der in selectedDERs)
                    {
                        DERModels.Remove(der);
                        tempDER.Remove(der);
                        if (NetworkDisplayViewModel.AddedToGrid != null)
                        {
                            var removedKey = NetworkDisplayViewModel.AddedToGrid.FirstOrDefault(
                            x => EqualityComparer<DERModel>.Default.Equals(x.Value, der)).Key;

                            if (!EqualityComparer<int>.Default.Equals(removedKey, default))
                            {
                                NetworkDisplayViewModel.AddedToGrid.Remove(removedKey);
                                List<int> connections = NetworkDisplayViewModel.FindAllConnections(removedKey);
                                foreach (int connectedTo in connections)
                                {
                                    int source = Math.Min(removedKey, connectedTo);
                                    int destination = Math.Max(removedKey, connectedTo);
                                    NetworkDisplayViewModel.DeleteLine(source, destination);
                                }
                            }
                        }
                    }
                    if (selectedDERs.Count > 0)
                    {
                        ToastNotify.RaiseToast("Success", "Successfully deleted selected entities.", NotificationType.Success);
                    }
                    else
                    {
                        ToastNotify.RaiseToast("Warning", "Please select entites to delete.", NotificationType.Warning);
                    }
                    MainWindowViewModel.DERs = tempDER;
                    break;
                default:
                    break;
            }
        }


        #region Undo
        private void SaveDeleteStatus()
        {
            MainWindowViewModel.UndoStack.Push(new SaveValue<CommandType, object>
                (CommandType.ENTITY_CHANGE,
                new ObservableCollection<DERModel>(DERModels)));
        }

        private void SaveAddStatus()
        {
            MainWindowViewModel.UndoStack.Push(new SaveValue<CommandType, object>
                (CommandType.VIEW_CHANGE, this.GetType()));
        }

        #endregion

        private void TextBoxGotFocus(object obj)
        {
            if (obj is TextBox textBox)
            {
                MainWindowViewModel.Instance.SelectedTextBox = textBox;
                MainWindowViewModel.Instance.SelectedTextBox.Focus();
                MainWindowViewModel.Instance.KeyboardVisibility = Visibility.Visible;
                MainWindowViewModel.Instance.IsKeyboardEnabled = true;
            }
        }
        private void TextBoxLostFocus(object obj)
        {
            if (obj is TextBox)
            {
                MainWindowViewModel.Instance.KeyboardVisibility = Visibility.Hidden;
                MainWindowViewModel.Instance.IsKeyboardEnabled = false;
            }
        }

    }
}
