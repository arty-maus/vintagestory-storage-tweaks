using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ProtoBuf;
using StorageTweaks.Gui;
using StorageTweaks.Patches;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace StorageTweaks;

[ProtoContract]
public class SortInventoryPacket
{
    [ProtoMember(1)] public required string InventoryId;

    [ProtoMember(2)] public bool StackPerishables;
}

[ProtoContract]
public class UnloadInventoryPacket
{
    [ProtoMember(1)] public required string InventoryId;

    [ProtoMember(2)] public bool StackPerishables;
}

[ProtoContract]
public class QuickStoreNearbyContainersPacket
{
    [ProtoMember(1)] public bool StackPerishables;
}

[ProtoContract]
public class UpdateFavoritesPacket
{
    /// <summary>
    ///     Collectable Code
    /// </summary>
    [ProtoMember(1)] public required string Code;

    [ProtoMember(2)] public bool IsFavorite;
}

public class ContainerEntry
{
    public string InventoryClass { get; init; } = "";
    public string Type { get; init; } = "";
}

public class StorageTweaksServerConfig
{
    public List<ContainerEntry> QuickStoreContainers { get; } =
    [
        // vanilla baskets
        new ContainerEntry { InventoryClass = "basket", Type = "reed" },
        new ContainerEntry { InventoryClass = "basket", Type = "papyrus" },
        new ContainerEntry { InventoryClass = "basket", Type = "aged" },

        // vanilla chests
        new ContainerEntry { InventoryClass = "chest", Type = "normal-labeled" },
        new ContainerEntry { InventoryClass = "chest", Type = "normal-generic" },
        new ContainerEntry { InventoryClass = "chest", Type = "normal" },
        new ContainerEntry { InventoryClass = "chest", Type = "normal-aged" },

        // vanilla crates
        new ContainerEntry { InventoryClass = "crate", Type = "crate" },

        // String Sense mod containers
        new ContainerEntry { InventoryClass = "basket", Type = "bark" },
        new ContainerEntry { InventoryClass = "basket", Type = "vine" },
        new ContainerEntry { InventoryClass = "basket", Type = "mixed" },
        new ContainerEntry { InventoryClass = "basket", Type = "flax" },
        new ContainerEntry { InventoryClass = "basket", Type = "straw" },

        // Better Crates mod containers
        new ContainerEntry { InventoryClass = "bettercrate", Type = "bettercrate2sided" },
        new ContainerEntry { InventoryClass = "bettercrate", Type = "bettercrate" },

        // Extra Chests mod
        new ContainerEntry { InventoryClass = "chest", Type = "blackbronze" },
        new ContainerEntry { InventoryClass = "chest", Type = "iron" },
        new ContainerEntry { InventoryClass = "chest", Type = "bismuthbronze" },
        new ContainerEntry { InventoryClass = "chest", Type = "steel" },
        new ContainerEntry { InventoryClass = "chest", Type = "tinbronze" },
        new ContainerEntry { InventoryClass = "chest", Type = "copper" },

        // Containers Bundle mod
        new ContainerEntry { InventoryClass = "chest", Type = "strongbox" },
        new ContainerEntry { InventoryClass = "chest", Type = "metalcabinetnolabel" },
        new ContainerEntry { InventoryClass = "chest", Type = "bamboochest" },
        new ContainerEntry { InventoryClass = "chest", Type = "cupboardnolabel" },
        new ContainerEntry { InventoryClass = "chest", Type = "stonecasket" },
        new ContainerEntry { InventoryClass = "chest", Type = "cupboardwithlabel" },
        new ContainerEntry { InventoryClass = "chest", Type = "woodenbox" },
        new ContainerEntry { InventoryClass = "chest", Type = "exquisitechest" },
        new ContainerEntry { InventoryClass = "chest", Type = "wickerbasket" },
        new ContainerEntry { InventoryClass = "chest", Type = "foodcupboard" },
        new ContainerEntry { InventoryClass = "chest", Type = "longcrate" },
        new ContainerEntry { InventoryClass = "chest", Type = "linencrate" },
        new ContainerEntry { InventoryClass = "chest", Type = "foodcupboardwall" },

        // Purposeful Storage mod
        new ContainerEntry { InventoryClass = "pantsrack", Type = "pantsrack" },
        new ContainerEntry { InventoryClass = "necklacestand", Type = "necklacestand" },
        new ContainerEntry { InventoryClass = "shoerack", Type = "shoerack" },
        new ContainerEntry { InventoryClass = "hatrack", Type = "hatrack" },
        new ContainerEntry { InventoryClass = "wardrobe", Type = "wardrobe" },
        new ContainerEntry { InventoryClass = "swordpedestal", Type = "swordpedestal" },
        new ContainerEntry { InventoryClass = "gloverack", Type = "gloverack" },
        new ContainerEntry { InventoryClass = "blanketrack", Type = "blanketrack" },
        new ContainerEntry { InventoryClass = "weaponrack", Type = "weaponrack" },
        new ContainerEntry { InventoryClass = "belthooks", Type = "belthooks" },
        new ContainerEntry { InventoryClass = "butterflydisplaypanel", Type = "butterflydisplaypanel" },
        new ContainerEntry { InventoryClass = "swordplaque", Type = "swordplaque" },
        new ContainerEntry { InventoryClass = "gearrack", Type = "gearrack" },
        new ContainerEntry { InventoryClass = "medallionrack", Type = "medallionrack" },
        new ContainerEntry { InventoryClass = "saddlerack", Type = "saddlerack" },
        new ContainerEntry { InventoryClass = "schematicrack", Type = "schematicrack" },
        new ContainerEntry { InventoryClass = "tuningcylinderrack", Type = "tuningcylinderrack" },
        new ContainerEntry { InventoryClass = "resourcebin", Type = "resourcebin" },
        new ContainerEntry { InventoryClass = "spearrack", Type = "spearrack" },
        new ContainerEntry { InventoryClass = "glidermount", Type = "glidermount" },

        // Food Shelves mod — shelves & display
        new ContainerEntry { InventoryClass = "doubleshelf", Type = "doubleshelf" },
        new ContainerEntry { InventoryClass = "breadshelf", Type = "breadshelf" },
        new ContainerEntry { InventoryClass = "barshelf", Type = "barshelf" },
        new ContainerEntry { InventoryClass = "eggshelf", Type = "eggshelf" },
        new ContainerEntry { InventoryClass = "pieshelf", Type = "pieshelf" },
        new ContainerEntry { InventoryClass = "seedshelf", Type = "seedshelf" },
        new ContainerEntry { InventoryClass = "sushishelf", Type = "sushishelf" },
        new ContainerEntry { InventoryClass = "tablewshelf", Type = "tablewshelf" },
        new ContainerEntry { InventoryClass = "fooddisplaycase", Type = "fooddisplaycase" },
        new ContainerEntry { InventoryClass = "fooddisplayblock", Type = "fooddisplayblock" },
        new ContainerEntry { InventoryClass = "pumpkincase", Type = "pumpkincase" },

        // Food Shelves mod — specialty storage
        new ContainerEntry { InventoryClass = "floursack", Type = "floursack" },
        new ContainerEntry { InventoryClass = "jar", Type = "jar" },
        new ContainerEntry { InventoryClass = "jarlarge", Type = "jarlarge" },
        new ContainerEntry { InventoryClass = "jarstand", Type = "jarstand" },
        new ContainerEntry { InventoryClass = "ceilingrack", Type = "ceilingrack" },
        new ContainerEntry { InventoryClass = "seedbins", Type = "seedbins" },
        new ContainerEntry { InventoryClass = "buckethook", Type = "buckethook" },

        // Food Shelves mod — coolers
        new ContainerEntry { InventoryClass = "coolingcabinet", Type = "coolingcabinet" },
        new ContainerEntry { InventoryClass = "meatfreezer", Type = "meatfreezer" },
        new ContainerEntry { InventoryClass = "fruitcooler", Type = "fruitcooler" },
        new ContainerEntry { InventoryClass = "wallcabinet", Type = "wallcabinet" },

        // Food Shelves mod — baskets
        new ContainerEntry { InventoryClass = "fruitbasket", Type = "fruitbasket" },
        new ContainerEntry { InventoryClass = "vegetablebasket", Type = "vegetablebasket" },
        new ContainerEntry { InventoryClass = "eggbasket", Type = "eggbasket" },
        new ContainerEntry { InventoryClass = "mushroombasket", Type = "mushroombasket" },

        // Food Shelves mod — barrel/tun racks
        new ContainerEntry { InventoryClass = "barrelrack", Type = "barrelrack" },
        new ContainerEntry { InventoryClass = "tunrack", Type = "tunrack" },
    ];
}

