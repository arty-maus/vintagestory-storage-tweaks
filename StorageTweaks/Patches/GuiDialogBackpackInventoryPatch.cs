// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

using System;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace StorageTweaks.Patches;

[HarmonyPatch(typeof(GuiComposerHelpers), "AddDialogTitleBar")]
public class GuiDialogBackpackInventoryPatch
{
    private static SvgToggleButton? _favoriteToggleButton;

    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    public static void Postfix(GuiComposer composer)
    {
        var capi = composer.Api;
        if (capi == null) return;

        if (composer.DialogName != "inventory-backpack") return;

        if (composer["storagetweaks-sort"] != null)
            return;

        PatchUtils.AddButton(composer, "sort", -60,
            inventory => PatchUtils.SendPacket(capi, new SortInventoryPacket { InventoryId = inventory.InventoryID }), Lang.Get("storagetweaks:compact-and-sort"));
        
        PatchUtils.AddButton(composer, "store-nearby", -86, _ => PatchUtils.SendPacket(capi, new QuickStoreNearbyContainersPacket()), Lang.Get("storagetweaks:store-nearby"));

        AddFavoriteToggle(composer, capi);
        AddFavoritesHideToggle(composer, capi);
    }

    private static void AddFavoriteToggle(GuiComposer composer, ICoreClientAPI capi)
    {
        var iconAsset = new AssetLocation("storagetweaks", "textures/icons/favorite.svg");
        var icon = capi.Assets.TryGet(iconAsset);
        if (icon == null) return;

        var bounds = ElementBounds.Fixed(EnumDialogArea.RightTop, -112, 4, 24, 24);
        _favoriteToggleButton = new SvgToggleButton(
            capi,
            icon,
            () => true,
            active => { FavoritesManager.Get()!.IsFavoriteModeActive = active; },
            bounds,
            ColorUtil.ColorFromRgba(247, 250, 72, 255),
            ColorUtil.ColorFromRgba(222, 225, 65, 255)
        );
        _favoriteToggleButton.IsActive = FavoritesManager.Get()?.IsFavoriteModeActive ?? false;

        composer.AddInteractiveElement(_favoriteToggleButton, "storagetweaks-favorite")
            .AddHoverText(
                Lang.Get("storagetweaks:toggle-favorite-mode-help"),
                CairoFont.WhiteSmallText(),
                250,
                bounds.FlatCopy()
            );
    }

    private static void AddFavoritesHideToggle(GuiComposer composer, ICoreClientAPI capi)
    {
        var bounds = ElementBounds.Fixed(EnumDialogArea.RightTop, -138, 5, 24, 24);
        var toggleBtn = new GuiElementToggleButton(composer.Api, "", "", CairoFont.SmallButtonText(), on => GuiElementItemSlotGridPatch.HideFavorites = on, bounds, true);
        toggleBtn.On = GuiElementItemSlotGridPatch.HideFavorites;
        composer.AddInteractiveElement(toggleBtn, "storagetweaks-hide-favorites").AddDynamicCustomDraw(bounds,
            (_, surface, _) =>
            {
                var iconAsset = new AssetLocation("storagetweaks", "textures/icons/favorites-hide.svg");
                var icon = capi.Assets.TryGet(iconAsset);
                var iconSize = (int)GuiElement.scaled(20.0);
                var margin = (int)GuiElement.scaled(2);
                if (icon != null)
                    capi.Gui.DrawSvg(icon, surface, margin, margin, iconSize, iconSize, SvgButton.NormalColor);
            }).AddHoverText(Lang.Get("storagetweaks:toggle-hide-favorites"), CairoFont.WhiteSmallText(), 250,
            bounds.FlatCopy());
    }
}