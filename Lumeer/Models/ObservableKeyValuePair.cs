using Lumeer.Utils;

namespace Lumeer.Models
{
    public class ObservableKeyValuePair<K, V> : NotifyPropertyChanged
    {
        public K Key { get; }

        private V _value;
        public V Value
        {
            get { return _value; }
            set { SetValue(ref _value, value); }
        }

        public ObservableKeyValuePair(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }
}
