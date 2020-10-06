using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.ONVIF
{
    public class ONVIFVersion : ODEV.OnvifVersion
    {
        public ONVIFVersion(ODEV.OnvifVersion onvifVersion) : base()
        {
            this.Minor = onvifVersion.Minor;
            this.Major = onvifVersion.Major;
        }

        /// <summary>
        /// Возвращает строковое представление версии
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Major + "." + this.Minor;
        }
    }
}
