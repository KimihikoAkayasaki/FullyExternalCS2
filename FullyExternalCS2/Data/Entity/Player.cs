﻿using System;
using System.Collections.Generic;
using FullyExternalCS2.Core.Data;
using FullyExternalCS2.Data.Game;
using FullyExternalCS2.Graphics;
using FullyExternalCS2.Utils;
using SharpDX;

namespace FullyExternalCS2.Data.Entity;

public class Player : EntityBase
{
    private Matrix MatrixViewProjection { get; set; }
    public Matrix MatrixViewport { get; private set; }
    public Matrix MatrixViewProjectionViewport { get; private set; }
    private Vector3 ViewOffset { get; set; }
    public Vector3 EyePosition { get; private set; }
    private Vector3 ViewAngles { get; set; }
    public Vector3 AimPunchAngle { get; private set; }
    public Vector3 AimDirection { get; private set; }

    public Vector3 EyeDirection { get; private set; }


    public static int Fov => 90;

    public int FFlags { get; private set; }


    protected override IntPtr ReadControllerBase(GameProcess gameProcess)
    {
        return gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayerController);
    }

    protected override IntPtr ReadAddressBase(GameProcess gameProcess)
    {
        return gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayerPawn);
    }

    public override bool Update(GameProcess gameProcess)
    {
        if (!base.Update(gameProcess)) return false;


        MatrixViewProjection = Matrix.Transpose(gameProcess.ModuleClient.Read<Matrix>(Offsets.dwViewMatrix));
        MatrixViewport = Utility.GetMatrixViewport(gameProcess.WindowRectangleClient.Size);
        MatrixViewProjectionViewport = MatrixViewProjection * MatrixViewport;

        ViewOffset = gameProcess.Process.Read<Vector3>(AddressBase + Offsets.m_vecViewOffset);
        EyePosition = Origin + ViewOffset;
        ViewAngles = gameProcess.ModuleClient.Read<Vector3>(Offsets.dwViewAngles);
        AimPunchAngle = gameProcess.Process.Read<Vector3>(AddressBase + Offsets.m_AimPunchAngle);
        FFlags = gameProcess.Process.Read<int>(AddressBase + Offsets.m_fFlags);

        EyeDirection =
            GraphicsMath.GetVectorFromEulerAngles(ViewAngles.X.DegreeToRadian(), ViewAngles.Y.DegreeToRadian());
        AimDirection = GraphicsMath.GetVectorFromEulerAngles
        (
            (ViewAngles.X + AimPunchAngle.X * Offsets.WeaponRecoilScale).DegreeToRadian(),
            (ViewAngles.Y + AimPunchAngle.Y * Offsets.WeaponRecoilScale).DegreeToRadian()
        );

        return true;
    }

    public bool IsGrenade()
    {
        return new HashSet<string>
        {
            nameof(WeaponIndexes.Smokegrenade), nameof(WeaponIndexes.Flashbang), nameof(WeaponIndexes.Hegrenade),
            nameof(WeaponIndexes.Molotov)
        }.Contains(CurrentWeaponName);
    }
}