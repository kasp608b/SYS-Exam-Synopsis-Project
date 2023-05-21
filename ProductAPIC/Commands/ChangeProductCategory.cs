using SharedModels.EventStoreCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPIC.Commands
{
    public class ChangeProductCategory : ICommand
    {
        public Guid Id { get; set; }

        public string Category { get; set; }
        
    }
}
