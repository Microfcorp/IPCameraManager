using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Ionic.Zip;
using System.Windows.Forms;

namespace IPCamera.UI.Maps
{
    /// <summary>
    /// Работа с файлом карты
    /// </summary>
    public class MapsFile
    {
        ZipFile zip;
        /// <summary>
        /// Путь к файлу карты
        /// </summary>
        public string FilePath
        {
            get;
            set;
        }

        public const string MapsDirPrefix = "\\MapsFile\\";

        public static string[] GetMapsFile()
        {
            return Directory.GetFiles(Application.StartupPath + MapsDirPrefix, "*.micmaps");
        }

        public static string[] GetMapsFileName()
        {
            return Directory.GetFiles(Application.StartupPath + MapsDirPrefix, "*.micmaps").Select(t => Path.GetFileName(t)).ToArray();
        }

        public static void NewMapsFile(string ip = "noneip")
        {
            File.Copy(Application.StartupPath + "/defaultmaps.micmaps", Application.StartupPath + MapsDirPrefix + ip + "_" + DateTime.Now.ToString().Replace(":", ".") + "_maps.micmaps");
        }

        public static void DeleteMapsFile(string mapsfile)
        {
            File.Delete(mapsfile);
        }

        public MapsFile(string filePath)
        {
            FilePath = filePath;
            Load();
        }
        public MapsFile()
        {
            Manifest = MapsManifest.Default;
        }

        public void Save()
        {           
            File.WriteAllText("manifest.xml", Manifest.ToXml);
            zip.UpdateFile("manifest.xml");
            System.Threading.Thread.Sleep(300);
            zip.Save();
            File.Delete("manifest.xml");
        }

        public void Load()
        {
            if (File.Exists(FilePath))
            {
                zip = new ZipFile(FilePath);                
                Manifest = new MapsManifest(new StreamReader(zip["manifest.xml"].OpenReader()).ReadToEnd());
            }
            else
            {
                Manifest = MapsManifest.Default;
            }
        }

        /// <summary>
        /// Информация о карте
        /// </summary>
        public MapsManifest Manifest
        {
            get;
            private set;
        }

        /// <summary>
        /// Загружает изображение
        /// </summary>
        /// <param name="path">Смещение в карте</param>
        /// <returns></returns>
        public Image LoadImage(string path)
        {
            if(zip != null)
                return Image.FromStream(zip[path].OpenReader());
            return null;
        }
        public string[] GetAllImages()
        {
            return zip.Where(t => t.FileName.Split('.').LastOrDefault() == "png" || t.FileName.Split('.').LastOrDefault() == "jpg" || t.FileName.Split('.').LastOrDefault() == "bmp").Select(t => t.FileName).ToArray();
        }
        public void UploadFile(string[] files)
        {
            zip.UpdateFiles(files, "");
            System.Threading.Thread.Sleep(300);
            zip.Save();
        }
        public void UploadFile(string files)
        {
            zip.UpdateFile(files, "");
            System.Threading.Thread.Sleep(300);
            zip.Save();
        }
        /// <summary>
        /// Загружает изображение
        /// </summary>
        /// <returns></returns>
        public Image LoadMainImage()
        {
            return LoadImage(Manifest.MainImage);
        }
    }
}
