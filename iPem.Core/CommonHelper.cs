using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.IO;
using System.Management;

namespace iPem.Core {
    public partial class CommonHelper {
        private static readonly string[] DefaultSalts = new string[] { "91E709Fc", "1A8d", "8d1e", "a28822a6B010" };

        public static string GlobalSeparator {
            get { return "┆"; }
        }

        public static List<object> GetIntervalStore() {
            var data = new List<object>();
            data.Add(new { Id = 3600, Name = "小时/次" });
            data.Add(new { Id = 60, Name = "分钟/次" });
            data.Add(new { Id = 1, Name = "秒钟/次" });
            return data;
        }

        public static List<object> GetDbTypeStore() {
            var data = new List<object>();
            foreach(DatabaseType dbType in Enum.GetValues(typeof(DatabaseType))) {
                data.Add(new { Id = (int)dbType, Name = dbType.ToString() });
            }
            return data;
        }

        public static string ToDateString(DateTime current) {
            if(current == default(DateTime)) return string.Empty;

            return current.ToString("yyyy-MM-dd");
        }

        public static string ToTimeString(DateTime current) {
            if(current == default(DateTime)) return string.Empty;

            return current.ToString("HH:mm:ss");
        }

        public static string ToDateTimeString(DateTime current) {
            if(current == default(DateTime)) return string.Empty;

            return current.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string[] SplitKeys(string key) {
            if(string.IsNullOrWhiteSpace(key))
                return new string[] { };

            return key.Split(new string[] { GlobalSeparator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string JoinKeys(params string[] keys) {
            if(keys == null || keys.Length == 0)
                return string.Empty;

            return string.Join(GlobalSeparator, keys);
        }

        public static string[] SplitCondition(string conditions) {
            if (string.IsNullOrWhiteSpace(conditions))
                return new string[0];

            return conditions.Split(new char[] { ';', '；' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool ConditionStartWith(IEnumerable<string> values, string key) {
            if (values == null || string.IsNullOrWhiteSpace(key))
                return false;

            foreach (var value in values) {
                if (key.StartsWith(value))
                    return true;
            }

            return false;
        }

        public static bool ValidateFormula(string formula) {
            if (string.IsNullOrWhiteSpace(formula)) return false;
            formula = Regex.Replace(formula, @"\s+", "");

            if (Regex.IsMatch(formula, @"[\+\-\*\/]{2,}")) return false;
            if (Regex.IsMatch(formula, @"[\+\-\*\/]{2,}")) return false;

            var stack = new Stack<char>();
            foreach (var letter in formula) {
                if (letter == '(') {
                    stack.Push('(');
                } else if (letter == ')') {
                    if (stack.Count == 0) return false;
                    stack.Pop();
                }
            }

            if (stack.Count != 0) return false;
            if (Regex.IsMatch(formula, @"\([\+\-\*\/]")) return false;
            if (Regex.IsMatch(formula, @"[\+\-\*\/]\)")) return false;
            if (Regex.IsMatch(formula, @"[^\+\-\*\/\(]\(")) return false;
            if (Regex.IsMatch(formula, @"\)[^\+\-\*\/\)]")) return false;

            formula = Regex.Replace(formula, @"\(|\)", "");
            formula = Regex.Replace(formula, @"[\+\-\*\/]", GlobalSeparator);
            var variables = SplitKeys(formula);
            foreach (var variable in variables) {
                if (Regex.IsMatch(variable, @"^\d+(\.\d+)?$")) continue;
                if (!Regex.IsMatch(variable, @"^@.+>>.+$")) return false;
                var starts = Regex.Matches(variable, @"@");
                if (starts.Count != 1) return false;
                var separators = Regex.Matches(variable, @">>");
                if (separators.Count != 1) return false;
            }

            return true;
        }

        public static List<string> GetFormulaVariables(string formula) {
            if(string.IsNullOrWhiteSpace(formula)) return null;
            formula = Regex.Replace(formula, @"\s+", "");
            formula = Regex.Replace(formula, @"\(|\)", "");
            formula = Regex.Replace(formula, @"[\+\-\*\/]", GlobalSeparator);
            var variables = SplitKeys(formula);
            var result = new List<string>();
            foreach(var variable in variables){
                if (Regex.IsMatch(variable, @"^\d+(\.\d+)?$")) continue;
                if(result.Contains(variable)) continue;
                result.Add(variable);
            }

            return result;
        }

        public static List<DateTime> GetDateSpan(DateTime start, DateTime end) {
            start = start.Date; end = end.Date;
            var dates = new List<DateTime>();
            while(start <= end) {
                dates.Add(start);
                start = start.AddDays(1);
            }
            return dates;
        }

        public static List<DateTime> GetHourSpan(DateTime start, DateTime end) {
            var dates = new List<DateTime>();
            while (start <= end) {
                dates.Add(start);
                start = start.AddHours(1);
            }
            return dates;
        }

        public static int GetUnitState(string unitState, string name, int defaultValue = 2) {
            var units = unitState.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var unit in units) {
                var flag = unit.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                if(flag.Length != 2) continue;
                if(flag[1].Contains(name)) 
                    return int.Parse(flag[0]);
            }

            return defaultValue;
        }

        public static void SetAutoRun(string file, string key, bool isAutoRun) {
            RegistryKey reg = null;
            try {
                if(!System.IO.File.Exists(file)) throw new Exception("未找到应用程序");

                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if(reg == null) reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");

                if(isAutoRun)
                    reg.SetValue(key, file);
                else
                    reg.DeleteValue(key, false);
            } finally {
                if(reg != null)
                    reg.Close();
            }
        }

        public static bool IsAutoRun(string key) {
            RegistryKey reg = null;
            try {
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
                if(reg == null) return false;

                var valueNames = reg.GetValueNames();
                return valueNames.Contains(key);
            } finally {
                if(reg != null)
                    reg.Close();
            }
        }

        public static void ResetIIS() {
            var psi = new ProcessStartInfo();
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = "iisreset.exe";
            Process.Start(psi);
        }

        public static string GetMD5(string key) {
            var buffers = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(key));

            var md5 = new StringBuilder();
            for (int i = 0; i < buffers.Length; i++) {
                md5.AppendFormat("{0:x2}", buffers[i]);
            }

            return md5.ToString();
        }

        public static string GetAlarmSerialNo(long key) {
            if (key > int.MaxValue) key = key % int.MaxValue;
            return key.ToString().PadLeft(10, '0');
        }

        public static XmlDocument GetXmlDocument(string filePath, string fileName) {
            filePath = String.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, filePath);
            fileName = String.Format(@"{0}\{1}", filePath, fileName);

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            var xmlDoc = new XmlDocument();
            if (!File.Exists(fileName)) {
                var decl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(decl);

                var xmlRoot = xmlDoc.CreateElement("root");
                xmlDoc.AppendChild(xmlRoot);
            } else {
                xmlDoc.Load(fileName);
            }

            return xmlDoc;
        }

        public static void SaveXmlDocument(string filePath, string fileName, XmlDocument xmlDoc) {
            filePath = String.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, filePath);
            fileName = String.Format(@"{0}\{1}", filePath, fileName);

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            xmlDoc.Save(fileName);
        }

        public static String GetMacId() {
            var cpu = GetCpuId();
            var hdd = GetHddId();
            var key = String.Format("{0}{1}{2}{3}{4}{5}{6}", DefaultSalts[0], hdd, DefaultSalts[3], cpu, DefaultSalts[2], hdd, DefaultSalts[1]);
            var bs = Encoding.UTF8.GetBytes(key);
            var hs = MD5.Create().ComputeHash(bs);
            return new Guid(hs).ToString("N").ToUpper();
        }

        public static String GetCpuId() {
            try {
                var cpus = new SortedSet<string>();
                using (var mc = new ManagementClass("Win32_Processor")) {
                    var moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc) {
                        cpus.Add(mo.Properties["ProcessorId"].Value.ToString());
                    }
                }

                if (cpus.Count > 0) return string.Join(".", cpus);
            } catch {
            }

            return "unknow";
        }

        public static String GetHddId() {
            try {
                var disks = new SortedList<int, string>();
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive")) {
                    foreach (ManagementObject mo in searcher.Get()) {
                        disks.Add(Convert.ToInt32(mo.Properties["Index"].Value), string.Format("{0:X}", mo.Properties["Signature"].Value));
                    }
                }

                if (disks.Count > 0) return string.Join(".", disks.Values);
            } catch {
            }

            return "unknow";
        }

        public static String DoubleToString(double value) {
            if (value == double.MinValue) return "NULL";
            return value.ToString();
        }

        public static String Int32ToString(int value) {
            if (value == int.MinValue) return "NULL";
            return value.ToString();
        }

        public static String StringToString(string value) {
            if (string.IsNullOrWhiteSpace(value)) return "NULL";
            return value;
        }
    }
}
