using IPCamera.ONVIF.PTZ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IPCamera.PTZScenes
{
    /// <summary>
    /// PTZ сценарий
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PTZScene
    {
        public readonly static PTZScene Null = new PTZScene("Default", new PTZTile[] { PTZTile.Null }, 100, TriggerPTZ.None, TriggerData.Null, TriggerData.Null);

        /// <summary>
        /// Название
        /// </summary>
        public string Name;
        /// <summary>
        /// Массив тайлов
        /// </summary>
        public PTZTile[] Tiles;
        /// <summary>
        /// Задержка перед выполнением
        /// </summary>
        public int Timeout;
        /// <summary>
        /// Триггер выполнения
        /// </summary>
        public TriggerPTZ Trigger;
        /// <summary>
        /// Текущие триггерные параметры
        /// </summary>
        private TriggerData CurrentTriggerData;
        /// <summary>
        /// Стандартные триггерные параметры
        /// </summary>
        public TriggerData DefaultTriggerData;

        public PTZScene(string name, PTZTile[] tiles, int timeout, TriggerPTZ trig, TriggerData currentTriggerData, TriggerData defaultTriggerData)
        {
            Name = name;
            Tiles = tiles;
            Timeout = timeout;
            Trigger = trig;
            CurrentTriggerData = currentTriggerData;
            DefaultTriggerData = defaultTriggerData;
        }
    }
    /// <summary>
    /// Тайл на движение
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PTZTile
    {
        /// <summary>
        /// Направление движения
        /// </summary>
        public ONVIF.PTZ.PTZParameters.Vector Vector;
        /// <summary>
        /// Количество шагов
        /// </summary>
        public int Steep;
        /// <summary>
        /// Задержка после выполнения комманды
        /// </summary>
        public int SleepTime;

        public readonly static PTZTile Null = new PTZTile(PTZParameters.Vector.RIGHT, 10, 500);

        public PTZTile(PTZParameters.Vector vector, int steep, int sleepTime)
        {
            Vector = vector;
            Steep = steep;
            SleepTime = sleepTime;
        }
    }

    /// <summary>
    /// Триггеры выполнения сценария PTZ
    /// </summary>
    public enum TriggerPTZ : int
    {
        /// <summary>
        /// Не использовать триггеры
        /// </summary>
        None = 0,
        /// <summary>
        /// Триггер по таймеру
        /// </summary>
        Timer = 1,
        /// <summary>
        /// Триггер по дате
        /// </summary>
        Date = 2,
    }

    /// <summary>
    /// Коллекция параметров триггеров
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TriggerData
    {
        /// <summary>
        /// Таймер на выполение
        /// </summary>
        public DateTime Timer_Timeout;
        /// <summary>
        /// Конкретная дата (по дням неделям)
        /// </summary>
        public DateTime Date_Period;
        /// <summary>
        /// ID камеры для триггера
        /// </summary>
        public uint CameraTrigger;

        public readonly static TriggerData Null = new TriggerData(DateTime.Now, DateTime.Now, 0);

        public TriggerData(DateTime timer_Timeout, DateTime date_Period, uint ct)
        {
            Timer_Timeout = timer_Timeout;
            Date_Period = date_Period;
            CameraTrigger = ct;
        }
    }
}
