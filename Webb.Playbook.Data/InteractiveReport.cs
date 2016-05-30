using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace Webb.Playbook.Data
{
    public class InteractiveReport
    {
        public InteractiveReport()
        {

        }

        public InteractiveReport(List<String> arrFiles)
        {
            Files.Clear();

            Files.AddRange(arrFiles);
        }

        private List<String> files;
        public List<String> Files
        {
            get
            {
                if (files == null)
                {
                    files = new List<string>();
                }

                return files;
            }
        }

        public string GetReportPath()
        {
            string strReportPath = string.Empty;

            RegistryKey reportKey = Registry.CurrentUser.OpenSubKey(@"Software\Webb Electronics\WebbReport\WebbReportViewer");

            if (reportKey != null)
            {
                if (reportKey.GetValueNames().Contains("FilePath"))
                {
                    strReportPath = reportKey.GetValue("FilePath").ToString();

                    if (!File.Exists(strReportPath))
                    {
                        strReportPath = string.Empty;
                    }
                }
            }

            return strReportPath;
        }

        public string CreateInw(string strReportTemplate, string strReportHeader)
        {
            //实例化一个文件流--->与写入文件相关联
            string strInw = AppDomain.CurrentDomain.BaseDirectory + "InteractiveReport.inw";

            if (File.Exists(strInw))
            {
                File.Delete(strInw);
            }

            FileStream fs = new FileStream(strInw, FileMode.Create);

            //实例化一个StreamWriter-->与fs相关联
            StreamWriter sw = new StreamWriter(fs);

            //开始写入
            sw.WriteLine(@"Path:" + strReportTemplate);
            sw.WriteLine(@"Action:[DIAGRAM:Offense],[WATERMARK:],[CLICKEVENT:0]");
            sw.WriteLine(@"Product:WebbPlaybook");
            sw.WriteLine(@"DBConn:");
            sw.WriteLine(@"SQLCmd:");
            sw.WriteLine(@"GameIDs:");
            sw.WriteLine(@"EdlIDs:");
            sw.WriteLine(@"FilterIDs:[Filters:,]");

            StringBuilder sb = new StringBuilder();
            foreach(string strFile in Files)
            {
                sb.AppendFormat(",{0}", strFile);
            }

            string strRH = strReportHeader == string.Empty ? "none" : strReportHeader;
            string strCmd = @"Files:[USER:?HEADER:" + strRH + @"]" + sb.ToString();
            sw.WriteLine(strCmd);
            sw.WriteLine(@"Print:0");

            //清空缓冲区
            sw.Flush();

            //关闭流
            sw.Close();
            fs.Close();

            return strInw;
        }
    }
}
