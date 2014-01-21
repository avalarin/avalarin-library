using System;
using System.Data;

namespace Avalarin.Data {
    public delegate T MapperWithIndex<out T>(IDataReader reader, Int32 index);
}