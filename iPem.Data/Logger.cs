using iPem.Core;
using iPem.Model;
using System;
using System.IO;
using System.Text;

namespace iPem.Data {
    public static class Logger {
        public static void Write(Event log) {
            try {
                var fullPath = String.Format(@"{0}\log", Environment.CurrentDirectory);
                var runName = String.Format(@"{0}\Run{1}.log", fullPath, DateTime.Today.ToString("yyyyMMdd"));
                var sysName = String.Format(@"{0}\Err{1}.log", fullPath, DateTime.Today.ToString("yyyyMMdd"));
                if(!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                if(log != null) {
                    var runFile = new FileInfo(runName);
                    if(runFile.Exists) {
                        using(var sw = runFile.AppendText()) {
                            sw.WriteLine(String.Format("{0} {1}", log.Time.ToString("MM/dd HH:mm:ss"), log.Message));
                            sw.Close();
                        }
                    } else {
                        using(var sw = runFile.CreateText()) {
                            sw.WriteLine(String.Format("{0} {1}", log.Time.ToString("MM/dd HH:mm:ss"), log.Message));
                            sw.Close();
                        }
                    }

                    if(log.Type == EventType.Error) {
                        var text = new StringBuilder();
                        text.AppendLine(String.Format("事件时间: {0}", log.Time.ToString("yyyy/MM/dd HH:mm:ss")));
                        text.AppendLine(String.Format("事件级别: {0}", log.Type));
                        text.AppendLine("事件内容:");
                        text.AppendLine(log.Message);
                        text.AppendLine(log.FullMessage);
                        text.AppendLine("=======================================================================================");

                        var sysFile = new FileInfo(sysName);
                        if(sysFile.Exists) {
                            using(var sw = sysFile.AppendText()) {
                                sw.WriteLine(text.ToString());
                                sw.Close();
                            }
                        } else {
                            using(var sw = sysFile.CreateText()) {
                                sw.WriteLine(text.ToString());
                                sw.Close();
                            }
                        }
                    }
                }
            } catch { }
        }

        public static void Write(EventType type, string message, string fullMessage) {
            Write(new Event {
                Id = Guid.NewGuid(),
                Type = type,
                Time = DateTime.Now,
                Message = message,
                FullMessage = fullMessage
            });
        }

        public static void Clear(DateTime fromTime, DateTime toTime) {
            var fullPath = String.Format(@"{0}\log", Environment.CurrentDirectory);
            if(!Directory.Exists(fullPath))
                return;

            while(fromTime <= toTime) {
                var runName = String.Format(@"{0}\Run{1}.log", fullPath, fromTime.ToString("yyyyMMdd"));
                var sysName = String.Format(@"{0}\Err{1}.log", fullPath, fromTime.ToString("yyyyMMdd"));

                var sysFile = new FileInfo(sysName);
                if(sysFile.Exists)
                    sysFile.Delete();

                var runFile = new FileInfo(runName);
                if(runFile.Exists)
                    runFile.Delete();

                fromTime = fromTime.AddDays(1);
            }
        }

        public static void Error(string message, Exception exception = null) {
            Write(EventType.Error, message, exception == null ? "" : exception.StackTrace);
        }

        public static void Warning(string message, Exception exception = null) {
            Write(EventType.Warning, message, exception == null ? "" : exception.StackTrace);
        }

        public static void Information(string message, Exception exception = null) {
            Write(EventType.Info, message, exception == null ? "" : exception.StackTrace);
        }
    }
}
