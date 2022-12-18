using System;
using System.Runtime.InteropServices;


namespace ListenToMixerForVolume
{
    public class Window32DLL {
        public static void GetActiveChildOfProcessId(IntPtr mainWindowHandle, out IntPtr childId)
        {
            Window32DLL.FetchFirstChildrenThatHasDimension(mainWindowHandle, out bool found, out childId);
        }
        public static void FetchFirstChildrenThatHasDimension(IntPtr intPtr, out bool foundChild, out IntPtr target)
        {
            RectPadValue rect = new RectPadValue();
            IntPtr[] ptrs = GetProcessIdChildrenWindows((int)intPtr);
            for (int i = 0; i < ptrs.Length; i++)
            {
                GetWindowRect(ptrs[i], ref rect);
                if (rect.IsNotZero())
                {
                    foundChild = true;
                    target = ptrs[i];
                    return;
                }
            }
            foundChild = false;
            target = IntPtr.Zero;
        }
        public static IntPtr[] GetProcessIdChildrenWindows(int process)
        {
            IntPtr[] apRet = (new IntPtr[256]);
            int iCount = 0;
            IntPtr pLast = IntPtr.Zero;
            do
            {
                pLast = FindWindowEx(IntPtr.Zero, pLast, null, null);
                int iProcess_;
                GetWindowThreadProcessId(pLast, out iProcess_);
                if (iProcess_ == process) apRet[iCount++] = pLast;
            } while (pLast != IntPtr.Zero);
            System.Array.Resize(ref apRet, iCount);
            return apRet;
        }

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref RectPadValue rectangle);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentWindow, IntPtr previousChildWindow, string windowClass, string windowTitle);


        public struct RectPadValue
        {
            //DONT CHANGE THE ORDER Of THE INT left, top, right, bot
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public bool IsNotZero()
            {
                return Left != 0 ||
                     Top != 0 ||
                     Right != 0 ||
                     Bottom != 0;
            }
            public bool IsEqualZero()
            {
                return Left == 0 &&
                    Top == 0 &&
                    Right == 0 &&
                    Bottom == 0;
            }

        }
    }

}
