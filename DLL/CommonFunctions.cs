using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IPCamera.DLL
{
    public static class CommonFunctions
    {
        /// <summary>
        /// Переместить элемент и изменить его размер
        /// </summary>
        /// <param name="hWnd">Элемент</param>
        /// <param name="X">Позиция по X</param>
        /// <param name="Y">Позиция по Y</param>
        /// <param name="nWidth">Размер в длинну</param>
        /// <param name="nHeight">Размер в ширину</param>
        /// <param name="bRepaint">Перерисовать элемент</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(this IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// Задать родительский элемент дочернему
        /// </summary>
        /// <param name="hWndChild">Дочерний элемент</param>
        /// <param name="hWndNewParent">Родительский элемент</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(this IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// Задать макет окна
        /// </summary>
        /// <param name="hWnd">Окно</param>
        /// <param name="nIndex"></param>
        /// <param name="dwNewLong"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(this IntPtr hWnd, int nIndex, uint dwNewLong);

        // This static method is required because legacy OSes do not support
        // SetWindowLongPtr
        /// <summary>
        /// Изменяет параметры окна
        /// </summary>
        /// <param name="hWnd">Окно</param>
        /// <param name="nIndex">Индекс параметра</param>
        /// <param name="dwNewLong">Параметр окна</param>
        /// <returns></returns>
        public static IntPtr SetWindowLongPtr(this IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// Изменить состояние окна
        /// </summary>
        /// <param name="hWnd">Окно</param>
        /// <param name="nCmdShow">Состояние</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(this IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Получить ID процесса
        /// </summary>
        /// <param name="handle">Дискрептор процесса</param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern int GetProcessId(this IntPtr handle);

        /// <summary>
        /// Убивает процесс
        /// </summary>
        /// <param name="handle"></param>
        public static void Close(this IntPtr handle)
        {
            Process.GetProcessById(handle.GetProcessId()).Kill();
        }

        /// <summary>
        /// Удалить файл
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeleteFile(this string path);

        /// <summary>
        /// Получить текущее окно консоли
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        /// <summary>
        /// Задает позицию окно
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="uFlags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]       
        public static extern bool SetWindowPos(
        this IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        int uFlags);

        public const int HWND_TOPMOST = -1;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOSIZE = 0x0001;

        /// <summary>
        /// Отправляет сигнал консоли приложения
        /// </summary>
        /// <param name="sigevent">Сигнал консоли</param>
        /// <param name="dwProcessGroupId">ID процесса</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GenerateConsoleCtrlEvent(this int dwProcessGroupId, ConsoleCtrlEvent sigevent);
        public enum ConsoleCtrlEvent
        {
            CTRL_C = 0,
            CTRL_BREAK = 1,
            CTRL_CLOSE = 2,
            CTRL_LOGOFF = 5,
            CTRL_SHUTDOWN = 6
        }

        /// <summary>
        /// Получить активный процесс
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]       
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32", SetLastError = true)]
        public static extern int GetWindowThreadProcessId([In]IntPtr hwnd, [Out]out int lProcessId);

        /// <summary>
        /// В фокусе ли данный процес по ID
        /// </summary>
        /// <param name="processId">ID процесса</param>
        /// <returns></returns>
        public static bool IsFocusWindow(this int processId)
        {
            int lProcessId;
            IntPtr foregroundWindow = GetForegroundWindow();
            GetWindowThreadProcessId(foregroundWindow, out lProcessId);
            return processId == lProcessId;
        }

        /// <summary>
        /// Нажата ли данная клавиша на клавиатуре
        /// </summary>
        /// <param name="vKey"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(this System.Windows.Forms.Keys vKey);

        /// <summary>
        /// Зарегистрировать горячую клавишу
        /// </summary>
        /// <param name="hWnd">Хандлер окна</param>
        /// <param name="id">ID хоткея</param>
        /// <param name="fsModifiers">Модификатор</param>
        /// <param name="vk">Клавиша</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(this IntPtr hWnd, int id, uint fsModifiers, uint vk);
        /// <summary>
        /// Унрегистрирвоать горячую клавишу
        /// </summary>
        /// <param name="hWnd">Хандлер окна</param>
        /// <param name="id">ID хоткея</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(this IntPtr hWnd, int id);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateJobObject(this IntPtr lpJobAttributes, string name);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AssignProcessToJobObject(this IntPtr job, IntPtr process);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(this IntPtr lParam, EnumWindowsProc lpEnumFunc);

        public class SearchData
        {
            // You can put any dicks or Doms in here...
            public string Wndclass;
            public string Title;
            public IntPtr hWnd;
        }

        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        public static IEnumerable<IntPtr> EnumerateProcessWindowHandles(this int processId)
        {
            var handles = new List<IntPtr>();

                EnumThreadWindows(Process.GetProcessById(processId).Threads[0].Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }

        public delegate bool EnumWindowsProc(IntPtr hWnd, SearchData data);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(this IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(this IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(this IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(this IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetClassName(this IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(this IntPtr hWnd);

        /// <summary>
        /// Индексы параметров окна
        /// </summary>
        public enum WindowLongFlags : int
        {
            GWL_EXSTYLE = -20,
            GWLP_HINSTANCE = -6,
            GWLP_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4,
            DWLP_USER = 0x8,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4
        }
        public enum nCmdShow : int
        {
            /// <summary>
            /// Скрыть окно и активизировать другое окно.
            /// </summary>
            SW_HIDE = 0,
            /// <summary>
            /// Развернуть окно.
            /// </summary>
            SW_MAXIMIZE = 3,
            /// <summary>
            /// Свернуть окно и активизировать следующее окно в Z-порядке(следующее под свернутым окном).
            /// </summary>
            SW_MINIMIZE = 6,
            /// <summary>
            /// ///Активизировать и отобразить окно.Если окно свернуто или развернуто,Windows восстанавливает его исходный размер и положение.
            /// </summary>
            SW_RESTORE = 9,
            /// <summary>
            ///Активизировать окно.
            /// </summary>
            SW_SHOW = 5,
            /// <summary>
            ///Отобразить окно в развернутом виде.
            /// </summary>
            SW_SHOWMAXIMIZED = 3,
            /// <summary>
            ///Отобразить окно в свернутом виде.
            /// </summary>
            SW_SHOWMINIMIZED = 2,
            /// <summary>
            ///Отобразить окно в свернутом виде.Активное окно остается активным.
            /// </summary>
            SW_SHOWMINNOACTIVE = 7,
            /// <summary>
            ///Отобразить окно в текущем состоянии.Активное окно остается активным.
            /// </summary>
            SW_SHOWNA = 8,
            /// <summary>
            ///Отобразить окно в соответствии с последними значениями позиции и размера.Активное окно остается активным.
            /// </summary>
            SW_SHOWNOACTIVATE = 4,
            /// <summary>
            ///Активизировать и отобразить окно.Если окно свернуто или развернуто,Windows восстанавливает его исходный размер и положение.Приложение должно указывать этот флаг при первом отображении окна.
            /// </summary>
            SW_SHOWNORMAL = 1,           
        }
    }
}
