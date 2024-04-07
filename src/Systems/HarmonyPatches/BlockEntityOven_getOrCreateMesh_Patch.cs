using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace DanaTweaks;

public static class BlockEntityOven_getOrCreateMesh_Patch
{
    public static bool Prefix(BlockEntityOven __instance, ref MeshData __result, ItemStack stack, int index)
    {
        ICoreClientAPI capi = __instance.GetField<ICoreClientAPI>("capi");
        EnumOvenContentMode OvenContentMode = __instance.GetProperty<EnumOvenContentMode>("OvenContentMode");
        if (OvenContentMode == EnumOvenContentMode.Firewood)
        {
            MeshData mesh = __instance.CallMethod<MeshData>("getMesh", stack);
            if (mesh != null)
            {
                __result = mesh;
                return false;
            }
            string shapeLoc = __instance.Block.Attributes["ovenFuelShape"].AsString();
            if (__instance.FuelSlot?.Itemstack?.Collectible.Attributes.KeyExists("ovenFuelShape") == true)
            {
                shapeLoc = __instance.FuelSlot.Itemstack.Collectible.Attributes["ovenFuelShape"].AsString();
            }

            __instance.SetField("nowTesselatingShape", Shape.TryGet(shapePath: AssetLocation.Create(shapeLoc, __instance.Block.Code.Domain).WithPathPrefixOnce("shapes/").WithPathAppendixOnce(".json"), api: capi));
            __instance.SetField("nowTesselatingObj", stack.Collectible);
            if (__instance.GetField<Shape>("nowTesselatingShape") == null)
            {
                capi.Logger.Error("Stacking model shape for collectible " + stack.Collectible.Code?.ToString() + " not found. Block will be invisible!");
                __result = null;
                return false;
            }
            capi.Tesselator.TesselateShape("ovenFuelShape", __instance.GetField<Shape>("nowTesselatingShape"), out mesh, __instance, null, 0, 0, 0, stack.StackSize);
            string key = __instance.CallMethod<string>("getMeshCacheKey", stack);
            Dictionary<string, MeshData> MeshCache = __instance.GetProperty<Dictionary<string, MeshData>>("MeshCache");
            MeshCache[key] = mesh;
            __result = mesh;
            return false;
        }
        __result = __instance.CallMethod<MeshData>("getOrCreateMesh", stack, index);
        return false;
    }
}