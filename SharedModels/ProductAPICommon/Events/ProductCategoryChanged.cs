using SharedModels.EventStoreCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.ProductAPICommon.Events
{
    internal class ProductCategoryChanged : IEvent
    {
        public Guid Id => throw new NotImplementedException();
    }
}
