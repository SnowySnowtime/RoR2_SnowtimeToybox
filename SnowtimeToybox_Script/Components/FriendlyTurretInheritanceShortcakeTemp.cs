using EntityStates.AffixVoid;
using R2API;
using RoR2;
using RoR2.EntityLogic;
using SnowtimeToybox;
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
    public class FriendlyTurretInheritanceShortcake : MonoBehaviour
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

        private void FriendlyTurretMirrorInventory()
        {
            List<ItemInfo> currentState = BuildInventoryState(self.inventory);
            EquipmentIndex currentEquipIndex = self.inventory.currentEquipmentIndex;
            List<ItemInfo> ownerState = BuildInventoryState(ownerInventory);
            EquipmentIndex ownerEquipIndex = self.minionOwnership.ownerMaster.inventory.currentEquipmentIndex;
            //Log.Debug("Inventory Updated...");
            self.inventory.CopyItemsFrom(ownerInventory, ItemFilter);

            foreach (ItemInfo info in currentState)
            {
                if (self.inventory.GetItemCountPermanent(info.index) < info.count)
                {
                    if (!(LastOwnerInventoryState.Where(x => x.index == info.index).Count() >= info.count && ownerState.Where(x => x.index == info.index).Count() < info.count))
                    {
                        self.inventory.RemoveItemPermanent(info.index, self.inventory.GetItemCountPermanent(info.index));
                        self.inventory.GiveItemPermanent(info.index, info.count);
                    }
                }
            }

            if (currentEquipIndex != ownerEquipIndex)
            {
                //Log.Debug("Checking if Equipment is an Elite Equipment");
                //Log.Debug(SnowtimeToyboxMod.eliteDefsEquipInherit.Contains(ownerEquipIndex));
                //Log.Debug("Owner Equipment Index: " + ownerEquipIndex);
                if (!SnowtimeToyboxMod.eliteDefsEquipInherit.Contains(ownerEquipIndex))
                {
                    //Log.Debug("Check Failed, checking if we have equipment to begin with.");
                    if (currentEquipIndex == EquipmentIndex.None) return;
                    //Log.Debug("We had an equipment prior, removing equipment.");
                    self.inventory.SetEquipmentIndex(EquipmentIndex.None, true);
                }
                else
                {
                    //Log.Debug("Check Passed.");
                    self.inventory.SetEquipmentIndex(ownerEquipIndex, false);
                }
            }

            LastOwnerInventoryState = ownerState;
        }

        private List<ItemInfo> BuildInventoryState(Inventory source)
        {
            List<ItemInfo> list = new();

            foreach (ItemIndex index in source.itemAcquisitionOrder)
            {
                list.Add(new()
                {
                    index = index,  
                    count = source.GetItemCountPermanent(index)
                });
            }

            return list;
        }

        private bool ItemFilter(ItemIndex index)
        {
            ItemDef item = ItemCatalog.GetItemDef(index);
            if (item.tier == ItemTier.NoTier) return false;
            //Log.Debug( "Item Token: " + item.nameToken + " Has Shortcake Tag: " + item.ContainsTag(ItemAPI.FindItemTagByName("turretShortcakeWhitelist")) );
            if (item.ContainsTag(ItemAPI.FindItemTagByName("turretShortcakeWhitelist")))
            {
                //Log.Debug("Item Token: " + item.nameToken + " passed check for Shortcake whitelist");
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