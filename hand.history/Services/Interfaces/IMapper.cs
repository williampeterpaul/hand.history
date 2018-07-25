using hand.history.DataObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace hand.history.Services.Interfaces
{
    public interface IMapper<TTable> where TTable : Table
    {
        TTable Map(string[] text);
    }
}
