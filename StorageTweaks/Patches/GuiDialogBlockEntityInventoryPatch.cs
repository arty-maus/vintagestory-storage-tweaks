// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using System;
using System.Linq;
using HarmonyLib;
using StorageTweaks.Gui;
using Vintagestory.API.Client;

// ReSharper disable UnusedParameter.Global

namespace StorageTweaks.Patches;

[HarmonyPatch(typeof(GuiDialogBlockEntityInventory), "OnGuiOpened")]
public static class GuiDialogBlockEntityInventoryPatch
{
    private static readonly string[] DialogNamePrefixes = ["blockentityinventory", "attachedcontainer"];

    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    public static void Postfix(GuiDialogBlockEntityInventory __instance)
    {
        var composer = __instance.SingleComposer;
        if (composer.DialogName == null)
        {
            composer.Api.Logger.Warning(
                "[StorageTweaks] GuiDialogBlockEntityInventory constructed without dialog name");
            return;
        }

        if (!DialogNamePrefixes.Any(prefix => composer.DialogName.StartsWith(prefix, StringComparison.Ordinal)))
        {
            composer.Api.Logger.Warning("[StorageTweaks] {0} not in whitelist for block entity dialog names",
                composer.DialogName);
            return;
        }

        var modSystem = composer.Api.ModLoader.GetModSystem<StorageTweaksModSystem>();

        if (modSystem == null)
        {
            composer.Api.Logger.Warning("[StorageTweaks] StorageTweaksModSystem not found");
            return;
        }

        modSystem.ContainerActionButtons!.ComposeGui(composer);
    }
}