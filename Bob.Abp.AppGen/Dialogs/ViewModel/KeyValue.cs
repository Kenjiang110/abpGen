using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Dialogs.ViewModel
{
    public class KeyValue<TKey, TValue> : NotifyClassBase
    {
        public TKey Key { get; set; }

        public TValue Value { get; set; }
    }
}
