using iPem.Core;
using System;

namespace iPem.Model {
    public partial class OrderEntity {
        public OrderId Id { get; set; }

        public string Param { get; set; }

        public DateTime Time { get; set; }
    }
}
