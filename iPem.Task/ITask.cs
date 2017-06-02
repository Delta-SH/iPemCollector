using iPem.Model;
using System;
using System.Collections.Generic;

namespace iPem.Task {
    public interface ITask {
        string Id { get; }

        string Name { get; }

        long Seconds { get; set; }

        DateTime Time { get; set; }

        DateTime Last { get; set; }

        DateTime Next { get; set; }

        List<Event> Events { get; set; }

        void Execute();
    }
}
