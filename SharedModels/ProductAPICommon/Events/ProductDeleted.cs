using SharedModels.EventStoreCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.ProductAPICommon.Events
{
    public class ProductDeleted : IEvent
    {
        public Guid Id { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
