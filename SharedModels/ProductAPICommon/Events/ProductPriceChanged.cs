﻿using SharedModels.EventStoreCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.ProductAPICommon.Events
{
    public class ProductPriceChanged : IEvent
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
