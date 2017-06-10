using System;

namespace iPem.Configurator {
    public partial class OrderEntity {
        public OrderId Id { get; set; }

        public string Param { get; set; }

        public DateTime Time { get; set; }
    }
}