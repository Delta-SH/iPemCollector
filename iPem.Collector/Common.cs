using iPem.Task;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace iPem.Collector {
    public partial class Common {
        public static List<IActTask> GetActTasks() {
            var _tasks = new List<IActTask>();
            var _dll = Assembly.LoadFrom("iPem.Task.dll");
            var _type = typeof(IActTask);
            var _classes = _dll.GetTypes().Where(t => t.IsClass && _type.IsAssignableFrom(t));
            foreach(var _class in _classes) {
                _tasks.Add((IActTask)Activator.CreateInstance(_class));
            }
            return _tasks;
        }

        public static List<IHisTask> GetHisTasks() {
            var _tasks = new List<IHisTask>();
            var _dll = Assembly.LoadFrom("iPem.Task.dll");
            var _type = typeof(IHisTask);
            var _classes = _dll.GetTypes().Where(t => t.IsClass && _type.IsAssignableFrom(t));
            foreach(var _class in _classes) {
                _tasks.Add((IHisTask)Activator.CreateInstance(_class));
            }
            return _tasks;
        }
    }
}
