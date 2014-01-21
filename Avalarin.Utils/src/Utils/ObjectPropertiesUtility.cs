using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Avalarin.Utils {
    public static class ObjectPropertiesUtility {
        private static readonly IDictionary<Type, IDictionary<string, IPropertyAccessor>> Cache =
            new ConcurrentDictionary<Type, IDictionary<string, IPropertyAccessor>>();
        private static readonly MethodInfo LambdaExpressionMethod = typeof(Expression).GetGenericMethod("Lambda",
            BindingFlags.Public | BindingFlags.Static, (type1) => new[] { typeof(Expression), typeof(ParameterExpression[]) });

        public static IDictionary<string, object> ToDictionary(this object data) {
            var dict = new Dictionary<string, object>();
            data.EachProperties((s, o) => dict[s] = o);
            return dict;
        }
        
        public static void EachProperties(this object data, Action<string, object> func) {
            if (data == null) throw new ArgumentNullException("data");
            if (func == null) throw new ArgumentNullException("func");
            var type = data.GetType();
            IDictionary<string, IPropertyAccessor> cache;
            if (Cache.TryGetValue(type, out cache)) {
                foreach (var item in cache) {
                    func(item.Key, item.Value.GetValue(data));
                }
            }
            else {
                cache = new Dictionary<string, IPropertyAccessor>();
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.CanRead)) {
                    var accessor = GetPropertyAccessor(type, property);
                    cache[property.Name] = accessor;
                    func(property.Name, accessor.GetValue(data));
                }
                Cache[type] = cache;
            }
        }

        private static IPropertyAccessor GetPropertyAccessor(Type type, PropertyInfo property) {
            if (type == null) throw new ArgumentNullException("type");
            if (property == null) throw new ArgumentNullException("property");
            var genericFastAccessorType = typeof(PropertyAccessor<,>).MakeGenericType(type, property.PropertyType);
            var parameterExpression = Expression.Parameter(type);
            var genericFuncType = typeof(Func<,>).MakeGenericType(type, property.PropertyType);
            var genericLambdaExpressionMethod = LambdaExpressionMethod.MakeGenericMethod(genericFuncType);
            var propertyAccessExpression = Expression.Property(parameterExpression, property);
            var lambdaExpression = (LambdaExpression)genericLambdaExpressionMethod.Invoke(null,
                new object[] { propertyAccessExpression, new[] { parameterExpression } });
            var compiledLambda = lambdaExpression.Compile();
            var accessor = (IPropertyAccessor)Activator.CreateInstance(genericFastAccessorType, compiledLambda);
            return accessor;
        } 

        public interface IPropertyAccessor {
            object GetValue(object obj);
        }
        private sealed class PropertyAccessor<T, T2> : IPropertyAccessor {
            private readonly Func<T, T2> _accessor;

            public PropertyAccessor(Func<T, T2> accessor) {
                _accessor = accessor;
            }

            public object GetValue(object obj) {
                return _accessor((T)obj);
            }
        }
    }
}