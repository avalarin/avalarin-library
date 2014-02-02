using System;
using System.Data;

namespace Avalarin.Data {
    internal interface IOutputParameter {
        string Name { get; }
        int Size { get; }
        DbType DbType { get; }

        void SetValue(object value);
    }

    internal sealed class OutputParameter : IOutputParameter {
        private readonly Action<object> _setter;

        public string Name { get; private set; }
        public int Size { get; private set; }
        public DbType DbType { get; private set; }

        public void SetValue(object value) {
            _setter(value);
        }

        public OutputParameter(string name, DbType type, int size, Action<object> setter) {
            if (name == null) throw new ArgumentNullException("name");
            if (setter == null) throw new ArgumentNullException("setter");
            Name = name;
            Size = size;
            _setter = setter;
            DbType = type;
        }
    }

    internal sealed class OutputParameter<T> : IOutputParameter {
        private readonly Action<T> _setter;

        public string Name { get; private set; }
        public int Size { get; private set; }
        public DbType DbType { get; private set; }

        public void SetValue(object value) {
            _setter((T)value);
        }

        public OutputParameter(string name, DbType type, int size, Action<T> setter) {
            if (name == null) throw new ArgumentNullException("name");
            if (setter == null) throw new ArgumentNullException("setter");
            Name = name;
            Size = size;
            _setter = setter;
            DbType = type;
        }

        public OutputParameter(string name, int size, Action<T> setter) 
            : this(name, DbTypeConverter.TypeToDbType(typeof(T)), size, setter) {  
        }
    }
}