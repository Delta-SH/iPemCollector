using iPem.Model;
using System;
using System.Collections.Generic;

namespace iPem.Task {
    public interface IActTask {
        string Id { get; }

        string Name { get; }

        long Seconds { get; set; }

        DateTime Start { get; set; }

        DateTime End { get; set; }

        DateTime Last { get; set; }

        DateTime Next { get; set; }

        List<Event> Events { get; set; }

        int Order { get; }

        void Execute();
    }
}
