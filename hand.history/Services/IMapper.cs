using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services
{
    public interface IMapper<TSummary> where TSummary : Table
    {
        TSummary Map();
    }
}
