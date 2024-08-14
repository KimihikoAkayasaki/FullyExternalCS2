using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FullyExternalCS2.Core;
using FullyExternalCS2.Core.Data;
using SharpDX;
using static System.Diagnostics.Process;
using Rectangle = System.Drawing.Rectangle;


namespace FullyExternalCS2.Utils;

public static class Utility
{
    private const double PiOver180 = Math.PI / 180.0;

    public static double DegreeToRadian(this double degree)
    {
        return degree * PiOver180;
    }

    public static float DegreeToRadian(this float degree)
    {
        return (float)(degree * PiOver180);
    }


    public static Rectangle GetClientRectangle(IntPtr handle)
    {
        return User32.ClientToScreen(handle, out var point) && User32.GetClientRect(handle, out var rect)
            ? new Rectangle(point.X, point.Y, rect.Right - rect.Left, rect.Bottom - rect.Top)
            : default;
    }


    public static Module GetModule(this System.Diagnostics.Process process, string moduleName)
    {
        var processModule = process.GetProcessModule(moduleName);
        return processModule is null || processModule.BaseAddress == IntPtr.Zero
            ? default
            : new Module(process, processModule);
    }


    private static ProcessModule GetProcessModule(this System.Diagnostics.Process process,
        string moduleName)
    {
        return process?.Modules.OfType<ProcessModule>()
            .FirstOrDefault(a => string.Equals(a.ModuleName.ToLower(), moduleName.ToLower()));
    }


    public static bool IsRunning(this System.Diagnostics.Process process)
    {
        try
        {
            GetProcessById(process.Id);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        return true;
    }

    public static Team ToTeam(this int teamNum)
    {
        return teamNum switch
        {
            1 => Team.Spectator,
            2 => Team.Terrorists,
            3 => Team.CounterTerrorists,
            _ => Team.Unknown
        };
    }

    #region memory

    public static T Read<T>(this System.Diagnostics.Process process, IntPtr lpBaseAddress)
        where T : unmanaged
    {
        return Read<T>(process.Handle, lpBaseAddress);
    }


    public static T Read<T>(this Module module, int offset)
        where T : unmanaged
    {
        return Read<T>(module.Process.Handle, module.ProcessModule.BaseAddress + offset);
    }


    private static T Read<T>(IntPtr hProcess, IntPtr lpBaseAddress)
        where T : unmanaged
    {
        var size = Marshal.SizeOf<T>();
        var buffer = default(T) as object;
        Kernel32.ReadProcessMemory(hProcess, lpBaseAddress, buffer, size, out var lpNumberOfBytesRead);
        return lpNumberOfBytesRead == size ? (T)buffer : default;
    }

    public static string ReadString(this System.Diagnostics.Process process, IntPtr lpBaseAddress, int maxLength = 256)
    {
        var buffer = ReadBytes(process.Handle, lpBaseAddress, maxLength);
        var nullCharIndex = Array.IndexOf(buffer, (byte)'\0');
        return nullCharIndex >= 0
            ? Encoding.UTF8.GetString(buffer.AsSpan(0, nullCharIndex + 1).ToArray()).Trim()
            : Encoding.UTF8.GetString(buffer).Trim();
    }

    private static byte[] ReadBytes(IntPtr hProcess, IntPtr lpBaseAddress, int maxLength)
    {
        var buffer = new byte[maxLength];
        Kernel32.ReadProcessMemory(hProcess, lpBaseAddress, buffer, maxLength, out _);
        return buffer;
    }


    public static Matrix GetMatrixViewport(Size screenSize)
    {
        return GetMatrixViewport(new Viewport
        {
            X = 0,
            Y = 0,
            Width = screenSize.Width,
            Height = screenSize.Height,
            MinDepth = 0,
            MaxDepth = 1
        });
    }

    private static Matrix GetMatrixViewport(in Viewport viewport)
    {
        return new Matrix
        {
            M11 = viewport.Width * 0.5f,
            M12 = 0,
            M13 = 0,
            M14 = 0,

            M21 = 0,
            M22 = -viewport.Height * 0.5f,
            M23 = 0,
            M24 = 0,

            M31 = 0,
            M32 = 0,
            M33 = viewport.MaxDepth - viewport.MinDepth,
            M34 = 0,

            M41 = viewport.X + viewport.Width * 0.5f,
            M42 = viewport.Y + viewport.Height * 0.5f,
            M43 = viewport.MinDepth,
            M44 = 1
        };
    }

    #endregion
}