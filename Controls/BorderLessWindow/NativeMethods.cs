using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace myCollections.Controls
{
    public static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public readonly int left;
            public readonly int top;
            public readonly int right;
            public readonly int bottom;
            public static readonly RECT Empty;
            public int Width
            {
                get
                {
                    return Math.Abs(right - left);
                }
            }
            public int Height
            {
                get
                {
                    return (bottom - top);
                }
            }
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            public bool IsEmpty
            {
                get
                {
                    if (left < right)
                    {
                        return (top >= bottom);
                    }
                    return true;
                }
            }
            public override string ToString()
            {
                if (this == Empty)
                {
                    return "RECT {Empty}";
                }
                return string.Concat(new object[] { "RECT { left : ", left, " / top : ", top, " / right : ", right, " / bottom : ", bottom, " }" });
            }

            public override bool Equals(object obj)
            {
                if (obj is RECT)
                    if (this == (RECT)obj)
                        return true;

                return false;
            }

            public override int GetHashCode()
            {
                return (((left.GetHashCode() + top.GetHashCode()) + right.GetHashCode()) + bottom.GetHashCode());
            }

            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return ((((rect1.left == rect2.left) && (rect1.top == rect2.top)) && (rect1.right == rect2.right)) && (rect1.bottom == rect2.bottom));
            }

            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }

            static RECT()
            {
                Empty = new RECT();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
 

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
 

        public static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO structure = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            IntPtr hMonitor = MonitorFromWindow(hwnd, 2);
            if (hMonitor != IntPtr.Zero)
            {
                MONITORINFO lpmi = new MONITORINFO();
                GetMonitorInfo(hMonitor, lpmi);
                RECT rcWork = lpmi.rcWork;
                RECT rcMonitor = lpmi.rcMonitor;
                structure.ptMaxPosition.x = Math.Abs(rcWork.left - rcMonitor.left);
                structure.ptMaxPosition.y = Math.Abs(rcWork.top - rcMonitor.top);
                structure.ptMaxSize.x = Math.Abs(rcWork.right - rcWork.left);
                structure.ptMaxSize.y = Math.Abs(rcWork.bottom - rcWork.top);
                structure.ptMinTrackSize.x = (int)Application.Current.MainWindow.MinWidth;
                structure.ptMinTrackSize.y = (int)Application.Current.MainWindow.MinHeight;
            }
            Marshal.StructureToPtr(structure, lParam, true);
        }

 

 

    }
}
