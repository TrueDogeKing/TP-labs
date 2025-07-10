using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace RecursiveObjectBrowser.Model
{
    public class ModelCollection<T> : ObservableCollection<T>
    {
        // sort by K
        public void SortBy<K>(string propertyName) where K : IComparable
        {
            PropertyInfo? property = typeof(T).GetProperty(propertyName);
            if (property == null || property.PropertyType.GetInterface(nameof(IComparable)) == null)
                return;

            var sorted = this.OrderBy(x => (K?)property.GetValue(x)).ToList();

            this.Clear();
            foreach (var item in sorted)
                this.Add(item);
        }

        // find by string or int32
        public T? FindBy(string propertyName, object value)
        {
            PropertyInfo? property = typeof(T).GetProperty(propertyName);
            if (property == null)
                return default;

            return this.FirstOrDefault(x =>
            {
                object? val = property.GetValue(x);
                return val != null && val.ToString() == value.ToString();
            });
        }
    }
}
