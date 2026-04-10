using System;
using HarmonyLib;
using UnityEngine;

namespace PoiStorageRescue.Patches
{
    [HarmonyPatch(typeof(PrefabInstance), nameof(PrefabInstance.ResetBlocksAndRebuild), new[] { typeof(World), typeof(FastTags<TagGroup.Global>) })]
    internal static class PoiResetPatch
    {
        private static void Prefix(PrefabInstance __instance, World _world)
        {
            RescuePlayerStorage(__instance, _world);
        }

        private static void RescuePlayerStorage(PrefabInstance prefabInstance, World world)
        {
            var rescued = 0;
            foreach (var chunkKey in prefabInstance.GetOccupiedChunks())
            {
                var chunk = world.ChunkClusters[0].GetChunkSync(chunkKey);
                if (chunk == null)
                {
                    continue;
                }

                var tileEntities = chunk.GetTileEntities().list;
                for (var i = tileEntities.Count - 1; i >= 0; i--)
                {
                    var tileEntity = tileEntities[i];
                    var worldPos = tileEntity.ToWorldPos();
                    if (!IsInsideBounds(prefabInstance, worldPos))
                    {
                        continue;
                    }

                    if (!tileEntity.TryGetSelfOrFeature<ITileEntityLootable>(out var lootContainer) || lootContainer == null)
                    {
                        continue;
                    }

                    if (lootContainer.IsEmpty())
                    {
                        continue;
                    }

                    var isOwnedSecureContainer = tileEntity is TileEntitySecureLootContainer secureLootContainer
                        && secureLootContainer.GetOwner() != null;
                    var isOwnedCompositeContainer = tileEntity is TileEntityComposite composite
                        && composite.Owner != null;

                    if (!lootContainer.bPlayerStorage && !isOwnedSecureContainer && !isOwnedCompositeContainer)
                    {
                        continue;
                    }

                    DropLootContainer(world, lootContainer, worldPos);
                    rescued++;
                }
            }

            if (rescued > 0)
            {
                Log.Out($"[PoiStorageRescue] Rescued {rescued} player storage container(s) before POI reset.");
            }
        }

        private static void DropLootContainer(World world, ITileEntityLootable lootContainer, Vector3i worldPos)
        {
            if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            {
                GameManager.Instance.DropContentOfLootContainerServer(lootContainer.blockValue, worldPos, lootContainer.EntityId, lootContainer);
                return;
            }

            var droppedEntityClass = "DroppedLootContainer";
            var spawnPos = worldPos.ToVector3() + new Vector3(0.5f, 0.75f, 0.5f);
            if (lootContainer.blockValue.Block.Properties.Values.ContainsKey("DroppedEntityClass"))
            {
                droppedEntityClass = lootContainer.blockValue.Block.Properties.Values["DroppedEntityClass"];
            }

            if (!lootContainer.bTouched)
            {
                GameManager.Instance.lootManager.LootContainerOpened(lootContainer, -1, lootContainer.blockValue.Block.Tags);
            }

            if (!lootContainer.IsEmpty())
            {
                var entityLootContainer = EntityFactory.CreateEntity(droppedEntityClass.GetHashCode(), spawnPos, Vector3.zero) as EntityLootContainer;
                if (entityLootContainer != null)
                {
                    entityLootContainer.SetContent(ItemStack.Clone(lootContainer.items));
                    world.SpawnEntityInWorld(entityLootContainer);
                }
            }

            lootContainer.SetEmpty();
        }

        private static bool IsInsideBounds(PrefabInstance prefabInstance, Vector3i pos)
        {
            var min = prefabInstance.boundingBoxPosition;
            var max = prefabInstance.boundingBoxPosition + prefabInstance.boundingBoxSize;

            return pos.x >= min.x
                && pos.y >= min.y
                && pos.z >= min.z
                && pos.x < max.x
                && pos.y < max.y
                && pos.z < max.z;
        }
    }
}
