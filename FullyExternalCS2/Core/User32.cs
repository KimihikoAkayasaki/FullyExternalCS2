using System;
using System.Runtime.InteropServices;
using FullyExternalCS2.Core.Data;
using Point = FullyExternalCS2.Core.Data.Point;

namespace FullyExternalCS2.Core;

public static class User32
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool ClientToScreen(IntPtr hWnd, out Point lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);
}