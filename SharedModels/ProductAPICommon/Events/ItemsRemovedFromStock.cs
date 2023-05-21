using SharedModels.EventStoreCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.ProductAPICommon.Events
{
    public class ItemsRemovedFromStock : IEvent
    {
        public Guid Id { get; set; }

        public int ItemsInStock { get; set; }

        public DateTime RemovedAt { get; set; }
    }
}
