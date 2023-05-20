using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.EventStoreCQRS
{
    public abstract class Aggregate<TId>
    {
        public TId Id { get; protected set; }
        public abstract void When(object @event);
    }
}