public class StorageTweaksClientConfig
{
    public bool HideFavorites { get; set; }

    /// When true, food with differing perish/spoil progress is stacked on unload,
    /// blending the transition state (same as a manual merge). Default false keeps
    /// the vanilla behavior of not auto-merging differently-perished stacks.
    public bool StackPerishables { get; set; }

    /// When true, the sort & compact button is hidden in inventory and container GUIs.
    /// Sorting via hotkey still works.
    public bool HideSortButton { get; set; }

    /// When true, the quick store nearby button is hidden in the inventory GUI.
    /// Quick store nearby via hotkey still works.
    public bool HideStoreNearbyButton { get; set; }

    /// When true, the force-stack on unload toggle is hidden in the inventory GUI.
    public bool HideStackPerishablesButton { get; set; }

    /// When true, the quick store button is hidden in container GUIs.
    /// Quick store via hotkey still works.
    public bool HideQuickStoreButton { get; set; }
}

// ReSharper disable once UnusedType.Global
// ReSharper disable once ClassNeverInstantiated.Global
public class StorageTweaksModSystem : ModSystem
{
    public InventoryActionButtons? InventoryActionButtons;
    public ContainerActionButtons? ContainerActionButtons;
    public FavoritesManager? FavoritesManager;

    private static readonly string[] SlotTypes =
    [
        "ItemSlotSurvival",
        "ItemSlotBagContent",
        // for overhaullib before 1.22, Quivers And Sheaths and Backpacks mod use this slot type before 1.22
        "ItemSlotBagContentWithWildcardMatch",
        // for https://mods.vintagestory.at/playerinventorylib used by backpacks mod in 1.22+
        "BackpackSlot",
        // for https://mods.vintagestory.at/playerinventorylib without the Backpacks mod
        "VanillaBagContentSlot"
    ];

