﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels
{
    public class CreditStandingChangedMessage
    {
        public Guid CustomerId { get; set; }
        public bool NewCreditStanding { get; set; }
    }
}
