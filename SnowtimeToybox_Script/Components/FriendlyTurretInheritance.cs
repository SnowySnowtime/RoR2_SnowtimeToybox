using EntityStates.AffixVoid;
using R2API;
using RoR2;
using RoR2.EntityLogic;
using SnowtimeToybox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.TextCore.LowLevel;

// Code from Collective Rewiring by pseudopulse
// TODO: Add a few configuration settings so that each turret can use this singular component
// Also handle equipment, somehow.
namespace SnowtimeToybox.Components
{
    public class FriendlyTurretInheritance : MonoBehaviour
    {
        public Inventory ownerInventory;
        public CharacterMaster self;
        public string turretType;
        private List<ItemInfo> LastOwnerInventoryState = new();
        public string turret;

        public void Start()
        {
            self = GetComponent<CharacterMaster>();
            turret = self.GetBody().baseNameToken;
            FriendlyTurretMirrorInventory();
        }
        public void Awake()
        {
            enabled = NetworkServer.active;
        }

        public void FixedUpdate()
        {
            if (self.inventory.GetItemCountPermanent(RoR2Content.Items.MinionLeash) > 0) return;
            self.inventory.GiveItemPermanent(RoR2Content.Items.MinionLeash);
        }
        public void FriendlyTurretMirrorInventory()
        {
            if (self?.inventory == null)
                return;

            var ownerInventory = self.minionOwnership?.ownerMaster?.inventory;
            if (ownerInventory == null)
                return;

            ChannelOwnerInventory(self, ownerInventory);

            EquipmentIndex ownerEquipment = ownerInventory.currentEquipmentIndex;
            if (!SnowtimeToyboxMod.eliteDefsEquipInherit.Contains(ownerEquipment))
            {
                ownerEquipment = EquipmentIndex.None;
            }

            if (self.inventory.currentEquipmentIndex != ownerEquipment)
            {
                self.inventory.SetEquipmentIndex(ownerEquipment, isRemovingEquipment: true);
            }
        }

        public void ChannelOwnerInventory(CharacterMaster self, Inventory ownerInventory)
        {
            int combinedItemsCount = 0;
            using var _1 = ItemCatalog.PerItemBufferPool.RequestTemp<ItemIndex>(out var combinedItemsAcquired);
            using var _2 = ItemCatalog.PerItemBufferPool.RequestTemp<bool>(out var indicesToUpdateSet);
            using var _3 = new Inventory.InventoryChangeScope(self.inventory);

            var selfIndicies = self.inventory.channeledItemStacks.GetNonZeroIndicesSpan();
            for (int i = 0; i < selfIndicies.Length; i++)
            {
                AddItem(selfIndicies[i]);
            }

            var ownerIndicies = self.inventory.permanentItemStacks.GetNonZeroIndicesSpan();
            for (int i = 0; i < ownerIndicies.Length; i++)
            {
                AddItem(ownerIndicies[i]);
            }

            var itemsAcquiredSpan = combinedItemsAcquired.AsSpan(0, combinedItemsCount);
            for (int i = 0; i < itemsAcquiredSpan.Length; i++)
            {
                var itemIndex = itemsAcquiredSpan[i];

                // this is your running count of given items
                // but i cant add an instance field, so im not bothering with covering this edgecase
                // 
                // private int[] previouslyProvidedItemBuffer = ItemCatalog.PerItemBufferPool.Request<int>();
                // ...
                int providedCount = self.inventory.GetItemCountChanneled(itemIndex);
                // ref int providedCount = ref previouslyProvidedItemBuffer[(int)itemIndex];

                int targetProvidedCount = ItemFilter(itemIndex) ? ownerInventory.GetItemCountPermanent(itemIndex) : 0;
                int itemCountToProvide = targetProvidedCount - providedCount;

                // for whatever reason, GiveItemChanneledImpl has an immutable property to allow negative item stacks.
                // fuck that. clamp above zero.
                int actualItemCount = self.inventory.GetItemCountChanneled(itemIndex);
                itemCountToProvide = Math.Max(0, actualItemCount + itemCountToProvide) - actualItemCount;

                if (itemCountToProvide != 0)
                {
                    self.inventory.ChangeItemStacksCount(new Inventory.GiveItemChanneledImpl
                    {
                        inventory = self.inventory
                    }, itemIndex, itemCountToProvide);
                    providedCount = targetProvidedCount;
                }
            }

            void AddItem(ItemIndex itemIndex)
            {
                if (!indicesToUpdateSet[(int)itemIndex])
                {
                    indicesToUpdateSet[(int)itemIndex] = true;
                    combinedItemsAcquired[combinedItemsCount++] = itemIndex;
                }
            }
        }

        private bool ItemFilter(ItemIndex index)
        {
            ItemDef item = ItemCatalog.GetItemDef(index);
            if (item.tier == ItemTier.NoTier) return false;
            if (item.ContainsTag(ItemAPI.FindItemTagByName("turretShortcakeWhitelist")))
            {
                return true;
            }

            return false;
        }

    private class ItemInfo
        {
            public ItemIndex index;
            public int count;
        }
    }
}