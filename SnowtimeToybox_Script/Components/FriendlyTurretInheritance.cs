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

// Code from Collective Rewiring by pseudopulse
// TODO: Add a few configuration settings so that each turret can use this singular component
// Also handle equipment, somehow.
namespace SnowtimeToybox.Components
{
    public class FriendlyTurretInheritance : MonoBehaviour
    {
        public Inventory ownerInventory;
        public EquipmentSlot ownerEquipment;
        public CharacterMaster self;
        private List<ItemInfo> LastOwnerInventoryState = new();
        private EquipmentSlot LastOwnerEquipmentState = new();
        public string turret;
        EquipmentDef[] friendlyTurretEquipmentWhitelist =
                [
                // Base
                RoR2Content.Elites.Fire.eliteEquipmentDef,
                RoR2Content.Elites.Lightning.eliteEquipmentDef,
                RoR2Content.Elites.Ice.eliteEquipmentDef,
                RoR2Content.Elites.Haunted.eliteEquipmentDef,
                RoR2Content.Elites.Poison.eliteEquipmentDef,
                RoR2Content.Elites.Lunar.eliteEquipmentDef,
                // DLC1
                DLC1Content.Elites.Void.eliteEquipmentDef,
                DLC1Content.Elites.Earth.eliteEquipmentDef,
                // DLC2
                DLC2Content.Elites.Bead.eliteEquipmentDef,
                DLC2Content.Elites.Aurelionite.eliteEquipmentDef,
                // DLC3
                DLC3Content.Elites.Collective.eliteEquipmentDef,
                ];

        public void Start()
        {
            self = GetComponent<CharacterMaster>();
            turret = self.GetBody().baseNameToken;
        }

        public void FixedUpdate()
        {
            if (!ownerEquipment)
            {
                if (self.minionOwnership && self.minionOwnership.ownerMaster)
                {
                    ownerEquipment = self.minionOwnership.ownerMaster.GetBody().equipmentSlot;
                }
            }
            if (!ownerInventory)
            {
                if (self.minionOwnership && self.minionOwnership.ownerMaster)
                {
                    ownerInventory = self.minionOwnership.ownerMaster.inventory;
                    ownerInventory.onInventoryChanged += FriendlyTurretMirrorInventory;
                    FriendlyTurretMirrorInventory();
                }
            }
        }

        private void FriendlyTurretMirrorInventory()
        {
            List<ItemInfo> currentState = BuildInventoryState(self.inventory);
            List<ItemInfo> ownerState = BuildInventoryState(ownerInventory);
            EquipmentSlot currentEquipState = (self.GetBody().equipmentSlot);
            EquipmentSlot ownerEquipState = ownerEquipment;
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

            LastOwnerInventoryState = ownerState;
            LastOwnerEquipmentState = ownerEquipState;
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
            if (turret == "FRIENDLYTURRET_SHORTCAKE_NAME")
            {
                if (item.ContainsTag(ItemAPI.FindItemTagByName("turretShortcakeWhitelist")))
                {
                    return true;
                }
            }
            else if (turret == "FRIENDLYTURRET_BORBO_NAME")
            {
                if (item.ContainsTag(ItemAPI.FindItemTagByName("turretBorboWhitelist")))
                {
                    return true;
                }
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