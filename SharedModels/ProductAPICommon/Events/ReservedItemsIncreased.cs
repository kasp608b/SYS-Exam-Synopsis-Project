using SharedModels.EventStoreCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.ProductAPICommon.Events
{
    public class ReservedItemsIncreased : IEvent
    {
        public Guid Id { get; set; }
        public int ItemsReserved { get; set; }
        public DateTime IncreasedAt { get; set; }
    }
}
