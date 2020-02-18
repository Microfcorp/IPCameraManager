using IPCamera.Settings;
using IPCamera.OPTZ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace IPCamera.ONVIF.PTZ
{
    /// <summary>
    /// Абстрактный класс с параметрами PTZ
    /// </summary>
    public abstract class PTZParameters
    {
        /// <summary>
        /// Вектор движения
        /// </summary>
        public enum Vector
        {
            /// <summary>
            /// Вверх
            /// </summary>
            UP,
            /// <summary>
            /// Вних
            /// </summary>
            DOWN,
            /// <summary>
            /// Влево
            /// </summary>
            LEFT,
            /// <summary>
            /// Вправо
            /// </summary>
            RIGHT,
            /// <summary>
            /// Остановка
            /// </summary>
            STOP,
        }
    }
    /// <summary>
    /// Контроллер движения для камеры
    /// </summary>
    public class PTZController
    {
        private readonly Structures set;
        private bool issup = false;
        /// <summary>
        /// Создание контроллера движений для камеры по ее структуре
        /// </summary>
        /// <param name="camera">Структура настроек для камеры</param>      
        public PTZController(Structures camera)
        {
            set = camera;
            //th.Start(this);
            issup = _IsSuported;
        }
        /// <summary>
        /// Скорость поворота камеры
        /// </summary>
        public int Speed
        {
            get;
            set;
        }
        /// <summary>
        /// Возвращает векторную скорость для движения
        /// </summary>
        public PTZSpeed PTZSpeed
        {
            get
            {
                var p = new PTZSpeed
                {
                    PanTilt = new Vector2D
                    {
                        x = Speed,
                        y = Speed
                    }
                };
                return p;
            }
        }
        /// <summary>
        /// Возвращает вектор движения и количество шагов
        /// </summary>
        /// <param name="vector">Вектор направления</param>
        /// <param name="steep">Количество шагов</param>
        /// <returns></returns>
        public PTZVector GetVector(PTZParameters.Vector vector, float steep = 5f)
        {
            var p = new PTZVector
            {
                PanTilt = new Vector2D()
            };
            if (vector == PTZParameters.Vector.UP) p.PanTilt.x = steep;
            else if (vector == PTZParameters.Vector.DOWN) p.PanTilt.x = -steep;
            else if (vector == PTZParameters.Vector.LEFT) p.PanTilt.x = -steep;
            else if (vector == PTZParameters.Vector.RIGHT) p.PanTilt.x = steep;
            return p;
        }

        /// <summary>
        /// Производит перемещение по указаному вектору с заданым колисеством шагов
        /// </summary>
        /// <param name="vector">Направление движения</param>
        /// <param name="steep">Количество шагов</param>
        public string Move(PTZParameters.Vector vector, float steep = 5f)
        {
            try
            {
                ONVIF.SendResponce.RotatePTZ(set, GetVector(vector, steep), PTZSpeed);
                return "OK";
            }
            catch { return "Fail"; }
        }

        /// <summary>
        /// Текущая позиция камеры
        /// </summary>
        public PTZVector Position
        {
            get
            {
                return SendResponce.PTZPosition(set);
            }
        }

        /// <summary>
        /// Возвращает камеру в домашнее положение
        /// </summary>
        public void MoveToHome()
        {
            if (!issup) return;
            ONVIF.SendResponce.PTZHome(set, PTZSpeed);
        }

        /// <summary>
        /// Устанавливает текущую позицию, как домашнюю
        /// </summary>
        public void SetHome()
        {
            if (!issup) return;
            SendResponce.PTZSetHome(set);
        }
        /// <summary>
        /// Статус системы
        /// </summary>
        public string Status
        {
            get
            {
                if (!issup) return "No Supported";
                return ONVIF.SendResponce.PTZStatus(set);
            }
        }

        readonly Thread th = new Thread(new ParameterizedThreadStart(_IS));

        static object locker = new object();
        private static void _IS(object p)
        {
            var a = p as PTZController;

            lock(locker) a.issup = a.IsSuported;
        }
        /// <summary>
        /// Определяет поддержку PTZ
        /// </summary>
        public bool _IsSuported
        {
            get
            {
                try
                {
                    if (!SendResponce.PTZSupport(set)) return false;

                    var pred = Position;
                    if (pred.PanTilt == null) return false;

                    Move(PTZParameters.Vector.LEFT, 1f);
                    var pred2 = Position;
                    Move(PTZParameters.Vector.RIGHT, 1f);
                    return (pred.PanTilt != null && pred2.PanTilt != null) && (pred.PanTilt.x != pred2.PanTilt.x);                    
                }
                catch
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Определяет поддержку PTZ
        /// </summary>
        public bool IsSuported
        {
            get => issup;
        }
    }
}
