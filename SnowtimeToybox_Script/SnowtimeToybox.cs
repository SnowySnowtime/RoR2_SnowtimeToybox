using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.SnowtimeToybox_FireHaloWeapon;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using R2API;
using RoR2;
using RoR2.Skills;
using ShaderSwapper;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Path = System.IO.Path;

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
        public const string Version = "1.0.0";
        public const string GUID = Author + "." + Name;
        // public static ConfigEntry<bool> ToggleBloodGulch { get; set; }

        public static SnowtimeToyboxMod instance;

        public static DifficultyDef SnowtimeLegendaryDiffDef;
        public static DifficultyIndex SnowtimeLegendaryDiffIndex;
        public static SkillDef SnowtimePlasmaRifleSkillDef;
        public static GameObject MuzzleFlashObject;
        public static GameObject TracerObject;
        public static GameObject HitObject;
        public static GameObject OrbObject;

        // friend walker turrets!
        public static DroneDef FriendlyTurretBorboDef;
        public static InteractableSpawnCard FriendlyTurretBorboIsc;
        public static SkillFamily FriendlyTurretBorboSkillFamily;
        public static SkillDef FriendlyTurretBorboSkillDef;
        public static GameObject FriendlyTurretBorboBody;
        public static GameObject FriendlyTurretBorboMaster;
        public static GameObject FriendlyTurretBorboBroken;
        //public static DroneDef FriendlyTurretTestDroneDef;

        public static bool Legendary = false;
        // Copied from RiskierRain, sorry borbo :(
        public static bool ModLoaded(string modGuid) { return !string.IsNullOrEmpty(modGuid) && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(modGuid); }
        public static bool riskierLoaded => ModLoaded("com.RiskOfBrainrot.RiskierRain");

        public void Awake()
        {
            instance = this;

            Log.Init(Logger);

            //ToggleBloodGulch = Config.Bind("Stage 1 Toggles", "Blood Gulch", true, "If true, Blood Gulch is added to the loop, otherwise it shall not appear");

            Language.collectLanguageRootFolders += CollectLanguageRootFolders;

            Run.onRunStartGlobal += (Run run) =>
            {
                Legendary = false;
                if (run.selectedDifficulty == SnowtimeLegendaryDiffIndex)
                {
                    Legendary = true;
                    CharacterMaster.onStartGlobal += CharacterMaster_OnStartGlobal;
                }
            };

            Run.onRunDestroyGlobal += (Run run) =>
            {
                Legendary = false;
                CharacterMaster.onStartGlobal -= CharacterMaster_OnStartGlobal;
            };

            GlobalEventManager.onServerDamageDealt += SnowtimeDamageTypes.GlobalEventManager_onServerDamageDealt;
        }

        public static String assetDirectory;
        public static AssetBundle _stdifficultyAssetBundle;
        public static AssetBundle _stcharacterAssetBundle;
        internal const string _stdifficultyAssetBundleName = "snowtimetoybox_difficulty";
        internal const string _stcharacterAssetBundleName = "snowtimetoybox_characters";

        private void Start()
        {
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
            //AddEffectDef();
            AddCustomSkills();
            AddCustomAllies();
            AddCustomBuffs();
        }

        public void AddCustomAllies()
        {
            Log.Debug(GUID);
            Log.Debug("Adding SnowtimeToybox Friend Drones...");
            // Entity States need to be added before the rest of the content is loaded.
            Log.Debug("Adding Friendly Turret Entity States");
            // borbo turret borbo turret
            // Add Borbo Turret
            Log.Debug("Defining Friendly Turret based on Borbo (2R4R)");
            GameObject FriendlyTurretBorboBody = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/_FriendlyTurretBorboBody.prefab");
            GameObject FriendlyTurretBorboMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/_FriendlyTurretBorboMaster.prefab");
            SkillFamily FriendlyTurretBorboSkillFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Skills/BorboPrimaryFamily.asset");
            SkillDef FriendlyTurretBorboSkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Skills/BorboBlast.asset");
            // I am being PEDANTIC but i dont care!
            FriendlyTurretBorboSkillDef.activationState = new SerializableEntityStateType(typeof(ChargeBorboLaser));
            GameObject FriendlyTurretBorboBroken = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/_mdlFriendlyTurretBorboBroken.prefab");
            DroneDef FriendlyTurretBorboDef = _stcharacterAssetBundle.LoadAsset<DroneDef>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/_FriendlyTurretBorbo.asset");
            // Add the Content
            Log.Debug("Adding Borbo Turret Assets");
            ContentAddition.AddEntityState(typeof(FireBorboLaser), out _);
            ContentAddition.AddEntityState(typeof(ChargeBorboLaser), out _);
            ContentAddition.AddBody(FriendlyTurretBorboBody);
            ContentAddition.AddMaster(FriendlyTurretBorboMaster);
            ContentAddition.AddSkillFamily(FriendlyTurretBorboSkillFamily);
            ContentAddition.AddSkillDef(FriendlyTurretBorboSkillDef);
            ContentAddition.AddEffect(FireBorboLaser.effectPrefabObject);
            ContentAddition.AddEffect(FireBorboLaser.hitEffectPrefabObject);
            ContentAddition.AddEffect(FireBorboLaser.tracerEffectPrefabObject);
            // ContentAddition.AddDroneDef();

            Log.Debug("Adding Friendly Turrets Interactables to Stages");
            ContentAddition.AddNetworkedObject(FriendlyTurretBorboBroken);
            // i want die
            InteractableSpawnCard FriendlyTurretBorboIsc = _stcharacterAssetBundle.LoadAsset<InteractableSpawnCard>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/_iscBrokenFriendlyTurretBorbo.asset");
            // Manage the new Interactables
            var directorCardFriendlyTurretBorbo = new DirectorCard // Borbo Turret Interactable
            {
                spawnCard = FriendlyTurretBorboIsc,
                selectionWeight = 1000, // the higher it is, the more common it is
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                minimumStageCompletions = 0,
                preventOverhead = false
            };
            Log.Debug("Borbo Turret Director Card Info");
            Log.Debug("spawnCard = " + directorCardFriendlyTurretBorbo.spawnCard);
            Log.Debug("selectionWeight = " + directorCardFriendlyTurretBorbo.selectionWeight);
            Log.Debug("spawnDistance = " + directorCardFriendlyTurretBorbo.spawnDistance);
            Log.Debug("minimumStageCompletions = " + directorCardFriendlyTurretBorbo.minimumStageCompletions);
            Log.Debug("preventOverhead = " + directorCardFriendlyTurretBorbo.preventOverhead);

            var directorCardHolderFriendlyTurretBorbo = new DirectorAPI.DirectorCardHolder
            {
                Card = directorCardFriendlyTurretBorbo,
                //CustomInteractableCategory = "SnowtimeFriendlyTurrets",
                InteractableCategory = DirectorAPI.InteractableCategory.Drones
                //InteractableCategorySelectionWeight = 1000,
            };
            Log.Debug("Borbo Turret Director Card Holder Info");
            Log.Debug("Card = " + directorCardHolderFriendlyTurretBorbo.Card);
            //Log.Debug("CustomInteractableCategory = " + directorCardHolderFriendlyTurretBorbo.CustomInteractableCategory);
            //Log.Debug("InteractableCategorySelectionWeight = " + directorCardHolderFriendlyTurretBorbo.InteractableCategorySelectionWeight);

            List<DirectorAPI.Stage> borboStageList = new List<DirectorAPI.Stage>();

            borboStageList.Add(DirectorAPI.Stage.TitanicPlains);
            borboStageList.Add(DirectorAPI.Stage.AbandonedAqueduct);
            borboStageList.Add(DirectorAPI.Stage.WetlandAspect);
            borboStageList.Add(DirectorAPI.Stage.AphelianSanctuary);
            borboStageList.Add(DirectorAPI.Stage.RallypointDelta);
            borboStageList.Add(DirectorAPI.Stage.ScorchedAcres);
            borboStageList.Add(DirectorAPI.Stage.SirensCall);
            borboStageList.Add(DirectorAPI.Stage.ConduitCanyon);
            borboStageList.Add(DirectorAPI.Stage.SkyMeadow);

            foreach (DirectorAPI.Stage stage in borboStageList)
            {
                Log.Debug("Adding Friendly Turrets to stage: " + stage);
                DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderFriendlyTurretBorbo, stage);
            }
        }


        private void CharacterMaster_OnStartGlobal(CharacterMaster obj)
        {
            if (obj.teamIndex != TeamIndex.Player)
            {
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.AlienHead, 1);
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.BoostAttackSpeed, 3);
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.BoostHp, 4);
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.PersonalShield, 5);
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.BoostDamage, 10);
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.ShinyPearl, 1);
            }
        }

        public void AddDifficulty()
        {
            Log.Debug(GUID);
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

        public static BuffDef BorboTurretDebuff;

        public void AddCustomBuffs()
        {
            Log.Debug(GUID);
            Log.Debug("Adding SnowtimeToybox Custom BuffDefs...");
            BuffDef BorboTurretDebuff = _stcharacterAssetBundle.LoadAsset<BuffDef>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Buff/BorboTurretDebuff.asset");
            Log.Debug(BorboTurretDebuff);
            ContentAddition.AddBuffDef(BorboTurretDebuff);
        }

        public void AddCustomSkills()
        {
            Log.Debug(GUID);
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
    }
}
