using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.SnowtimeToybox_FireHaloWeapon;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using On.RoR2.CharacterAI;
using On.RoR2.UI;
using R2API;
using Rewired.ComponentControls.Data;
using RoR2;
using RoR2.Skills;
using RoR2BepInExPack.GameAssetPaths;
using ShaderSwapper;
using SnowtimeToybox.Buffs;
using SnowtimeToybox.Components;
using SnowtimeToybox.FriendlyTurretChecks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityHotReloadNS;
using Path = System.IO.Path;
using SceneDirector = On.RoR2.SceneDirector;
using Ror2AggroTools;
using SnowtimeToybox.FriendlyTurrets;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace SnowtimeToybox
{
    // Dependencies and BepInPlugin initialization
    [BepInPlugin(GUID, Name, Version)]
    [BepInDependency(R2API.DirectorAPI.PluginGUID)]
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency(R2API.RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2API.DamageAPI.PluginGUID)]
    [BepInDependency(R2API.DifficultyAPI.PluginGUID)]
    [BepInDependency("com.RiskOfBrainrot.RiskierRain", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskOfBrainrot.SwanSongExtended", BepInDependency.DependencyFlags.SoftDependency)]
    public class SnowtimeToyboxMod : BaseUnityPlugin
    {
        public const string Author = "SnowySnowtime";
        public const string Name = nameof(SnowtimeToyboxMod);
        public const string Version = "1.2.3";
        public const string GUID = Author + "." + Name;

        public static SnowtimeToyboxMod instance;

        public static DifficultyDef SnowtimeLegendaryDiffDef;
        public static DifficultyIndex SnowtimeLegendaryDiffIndex;
        public static SkillDef SnowtimePlasmaRifleSkillDef;
        public static GameObject MuzzleFlashObject;
        public static GameObject TracerObject;
        public static GameObject HitObject;
        public static GameObject OrbObject;

        public static CharacterBody.BodyFlags bodyFlags;
        
        // KEEP YOURSELF SAFE
        public static DamageAPI.ModdedDamageType HaloRicochetOnHit = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType BorboSuperDebuffOnHit = DamageAPI.ReserveDamageType();
        
        // turretlings
        public static DroneDef FriendlyTurretTurretlingDef;
        public static InteractableSpawnCard FriendlyTurretTurretlingIsc;
        public static SkillFamily FriendlyTurretTurretlingPrimarySkillFamily;
        public static SkillFamily FriendlyTurretTurretlingSecondarySkillFamily;
        public static SkillFamily FriendlyTurretTurretlingUtilSkillFamily;
        public static SkillDef FriendlyTurretTurretlingPrimarySkillDef;
        public static SkillDef FriendlyTurretTurretlingSecondarySkillDef;
        public static SkillDef FriendlyTurretTurretlingUtilSkillDef;
        public static GameObject FriendlyTurretTurretlingBody;
        public static GameObject FriendlyTurretTurretlingMaster;
        public static GameObject FriendlyTurretTurretlingBroken;
        // turretling variants
        public static GameObject AcanthiTurretlingBody;
        public static GameObject AcanthiTurretlingMaster;
        public static GameObject BorboTurretlingBody;
        public static GameObject BorboTurretlingMaster;
        public static GameObject BreadTurretlingBody;
        public static GameObject BreadTurretlingMaster;
        public static GameObject ShortcakeTurretlingBody;
        public static GameObject ShortcakeTurretlingMaster;
        public static GameObject SnowtimeTurretlingBody;
        public static GameObject SnowtimeTurretlingMaster;
        
        //public static DroneDef FriendlyTurretTestDroneDef;

        public static List<FriendlyTurretBase> friendlyTurretList = [];

        public static bool Legendary = false;
        // Copied from RiskierRain, sorry borbo :(
        public static bool ModLoaded(string modGuid) { return !string.IsNullOrEmpty(modGuid) && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(modGuid); }
        public static bool riskierLoaded => ModLoaded("com.RiskOfBrainrot.RiskierRain");

        public static String assetDirectory;
        public static AssetBundle _stdifficultyAssetBundle;
        public static AssetBundle _stcharacterAssetBundle;
        internal const string _stdifficultyAssetBundleName = "snowtimetoybox_difficulty";
        internal const string _stcharacterAssetBundleName = "snowtimetoybox_characters";

        public static ConfigEntry<bool> ToggleSpawnMessages { get; set; }
        public static ConfigEntry<bool> FriendlyTurretImmuneVoidDeath { get; set; }
        public static ConfigEntry<bool> TurretlingImmuneVoidDeath { get; set; }
        public static ConfigEntry<bool> FriendlyTurretFallImmunity { get; set; }
        public static ConfigEntry<bool> FriendlyTurretDrone { get; set; }
        public static ConfigEntry<bool> FriendlyTurretShortcakeAggroType { get; set; }
        public static ConfigEntry<float> TurretlingSpawnChance { get; set; }
        public static ConfigEntry<float> TurretlingRainbowChance { get; set; }
        public static ConfigEntry<string> TurretlingRainbowBonusItems { get; set; }
        public static ConfigEntry<bool> TurretlingKillOriginalTurrets { get; set; }
        public static ConfigEntry<float> TurretlingReviveCostMult { get; set; }
        public static ConfigEntry<float> TurretlingBaseDamage { get; set; }
        public static ConfigEntry<float> TurretlingBaseDamagePerLevel { get; set; }

        public void Awake()
        {
            instance = this;

            Log.Init(Logger);

            ToggleSpawnMessages = Config.Bind("Friendly Turret Functions", "Spawn Message", true, "If true, the Friendly Turrets will give a message on every stage they spawn on, for insight on if and which turret spawned. Otherwise, friendly turrets are shy, and are also sad!");
            FriendlyTurretShortcakeAggroType = Config.Bind("Friendly Turret Functions", "Strawberry Shortcake Aggro Method", false, "If true, the Strawberry Shortcake Turret will spawn with a native increase to its aggro. Else, it only gains aggro for ~0.5s when its main skill fires.");
            FriendlyTurretImmuneVoidDeath = Config.Bind("Friendly Turret Flags", "Void Death Immunity", true, "If true, Friendly Turrets are immune to Void Death (Void Reaver implosions), this is because they are awful at avoiding them even with mods to make allies avoid them, and we get sad when they are detained.");
            FriendlyTurretFallImmunity = Config.Bind("Friendly Turret Flags", "Fall Damage Immunity", true, "If true, Friendly Turrets (and turretlings) are immune to fall damage, as navigating some maps can be a little difficult for them. Prevents any unexpected turret deaths, as we cant simply 'replace' them like Engineer can.");
            FriendlyTurretDrone = Config.Bind("Friendly Turret Flags", "Drone", false, "If true, Friendly Turrets (and turretlings) are flagged as drones. Probably comes with some oddities.");
            TurretlingSpawnChance = Config.Bind("Turretlings", "Turretling Variant Spawn Chance ,,.", 100f, "chance to get a turretling when buying a friendly turret !!!");
            TurretlingImmuneVoidDeath = Config.Bind("Turretlings", "Void Death Immunity", false, "If true, All turretlings are immune to Void Death (Void Reaver implosions). Keep the scrunglies safe.");
            TurretlingReviveCostMult = Config.Bind("Turretlings", "turretling revive cost mult .,.", 0.6f, "price multiplier for reviving turretlings ,.. ,.");
            TurretlingKillOriginalTurrets = Config.Bind("Turretlings", "kill original turrets .,,.", false, "kills normal(gunner) turrets and replaces them with turretlings ,. ,.");
            TurretlingRainbowChance = Config.Bind("Turretlings", "turretling rainbow chance ,,.", 1f, "% chance to get a powerful and prideful rainbow turretling ,.,.");
            TurretlingRainbowBonusItems = Config.Bind("Turretlings", "turretling rainbow bonus items ,,.", "syringe,50,alienhead,5,extralife,1,moremissile,1,adaptivearmor,1,powercube,1,shockdamageaura,1", "give rainbow turretlings bonus items !!! follows (internalitemname),(count)");
            TurretlingBaseDamage = Config.Bind("Turretling Stats", "Base Damage", 12f, "Damage the turretling deals. Blaster deals 100%(1x) base damage, Pixi Launcher deal 200%(2x) base damage. Does not affect Turretling variants.");
            TurretlingBaseDamagePerLevel = Config.Bind("Turretling Stats", "Base Damage Per Level", 3f, "Base Damage increase per level. Does not affect Turretling variants.");
            Language.collectLanguageRootFolders += CollectLanguageRootFolders;

            Hooks.Hook();

            instance = this;

            var assetsFolderFullPath = Path.Combine(Path.GetDirectoryName(typeof(SnowtimeToyboxMod).Assembly.Location), "assetbundles");
            assetDirectory = assetsFolderFullPath;
            Debug.Log("Ran Start!");
            _stcharacterAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetsFolderFullPath, _stcharacterAssetBundleName));
            base.StartCoroutine(_stcharacterAssetBundle.UpgradeStubbedShadersAsync());
            _stdifficultyAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetsFolderFullPath, _stdifficultyAssetBundleName));
            Debug.Log(_stcharacterAssetBundle);
            Debug.Log(_stdifficultyAssetBundle);

            AddDifficulty();
            AddCustomSkills();
            AddCustomAllies();
            AddCustomBuffs();

            ItemTag FriendTurret_Borbo_Whitelist = ItemAPI.AddItemTag("FriendTurret_Borbo_Whitelist");
            Log.Debug("FriendTurret_Borbo_Whitelist: " + FriendTurret_Borbo_Whitelist);

            ItemTag FriendTurret_Shortcake_Whitelist = ItemAPI.AddItemTag("FriendTurret_Shortcake_Whitelist");
            Log.Debug("FriendTurret_Shortcake_Whitelist: " + FriendTurret_Shortcake_Whitelist);

            ItemTag FriendTurret_Snowtime_Whitelist = ItemAPI.AddItemTag("FriendTurret_Snowtime_Whitelist");
            Log.Debug("FriendTurret_Snowtime_Whitelist: " + FriendTurret_Snowtime_Whitelist);

            ItemTag FriendTurret_Acanthi_Whitelist = ItemAPI.AddItemTag("FriendTurret_Acanthi_Whitelist");
            Log.Debug("FriendTurret_Acanthi_Whitelist: " + FriendTurret_Acanthi_Whitelist);

            ItemTag FriendTurret_Bread_Whitelist = ItemAPI.AddItemTag("FriendTurret_Bread_Whitelist");
            Log.Debug("FriendTurret_Acanthi_Whitelist: " + FriendTurret_Bread_Whitelist);

            ItemTag globalWhitelist = ItemAPI.AddItemTag("GlobalFriendTurret_Whitelist");
            Log.Debug("GlobalFriendTurret_Whitelist: " + globalWhitelist);

            ItemTag TurretlingNoneWhitelist = ItemAPI.AddItemTag("FriendTurret_None_Whitelist");
            Log.Debug("None FriendTurret_Whitelist: " + TurretlingNoneWhitelist);

            ItemCatalog.availability.CallWhenAvailable(AddCustomTagsToItems);
            EquipmentCatalog.availability.CallWhenAvailable(AddElitesToList);
        }
        
        Dictionary<string, string> itemStuff = new()
        {
            {"ITEM_LUNARSOAP_NAME", "FriendTurret_Acanthi_Whitelist"},
            {"SEEKINTHEVOID_STRAWPAIRY_NAME", "FriendTurret_Shortcake_Whitelist,FriendTurret_Acanthi_Whitelist"}, //seeking it ,.,.
            {"NT_ITEM_HYDRATOOTH_NAME", "FriendTurret_Acanthi_Whitelist"}, // nautilus
            {"NT_ITEM_SHIMMERINGNAUTILUS_NAME", "FriendTurret_Shortcake_Whitelist"},
            {"ROB_ITEM_FIRST_AID_SPRAY_NAME", "FriendTurret_Shortcake_Whitelist"}, // rob
            {"ROB_ITEM_PERFECT_APPLE_NAME", "FriendTurret_Shortcake_Whitelist"}, 
            {"ITEM_BORBOFUSE", "FriendTurret_Shortcake_Whitelist,FriendTurret_Snowtime_Whitelist"}, // swansong
            {"ITEM_CHOCYCOIN", "FriendTurret_Acanthi_Whitelist"}, 
            {"ITEM_REWORKRACK", "FriendTurret_Acanthi_Whitelist"}, 
            {"ITEM_ICHORVIOLET", "FriendTurret_Shortcake_Whitelist"}, 
            {"ITEM_DESIGNANOMALY", "FriendTurret_Shortcake_Whitelist"}, 
            {"ITEM_VOIDLASERTURBINE", "FriendTurret_Borbo_Whitelist"}, 
            {"ITEM_GOODEXECUTIONITEM", "FriendTurret_Snowtime_Whitelist"}, 
            {"ITEM_SANDSWEPT_AMBER_KNIFE", "FriendTurret_Acanthi_Whitelist"}, // sands, ,..,swept ,.,. 
            {"ITEM_SANDSWEPT_BLEEDING_WITNESS", "FriendTurret_Acanthi_Whitelist"},
            {"ITEM_SANDSWEPT_PRESERVED_ATOLL", "FriendTurret_Shortcake_Whitelist"},
            {"ITEM_SANDSWEPT_SMOULDERING_DOCUMENT", "FriendTurret_Acanthi_Whitelist"},
            {"Bork", "FriendTurret_Borbo_Whitelist,FriendTurret_Shortcake_Whitelist,FriendTurret_Acanthi_Whitelist"}, // idk ,.
            {"GuinsoosRageblade", "FriendTurret_Acanthi_Whitelist"},
            {"ImperialMandate", "FriendTurret_Acanthi_Whitelist"},
            {"KrakenSlayer", "FriendTurret_Acanthi_Whitelist,FriendTurret_Snowtime_Whitelist"},
            {"VV_ITEM_CRYOCANISTER_ITEM", "FriendTurret_Snowtime_Whitelist"}, //vanillas boi d.,
            {"SS2_ITEM_ARMEDBACKPACK_NAME", "FriendTurret_Shortcake_Whitelist"}, //star my storm ,.. 
            {"SS2_ITEM_STRANGECAN_NAME", "FriendTurret_Acanthi_Whitelist"},
            {"SS2_ITEM_ICETOOL_NAME", "FriendTurret_Snowtime_Whitelist"},
            {"ITEM_WoodenToolKit_Name", "FriendTurret_Bread_Whitelist"}, //bnr :plead
            {"SEEKINTHEVOID_COASTALCORAL_NAME", "GlobalFriendTurret_Whitelist"}, // globals - seekings itt ,..
            {"NT_ITEM_VISCOUSPOT_NAME", "GlobalFriendTurret_Whitelist"}, // globals - nauting it .,.,
            {"NT_ITEM_MOTHEROFPEARL_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"NT_ITEM_MOBIUSNODE_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"NT_ITEM_OSMIUMSHACKLES_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"ROB_ITEM_GREENHERB_NAME", "GlobalFriendTurret_Whitelist"}, // globals - rob ,,.
            {"ROB_ITEM_REDHERB_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"ROB_ITEM_MIXEDHERB_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"ROB_ITEM_GOLDEN_APPLE_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"ROB_ITEM_HEAVY_BOOT_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_EGG", "GlobalFriendTurret_Whitelist"}, // globals - swan ,.,
            {"ITEM_CUCKLER", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_DANGERCRIT", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_GAMMAKNIFE", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_FROZENSHELL", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_MASSANOMALY", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_BLOODANOMALY", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_BORBOBIGBATTERY", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_BORBOMANAFLOWER", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_SANDSWEPT_CROWNS_DIAMOND", "GlobalFriendTurret_Whitelist"}, // globals - sand my swpet, ,..
            {"Rabadons", "GlobalFriendTurret_Whitelist"}, // globals - idk ,. -- this is league of legends items :wilted_rose:
            {"VV_ITEM_ADZE_ITEM", "GlobalFriendTurret_Whitelist"}, // globals - vanillas ,,. boid .,.
            {"SS2_ITEM_HUNTERSSIGIL_NAME", "GlobalFriendTurret_Whitelist"}, // globals - stasrsing it ,,. by it ,.,. storm ,.,.
        };

        public void AddCustomTagsToItems()
        {
            Log.Debug("SnowtimeToybox is adding custom tags to items for Friendly Turrets/Drones...");

            ItemDef[] whitelistGlobalTurret = [
                RoR2Content.Items.Pearl,
                RoR2Content.Items.ShinyPearl,
                RoR2Content.Items.FallBoots,
                RoR2Content.Items.BonusGoldPackOnKill,
                RoR2Content.Items.FlatHealth,
                RoR2Content.Items.Knurl,
                // DLC3
                DLC3Content.Items.CookedSteak,
            ];
            ItemDef[] whitelistBorboVars = [
                // Base
                RoR2Content.Items.Syringe,
                RoR2Content.Items.BossDamageBonus,
                // DLC1
                DLC1Content.Items.PermanentDebuffOnHit,
                DLC1Content.Items.MoreMissile,
                // DLC2
                DLC2Content.Items.MeteorAttackOnHighDamage,
                DLC2Content.Items.AttackSpeedPerNearbyAllyOrEnemy,
                // DLC3
            ];
            ItemDef[] whitelistShortcakeVars = [
                // Base
                RoR2Content.Items.Thorns,
                RoR2Content.Items.BarrierOnKill,
                RoR2Content.Items.HealWhileSafe,
                RoR2Content.Items.ArmorPlate,
                RoR2Content.Items.PersonalShield,
                RoR2Content.Items.Infusion,
                RoR2Content.Items.ChainLightning,
                RoR2Content.Items.BarrierOnOverHeal,
                RoR2Content.Items.Plant,
                RoR2Content.Items.BounceNearby,
                RoR2Content.Items.ShockNearby,
                // DLC1
                DLC1Content.Items.OutOfCombatArmor,
                DLC1Content.Items.HalfSpeedDoubleHealth,
                DLC1Content.Items.MissileVoid,
                DLC1Content.Items.ChainLightningVoid,
                DLC1Content.Items.MoreMissile,
                // DLC2
                // DLC3
                DLC3Content.Items.ShieldBooster,
                DLC3Content.Items.ShockDamageAura,
            ];
            ItemDef[] whitelistSnowtimeVars = [
                // Base
                RoR2Content.Items.Syringe,
                RoR2Content.Items.IceRing,
                RoR2Content.Items.PersonalShield,
                RoR2Content.Items.SlowOnHit,
                // DLC1
                DLC1Content.Items.ElementalRingVoid,
                DLC1Content.Items.SlowOnHitVoid,
                DLC1Content.Items.MoreMissile,
                // DLC2
                // DLC3
                DLC3Content.Items.ShieldBooster,
            ];
            ItemDef[] whitelistAcanthiVars = [
                // Base
                RoR2Content.Items.Tooth,
                RoR2Content.Items.BleedOnHit,
                RoR2Content.Items.Syringe,
                RoR2Content.Items.Clover,
                RoR2Content.Items.LunarBadLuck,
                RoR2Content.Items.DeathMark,
                RoR2Content.Items.Seed,
                RoR2Content.Items.Infusion,
                RoR2Content.Items.IncreaseHealing,
                RoR2Content.Items.NovaOnHeal,
                // DLC1
                DLC1Content.Items.BleedOnHitVoid,
                DLC1Content.Items.MoreMissile,
                // DLC2
                DLC2Content.Items.TriggerEnemyDebuffs,
                // DLC3
                DLC3Content.Items.UltimateMeal,
            ];
            ItemDef[] whitelistBreadVars = [
                // Base
                RoR2Content.Items.WardOnLevel,
                RoR2Content.Items.Medkit,
                RoR2Content.Items.BarrierOnKill,
                RoR2Content.Items.BarrierOnOverHeal,
                RoR2Content.Items.SprintArmor,
                RoR2Content.Items.ArmorReductionOnHit,
                RoR2Content.Items.IncreaseHealing,
                // DLC1
                DLC1Content.Items.MoreMissile,
                // DLC2
                DLC2Content.Items.BoostAllStats,
                // DLC3
            ];
            foreach (ItemDef item in whitelistGlobalTurret)
            {
                Log.Debug("Added " + item.name + " to global friendly turret item whitelist");
                ItemAPI.ApplyTagToItem("GlobalFriendTurret_Whitelist", item);
            }
            foreach (ItemDef item in whitelistBorboVars)
            {
                Log.Debug("Added " + item.name + " to borbo turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_Borbo_Whitelist", item);
            }
            foreach (ItemDef item in whitelistShortcakeVars)
            {
                Log.Debug("Added " + item.name + " to Strawberry Shortcake Turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_Shortcake_Whitelist", item);
            }
            foreach (ItemDef item in whitelistSnowtimeVars)
            {
                Log.Debug("Added " + item.name + " to Snowtime Turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_Snowtime_Whitelist", item);
            }
            foreach (ItemDef item in whitelistAcanthiVars)
            {
                Log.Debug("Added " + item.name + " to acanthi turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_Acanthi_Whitelist", item);
            }
            foreach (ItemDef item in whitelistBreadVars)
            {
                Log.Debug("Added " + item.name + " to bread turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_Bread_Whitelist", item);
            }
            AddCustomTagsToModdedItems();
        }
        public void AddCustomTagsToModdedItems()
        {
            Log.Debug("SnowtimeToybox is adding custom tags to Modded items for Friendly Turrets/Drones...");

            foreach (ItemDef itemDef in ItemCatalog.itemDefs)
            {
                string whitelistKey = "";

                if (itemStuff.TryGetValue(itemDef.nameToken, out string nameToken))
                {
                    whitelistKey = nameToken;
                } 
                else if (itemDef.nameToken.Length > 6 && itemStuff.TryGetValue(itemDef.nameToken[..^5], out string nameTokenNoName)) // without _NAME since some are weird .,.,
                {
                    whitelistKey = nameTokenNoName;
                }
                else if (itemStuff.TryGetValue(itemDef.name, out string itemDefName)) // base name since some use that ,.,.
                {
                    whitelistKey = itemDefName;
                }

                if (whitelistKey.IsNullOrWhiteSpace()) continue;
                
                foreach (string whitelist in whitelistKey.Split(','))
                {
                    Log.Debug($"adding {itemDef.nameToken} to {whitelist} whitelist");
                    ItemAPI.ApplyTagToItem(whitelist, itemDef);
                }
            }
        }
        
        public static List<EquipmentIndex> eliteDefsEquipInherit = [];
        public void AddElitesToList()
        {
            Log.Debug("SnowtimeToybox is listing Elite equipment for inheritance...");
            eliteDefsEquipInherit = [];
            foreach (var eliteDef in EliteCatalog.eliteDefs)
            {
                if (eliteDef.eliteEquipmentDef == null) return;
                if (eliteDef.eliteEquipmentDef?.equipmentIndex == null) return;
                eliteDefsEquipInherit.Add(eliteDef.eliteEquipmentDef.equipmentIndex);
                Log.Debug("Elite Equipment: " + eliteDef.eliteEquipmentDef + " Index: " + eliteDef.eliteEquipmentDef.equipmentIndex);
            }
        }

        public void AddCustomAllies()
        {
            bodyFlags = new CharacterBody.BodyFlags();
            if (FriendlyTurretImmuneVoidDeath.Value)
            {
                bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE;
            }
            if (FriendlyTurretFallImmunity.Value)
            {
                bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            if (FriendlyTurretDrone.Value)
            {
                bodyFlags |= CharacterBody.BodyFlags.Drone;
            }
            
            IEnumerable<Type> friendlyTurrets = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(FriendlyTurretBase)));
            foreach (Type friendlyTurret in friendlyTurrets)
            {
                FriendlyTurretBase turret = (FriendlyTurretBase)System.Activator.CreateInstance(friendlyTurret);
                turret.Initalization();
                turret.ContentAdditionFuncs();
                turret.PostInit();
            }

            // add turretling
            Log.Debug("Defining Turretling(s)...");
            string turretlingPath = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/";
            FriendlyTurretTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "_TurretlingBody.prefab");
            // update stats
            FriendlyTurretTurretlingBody.GetComponent<CharacterBody>().baseDamage = TurretlingBaseDamage.Value;
            FriendlyTurretTurretlingBody.GetComponent<CharacterBody>().levelDamage = TurretlingBaseDamagePerLevel.Value;
            FriendlyTurretTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "_TurretlingMaster.prefab");
            FriendlyTurretTurretlingPrimarySkillFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingPrimaryFamily.asset");
            FriendlyTurretTurretlingPrimarySkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Primary.asset");
            FriendlyTurretTurretlingPrimarySkillDef.activationState = new SerializableEntityStateType(typeof(TurretlingBlaster));
            FriendlyTurretTurretlingSecondarySkillFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingSecondaryFamily.asset");
            FriendlyTurretTurretlingSecondarySkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Secondary.asset");
            FriendlyTurretTurretlingSecondarySkillDef.activationState = new SerializableEntityStateType(typeof(TurretlingMissile));
            FriendlyTurretTurretlingUtilSkillFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingUtilityFamily.asset");
            FriendlyTurretTurretlingUtilSkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/TurretlingShenanigans.asset");
            FriendlyTurretTurretlingUtilSkillDef.activationState = new SerializableEntityStateType(typeof(Shenanigans));
            FriendlyTurretTurretlingDef = _stcharacterAssetBundle.LoadAsset<DroneDef>(turretlingPath + "_FriendlyTurretling.asset");
            FriendlyTurretTurretlingMaster.AddComponent<TurretlingRainbow>();
            ContentAddition.AddEntityState(typeof(TurretlingBlaster), out _);
            ContentAddition.AddEntityState(typeof(TurretlingMissile), out _);
            ContentAddition.AddBody(FriendlyTurretTurretlingBody);
            ContentAddition.AddMaster(FriendlyTurretTurretlingMaster);
            ContentAddition.AddSkillFamily(FriendlyTurretTurretlingPrimarySkillFamily);
            ContentAddition.AddSkillDef(FriendlyTurretTurretlingPrimarySkillDef);
            ContentAddition.AddSkillFamily(FriendlyTurretTurretlingSecondarySkillFamily);
            ContentAddition.AddSkillDef(FriendlyTurretTurretlingSecondarySkillDef);
            ContentAddition.AddEffect(SnowtimeOrbs.orbTurretlingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbTurretlingMissileImpactObject);
            // add turretling variants (spawned with a friendly turret)
            AcanthiTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Acanthi.prefab");
            AcanthiTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Acanthi.prefab");
            BorboTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Borbo.prefab");
            BorboTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Borbo.prefab");
            BreadTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Bread.prefab");
            BreadTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Bread.prefab");
            ShortcakeTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Shortcake.prefab");
            ShortcakeTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Shortcake.prefab");
            SnowtimeTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Snowtime.prefab");
            SnowtimeTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Snowtime.prefab");
            ContentAddition.AddBody(AcanthiTurretlingBody);
            ContentAddition.AddMaster(AcanthiTurretlingMaster);
            ContentAddition.AddEffect(TurretlingBlaster.muzzlefx_acanthi);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_acanthi);
            ContentAddition.AddEffect(TurretlingBlaster.tracerfx_acanthi);
            ContentAddition.AddEffect(SnowtimeOrbs.orbAcanthilingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbAcanthilingMissileImpactObject);
            ContentAddition.AddBody(BorboTurretlingBody);
            ContentAddition.AddMaster(BorboTurretlingMaster);
            ContentAddition.AddEffect(TurretlingBlaster.muzzlefx_borbo);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_borbo);
            ContentAddition.AddEffect(TurretlingBlaster.tracerfx_borbo);
            ContentAddition.AddEffect(SnowtimeOrbs.orbBorbolingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbBorbolingMissileImpactObject);
            ContentAddition.AddBody(BreadTurretlingBody);
            ContentAddition.AddMaster(BreadTurretlingMaster);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_bread);
            ContentAddition.AddEffect(SnowtimeOrbs.orbBreadlingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbBreadlingMissileImpactObject);
            ContentAddition.AddBody(ShortcakeTurretlingBody);
            ContentAddition.AddMaster(ShortcakeTurretlingMaster);
            ContentAddition.AddEffect(TurretlingBlaster.muzzlefx_shortcake);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_shortcake);
            ContentAddition.AddEffect(TurretlingBlaster.tracerfx_shortcake);
            ContentAddition.AddEffect(SnowtimeOrbs.orbShortcakelingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbShortcakelingMissileImpactObject);
            ContentAddition.AddBody(SnowtimeTurretlingBody);
            ContentAddition.AddMaster(SnowtimeTurretlingMaster);
            ContentAddition.AddEffect(TurretlingBlaster.muzzlefx_snowtime);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_snowtime);
            ContentAddition.AddEffect(TurretlingBlaster.tracerfx_snowtime);
            ContentAddition.AddEffect(SnowtimeOrbs.orbSnowtimelingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbSnowtimelingMissileImpactObject);

            ContentAddition.AddEffect(TurretlingBlaster.muzzlefx_rainbow);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_rainbow);
            ContentAddition.AddEffect(TurretlingBlaster.tracerfx_rainbow);
            ContentAddition.AddEffect(SnowtimeOrbs.orbRainbowMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbRainbowMissileImpactObject);

            ContentAddition.AddEntityState(typeof(Shenanigans), out _);
            
            // Add the Turretling to stages interactable spawncards, as it is a standard walking turret and NOT a Friendly Turret, as its internal name may imply
            FriendlyTurretTurretlingBroken = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/_mdlTurretlingBroken.prefab");
            FriendlyTurretTurretlingIsc = _stcharacterAssetBundle.LoadAsset<InteractableSpawnCard>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/_iscBrokenTurretling.asset");
            ContentAddition.AddNetworkedObject(FriendlyTurretTurretlingBroken);
            var directorCardFriendlyTurretTurretling = new DirectorCard // Borbo Turret Interactable
            {
                spawnCard = FriendlyTurretTurretlingIsc,
                selectionWeight = 14, // the higher it is, the more common it is
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                minimumStageCompletions = 0,
                preventOverhead = false
            };

            var directorCardHolderFriendlyTurretTurretling = new DirectorAPI.DirectorCardHolder
            {
                Card = directorCardFriendlyTurretTurretling,
                InteractableCategory = DirectorAPI.InteractableCategory.Drones
            };

            List<DirectorAPI.Stage> turretlingStageList = new List<DirectorAPI.Stage>();
            List<String> turretlingCustomStageList = new List<String>();

            // Stage 1
            turretlingStageList.Add(DirectorAPI.Stage.TitanicPlains);
            turretlingStageList.Add(DirectorAPI.Stage.DistantRoost);
            turretlingStageList.Add(DirectorAPI.Stage.SiphonedForest);
            turretlingStageList.Add(DirectorAPI.Stage.VerdantFalls);
            turretlingStageList.Add(DirectorAPI.Stage.ViscousFalls);
            // Stage 2
            turretlingStageList.Add(DirectorAPI.Stage.AbandonedAqueduct);
            turretlingStageList.Add(DirectorAPI.Stage.AphelianSanctuary);
            turretlingStageList.Add(DirectorAPI.Stage.PretendersPrecipice);
            // Stage 3
            turretlingStageList.Add(DirectorAPI.Stage.RallypointDelta);
            turretlingStageList.Add(DirectorAPI.Stage.ScorchedAcres);
            turretlingStageList.Add(DirectorAPI.Stage.IronAlluvium);
            turretlingStageList.Add(DirectorAPI.Stage.IronAuroras);
            // Stage 4
            turretlingStageList.Add(DirectorAPI.Stage.SirensCall);
            turretlingStageList.Add(DirectorAPI.Stage.SunderedGrove);
            turretlingStageList.Add(DirectorAPI.Stage.RepurposedCrater);
            turretlingStageList.Add(DirectorAPI.Stage.ConduitCanyon);
            // Stage 5
            turretlingStageList.Add(DirectorAPI.Stage.SkyMeadow);
            // Mods
            turretlingCustomStageList.Add("FBLScene");
            turretlingCustomStageList.Add("broadcastperch_wormsworms");
            turretlingCustomStageList.Add("tropics_wormsworms");
            turretlingCustomStageList.Add("tropicsnight_wormsworms");
            turretlingCustomStageList.Add("hollowsummit_wormsworms");
            turretlingCustomStageList.Add("hollowsummitnight_wormsworms");
            turretlingCustomStageList.Add("catacombs_DS1_Catacombs");
            turretlingCustomStageList.Add("snowtime_bloodgulch");
            turretlingCustomStageList.Add("snowtime_deathisland");
            turretlingCustomStageList.Add("snowtime_gephyrophobia");
            turretlingCustomStageList.Add("snowtime_gmconstruct");
            turretlingCustomStageList.Add("snowtime_gmflatgrass");
            turretlingCustomStageList.Add("snowtime_halo");
            turretlingCustomStageList.Add("snowtime_halo2");
            turretlingCustomStageList.Add("snowtime_highcharity");
            turretlingCustomStageList.Add("snowtime_icefields");
            turretlingCustomStageList.Add("snowtime_newmombasabridge");
            turretlingCustomStageList.Add("snowtime_odstmombasa");
            turretlingCustomStageList.Add("snowtime_plrhightower");
            turretlingCustomStageList.Add("snowtime_sandtrap");
            turretlingCustomStageList.Add("snowtime_sidewinder");

            foreach (DirectorAPI.Stage stage in turretlingStageList)
            {
                Log.Debug("Adding Friendly Turrets to stage: " + stage);
                DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderFriendlyTurretTurretling, stage);
            }
            foreach (string stage in turretlingCustomStageList)
            {
                Log.Debug("Adding Friendly Turrets to stage: " + stage);
                DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderFriendlyTurretTurretling, DirectorAPI.Stage.Custom, stage);
            }
            
            List<GameObject> turretlingBodies =
            [
                FriendlyTurretTurretlingBody,
                AcanthiTurretlingBody,
                BorboTurretlingBody,
                ShortcakeTurretlingBody,
                SnowtimeTurretlingBody,
                BreadTurretlingBody
            ];
            foreach (var turretling in turretlingBodies)
            {
                turretling.AddComponent<TurretlingMissileTracker>();
                turretling.AddComponent<EquipmentSlot>();
            }
            
            FriendlyTurretTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
            BorboTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Borbo_Whitelist";
            SnowtimeTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Snowtime_Whitelist";
            AcanthiTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Acanthi_Whitelist";
            BreadTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Bread_Whitelist";
            ShortcakeTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Shortcake_Whitelist";
            
            foreach (var turretling in turretlingBodies)
            {
                turretling.GetComponent<CharacterBody>().bodyFlags |= bodyFlags;
            }
            
            if (TurretlingImmuneVoidDeath.Value)
            {
                FriendlyTurretTurretlingBody.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE;
            }

            On.RoR2.PurchaseInteraction.GetInteractability += GetInteractabilityFriendlyTurrets;
            // i want die
            ContentAddition.AddEffect(BorboCheck.turretUseEffect);

            // update friendly turret targeting
            BaseAI.UpdateTargets += UpdateFriendlyTurretTargeting;
            
            //overlay amanger ,.,. 
            On.RoR2.CharacterModel.UpdateOverlays += CharacterModelOnUpdateOverlays;
        }
        
        private void CharacterModelOnUpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, RoR2.CharacterModel self)
        {
            orig(self);
            
            if (self && self.body)
            {
                if (self.body.HasBuff(BreadFriendlyTurret.BreadTurretBuffFortune))
                {
                    FriendlyTurretOverlayManager friendlyTurretOverlayManager = self.body.GetComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager) friendlyTurretOverlayManager = self.body.gameObject.AddComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager.hasOverlay("matBreadFortune"))
                    {
                        friendlyTurretOverlayManager.Body = self.body;
                        var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.gameObject);
                        temporaryOverlay.duration = float.PositiveInfinity;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Bread/matBreadFortune.mat");
                        temporaryOverlay.AddToCharacterModel(self);
                        friendlyTurretOverlayManager.Overlay.Add(temporaryOverlay);
                    }
                }
                
                if (self.body.HasBuff(AcanthiFriendlyTurret.AcanthiTurretDebuff))
                {
                    FriendlyTurretOverlayManager friendlyTurretOverlayManager = self.body.GetComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager) friendlyTurretOverlayManager = self.body.gameObject.AddComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager.hasOverlay("acanthidebuffoverlay"))
                    {
                        friendlyTurretOverlayManager.Body = self.body;
                        var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.gameObject);
                        temporaryOverlay.duration = float.PositiveInfinity;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Acanthi/acanthidebuffoverlay.mat");
                        temporaryOverlay.AddToCharacterModel(self);
                        friendlyTurretOverlayManager.Overlay.Add(temporaryOverlay);
                    }
                }
                
                if (self.body.HasBuff(BorboFriendlyTurret.BorboTurretDebuff))
                {
                    FriendlyTurretOverlayManager friendlyTurretOverlayManager = self.body.GetComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager) friendlyTurretOverlayManager = self.body.gameObject.AddComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager.hasOverlay("borboturretdebuffoverlay"))
                    {
                        friendlyTurretOverlayManager.Body = self.body;
                        var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.gameObject);
                        temporaryOverlay.duration = float.PositiveInfinity;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/borboturretdebuffoverlay.mat");
                        temporaryOverlay.AddToCharacterModel(self);
                        friendlyTurretOverlayManager.Overlay.Add(temporaryOverlay);
                    }
                }

                if (self.body.HasBuff(BreadFriendlyTurret.BreadTurretBuffNearbyAllies))
                {
                    FriendlyTurretOverlayManager friendlyTurretOverlayManager = self.body.GetComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager) friendlyTurretOverlayManager = self.body.gameObject.AddComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager.hasOverlay("matBreadGraceOverlay"))
                    {
                        friendlyTurretOverlayManager.Body = self.body;
                        var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.gameObject);
                        temporaryOverlay.duration = float.PositiveInfinity;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Bread/matBreadGraceOverlay.mat");
                        temporaryOverlay.AddToCharacterModel(self);
                        friendlyTurretOverlayManager.Overlay.Add(temporaryOverlay);
                    }
                }
            }
        }

        private void UpdateFriendlyTurretTargeting(BaseAI.orig_UpdateTargets orig, RoR2.CharacterAI.BaseAI self)
        {
            // Default targeting type: Closest enemy(?)
            orig(self);
            CharacterBody body = self.body;
            string bodyname = body.baseNameToken;
            float target_maxdist = 250f;
            //Log.Debug(bodyname);
            // Target High Value Targets (enemies with greatest combinedhealth)
            if (bodyname.Contains("FRIENDLYTURRET_BORBO") || bodyname.Contains("FRIENDLYTURRET_SNOWTIME") || bodyname.Contains("FRIENDLYTURRET_ACANTHI"))
            {
                if (bodyname.Contains("FRIENDLYTURRET_SNOWTIME"))
                {
                    target_maxdist = 125f;
                }
                if (bodyname.Contains("FRIENDLYTURRET_ACANTHI"))
                {
                    target_maxdist = 80f;
                }
                //Log.Debug("Found appropriate turret AI: " + bodyname);
                InputBankTest inputBank = body.inputBank;
                TeamComponent teamComponent = body.teamComponent;
                if (body == null || inputBank == null || teamComponent == null)
                {
                    return;
                }
                BullseyeSearch bullseyeSearch = new BullseyeSearch
                {
                    searchOrigin = body.corePosition,
                    searchDirection = inputBank.aimDirection,
                    teamMaskFilter = TeamMask.GetEnemyTeams(teamComponent.teamIndex),
                    maxDistanceFilter = target_maxdist,
                    filterByLoS = true,
                    sortMode = BullseyeSearch.SortMode.None
                };
                bullseyeSearch.RefreshCandidates();
                HurtBox hurtBox = null;
                float num = float.NegativeInfinity;
                foreach (HurtBox result in bullseyeSearch.GetResults())
                {
                    float maxCombinedHealth = result.healthComponent.fullCombinedHealth;
                    if (maxCombinedHealth > num)
                    {
                        num = maxCombinedHealth;
                        hurtBox = result;
                    }
                }
                if ((bool)hurtBox)
                {
                    self.currentEnemy.bestHurtBox = hurtBox;
                }
            }
            // Clean up the small fry (enemies with lowest current health)
            // Nothing uses this yet but I think it might be worth on some turrets, keeping for the future.
            if (bodyname.Contains("sometheoreticalturret"))
            {
                //Log.Debug("Found appropriate turret AI: " + bodyname);
                InputBankTest inputBank = body.inputBank;
                TeamComponent teamComponent = body.teamComponent;
                if (body == null || inputBank == null || teamComponent == null)
                {
                    return;
                }
                BullseyeSearch bullseyeSearch = new BullseyeSearch
                {
                    searchOrigin = body.corePosition,
                    searchDirection = inputBank.aimDirection,
                    teamMaskFilter = TeamMask.GetEnemyTeams(teamComponent.teamIndex),
                    maxDistanceFilter = 250f,
                    filterByLoS = true,
                    sortMode = BullseyeSearch.SortMode.None
                };
                bullseyeSearch.RefreshCandidates();
                HurtBox hurtBox = null;
                float num = float.NegativeInfinity;
                foreach (HurtBox result in bullseyeSearch.GetResults())
                {
                    float currenthealth = result.healthComponent.health;
                    if (currenthealth < num)
                    {
                        num = currenthealth;
                        hurtBox = result;
                    }
                }
                if ((bool)hurtBox)
                {
                    self.currentEnemy.bestHurtBox = hurtBox;
                }
            }
        }

        string interactablesuffering = " (UnityEngine.GameObject)";
        string charactersuffering = "(Clone) (UnityEngine.GameObject)";
        string interactablesmaster;
        string charactersmaster;
        float initialprice = 0f;

        private Interactability GetInteractabilityFriendlyTurrets(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            //Log.Debug(self.displayNameToken);
            if (!self.displayNameToken.StartsWith("FRIENDLYTURRET_"))
            {
                return orig(self, activator);
            }

            CharacterBody[] minionBodies = activator.gameObject.GetComponent<CharacterBody>()?.GetMinionBodies();
            if (minionBodies == null) return orig(self, activator);
            
            foreach (CharacterBody body in minionBodies)
            {
                if (!body.baseNameToken.StartsWith("FRIENDLYTURRET_"))
                {
                    continue;
                }

                //Log.Debug("Minion: " + body.baseNameToken);
                interactablesmaster = self.GetComponent<SummonMasterBehavior>().masterPrefab.gameObject.ToString();
                if (interactablesmaster.EndsWith(interactablesuffering))
                {
                    interactablesmaster = interactablesmaster.Substring(0, interactablesmaster.LastIndexOf(interactablesuffering));
                }
                charactersmaster = body.master.gameObject.ToString();
                if (charactersmaster.EndsWith(charactersuffering))
                {
                    charactersmaster = charactersmaster.Substring(0, charactersmaster.LastIndexOf(charactersuffering));
                }
                //Log.Debug("Interactable: " + self.displayNameToken + " Minion CharacterBody: " + body.baseNameToken);
                //Log.Debug("Cleaned Interactable Summonable: " + interactablesmaster + " Minion Master: " + charactersmaster);
                //Log.Debug("Does Interactable Summon Master match CharacterBody Master?");
                if (charactersmaster.Contains(interactablesmaster))
                {
                    //Log.Debug("Previous query returned true");
                    return Interactability.Disabled;
                }
                // Implement cost reduction later.
                //networkUser.id.steamId

                //if (FriendlyTurretReducedCostForPartnersOrSelf.Value)
                //{
                //    if (initialprice == 0f)
                //    {
                //        initialprice = (self.cost / 2);
                //    }
                //}
            }

            return orig(self, activator);
        }
        
        public void AddDifficulty()
        {
            Log.Debug("Adding SnowtimeToybox Custom Difficulty...");
            if (riskierLoaded)
            {
                Log.Debug("2R4R detected, updating name to LASO and its description to match new gimmicks.");
                SnowtimeLegendaryDiffDef = new(3.5f, "SNOWTIME_LASO_NAME", "SNOWTIME_LEGENDARY_ICON", "SNOWTIME_LASO_DESC", new Color32(168, 50, 50, 255), "stLeg", false);
            }
            else
            {
                Log.Debug("2R4R not detected, adding Legendary.");
                SnowtimeLegendaryDiffDef = new(3.5f, "SNOWTIME_LEGENDARY_NAME", "SNOWTIME_LEGENDARY_ICON", "SNOWTIME_LEGENDARY_DESC", new Color32(100, 170, 255, 255), "stLeg", false);
            }
            SnowtimeLegendaryDiffDef.iconSprite = _stdifficultyAssetBundle.LoadAsset<Sprite>(@"Assets/SnowtimeMod/Assets/_difficulty/texSnowtimeLegendaryPLNK.png");
            SnowtimeLegendaryDiffDef.foundIconSprite = true;
            SnowtimeLegendaryDiffIndex = DifficultyAPI.AddDifficulty(SnowtimeLegendaryDiffDef);
        }
        
        public void AddCustomBuffs()
        {
            Log.Debug("Adding SnowtimeToybox Custom BuffDefs...");
            // Assets not implemented yet
            
            IEnumerable<Type> buffTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(BuffBase)));
            foreach (Type buffType in buffTypes)
            {
                Log.Debug(buffType.Name);
                BuffBase buff = (BuffBase)System.Activator.CreateInstance(buffType);
                buff.Create();
            }
        }

        public void AddCustomSkills()
        {
            Log.Debug("Adding SnowtimeToybox Custom Skills...");

            // Effects First

            ContentAddition.AddEffect(FirePlasmaRifle.MuzzleFlashObject);
            ContentAddition.AddEffect(FirePlasmaRifle.TracerObject);
            ContentAddition.AddEffect(FirePlasmaRifle.HitObject);
            ContentAddition.AddEffect(SnowtimeHaloRicochetOrb.orbEffectObject);
            ContentAddition.AddEffect(_stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/PlasmaRifleImpactVFXRico.prefab"));

            GameObject DroneTechBodyPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.DroneTechBody_prefab).WaitForCompletion();
            SkillLocator skillLocator = DroneTechBodyPrefab.GetComponent<SkillLocator>();
            SkillFamily skillFamily = skillLocator.primary.skillFamily;
            SkillDef SnowtimePlasmaRifleSkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/entskilldefFirePlasmaRifle.asset");

            SnowtimePlasmaRifleSkillDef.activationState = new SerializableEntityStateType(typeof(FirePlasmaRifle));

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[^1] = new SkillFamily.Variant
            {
                skillDef = SnowtimePlasmaRifleSkillDef,
                viewableNode = new ViewablesCatalog.Node(SnowtimePlasmaRifleSkillDef.skillNameToken, false)
            };
            ContentAddition.AddEntityState(typeof(FirePlasmaRifle), out _);
            ContentAddition.AddSkillDef(SnowtimePlasmaRifleSkillDef);
            // done!
        }

        public void CollectLanguageRootFolders(List<string> folders)
        {
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), "Language"));
        }
        private void Destroy()
        {
            Language.collectLanguageRootFolders -= CollectLanguageRootFolders;
        }

        private void Update()
        {
#if DEBUG
            if (Input.GetKeyUp(KeyCode.F3))
            {
                UnityHotReload.LoadNewAssemblyVersion(typeof(SnowtimeToyboxMod).Assembly,
                    Path.Combine(Path.GetDirectoryName(Info.Location)!, "SnowtimeToybox.dll"));
            }
#endif
        }
        
        [ConCommand(commandName = "friendturretpos", flags = ConVarFlags.None, helpText = "get image friend !! find her lost girl friend inside me .,.,,. ")]
        public static void friendturretfindher(ConCommandArgs args)
        {
            /*
             * case "lakes":
               stagePositions.Add(new Vector3(139f, 59.07873f, -181.3314f), Quaternion.Euler(355f, 325f, 0)); //behind a waterfall on the map's edge (how is there not already a secret here??)
               break;
             */
            Log.Debug($"case \"{SceneManager.GetActiveScene().name}\":");
            Log.Debug($"    stagePositions.Add(new Vector3({args.senderBody.footPosition.x}f, {args.senderBody.footPosition.y}f, {args.senderBody.footPosition.z}f), Quaternion.Euler({args.senderBody.modelLocator.modelTransform.rotation.eulerAngles.x}f, {args.senderBody.modelLocator.modelTransform.rotation.eulerAngles.y}f, {args.senderBody.modelLocator.modelTransform.rotation.eulerAngles.z}f));");
            Log.Debug($"    break;");
        }
        
        [ConCommand(commandName = "spawn_turret", flags = ConVarFlags.None, helpText = "spawn a turret !!!")]
        public static void spawnturret(ConCommandArgs args)
        {
            Log.Info($"tryings to find {args[0]}.,,. ");
            int indexTurret = -1;

            FriendlyTurretBase friend = friendlyTurretList.First(friendlyTurret => friendlyTurret.turretName.Contains(args[0], StringComparison.InvariantCultureIgnoreCase));

            Instantiate(friend.broken, args.senderBody.footPosition, args.senderBody.transform.rotation);
        }
        
        [ConCommand(commandName = "list_turret", flags = ConVarFlags.None, helpText = "list available turrets !!!")]
        public static void listturret(ConCommandArgs args)
        {
            foreach (FriendlyTurretBase friendlyTurret in friendlyTurretList)
            {
                Log.Info(friendlyTurret.turretName);
            }
        }
    }
}
