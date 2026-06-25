using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class MeteringValue<TFirst, TSecond>
    {
        public TFirst Time { get; set; }
        public TSecond Power { get; set; }

        public MeteringValue() { }
        public MeteringValue(TFirst time, TSecond power)
        {
            Time = time;
            Power = power;
        }
    }
}
