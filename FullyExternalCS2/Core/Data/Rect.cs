using System.Runtime.InteropServices;

namespace FullyExternalCS2.Core.Data;

[StructLayout(LayoutKind.Sequential)]
public struct Rect
{
    public int Left, Top, Right, Bottom;
}