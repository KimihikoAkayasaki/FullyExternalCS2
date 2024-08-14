using System.Runtime.InteropServices;
using SharpDX;

namespace FullyExternalCS2.Core.Data;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public Vector4 Position;
    public ColorBGRA Color;
}