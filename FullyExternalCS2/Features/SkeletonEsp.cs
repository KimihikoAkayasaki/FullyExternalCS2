using System;
using FullyExternalCS2.Data.Entity;
using Color = SharpDX.Color;

namespace FullyExternalCS2.Features;

public static class SkeletonEsp
{
    public static void Draw(Graphics.Graphics graphics)
    {
        foreach (var entity in graphics.GameData.Entities)
        {
            if (!entity.IsAlive() || entity.AddressBase == graphics.GameData.Player.AddressBase) continue;

            var colorBones =
                entity.Team == graphics.GameData.Player.Team
                    ? Color.White
                    : Color.FromRgba(RgbaToHex((int)((1.0f - entity.Health / 100.0f) * 255), (int)(entity.Health / 100.0f * 255), 0, 255));

            DrawBones(graphics, entity, colorBones);
        }
    }

    public static uint RgbaToHex(int r, int g, int b, int a)
    {
        return (uint)((a.Clamp(0, 255) << 24) | (b << 16) | (g << 8) | r.Clamp(0, 255));
    }

    private static void DrawBones(Graphics.Graphics graphics, Entity entity, Color color)
    {
        (string, string)[] bones =
        [
            ("head", "neck_0"),
            ("neck_0", "spine_1"),
            ("spine_1", "spine_2"),
            ("spine_2", "pelvis"),
            ("spine_1", "arm_upper_L"),
            ("arm_upper_L", "arm_lower_L"),
            ("arm_lower_L", "hand_L"),
            ("spine_1", "arm_upper_R"),
            ("arm_upper_R", "arm_lower_R"),
            ("arm_lower_R", "hand_R"),
            ("pelvis", "leg_upper_L"),
            ("leg_upper_L", "leg_lower_L"),
            ("leg_lower_L", "ankle_L"),
            ("pelvis", "leg_upper_R"),
            ("leg_upper_R", "leg_lower_R"),
            ("leg_lower_R", "ankle_R")
        ];

        foreach (var (startBone, endBone) in bones)
            graphics.DrawLineWorld(color, entity.BonePos[startBone], entity.BonePos[endBone]);
    }
}

public static class Extensions
{
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0) return min;
        else if (val.CompareTo(max) > 0) return max;
        else return val;
    }
}