using iPem.Core;
using iPem.Model;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace iPem.Data {
    public static class Logger {
        //日志文件大于2M，创建新文件
        private const int maxFileSize = 1024 * 1024 * 2;
        //日志目录大于200M，自动清理7天前的日志文件。
        private const int maxDirSize = 1024 * 1024 * 200;
        //日志目录
        private static string fullPath = String.Format(@"{0}\log", AppDomain.CurrentDomain.BaseDirectory);
        //静态读写锁
        private static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();

        public static void Write(Event log) {
            try {
                //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
                //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
                //从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
                //因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
                LogWriteLock.EnterWriteLock();

                var runName = String.Format(@"{0}\Run{1}.log", fullPath, DateTime.Today.ToString("yyyyMMdd"));
                var sysName = String.Format(@"{0}\Err{1}.log", fullPath, DateTime.Today.ToString("yyyyMMdd"));
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);

                if (log != null) {
                    if (log.Type == EventType.Error) {
                        var text = new StringBuilder();
                        text.AppendLine(String.Format("日志名称: {0}", log.Message));
                        text.AppendLine(String.Format("日志时间: {0}", log.Time.ToString("yyyy/MM/dd HH:mm:ss")));
                        text.AppendLine(String.Format("日志级别: {0}", log.Type));
                        text.Append("日志详情: ");
                        text.AppendLine(log.FullMessage);
                        text.AppendLine("=======================================================================================");

                        var file = new FileInfo(sysName);
                        if (!file.Exists) {
                            using (var sw = file.CreateText()) {
                                sw.WriteLine(text.ToString());
                                sw.Close();
                            }
                        } else {
                            if (file.Length > maxFileSize) {
                                file.MoveTo(string.Format("{0}.{1}", file.FullName, GetFileNo(file.Name)));

                                file = new FileInfo(sysName);
                                if (!file.Exists) {
                                    using (file.Create()) { }
                                }
                            }

                            using (var sw = file.AppendText()) {
                                sw.WriteLine(text.ToString());
                                sw.Close();
                            }
                        }
                    } else {
                        var file = new FileInfo(runName);
                        var text = String.Format("{0} {1}", log.Time.ToString("MM/dd HH:mm:ss"), log.Message);
                        if (!file.Exists) {
                            using (var sw = file.CreateText()) {
                                sw.WriteLine(text);
                                sw.Close();
                            }
                        } else {
                            if (file.Length > maxFileSize) {
                                file.MoveTo(string.Format("{0}.{1}", file.FullName, GetFileNo(file.Name)));

                                file = new FileInfo(runName);
                                if (!file.Exists) {
                                    using (file.Create()) { }
                                }
                            }

                            using (var sw = file.AppendText()) {
                                sw.WriteLine(text);
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

        public static void Clear() {
            try {
                //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
                //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
                //从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
                //因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
                LogWriteLock.EnterWriteLock();

                var directory = new DirectoryInfo(fullPath);
                if (!directory.Exists)
                    return;

                long size = 0;
                var files = directory.GetFiles();
                foreach (var file in files) {
                    size += file.Length;
                }

                if (size < maxDirSize)
                    return;

                foreach (var file in files) {
                    if (file.LastWriteTime < DateTime.Today.AddDays(-7))
                        file.Delete();
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

        public static void Error(string message, Exception exception) {
            Write(EventType.Error, message, string.Format("{0}{1}{2}", exception.ToString(), Environment.NewLine, exception.Source ?? ""));
        }

        public static void Warning(string message) {
            Write(EventType.Warning, message, "");
        }

        public static void Information(string message) {
            Write(EventType.Info, message, "");
        }

        public static int GetFileNo(string name) {
            var max = 0;
            var directory = new DirectoryInfo(fullPath);
            if (!directory.Exists)
                return max++;

            var pattern = string.Format("{0}.", name);
            var files = directory.GetFiles();
            foreach (var file in files) {
                if (!file.Name.StartsWith(pattern))
                    continue;

                int current;
                if (int.TryParse(file.Name.Substring(pattern.Length), out current) == false) {
                    continue;
                }

                if (current > max)
                    max = current;
            }

            return ++max;
        }
    }
}
