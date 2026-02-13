using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

// Original code from Collective Rewiring by pseudopulse; rewritten by .score
namespace SnowtimeToybox.Components
{
    [RequireComponent(typeof(CharacterMaster))]
    public class FriendlyTurretInheritance : MonoBehaviour
    {
        private int[] previouslyProvidedItemBuffer;
        public Inventory ownerInventory;
        public CharacterMaster self;
        public string whitelistedTag;

        public void Awake()
        {
            enabled = NetworkServer.active;
            self = GetComponent<CharacterMaster>();
            previouslyProvidedItemBuffer = ItemCatalog.PerItemBufferPool.Request<int>();
        }

        public void FixedUpdate()
        {
            if (!ownerInventory)
            {
                if (self.minionOwnership && self.minionOwnership.ownerMaster)
                {
                    ownerInventory = self.minionOwnership.ownerMaster.inventory;
                    ownerInventory.onInventoryChanged += FriendlyTurretMirrorInventory;
                    FriendlyTurretMirrorInventory();
                }
            }
            if (self.inventory.GetItemCountPermanent(RoR2Content.Items.MinionLeash) > 0) return;
            self.inventory.GiveItemPermanent(RoR2Content.Items.MinionLeash);
        }

        public void OnDestroy()
        {
            if (!ReferenceEquals(ownerInventory, null))
            {
                ownerInventory.onInventoryChanged -= FriendlyTurretMirrorInventory;
            }
        }

        public void FriendlyTurretMirrorInventory()
        {
            if (!self?.inventory || !ownerInventory)
                return;

            //Log.Debug("starting channel");
            ChannelOwnerInventory();

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

        public void ChannelOwnerInventory()
        {
            int combinedItemsCount = 0;
            using var _1 = ItemCatalog.PerItemBufferPool.RequestTemp<ItemIndex>(out var combinedItemsAcquired);
            using var _2 = ItemCatalog.PerItemBufferPool.RequestTemp<bool>(out var indicesToUpdateSet);
            using var _3 = new Inventory.InventoryChangeScope(self.inventory);
            //Log.Debug("Running ChannelOwnerInventory");

            var selfIndicies = self.inventory.permanentItemStacks.GetNonZeroIndicesSpan();
            for (int i = 0; i < selfIndicies.Length; i++)
            {
                //Log.Debug("selfindicies " + selfIndicies[i]);
                AddItem(selfIndicies[i]);
            }

            var ownerIndicies = ownerInventory.permanentItemStacks.GetNonZeroIndicesSpan();
            for (int i = 0; i < ownerIndicies.Length; i++)
            {
                //Log.Debug("ownerindicies " + ownerIndicies[i]);
                AddItem(ownerIndicies[i]);
            }

            var itemsAcquiredSpan = combinedItemsAcquired.AsSpan(0, combinedItemsCount);
            for (int i = 0; i < itemsAcquiredSpan.Length; i++)
            {
                var itemIndex = itemsAcquiredSpan[i];
                //Log.Debug("for iteration " + i + " : item index " + itemIndex);

                ref int providedCount = ref previouslyProvidedItemBuffer[(int)itemIndex];
                //Log.Debug("providedCount " + providedCount);

                int targetProvidedCount = ItemFilter(itemIndex) ? ownerInventory.GetItemCountPermanent(itemIndex) : 0;
                //Log.Debug("targetProvidedCount " + targetProvidedCount);

                int itemCountToProvide = targetProvidedCount - providedCount;
                if (itemCountToProvide != 0)
                {
                    //Log.Debug("Giving " + itemCountToProvide + " of " + ItemCatalog.GetItemDef(itemIndex).nameToken);

                    self.inventory.ChangeItemStacksCount(new Inventory.GiveItemPermanentImpl
                    {
                        inventory = self.inventory
                    }, itemIndex, itemCountToProvide);

                    providedCount = targetProvidedCount;
                    //Log.Debug("Provided a total of " + providedCount + " of item " + ItemCatalog.GetItemDef(itemIndex).nameToken);
                }
            }
            void AddItem(ItemIndex idx)
            {
                //Log.Debug("AddItem");
                if (!indicesToUpdateSet[(int)idx])
                {
                    indicesToUpdateSet[(int)idx] = true;
                    combinedItemsAcquired[combinedItemsCount++] = idx;
                }
            }
        }
        public bool ItemFilter(ItemIndex index)
        {
            //Log.Debug("Running ItemFilter");
            ItemDef item = ItemCatalog.GetItemDef(index);
            //Log.Debug("Item Token: " + item.nameToken);
            if (item.tier == ItemTier.NoTier) return false;
            ItemTag specificTag = (ItemAPI.FindItemTagByName(whitelistedTag));
            ItemTag globalTag = ItemAPI.FindItemTagByName("GlobalFriendTurret_Whitelist");
            Log.Debug("Item Token: " + item.nameToken + " Has Whitelisted Tag: " + item.ContainsTag(ItemAPI.FindItemTagByName(whitelistedTag)));
            Log.Debug("specificTag: " + specificTag + " globalTag: " + globalTag);
            if (item.ContainsTag(specificTag) || item.ContainsTag(globalTag))
            {
                //Log.Debug("Item Token: " + item.nameToken + " passed check for Shortcake whitelist");
                return true;
            }

            return false;
        }
    }
}