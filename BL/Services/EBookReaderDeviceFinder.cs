using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using myCollections.Data;
using myCollections.Data.SqlLite;

namespace myCollections.BL.Services
{
    class EBookReaderServices
    {
        private const string Kindle = "Kindle Internal Storage USB Device";
        private const string KindleGenApp = @"plugins\kindlegen.exe ";
        private const string KindleFolder = @"documents\myCollections";

        public static bool CopyToReader(EBookReaderDevices reader, string id)
        {

            Books book = new BookServices().Get(id) as Books;

            if (book != null)
            {
                string filePath = Path.Combine(book.FilePath, book.FileName);
                if (reader != null && string.IsNullOrWhiteSpace(filePath) == false)
                {
                    if (File.Exists(KindleGenApp))
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo();
                        processStartInfo.FileName = KindleGenApp;
                        processStartInfo.Arguments = string.Format("\"{0}\"", filePath);
                        processStartInfo.RedirectStandardOutput = true;
                        processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        processStartInfo.CreateNoWindow = true;
                        processStartInfo.UseShellExecute = false;
                        Process convertProcess = Process.Start(processStartInfo);
                        convertProcess.WaitForExit(10000);

                        string output = Path.ChangeExtension(filePath, ".mobi");

                        if (File.Exists(output))
                        {
                            FileInfo file=new FileInfo(output);
                            if (Directory.Exists(Path.Combine(reader.LogicalDisk + @"\", KindleFolder)) == false)
                                Directory.CreateDirectory(Path.Combine(reader.LogicalDisk + @"\", KindleFolder));

                            File.Copy(output, Path.Combine(reader.LogicalDisk + @"\", KindleFolder,file.Name));

                            return true;
                        }
                    }
                }
            }

            return false;
        }


        public static EBookReaderDevices GetReader()
        {
            IList<EBookReaderDevices> devices = GetUsbDevices();
            if (devices.Count > 0)
                return devices[0];
            else
                return null;
        }
        private static IList<EBookReaderDevices> GetUsbDevices()
        {
            List<EBookReaderDevices> devices = new List<EBookReaderDevices>();

            ManagementObjectCollection collection;
            SelectQuery selectQuery = new SelectQuery("Win32_DiskDrive");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery))
                collection = searcher.Get();

            foreach (ManagementObject device in collection)
            {
                EBookReaderDevices reader = new EBookReaderDevices();
                reader.DeviceID = device.GetPropertyValue("DeviceID").ToString();
                reader.PNPDeviceID = device.GetPropertyValue("PNPDeviceID").ToString();
                reader.Description = device.GetPropertyValue("Description").ToString();
                reader.Manufacturer = device.GetPropertyValue("Manufacturer").ToString();
                reader.Model = device.GetPropertyValue("Model").ToString();

                foreach (ManagementObject partition in device.GetRelated("Win32_DiskPartition"))
                {
                    reader.Name = partition.GetPropertyValue("Name").ToString();

                    foreach (ManagementObject logical in partition.GetRelated("Win32_LogicalDisk"))
                    {
                        reader.LogicalDisk = logical.GetPropertyValue("Name").ToString();
                    }
                }
                if (reader.Model == Kindle)
                    devices.Add(reader);
            }

            collection.Dispose();
            return devices;
        }

    }
}
