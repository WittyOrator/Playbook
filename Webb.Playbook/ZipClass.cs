using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;

namespace Webb.Playbook
{
    public class ZipClass
    {
        /// <summary> 
        /// 压缩文件 
        /// </summary> 
        /// <param name="sourceFilePath"></param> 
        /// <param name="destinationZipFilePath"></param> 
        public static void CreateZip(string sourceFilePath, string destinationZipFilePath)
        {
            if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                sourceFilePath += System.IO.Path.DirectorySeparatorChar;
            ZipOutputStream zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
            zipStream.SetLevel(6); // 压缩级别 0-9 
            CreateZipFiles(sourceFilePath, zipStream, sourceFilePath);
            zipStream.Finish();
            zipStream.Close();
        }

        /// <summary> 
        /// 递归压缩文件 
        /// </summary> 
        /// <param name="sourceFilePath">待压缩的文件或文件夹路径</param> 
        /// <param name="zipStream">打包结果的zip文件路径（类似 D:\WorkSpace\a.zip）,全路径包括文件名和.zip扩展名</param> 
        /// <param name="staticFile"></param> 
        private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream, string staticFile)
        {
            Crc32 crc = new Crc32();
            string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
            foreach (string file in filesArray)
            {
                if (Directory.Exists(file)) //如果当前是文件夹，递归 
                {
                    CreateZipFiles(file, zipStream, staticFile);
                }
                else //如果是文件，开始压缩
                {
                    FileStream fileStream = File.OpenRead(file);
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    string tempFile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempFile);
                    entry.DateTime = DateTime.Now;
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);
                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        ///   <summary> 
        ///   解压缩一个   zip   文件。 
        ///   </summary> 
        ///   <param   name= "zipFileName "> 要解压的   zip   文件。 </param> 
        ///   <param   name= "extractLocation "> zip   文件的解压目录。 </param> 
        ///   <param   name= "password "> zip   文件的密码。 </param> 
        ///   <param   name= "overWrite "> 是否覆盖已存在的文件。 </param> 
        public static void UnZip(string zipFileName, string extractLocation, bool overWrite)
        {
            #region   实现
            string myExtractLocation = extractLocation;
            if (myExtractLocation == string.Empty)
                myExtractLocation = Directory.GetCurrentDirectory();
            if (!myExtractLocation.EndsWith("\\"))
                myExtractLocation = myExtractLocation + "\\";
            ZipInputStream s = new ZipInputStream(File.OpenRead(zipFileName));
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = string.Empty;
                string pathToZip = string.Empty;
                pathToZip = theEntry.Name;

                if (pathToZip != string.Empty)
                    directoryName = Path.GetDirectoryName(pathToZip) + "\\";
                string fileName = Path.GetFileName(pathToZip);
                Directory.CreateDirectory(myExtractLocation + directoryName);
                if (fileName != string.Empty)
                {
                    if ((File.Exists(myExtractLocation + directoryName + fileName) && overWrite) ||
                    (!File.Exists(myExtractLocation + directoryName + fileName)))
                    {
                        FileStream streamWriter = File.Create(myExtractLocation + directoryName + fileName);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                                streamWriter.Write(data, 0, size);
                            else
                                break;
                        }
                        streamWriter.Close();
                    }
                    else
                    { 
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0) { }
                            else
                                break;
                        }
                    }
                }
            }
            s.Close();
            #endregion
        }
    }
}
