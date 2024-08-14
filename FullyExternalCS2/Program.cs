using System;
using Figgle;
using FullyExternalCS2.Data.Game;
using FullyExternalCS2.Graphics;
using FullyExternalCS2.Utils;
using static FullyExternalCS2.Core.User32;
using Application = System.Windows.Application;

namespace FullyExternalCS2;

public class Program :
    Application,
    IDisposable
{
    private Program()
    {
        _ = Offsets.UpdateOffsets();
        Startup += (_, _) => InitializeComponent();
        Exit += (_, _) => Dispose();
    }

    private GameProcess GameProcess { get; set; } = null!;

    private GameData GameData { get; set; } = null!;

    private WindowOverlay WindowOverlay { get; set; } = null!;

    private Graphics.Graphics Graphics { get; set; } = null!;

    public void Dispose()
    {
        GameProcess.Dispose();
        GameProcess = default!;

        GameData.Dispose();
        GameData = default!;

        WindowOverlay.Dispose();
        WindowOverlay = default!;

        Graphics.Dispose();
        Graphics = default!;
    }

    public static void Main()
    {
        Console.WriteLine(FiggleFonts.Standard.Render("FullyExternalCS2"));
        new Program().Run();
    }

    private void InitializeComponent()
    {
        Console.WriteLine("Searching for the game process...");
        GameProcess = new GameProcess();
        GameProcess.Start();

        Console.WriteLine("Starting the game data stream...");
        GameData = new GameData(GameProcess);
        GameData.Start();

        Console.WriteLine("Initializing the window overlay...");
        WindowOverlay = new WindowOverlay(GameProcess);
        WindowOverlay.Start();

        Console.WriteLine("Starting the drawing handler...");
        Graphics = new Graphics.Graphics(GameProcess, GameData, WindowOverlay);
        Graphics.Start();

        SetWindowDisplayAffinity(WindowOverlay.Window.Handle, 0x00000011);
    }
}