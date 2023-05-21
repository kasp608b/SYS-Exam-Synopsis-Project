using SharedModels.EventStoreCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPIC.Commands
{
    public class AddItemsToStock : ICommand
    {
        public Guid Id { get; set; }

        public int ItemsInStock { get; set; }

        public DateTime AddedAt { get; set; }
    }
}
