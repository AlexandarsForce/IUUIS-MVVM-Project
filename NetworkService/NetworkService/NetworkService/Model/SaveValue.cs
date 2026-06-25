using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public enum CommandType { VIEW_CHANGE, ENTITY_CHANGE, DISPLAY_CHANGE }

    public class SaveValue<TFirst, TSecond>
    {
        public SaveValue(TFirst commandType, TSecond state)
        {
            CommandType = commandType;
            SavedState = state;
        }
        public SaveValue()
        {

        }
        public TFirst CommandType { get; set; }
        public TSecond SavedState { get; set; }
    }
}
