using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPCamera.ODEV;

namespace IPCamera.ONVIF
{
    public class CameraDateTime : ODEV.DateTime
    {
        /// <summary>
        /// Часовой пояс
        /// </summary>
        public string TZ;
        /// <summary>
        /// Маска для преобразования в строковое представление
        /// </summary>
        public const string Mask = "{0}-{1}-{2} {3}:{4}:{5}";

        /// <summary>
        /// Возникает при смене значения свойств
        /// </summary>
        public new event System.ComponentModel.ProgressChangedEventHandler PropertyChanged;

        public CameraDateTime(ODEV.DateTime dateTime, string tz)
        {
            Time = dateTime.Time;
            Date = dateTime.Date;
            TZ = tz;
            dateTime.PropertyChanged += DateTime_PropertyChanged; ;
        }

        private void DateTime_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropertyChanged += PropertyChanged;
        }

        /// <summary>
        /// Возвращает строковое представление данной даты и данного времени
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(Mask, Date.Day, Date.Month, Date.Year, Time.Hour, Time.Minute, Time.Second);
        }

        /// <summary>
        /// Преобразует DateTime в строковое представление
        /// </summary>
        public string Parse
        {
            get => DateTime.ToString();
            
        }

        /// <summary>
        /// Преобразовывает данное представление в DateTime
        /// </summary>
        public System.DateTime DateTime
        {
            get
            {
                return System.DateTime.Parse(ToString()).ToLocalTime();
            }
        }
    }
}
