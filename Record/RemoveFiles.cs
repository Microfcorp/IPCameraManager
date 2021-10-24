using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IPCamera.Settings.Record;
using IPCamera.Settings;
using IPCamera.DLL;

namespace IPCamera.Record
{
    public class RemoveFiles
    {
        /// <summary>
        /// Удалить старые файлы
        /// </summary>
        /// <param name="CameraID">Номер камеры</param>
        public static void RemoveOLDFiles(int CameraID)
        {
            var _dir = Structures.Load()[CameraID].Records.RecordFolder;
            var dir = Directory.GetFiles(_dir);
            foreach (string file in dir)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.LastWriteTime < DateTime.Now.AddDays(-Structures.Load()[CameraID].Records.RecordsCount))
                {
                    file.DeleteFile();
                }
            }
        }
    }
}
