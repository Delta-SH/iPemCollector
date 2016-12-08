using iPem.Model;
using System;
using System.Collections.Generic;

namespace iPem.Task {
    public interface IHisTask {
        string Id { get; }

        string Name { get; }

        long Seconds { get; set; }

        DateTime Time { get; set; }

        DateTime Last { get; set; }

        DateTime Next { get; set; }

        List<Event> Events { get; set; }

        int Order { get; }

        void Execute();
    }
}
