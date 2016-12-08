using System;

namespace iPem.Core {
    [Serializable]
    public class ListItem<T> {
        public int Index { get; set; }

        public T Id { get; set; }

        public string Text { get; set; }

        public string Comment { get; set; }
    }
}
