using System;
using System.Collections.Generic;
using System.Linq;
using FullyExternalCS2.Core.Data;
using FullyExternalCS2.Data.Entity;
using FullyExternalCS2.Graphics;
using SharpDX;
using SharpDX.Direct3D9;
using Color = SharpDX.Color;

namespace FullyExternalCS2.Features;

public static class EspBox
{
    private const int OutlineThickness = 2;

    public static void Draw(Graphics.Graphics graphics)
    {
        var boundingBoxes = new Dictionary<Entity, (Vector2, Vector2)>();

        foreach (var entity in graphics.GameData.Entities)
        {
            if (!entity.IsAlive() || entity.AddressBase == graphics.GameData.Player.AddressBase)
                continue;

            var boundingBox = GetEntityBoundingBox(graphics, entity);
            boundingBoxes.Add(entity, boundingBox);
        }

        foreach (var entity in boundingBoxes.Keys)
        {
            var colorBox = entity.Team == Team.Terrorists ? Color.DarkRed : Color.DarkBlue;
            DrawEntityRectangle(graphics, entity, colorBox, boundingBoxes[entity]);
        }
    }

    private static void DrawEntityRectangle(Graphics.Graphics graphics, Entity entity, Color color,
        (Vector2, Vector2) boundingBox)
    {
        DrawWeaponName(graphics, boundingBox, entity.CurrentWeaponName);
        DrawFlags(graphics, boundingBox, entity);
    }

    private static void DrawWeaponName(Graphics.Graphics graphics, (Vector2, Vector2) boundingBox,
        string currentWeaponName)
    {
        var textWidth = graphics.FontConsolas32.MeasureText(null, currentWeaponName ?? "NONE", FontDrawFlags.Center)
            .Right + 30;
        var weaponNamePosition = new Vector2((boundingBox.Item1.X + boundingBox.Item2.X - textWidth / 2.0f) / 2,
            boundingBox.Item2.Y + 5f);
        graphics.FontConsolas32.DrawText(default, currentWeaponName ?? "NONE", (int)weaponNamePosition.X,
            (int)weaponNamePosition.Y, Color.White);
    }

    private static void DrawFlags(Graphics.Graphics graphics, (Vector2, Vector2) boundingBox, Entity entity)
    {
        var flagsPosition = new Vector2(boundingBox.Item2.X + 5f, boundingBox.Item1.Y);

        if (entity.IsinScope == 1)
            graphics.FontConsolas32.DrawText(default, "Scoped", (int)flagsPosition.X, (int)flagsPosition.Y,
                Color.White);

        if (entity.FlashAlpha > 7)
            graphics.FontConsolas32.DrawText(default, "Flashed", (int)flagsPosition.X, (int)flagsPosition.Y + 15,
                Color.White);

        switch (entity.IsinScope)
        {
            case 256:
                graphics.FontConsolas32.DrawText(default, "Shifting", (int)flagsPosition.X, (int)flagsPosition.Y + 30,
                    Color.White);
                break;
            case 257:
                graphics.FontConsolas32.DrawText(default, "Shifting in scope", (int)flagsPosition.X,
                    (int)flagsPosition.Y + 45, Color.White);
                break;
        }
    }

    private static (Vector2, Vector2) GetEntityBoundingBox(Graphics.Graphics graphics, Entity entity)
    {
        const float padding = 5.0f;
        var minScreenPos = new Vector2(float.MaxValue, float.MaxValue);
        var maxScreenPos = new Vector2(float.MinValue, float.MinValue);

        foreach (var transformedPos in entity.BonePos.ToList()
                     .Select(bonePos => graphics.GameData.Player.MatrixViewProjectionViewport.Transform(bonePos.Value))
                     .Where(transformedPos => transformedPos.Z < 1))
        {
            minScreenPos.X = Math.Min(minScreenPos.X, transformedPos.X);
            minScreenPos.Y = Math.Min(minScreenPos.Y, transformedPos.Y);
            maxScreenPos.X = Math.Max(maxScreenPos.X, transformedPos.X);
            maxScreenPos.Y = Math.Max(maxScreenPos.Y, transformedPos.Y);
        }

        var healthPercentage = (float)entity.Health / 100;
        var sizeMultiplier = 1.0f + (1.0f - healthPercentage);
        minScreenPos -= new Vector2(padding * sizeMultiplier, padding * sizeMultiplier);
        maxScreenPos += new Vector2(padding * sizeMultiplier, padding * sizeMultiplier);

        return (minScreenPos, maxScreenPos);
    }
}