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
        private List<MeteringValue<DateTime, int>> lastPowers;
        private DERState state;

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
            set { power = value; OnPropertyChanged("Power"); ValidateSelf(); }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public DERState State
        {
            get { return state; }
            set { state = value; OnPropertyChanged("State"); }
        }

        public List<MeteringValue<DateTime, int>> LastPowers
        {
            get { return lastPowers; }
            set { lastPowers = value; OnPropertyChanged("LastPowers"); }
        }

        public DERModel() { Id = totalId++; }

        public DERModel(int id, string name, DERModelType type)
        {
            Id = id;
            Name = name;
            Type = type;
            State = DERState.LOW;
            lastPowers = new List<MeteringValue<DateTime, int>>();
            totalId++;
        }

        protected override void ValidateSelf()
        {
            if(Power < 1)
            {
                State = DERState.LOW;
            }
            else if(Power > 5)
            {
                State = DERState.HIGH;
            }
            else
            {
                State = DERState.NORMAL;
            }
        }
    }
}
