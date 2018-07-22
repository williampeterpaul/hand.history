using hand.history.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services
{
    public interface IMapper<TTable> where TTable : Table
    {
        TTable Map();
    }
}
