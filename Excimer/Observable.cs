using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Excimer
{
    public interface IObservable
    {
        string GetJson();
        void SetFromJson(string value);
        void Subscribe(Action<object> action);
    }
    
    public class Observable<T> : IObservable
    {
        private T _value;
        private List<Action<object>> _subscriptions = new List<Action<object>>();

        public void Set(T value)
        {
            _value = value;
            foreach (var subscription in _subscriptions)
                subscription(value);
        }

        public T Get()
        {
            return _value;
        }

        public string GetJson()
        {
            return JsonSerializer.Serialize(_value);
        }

        public void SetFromJson(string value)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Action<object> action)
        {
            _subscriptions.Add(action);
        }
    }

    public class ObservableArray<T> : IObservable
    {
        private List<T> _list = new List<T>();
        private List<Action<object>> _subscriptions = new List<Action<object>>();

        public string GetJson()
        {
            throw new NotImplementedException();
        }

        public void SetFromJson(string value)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Action<object> action)
        {
            _subscriptions.Add(action);
        }
    }

    public static class ObservableExtensions
    {
        public static IEnumerable<FieldInfo> ObservableFields(this Type type)
        {
            return type.GetFields().Where(field => typeof(IObservable).IsAssignableFrom(field.FieldType));
        }
    }

}
