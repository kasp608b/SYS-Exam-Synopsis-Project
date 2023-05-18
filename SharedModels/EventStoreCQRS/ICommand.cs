using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.EventStoreCQRS
{
    public interface ICommand
    {
        Guid Id { get; }
    }
}
