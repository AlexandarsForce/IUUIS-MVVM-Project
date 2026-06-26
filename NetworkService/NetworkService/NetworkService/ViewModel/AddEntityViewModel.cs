using NetworkService.Helpers;
using NetworkService.Model;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NetworkService.ViewModel
{
    public class AddEntityViewModel : BindableBase
    {
        public List<DERType> DERTypes { get; set; } = new List<DERType> { DERType.SOLAR_PANEL, DERType.WIND_TURBINE };
        public MyICommand AddEntityCommand { get; set; }
        public MyICommand<object> TextBoxGotFocusCommand { get; set; }
        public MyICommand<object> TextBoxLostFocusCommand { get; set; }

        private string idTextBox = string.Empty;
        private string nameTextBox = string.Empty;
        private string idErrorLabelText = string.Empty;
        private DERType? typeComboBox = null;
        private Visibility idErrorLabel = Visibility.Hidden;
        private Visibility idErrorBorder = Visibility.Hidden;
        private Visibility nameErrorLabel = Visibility.Hidden;
        private Visibility nameErrorBorder = Visibility.Hidden;
        private Visibility typeErrorLabel = Visibility.Hidden;
        private Visibility typeErrorBorder = Visibility.Hidden;

        public string IDTextBox { get { return idTextBox; } set { idTextBox = value; OnPropertyChanged("IDTextBox"); } }
        public string NameTextBox { get { return nameTextBox; } set { nameTextBox = value; OnPropertyChanged("NameTextBox"); } }
        public string IDErrorLabelText { get { return idErrorLabelText; } set { idErrorLabelText = value; OnPropertyChanged("IDErrorLabelText"); } }
        public DERType? TypeComboBox { get { return typeComboBox; } set { typeComboBox = value; OnPropertyChanged("TypeComboBox"); } }
        public Visibility IDErrorLabel { get { return idErrorLabel; } set { idErrorLabel = value; OnPropertyChanged("IDErrorLabel"); } }
        public Visibility IDErrorBorder { get { return idErrorBorder; } set { idErrorBorder = value; OnPropertyChanged("IDErrorBorder"); } }
        public Visibility NameErrorLabel { get { return nameErrorLabel; } set { nameErrorLabel = value; OnPropertyChanged("NameErrorLabel"); } }
        public Visibility NameErrorBorder { get { return nameErrorBorder; } set { nameErrorBorder = value; OnPropertyChanged("NameErrorBorder"); } }
        public Visibility TypeErrorLabel { get { return typeErrorLabel; } set { typeErrorLabel = value; OnPropertyChanged("TypeErrorLabel"); } }
        public Visibility TypeErrorBorder { get { return typeErrorBorder; } set { typeErrorBorder = value; OnPropertyChanged("TypeErrorBorder"); } }
        public AddEntityViewModel()
        {
            AddEntityCommand = new MyICommand(OnAddEntity);
            TextBoxGotFocusCommand = new MyICommand<object>(TextBoxGotFocus);
            TextBoxLostFocusCommand = new MyICommand<object>(TextBoxLostFocus);
        }

        public void ResetFields()
        {
            idTextBox = string.Empty;
            nameTextBox = string.Empty;
            typeComboBox = null;
            idErrorLabel = Visibility.Hidden;
            idErrorBorder = Visibility.Hidden;
            nameErrorLabel = Visibility.Hidden;
            nameErrorBorder = Visibility.Hidden;
            typeErrorLabel = Visibility.Hidden;
            typeErrorBorder = Visibility.Hidden;
        }

        private void OnAddEntity()
        {
            SaveStatus();
            bool isValid = true;
            int newId = -1;

            if (string.IsNullOrEmpty(IDTextBox.Trim()))
            {
                IDErrorLabelText = "Invalid - enter number";
                IDErrorBorder = Visibility.Visible;
                IDErrorLabel = Visibility.Visible;
                isValid = false;
            }
            else
            {
                if (int.TryParse(IDTextBox.Trim(), out newId))
                {
                    foreach (DERModel d in MainWindowViewModel.DERs)
                    {
                        if (d.Id == newId)
                        {
                            newId = -1;
                            break;
                        }
                    }
                    if (newId == -1)
                    {
                        IDErrorLabelText = "Invalid - ID already used";
                        IDErrorBorder = Visibility.Visible;
                        IDErrorLabel = Visibility.Visible;
                        isValid = false;
                    }
                    else
                    {
                        IDErrorBorder = Visibility.Hidden;
                        IDErrorLabel = Visibility.Hidden;
                    }
                }
                else
                {
                    IDErrorLabelText = "Invalid - enter number";
                    IDErrorBorder = Visibility.Visible;
                    IDErrorLabel = Visibility.Visible;
                    isValid = false;
                }
            }

            if (string.IsNullOrEmpty(NameTextBox.Trim()))
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
                DERModel newDER = new DERModel(newId,NameTextBox,new DERModelType((DERType)TypeComboBox));
                MainWindowViewModel.DERs.Add(newDER);
                ToastNotify.RaiseToast("Success", "Successfully added new entity.", NotificationType.Success);
                MainWindowViewModel.Instance.CurrentViewModel = MainWindowViewModel.Instance.networkEntitiesViewModel;
            }
            else
            {
                ToastNotify.RaiseToast("Error", "Invalid entity properties, enter correct values.", NotificationType.Error);
            }
        }

        private void SaveStatus()
        {
            MainWindowViewModel.UndoStack.Push(new SaveValue<CommandType, object>
                (CommandType.ENTITY_CHANGE,
                new ObservableCollection<DERModel>(MainWindowViewModel.DERs)));
        }

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
