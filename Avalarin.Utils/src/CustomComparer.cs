using System;
using System.Collections.Generic;

namespace Avalarin {
    public class CustomComparer<T, TKey> : IComparer<T> {
        private readonly IComparer<TKey> _baseComparer;
        private readonly Func<T, TKey> _keySelector;

        public CustomComparer(IComparer<TKey> baseComparer, Func<T, TKey> keySelector) {
            _baseComparer = baseComparer;
            _keySelector = keySelector;
        }

        public CustomComparer(Func<T, TKey> keySelector) {
            _baseComparer = Comparer<TKey>.Default;
            _keySelector = keySelector;
        }

        public int Compare(T x, T y) {
            return _baseComparer.Compare(_keySelector(x), _keySelector(y));
        }
    }
}