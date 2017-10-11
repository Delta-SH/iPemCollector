using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.Win32;
using iPem.Core.Rs;

namespace iPem.Core {
    public partial class CommonHelper {
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

        public static bool ValidateFormula(string formula) {
            if(string.IsNullOrWhiteSpace(formula)) return false;
            formula = Regex.Replace(formula, @"\s+", "");

            if(Regex.IsMatch(formula, @"[\+\-\*\/]{2,}")) return false;
            if(Regex.IsMatch(formula, @"[\+\-\*\/]{2,}")) return false;

            var stack = new Stack<char>();
            foreach(var letter in formula) {
                if(letter == '(') {
                    stack.Push('(');
                } else if(letter == ')') {
                    if(stack.Count == 0) return false;
                    stack.Pop();
                }
            }

            if(stack.Count != 0) return false;
            if(Regex.IsMatch(formula, @"\([\+\-\*\/]")) return false;
            if(Regex.IsMatch(formula, @"[\+\-\*\/]\)")) return false;
            if(Regex.IsMatch(formula, @"[^\+\-\*\/\(]\(")) return false;
            if(Regex.IsMatch(formula, @"\)[^\+\-\*\/\)]")) return false;

            formula = Regex.Replace(formula, @"\(|\)", "");
            formula = Regex.Replace(formula, @"[\+\-\*\/]", GlobalSeparator);
            var variables = SplitKeys(formula);
            foreach(var variable in variables) {
                if(Regex.IsMatch(formula, @"^\d+(\.\d+)?$")) continue;
                if(!Regex.IsMatch(formula, @"^@.+>>.+$")) return false;
                var starts = Regex.Matches(variable, @"@");
                if(starts.Count > 1) return false;
                var separators = Regex.Matches(variable, @">>");
                if(separators.Count > 1) return false;
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
                if(Regex.IsMatch(formula, @"^\d+(\.\d+)?$")) continue;
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

        public static string GetIdAsString() {
            return GetIdAsLong().ToString();
        }

        public static long GetIdAsLong() {
            return Math.Abs(DateTime.Now.Subtract(new DateTime(2017, 6, 21)).Ticks);
        }
    }
}
