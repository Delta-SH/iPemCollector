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

        public static List<ITask> GetHisTasks() {
            var _tasks = new List<ITask>();
            var _dll = Assembly.LoadFrom("iPem.Task.dll");
            var _type = typeof(ITask);
            var _classes = _dll.GetTypes().Where(t => t.IsClass && _type.IsAssignableFrom(t));
            foreach(var _class in _classes) {
                _tasks.Add((ITask)Activator.CreateInstance(_class));
            }
            return _tasks;
        }
    }
}
