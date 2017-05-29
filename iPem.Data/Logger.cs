using iPem.Core;
using iPem.Model;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace iPem.Data {
    public static class Logger {
        //静态读写锁
        private static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();

        public static void Write(Event log) {
            try {
                //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
                //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
                //从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
                //因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
                LogWriteLock.EnterWriteLock();

                var fullPath = String.Format(@"{0}\log", Environment.CurrentDirectory);
                var runName = String.Format(@"{0}\Run{1}.log", fullPath, DateTime.Today.ToString("yyyyMMdd"));
                var sysName = String.Format(@"{0}\Err{1}.log", fullPath, DateTime.Today.ToString("yyyyMMdd"));
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                if (log != null) {
                    var runFile = new FileInfo(runName);
                    if (runFile.Exists) {
                        using (var sw = runFile.AppendText()) {
                            sw.WriteLine(String.Format("{0} {1}", log.Time.ToString("MM/dd HH:mm:ss"), log.Message));
                            sw.Close();
                        }
                    } else {
                        using (var sw = runFile.CreateText()) {
                            sw.WriteLine(String.Format("{0} {1}", log.Time.ToString("MM/dd HH:mm:ss"), log.Message));
                            sw.Close();
                        }
                    }

                    if (log.Type == EventType.Error) {
                        var text = new StringBuilder();
                        text.AppendLine(String.Format("事件时间: {0}", log.Time.ToString("yyyy/MM/dd HH:mm:ss")));
                        text.AppendLine(String.Format("事件级别: {0}", log.Type));
                        text.AppendLine("事件内容:");
                        text.AppendLine(log.Message);
                        text.AppendLine(log.FullMessage);
                        text.AppendLine("=======================================================================================");

                        var sysFile = new FileInfo(sysName);
                        if (sysFile.Exists) {
                            using (var sw = sysFile.AppendText()) {
                                sw.WriteLine(text.ToString());
                                sw.Close();
                            }
                        } else {
                            using (var sw = sysFile.CreateText()) {
                                sw.WriteLine(text.ToString());
                                sw.Close();
                            }
                        }
                    }
                }
            } catch {
            } finally {
                //退出写入模式，释放资源占用
                //注意：一次请求对应一次释放
                //若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
                //若请求处理完成后未释放将会触发异常[此模式不下允许以递归方式获取写入锁定]
                LogWriteLock.ExitWriteLock();
            }
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
