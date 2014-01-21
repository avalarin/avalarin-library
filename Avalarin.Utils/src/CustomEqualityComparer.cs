using System;
using System.Collections.Generic;

namespace Avalarin {
    public class CustomEqualityComparer<T, TKey> : IEqualityComparer<T> {
        private readonly IEqualityComparer<TKey> _baseComparer;
        private readonly Func<T, TKey> _keySelector;

        public CustomEqualityComparer(IEqualityComparer<TKey> baseComparer, Func<T, TKey> keySelector) {
            _baseComparer = baseComparer;
            _keySelector = keySelector;
        }

        public CustomEqualityComparer(Func<T, TKey> keySelector) {
            _baseComparer = EqualityComparer<TKey>.Default;
            _keySelector = keySelector;
        }

        public bool Equals(T x, T y) {
            return _baseComparer.Equals(_keySelector(x), _keySelector(y));
        }

        public int GetHashCode(T obj) {
            return _baseComparer.GetHashCode(_keySelector(obj));
        }
    }
}