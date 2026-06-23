using NetworkService.Helpers;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkService.ViewModel
{
    public class AddEntityViewModel : BindableBase
    {
        public List<DERType> DERTypes { get; set; } = new List<DERType> { DERType.SOLAR_PANEL, DERType.WIND_TURBINE };
        public MyICommand AddEntityCommand { get; set; }

        private string nameTextBox = string.Empty;
        private DERType? typeComboBox = null;
        private Visibility nameErrorLabel = Visibility.Hidden;
        private Visibility nameErrorBorder = Visibility.Hidden;
        private Visibility typeErrorLabel = Visibility.Hidden;
        private Visibility typeErrorBorder = Visibility.Hidden;

        public string NameTextBox { get { return nameTextBox; } set { nameTextBox = value; OnPropertyChanged("NameTextBox"); } }
        public DERType? TypeComboBox { get { return typeComboBox; } set { typeComboBox = value; OnPropertyChanged("TypeComboBox"); } }
        public Visibility NameErrorLabel { get { return nameErrorLabel; } set { nameErrorLabel = value; OnPropertyChanged("NameErrorLabel"); } }
        public Visibility NameErrorBorder { get { return nameErrorBorder; } set { nameErrorBorder = value; OnPropertyChanged("NameErrorBorder"); } }
        public Visibility TypeErrorLabel { get { return typeErrorLabel; } set { typeErrorLabel = value; OnPropertyChanged("TypeErrorLabel"); } }
        public Visibility TypeErrorBorder { get { return typeErrorBorder; } set { typeErrorBorder = value; OnPropertyChanged("TypeErrorBorder"); } }
        public AddEntityViewModel()
        {
            AddEntityCommand = new MyICommand(OnAddEntity);
        }

        public void ResetFields()
        {
            nameTextBox = string.Empty;
            typeComboBox = null;
            nameErrorLabel = Visibility.Hidden;
            nameErrorBorder = Visibility.Hidden;
            typeErrorLabel = Visibility.Hidden;
            typeErrorBorder = Visibility.Hidden;
        }

        private void OnAddEntity()
        {
            bool isValid = true;
            if(string.IsNullOrEmpty(NameTextBox.Trim()))
            {
                NameErrorBorder = Visibility.Visible;
                NameErrorLabel = Visibility.Visible;
                isValid = false;
            }
            else
            {
                NameErrorBorder = Visibility.Hidden;
                NameErrorLabel = Visibility.Hidden;
            }

            if (typeComboBox == null)
            {
                TypeErrorBorder = Visibility.Visible;
                TypeErrorLabel = Visibility.Visible;
                isValid = false;
            }
            else
            {
                TypeErrorBorder = Visibility.Hidden;
                TypeErrorLabel = Visibility.Hidden;
            }
            
            if (isValid)
            {
                DERModel newDER = new DERModel(NameTextBox,new DERModelType((DERType)TypeComboBox));
                MainWindowViewModel.DERs.Add(newDER);
                ToastNotify.RaiseToast("Success", "Successfully added new entity", NotificationType.Success);
                MainWindowViewModel.Instance.CurrentViewModel = MainWindowViewModel.Instance.networkEntitiesViewModel;
            }
            else
            {
                ToastNotify.RaiseToast("Error", "Invalid entity properties, enter correct values", NotificationType.Error);
            }
        }
    }
}
