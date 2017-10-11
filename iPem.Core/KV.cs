using System;

namespace iPem.Core {
    /// <summary>
    /// KeyValue Pair
    /// </summary>
    [Serializable]
    public class KV<K, V> {
        /// <summary>
        /// Class Constructor
        /// </summary>
        public KV() { }

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public KV(K key, V value) {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// Key
        /// </summary>
        public K Key { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public V Value { get; set; }
    }
}