    private static StorageTweaksClientConfig config = new StorageTweaksClientConfig();

    /// A list of quality foods and tools to exclude from automatic unloading
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly List<string> ToolAndFoodCodes = [];

    private ICoreClientAPI? capi;
    private Harmony? harmony;
    private ICoreServerAPI? sapi;
    private static ILogger? logger;

    // ReSharper disable once MemberCanBePrivate.Global
    public static ILogger Logger() => logger!;

    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return true;
    }

    public override void Start(ICoreAPI api)
    {
        api.Logger.VerboseDebug("[StorageTweaks] Starting StorageTweaksModSystem {0}", api.GetType().Name);
        logger = api.Logger;
    }

    public override void StartPre(ICoreAPI api)
    {
        api.Logger.VerboseDebug("[StorageTweaks] PreStart StorageTweaksModSystem {0}", api.GetType().Name);
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        capi = api;
        capi.Logger.VerboseDebug("[StorageTweaks] Starting StorageTweaksModSystem client side");
        LoadClientConfig(api);
        capi.Logger.VerboseDebug("Loaded client config");
        api.Network.RegisterChannel("storagetweaks")
            .RegisterMessageType<SortInventoryPacket>()
            .RegisterMessageType<UnloadInventoryPacket>()
            .RegisterMessageType<UpdateFavoritesPacket>()
            .RegisterMessageType<QuickStoreNearbyContainersPacket>();
        capi.Logger.VerboseDebug("[StorageTweaks] Registered channels client side");

        FavoritesManager = new FavoritesManager(capi);
        capi.Logger.VerboseDebug("[StorageTweaks] Initialized favorites manager client side");
        InventoryActionButtons = new InventoryActionButtons(capi);
        capi.Logger.VerboseDebug("[StorageTweaks] Initialized inventory action buttons");
        ContainerActionButtons = new ContainerActionButtons(capi);
        capi.Logger.VerboseDebug("[StorageTweaks] Initialized container action buttons");
        harmony = new Harmony("storagetweaks");
        harmony.PatchAll();
        capi.Logger.VerboseDebug("[StorageTweaks] Completed harmony patches");

        RegisterHotkeys(api);

        capi.Logger.VerboseDebug("[StorageTweaks] Started StorageTweaksModSystem client side");
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        sapi = api;
        sapi.Logger.VerboseDebug("[StorageTweaks] Starting StorageTweaksModSystem server side");
        api.Network.RegisterChannel("storagetweaks")
            .RegisterMessageType<SortInventoryPacket>()
            .RegisterMessageType<UnloadInventoryPacket>()
            .RegisterMessageType<UpdateFavoritesPacket>()
            .RegisterMessageType<QuickStoreNearbyContainersPacket>()
            .SetMessageHandler<SortInventoryPacket>(SortSystem.HandleSortInventory)
            .SetMessageHandler<UnloadInventoryPacket>(HandleUnloadInventory)
            .SetMessageHandler<UpdateFavoritesPacket>(HandleUpdateFavorites)
            .SetMessageHandler<QuickStoreNearbyContainersPacket>(QuickStoreNearbyContainerSystem
                .HandleQuickStoreNearbyContainers);
        sapi.Logger.VerboseDebug("[StorageTweaks] Registered channels server side");

        PopulateToolAndFoodCodes(api);
        sapi.Logger.VerboseDebug("[StorageTweaks] Populated tool and food codes");

        LoadServerConfig(api);
        sapi.Logger.VerboseDebug("[StorageTweaks] Loaded server config");

        api.Event.PlayerJoin += OnPlayerJoin;

        sapi.Logger.VerboseDebug("[StorageTweaks] Starting StorageTweaksModSystem server side");
    }

    /// <summary>
    ///     When a player joins, we check if the "storageTweaksFavorites" attribute is set and if not, set it to a default
    ///     list.
    /// </summary>
    private static void OnPlayerJoin(IServerPlayer player)
    {
        var tree = player.Entity?.WatchedAttributes;
        if (tree == null) return;

        var favoritesAttr = tree.GetTreeAttribute(FavoritesManager.FavoritesKey);
        if (favoritesAttr != null) return;

        favoritesAttr = new TreeAttribute();
        foreach (var code in ToolAndFoodCodes) favoritesAttr.SetBool(code, true);

        tree[FavoritesManager.FavoritesKey] = favoritesAttr;
        tree.MarkPathDirty(FavoritesManager.FavoritesKey);
    }

    private static void PopulateToolAndFoodCodes(ICoreAPI api)
    {
        ToolAndFoodCodes.Clear();

        var keywords = new[]
        {
            "axe",
            "knife",
            "pickaxe",
            "pie",
            "tongs",
            "arrow",
            "bow",
            "bowl",
            "cleaver",
            "cookingpot",
            "crock",
            "falx",
            "hammer",
            "hoe",
            "lantern",
            "saw",
            "scythe",
            "shears",
            "shield",
            "shovel",
            "spear",
            "sword",
            "torch"
        };
        var excludeKeywords = new[]
        {
            "blade", "part", "raw", "stackrandomizer", "toolmold", "-down", "-north", "-east", "-south", "-west"
        };

        foreach (var collectible in api.World.Items.Concat(api.World.Collectibles))
        {
            if (collectible.Code == null) continue;

            var code = collectible.Code.ToString();
            var parts = code.Split(':', '-');

            // Check if any part matches a keyword
            if (!keywords.Any(k => parts.Any(p => string.Equals(p, k, StringComparison.OrdinalIgnoreCase)))) continue;

            // Exclude unwanted items
            if (excludeKeywords.Any(k => code.Contains(k, StringComparison.OrdinalIgnoreCase))) continue;


            ToolAndFoodCodes.Add(code);
        }

        api.World.Logger.Debug("[StorageTweaks] Populated {0} tool and food codes.", ToolAndFoodCodes.Count);
    }

    private static void HandleUpdateFavorites(IServerPlayer fromPlayer, UpdateFavoritesPacket packet)
    {
        var tree = fromPlayer.Entity?.WatchedAttributes;
        if (tree == null) return;

        var favoritesAttr = tree.GetTreeAttribute(FavoritesManager.FavoritesKey);

        if (favoritesAttr == null)
        {
            fromPlayer.Entity?.World.Logger.Error("[StorageTweaks] Favorites attribute not initialized.");
            return;
        }


        if (packet.IsFavorite) favoritesAttr.SetBool(packet.Code, packet.IsFavorite);
        else favoritesAttr.RemoveAttribute(packet.Code);

        tree.MarkPathDirty(FavoritesManager.FavoritesKey);
    }

    private static void HandleUnloadInventory(IServerPlayer fromPlayer, UnloadInventoryPacket packet)
    {
        // should probably add checks if the player is allowed to access the inventory

        var destInventory = fromPlayer.InventoryManager.GetInventory(packet.InventoryId);
        if (destInventory == null)
        {
            Logger().Debug(
                "[StorageTweaks] HandleUnloadInventory: Destination inventory not found");
            return;
        }

        UnloadInventory(fromPlayer, destInventory, packet.StackPerishables);
    }

    public static void UnloadInventory(IServerPlayer fromPlayer, IInventory destInventory,
        bool stackPerishables = false)
    {
        var playerInv = fromPlayer.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName);
        if (playerInv == null)
        {
            Logger().Debug(
                "[StorageTweaks] HandleUnloadInventory: Player backpack inventory not found");
            return;
        }

        var playerHotbar = fromPlayer.InventoryManager.GetOwnInventory(GlobalConstants.hotBarInvClassName);
        if (playerHotbar == null)
        {
            Logger().Debug(
                "[StorageTweaks] HandleUnloadInventory: Player hotbar inventory not found");
            return;
        }

        // list of item codes that are already in the destination inventory
        var existingCodes = new HashSet<string>();
        foreach (var destSlot in destInventory)
        {
            if (destSlot.Empty) continue;
            if (FavoritesManager.IsFavorite(fromPlayer, destSlot.Itemstack)) continue;

            existingCodes.Add(destSlot.Itemstack.Collectible.Code.ToString());
        }

        if (existingCodes.Count == 0) return;

        ProcessInventorySlots(playerInv, destInventory, existingCodes, fromPlayer, stackPerishables);
        ProcessInventorySlots(playerHotbar, destInventory, existingCodes, fromPlayer, stackPerishables);
    }

    private static void ProcessInventorySlots(IInventory sourceInventory, IInventory destInventory,
        HashSet<string> existingCodes, IServerPlayer fromPlayer, bool stackPerishables)
    {
        List<ItemSlot> ignoredSlots = [];
        foreach (var slot in sourceInventory)
        {
            if (slot.Empty) continue;
            if (!existingCodes.Contains(slot.Itemstack.Collectible.Code.ToString())) continue;
            if (IsExcludedSlot(slot)) continue;

            ignoredSlots.Clear();
            var world = fromPlayer.Entity.World;
            // DirectMerge blends transition state so differently-perished food stacks;
            // AutoMerge (vanilla) refuses to merge stacks with mismatched perish progress.
            var mergePriority = stackPerishables ? EnumMergePriority.DirectMerge : EnumMergePriority.AutoMerge;
            while (true)
            {
                var op = new ItemStackMoveOperation(world, EnumMouseButton.Left, 0, mergePriority,
                    slot.StackSize);
                var suitedSlot = destInventory.GetBestSuitedSlot(slot, op, ignoredSlots);
                if (suitedSlot.slot == null || suitedSlot.weight == 0) break;

                slot.TryPutInto(suitedSlot.slot, ref op);
                if (slot.Empty) break;

                ignoredSlots.Add(suitedSlot.slot);
            }
        }
    }

    public static bool IsExcludedSlot(ItemSlot slot)
    {
        return !SlotTypes.Contains(slot.GetType().Name);
    }

    private static void LoadServerConfig(ICoreServerAPI api)
    {
        StorageTweaksServerConfig? serverConfig;
        try
        {
            serverConfig = api.LoadModConfig<StorageTweaksServerConfig>("storagetweaks-server.json");
            if (serverConfig == null)
            {
                serverConfig = new StorageTweaksServerConfig();
                api.StoreModConfig(serverConfig, "storagetweaks-server.json");
            }
        }
        catch (Exception)
        {
            serverConfig = new StorageTweaksServerConfig();
            api.StoreModConfig(serverConfig, "storagetweaks-server.json");
        }

        QuickStoreNearbyContainerSystem.Initialize(serverConfig.QuickStoreContainers);
    }

    private static void LoadClientConfig(ICoreAPI api)
    {
        try
        {
            config = api.LoadModConfig<StorageTweaksClientConfig>("storagetweaks.json");
            if (config != null) return;

            config = new StorageTweaksClientConfig();
            api.StoreModConfig(config, "storagetweaks.json");
        }
        catch (Exception)
        {
            config = new StorageTweaksClientConfig();
            api.StoreModConfig(config, "storagetweaks.json");
        }
    }

    public static StorageTweaksClientConfig GetClientConfig()
    {
        return config;
    }

    private static void RegisterHotkeys(ICoreClientAPI api)
    {
        api.Input.RegisterHotKey("storagetweaks.sort",
            Lang.Get("storagetweaks:hotkey-sort-inventory"),
            GlKeys.A, HotkeyType.InventoryHotkeys, true, true, true);

        api.Input.RegisterHotKey("storagetweaks.sortcontainer",
            Lang.Get("storagetweaks:hotkey-sort-container"),
            GlKeys.B, HotkeyType.InventoryHotkeys, true, true, true);

        api.Input.RegisterHotKey("storagetweaks.storenearby",
            Lang.Get("storagetweaks:hotkey-store-nearby"),
            GlKeys.C, HotkeyType.InventoryHotkeys, true, true, true);

        api.Input.SetHotKeyHandler("storagetweaks.sort", _ =>
        {
            var inv = api.World.Player.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName);
            if (inv == null) return false;

            PatchUtils.SendPacket(api, new SortInventoryPacket
            {
                InventoryId = inv.InventoryID,
                StackPerishables = GetClientConfig().StackPerishables
            });
            return true;
        });

        api.Input.SetHotKeyHandler("storagetweaks.sortcontainer", _ =>
        {
            var stackPerishables = GetClientConfig().StackPerishables;
            var count = 0;
            foreach (var dialog in api.Gui.OpenedGuis)
            {
                var composer = dialog.SingleComposer;
                if (composer?.DialogName == null) continue;
                if (!GuiDialogBlockEntityInventoryPatch.DialogNamePrefixes.Any(prefix =>
                        composer.DialogName.StartsWith(prefix, StringComparison.Ordinal))) continue;

                var inv = PatchUtils.GetInventoryForComposer(composer);

                if (inv == null) continue;

                PatchUtils.SendPacket(api, new SortInventoryPacket
                {
                    InventoryId = inv.InventoryID,
                    StackPerishables = stackPerishables
                });
                count += 1;
            }

            return count > 0;
        });

        api.Input.SetHotKeyHandler("storagetweaks.storenearby", _ =>
        {
            PatchUtils.SendPacket(api, new QuickStoreNearbyContainersPacket
            {
                StackPerishables = GetClientConfig().StackPerishables
            });
            return true;
        });
    }

    public override void Dispose()
    {
        harmony?.UnpatchAll("storagetweaks");
        capi?.StoreModConfig(GetClientConfig(), "storagetweaks.json");
        if (sapi != null) sapi.Event.PlayerJoin -= OnPlayerJoin;
    }
}