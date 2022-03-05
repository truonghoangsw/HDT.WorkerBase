using HDT.Core.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDT.EventBus.Local
{
    public class HDTLocalEventBusOptions
    {
        public ITypeList<IEventHandler> Handlers { get; }
        public HDTLocalEventBusOptions()
        {
            Handlers = new TypeList<IEventHandler>();
        }

    }
}
