using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IPCamera.Settings.Record
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Records
    {
        /// <summary>
        /// Тип записи
        /// </summary>
        public RecEnamble Enamble;
        /// <summary>
        /// Автозапуск записи
        /// </summary>
        public AutoEnabmle AutoLoad;
        /// <summary>
        /// Тип потока записи
        /// </summary>
        public StreamType StreamType;
        /// <summary>
        /// Путь для записи
        /// </summary>
        public string RecordFolder;
        /// <summary>
        /// Длинна записи
        /// </summary>
        public int RecoedDuration;
        /// <summary>
        /// Дней хранения записи
        /// </summary>
        public int RecordsCount;
        /// <summary>
        /// Время начала записи
        /// </summary>
        public DateTime StartRecord;
        /// <summary>
        /// Время окончания записи
        /// </summary>
        public DateTime StopRecord;

        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public static Records Default = new Records(RecEnamble.OFF, AutoEnabmle.OFF, StreamType.Primary, "", 300, 10, DateTime.Now, DateTime.Now);

        public Records(RecEnamble enamble, AutoEnabmle autoLoad, StreamType streamType, string recordFolder, int recoedDuration, int recordsCount, DateTime startRecord, DateTime stopRecord)
        {
            Enamble = enamble;
            AutoLoad = autoLoad;
            StreamType = streamType;
            RecordFolder = recordFolder;
            RecoedDuration = recoedDuration;
            RecordsCount = recordsCount;
            StartRecord = startRecord;
            StopRecord = stopRecord;
        }
    }

    /// <summary>
    /// Тип записи
    /// </summary>
    public enum RecEnamble : byte
    {
        /// <summary>
        /// Всегда включена
        /// </summary>
        ON,
        /// <summary>
        /// Всегда выключена
        /// </summary>
        OFF,
        /// <summary>
        /// Автоматически
        /// </summary>
        Auto,
    }
    /// <summary>
    /// Автозапуск записи
    /// </summary>
    public enum AutoEnabmle : byte
    {
        /// <summary>
        /// Включен
        /// </summary>
        ON,
        /// <summary>
        /// Выключен
        /// </summary>
        OFF,
    }
    /// <summary>
    /// Тип потока записи
    /// </summary>
    public enum StreamType : byte
    {
        /// <summary>
        /// Первичный (Большой)
        /// </summary>
        Primary,
        /// <summary>
        /// Вторичный (Маленький)
        /// </summary>
        Secand,
    }
}
