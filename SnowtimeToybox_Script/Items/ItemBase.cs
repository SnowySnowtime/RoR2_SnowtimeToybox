using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SnowtimeToybox.Items
{

    // The directly below is entirely from TILER2 API (by ThinkInvis) specifically the Item module. Utilized to implement instancing for classes.
    // TILER2 API can be found at the following places:
    // https://github.com/ThinkInvis/RoR2-TILER2
    // https://thunderstore.io/package/ThinkInvis/TILER2/

    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
    {
        //This, which you will see on all the -base classes, will allow both you and other modders to enter through any class with this to access internal fields/properties/etc as if they were a member inheriting this -Base too from this class.
        public static T instance { get; private set; }

        public ItemBase()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBase was instantiated twice");
            instance = this as T;
        }
    }
    public abstract class ItemBase
    {

        public abstract ItemDef ItemDef { get; }
        public bool AIBlacklisted { get; set; }
        public string ItemName;

        /// <summary>
        /// This method structures your code execution of this class. An example implementation inside of it would be:
        /// <para>CreateConfig(config);</para>
        /// <para>CreateLang();</para>
        /// <para>CreateItem();</para>
        /// <para>Hooks();</para>
        /// <para>This ensures that these execute in this order, one after another, and is useful for having things available to be used in later methods.</para>
        /// <para>P.S. CreateItemDisplayRules(); does not have to be called in this, as it already gets called in CreateItem();</para>
        /// </summary>
        /// <param name="config">The config file that will be passed into this from the main class.</param>
        public abstract void Init(ConfigFile config);

        public virtual void CreateConfig(ConfigFile config) { }

        protected virtual void CreateLang()
        {
            // LanguageAPI.Add(ItemDef.nameToken, "bwa name");
            // LanguageAPI.Add(ItemDef.pickupToken, "bwah pickup");
            // LanguageAPI.Add(ItemDef.descriptionToken, "bwa desc");
            // LanguageAPI.Add(ItemDef.loreToken, "bwa lore");
        }

        public abstract ItemDisplayRuleDict CreateItemDisplayRules();
        protected void CreateItem()
        {
            // ignore obsolete, didnt want to have to fix it.
            if (!ItemDef.pickupModelPrefab)
                ItemDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Mystery.PickupMystery_prefab).WaitForCompletion();

            CreateLang(); //just for autogeneration of lang file wiht https://thunderstore.io/c/riskofrain2/p/TheTimesweeper/LanguageFileGenerator/ older build with this could also work thouvh ,.,.

            // not sure how to replicate this
            //if (Language.GetString(ItemDef.loreToken) == "")
            //{
            //    ItemDef.loreToken = ItemHelpers.OrderManifestLoreFormatter(ItemName, "sunfall !!", "sunfall hq !!!!!!", "super sunfall !!!", Language.GetString(ItemDef.pickupToken), "sunfall ues awesome !!", "sunfall epic !!");
            //}

            ContentAddition.AddItemDef(ItemDef);
            if (AIBlacklisted && !ItemDef.tags.Contains(ItemTag.AIBlacklist))
            {
                ItemDef.tags = ItemDef.tags.Append(ItemTag.AIBlacklist).ToArray();
            }
        }

        public void RegItemName()
        {
            if (ItemDef != null)
                ItemName = ItemDef.name;
        }

        public virtual void Hooks() { }

        //Based on ThinkInvis' methods
        public int GetCount(CharacterBody body)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCountEffective(ItemDef);
        }

        public int GetCount(CharacterMaster master)
        {
            if (!master || !master.inventory) { return 0; }

            return master.inventory.GetItemCountEffective(ItemDef);
        }

        public bool HasItem(CharacterBody body)
        {
            if (!body || !body.inventory) { return false; }

            return body.inventory.GetItemCountEffective(ItemDef) > 0;
        }

        public bool HasItem(CharacterMaster master)
        {
            if (!master || !master.inventory) { return false; }

            return master.inventory.GetItemCountEffective(ItemDef) > 0;
        }

        public int GetCountSpecific(CharacterBody body, ItemDef itemDef)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCountEffective(itemDef);
        }
    }
}
