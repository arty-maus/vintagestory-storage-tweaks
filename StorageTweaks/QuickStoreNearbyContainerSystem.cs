using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace StorageTweaks;

public static class QuickStoreNearbyContainerSystem
{
    private const int SearchRadius = 8;

    private static HashSet<(string, string)> _quickStackChestTypes = [];

    public static void Initialize(IEnumerable<ContainerEntry> entries)
    {
        _quickStackChestTypes = entries.Select(e => (e.InventoryClass, e.Type)).ToHashSet();
    }

    private static List<BlockEntityContainer> GetNearbyContainers(IWorldAccessor world, BlockPos position,
        int radius)
    {
        var minPos = position - radius;
        var maxPos = position + radius;
        var nearbyContainers = new List<BlockEntityContainer>();
        world.BlockAccessor.WalkBlocks(minPos, maxPos, (_, x, y, z) =>
        {
            var be = world.BlockAccessor.GetBlockEntity(new BlockPos(x, y, z));

            if (be is BlockEntityGenericTypedContainer container)
            {
                if (!_quickStackChestTypes.Contains((container.InventoryClassName, container.type)))
                {
                    world.Logger.Debug(
                        $"[StorageTweaks] Skipped typed container for quick store nearby since it is not in the white list: (\"{container.inventoryClassName}\", \"{container.type}\")");
                    return;
                }

                nearbyContainers.Add(container);
                return;
            }

            if (be is not BlockEntityContainer bc)
            {
                return;
            }
            if (!_quickStackChestTypes.Contains((bc.InventoryClassName,
                    bc.Block.Code.FirstCodePart())))
            {
                world.Logger.Debug(
                    $"[StorageTweaks] Skipped container for quick store nearby since it is not in the white list: (\"{bc.InventoryClassName}\", \"{bc.Block.Code.FirstCodePart()}\"),");
                return;
            }

            nearbyContainers.Add(bc);
        }, true);

        return nearbyContainers;
    }

    public static void HandleQuickStoreNearbyContainers(
        IServerPlayer fromPlayer,
        QuickStoreNearbyContainersPacket packet
    )
    {
        var logger = fromPlayer.Entity.Api.Logger;
        // In 1.22 Entity.Pos changed from a field to a property
        var pos = Util.TryGetFieldOrProperty<EntityPos>(fromPlayer.Entity, "Pos");

        if (pos is null)
        {
            logger.Error("[StorageTweaks] Failed to get entity position from player");
            return;
        }

        var nearbyContainers = GetNearbyContainers(
            fromPlayer.Entity.World,
            pos.AsBlockPos,
            SearchRadius
        );

        foreach (var container in nearbyContainers)
        {
            StorageTweaksModSystem.UnloadInventory(fromPlayer, container.Inventory, packet.StackPerishables);
            if (container.Block.Code.Domain == "purposefulstorage")
            {
                container.MarkDirty(true);
            }
        }
    }
}
