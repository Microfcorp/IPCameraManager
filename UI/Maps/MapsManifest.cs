using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace IPCamera.UI.Maps
{
    public enum MapsOType : byte
    {
        Camera,
        Delete,
        None,
    }
    /// <summary>
    /// Манифест карты
    /// </summary>
    public class MapsManifest
    {
        /// <summary>
        /// Создание манифеста карты из xml строки
        /// </summary>
        /// <param name="text"></param>
        public MapsManifest(string text)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(text);
            Name = xDoc["Manifest"]["Name"].InnerText;
            Descreption = xDoc["Manifest"]["Descreption"].InnerText;
            MainImage = xDoc["Manifest"]["BackgroundImage"].InnerText;
            MainText = xDoc["Manifest"]["MainText"].InnerText;
            List<MapsObject> mp = new List<MapsObject>();
            foreach (var item in xDoc["Manifest"]["Objects"])
            {
                var t = item as XmlElement;
                MapsObject m = new MapsObject(t.GetAttribute("name"), XMLElementToPoint(t), (MapsOType)int.Parse(t["Type"].InnerText), int.Parse(t["OID"].InnerText), t["Icon"].InnerText);
                mp.Add(m);
            }
            Objects = mp.ToArray();
        }

        private MapsManifest()
        {

        }
        /// <summary>
        /// Преобразует объект в xml объект
        /// </summary>
        public string ToXml
        {
            get
            {
                var t = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>"+Environment.NewLine;
                t += "<Manifest>" + Environment.NewLine;
                t += "<Name>"+Name+"</Name>" + Environment.NewLine;
                t += "<Descreption>"+Descreption+"</Descreption>" + Environment.NewLine;
                t += "<BackgroundImage>"+MainImage+"</BackgroundImage>" + Environment.NewLine;
                t += "<MainText>"+MainText+ "</MainText>" + Environment.NewLine;
                t += "<Objects>" + Environment.NewLine;
                foreach (var item in Objects)
                    t += item.ToXml + Environment.NewLine;
                t += "</Objects>" + Environment.NewLine;
                t += "</Manifest>" + Environment.NewLine;
                return t;
            }
        }

        /// <summary>
        /// Преобразует xml елемент точки к стандартной точки
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Point XMLElementToPoint(XmlElement element)
        {
            return new Point(int.Parse(element["Point"].GetAttribute("x")), int.Parse(element["Point"].GetAttribute("y")));
        }

        /// <summary>
        /// Стандартный манифест
        /// </summary>
        public static MapsManifest Default
        {
            get
            {
                var t = new MapsManifest()
                {
                    Descreption = "Пустая карта",
                    MainImage = "",
                    MainText = "Новая карта",
                    Name = "Новая карта",
                    Objects = new MapsObject[0],
                };
                return t;
            }
        }

        /// <summary>
        /// Имя карты
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Описание карты
        /// </summary>
        public string Descreption
        {
            get;
            set;
        }
        /// <summary>
        /// Текст на основной панели
        /// </summary>
        public string MainText
        {
            get;
            set;
        }
        /// <summary>
        /// Относительный путь до основного изображения
        /// </summary>
        public string MainImage
        {
            get;
            set;
        }
        /// <summary>
        /// Объекты на карте
        /// </summary>
        public MapsObject[] Objects
        {
            get;
            set;
        }
    }
    /// <summary>
    /// Объект карты
    /// </summary>
    public class MapsObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="point"></param>
        /// <param name="type"></param>
        /// <param name="oID"></param>
        /// <param name="image"></param>
        public MapsObject(string name, Point point, MapsOType type, int oID, string image)
        {
            Name = name;
            Point = point;
            Type = type;
            OID = oID;
            Image = image;
        }
        /// <summary>
        /// Преобразует объект в xml объект
        /// </summary>
        public string ToXml
        {
            get
            {
                var t = "<Object name=\""+Name+"\">" + Environment.NewLine;
                t += "<Point x=\""+Point.X+"\" y=\""+Point.Y+"\" />" + Environment.NewLine;
                t += "<Type>"+(int)Type+"</Type>" + Environment.NewLine;
                t += "<OID>"+OID+"</OID>" + Environment.NewLine;
                t += "<Icon>"+Image+"</Icon>" + Environment.NewLine;
                t += "</Object>";
                return t;
            }
        }
        /// <summary>
        /// Имя объект
        /// </summary>

        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Точка на карте
        /// </summary>
        public Point Point
        {
            get;
            set;
        }
        /// <summary>
        /// Тип объекта
        /// </summary>
        public MapsOType Type
        {
            get;
            set;
        }
        /// <summary>
        /// Операуионный ID
        /// </summary>
        public int OID
        {
            get;
            set;
        }
        /// <summary>
        /// Относительный путь до изображения
        /// </summary>
        public string Image
        {
            get;
            set;
        }
    }
}
