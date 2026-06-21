using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class DERModel : ValidationBase
    {
        private static int totalId = 0;
        private int id;
        private string name;
        private DERModelType type;
        private int power;
        private bool isSelected;

        public int Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged("Id"); }
        }

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        public DERModelType Type
        {
            get { return type; }
            set { type = value; OnPropertyChanged("Type"); }
        }

        public int Power
        {
            get { return power; }
            set { power = value; OnPropertyChanged("Power"); }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public DERModel() { Id = ++totalId; }

        public DERModel(string name, DERModelType type)
        {
            Id = ++totalId;
            Name = name;
            Type = type;
        }

        protected override void ValidateSelf()
        {
            if(string.IsNullOrWhiteSpace(Name))
            {
                ValidationErrors["Name"] = "Name is required.";
            }
            if(Power < 1 || Power > 5)
            {
                ValidationErrors["Power"] = "Power must be between 1 and 5.";
            }
        }
    }
}
