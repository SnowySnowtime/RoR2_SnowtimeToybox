using EntityStates;
using EntityStates.SnowtimeToybox_FireHaloWeapon;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using HG;
using R2API;
using RoR2;
using RoR2.Skills;
using SnowtimeToybox.Buffs;
using SnowtimeToybox.Components;
using SnowtimeToybox.FriendlyTurretChecks;
using SnowtimeToybox.FriendlyTurrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ReadOnlyContentPack = RoR2.ContentManagement.ReadOnlyContentPack;

namespace SnowtimeToybox;

public class Content
{
    // operator custom skill
    public static SkillDef SnowtimePlasmaRifleSkillDef;
    // turretlings
    public static DroneDef FriendlyTurretTurretlingDef;
    public static InteractableSpawnCard FriendlyTurretTurretlingIsc;
    public static SkillFamily FriendlyTurretTurretlingPrimarySkillFamily;
    public static SkillFamily FriendlyTurretTurretlingPrimaryMinionSkillFamily;
    public static SkillFamily FriendlyTurretTurretlingPrimaryMeleeMinionSkillFamily;
    public static SkillFamily FriendlyTurretTurretlingSecondarySkillFamily;
    public static SkillFamily FriendlyTurretTurretlingSecondaryPlayerSkillFamily;
    public static SkillFamily FriendlyTurretTurretlingSecondaryAltSkillFamily;
    public static SkillFamily FriendlyTurretTurretlingUtilSkillFamily;
    public static SkillDef FriendlyTurretTurretlingPrimarySkillDef;
    public static SkillDef FriendlyTurretTurretlingPrimaryMeleeSkillDef;
    public static SkillDef FriendlyTurretTurretlingPrimaryMeleeMinionSkillDef;
    public static SkillDef FriendlyTurretTurretlingPrimaryMinionSkillDef;
    public static SkillDef FriendlyTurretTurretlingPrimaryScepterSkillDef;
    public static SkillDef FriendlyTurretTurretlingPrimaryScepterMinionSkillDef;
    public static SkillDef FriendlyTurretTurretlingSecondarySkillDef;
    public static SkillDef FriendlyTurretTurretlingSecondaryAltSkillDef;
    public static SkillDef FriendlyTurretTurretlingUtilSkillDef;
    public static GameObject FriendlyTurretTurretlingBody;
    public static GameObject FriendlyTurretTurretlingBodyRemoteOp;
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
    public static DroneDef DTTurretlingDef;
    public static GameObject DTTurretlingBody;
    public static GameObject DTTurretlingMaster;
    public static GameObject DTTurretlingBroken;
    public static SkillFamily DTTurretlingSkillFamily;
    public static SkillDef DTTurretlingSkillDef;
    public static DroneDef DTDemoTurretlingDef;
    public static GameObject DTDemoTurretlingBody;
    public static GameObject DTDemoTurretlingMaster;
    public static GameObject DTDemoTurretlingBroken;
    public static SkillFamily ArtiPassiveFamily;
    public static SkillDef ArtiTurretSkill;
    public static SkillDef ArtiNoTurretSkill;
    public static DroneDef ArtiTurretlingDef;
    public static GameObject ArtiTurretlingBody;
    public static GameObject ArtiTurretlingMaster;
    public static GameObject ArtiTurretlingBroken;
    public static DroneDef DemoTurretlingDef;
    public static GameObject DemoTurretlingBody;
    public static GameObject DemoTurretlingMaster;
    public static GameObject DemoTurretlingBroken;
    public static GameObject DemoTurretlingProjectile;
    public static GameObject DemoTurretlingProjectileGhost;
    public static GameObject DemoTurretlingExplosEffect;
    public static InteractableSpawnCard DemoTurretlingIsc;
    public static SkillFamily DemoTurretlingPrimaryFamily;
    public static SkillDef DemoTurretlingPrimarySkill;
    public static SkillDef PassiveDemoTurretSkill;
    public static DroneDef PassiveDemoTurretlingDef;
    public static GameObject PassiveDemoTurretlingBody;
    public static GameObject PassiveDemoTurretlingMaster;
    public static GameObject PassiveDemoTurretlingBroken;
    // turretling survivor
    public static SurvivorDef SwarmlingDef;
    public static GameObject SwarmlingBody;
    public static GameObject SwarmlingMaster;
    public static DroneDef SwarmlingMinionDef;
    public static GameObject SwarmlingMinionBody;
    public static GameObject SwarmlingMinionBroken;
    public static GameObject SwarmlingMinionMaster;
    public static DroneDef SwarmlingDemoMinionDef;
    public static GameObject SwarmlingDemoMinionBody;
    public static GameObject SwarmlingDemoMinionBroken;
    public static GameObject SwarmlingDemoMinionMaster;
    public static DroneDef SwarmlingMeleeMinionDef;
    public static GameObject SwarmlingMeleeMinionBody;
    public static GameObject SwarmlingMeleeMinionBroken;
    public static GameObject SwarmlingMeleeMinionMaster;
    public static SkillFamily Swarmling_PassiveFamily1;
    public static SkillFamily Swarmling_PassiveFamily2;
    public static SkillFamily Swarmling_PassiveFamily3;
    public static SkillFamily Swarmling_PassiveFamily4;
    public static SkillFamily Swarmling_PassiveFamily5;
    public static SkillFamily Swarmling_PassiveFamily6;
    public static SkillDef SwarmlingPassiveMinion;
    public static SkillDef SwarmlingDemoPassiveMinion;
    public static SkillDef SwarmlingMeleePassiveMinion;
    public static SkillFamily SwarmlingSpecialFamily;
    public static SkillDef SwarmlingSpecialSkill;
    public static SkillFamily SwarmlingUtilityFamily;
    public static SkillDef SwarmlingUtilitySkill;
    public static SkillDef SwarmlingUtilityAltSkill;
    public static DamageColorIndex BlasterScepterColor1;
    public static DamageColorIndex BlasterScepterColor2;
    public static DamageColorIndex BlasterScepterColor3;
    public static DamageColorIndex NeedlerColor;
    // oh god effects... effects... effects...
    public static GameObject muzzlefx_acanthi;
    public static GameObject hitfx_acanthi;
    public static GameObject tracerfx_acanthi;
    public static GameObject muzzlefx_borbo;
    public static GameObject hitfx_borbo;
    public static GameObject tracerfx_borbo;
    public static GameObject hitfx_bread;
    public static GameObject muzzlefx_shortcake;
    public static GameObject hitfx_shortcake;
    public static GameObject tracerfx_shortcake;
    public static GameObject muzzlefx_snowtime;
    public static GameObject hitfx_snowtime;
    public static GameObject tracerfx_snowtime;
    public static GameObject muzzlefx_rainbow;
    public static GameObject hitfx_rainbow;
    public static GameObject tracerfx_rainbow;
    public static GameObject orbTurretlingMissileObject;
    public static GameObject orbTurretlingMissileImpactObject;
    public static GameObject orbAcanthilingMissileObject;
    public static GameObject orbAcanthilingMissileImpactObject;
    public static GameObject orbBorbolingMissileObject;
    public static GameObject orbBorbolingMissileImpactObject;
    public static GameObject orbBreadlingMissileObject;
    public static GameObject orbBreadlingMissileImpactObject;
    public static GameObject orbShortcakelingMissileObject;
    public static GameObject orbShortcakelingMissileImpactObject;
    public static GameObject orbSnowtimelingMissileObject;
    public static GameObject orbSnowtimelingMissileImpactObject;
    public static GameObject orbRainbowMissileObject;
    public static GameObject orbRainbowMissileImpactObject;
    public static GameObject orbPlayerMissileObject;
    public static GameObject orbPlayerMissileImpactObject;
    public static GameObject novafx;
    public static GameObject orbShortcakeRetaliateObject;
    public static GameObject orbShortcakeRetaliateImpactObject;
    public static GameObject orbShortcakeRetaliateFriendlyObject;
    public static GameObject orbShortcakeRetaliateFriendlyImpactObject;
    public static GameObject orbShortcakeTauntObject;
    public static GameObject orbShortcakeTauntImpactObject;
    public static GameObject deathfx;
    public static GameObject grenadeObject;
    public static GameObject grenadePlayerObject;
    public static GameObject grenadeGhostObject;
    public static GameObject grenadeImpactObject;
    public static GameObject grenadeImpactRainbowObject;
    public static GameObject effectPrefabObject;
    public static GameObject hitEffectPrefabObject;
    public static GameObject tracerEffectPrefabObject;
    public static GameObject muzzleflashEffectObject;
    public static GameObject projectileObject;
    public static GameObject projectileGhostObject;
    public static GameObject projectileExplosionObject;
    public static GameObject HaloMuzzleFlashObject;
    public static GameObject HaloTracerObject;
    public static GameObject HaloHitObject;
    public static GameObject HaloorbEffectObject;
    public static GameObject SwarmNeedlerOrb;
    public static GameObject SwarmNeedlerImpact;
    public static GameObject SwarmNeedlerExpire;
    public static GameObject SwarmNeedlerMuzzle;
    public static GameObject SwarmNeedlerSuperCombine;
    public static BuffDef SwarmlingMeleeArmorStrip;
    public static BuffDef SwarmlingMeleeBarrierHandler;
    public static BuffDef SwarmlingMeleeBarrierDecayDelayHandler;
    public static BuffDef SwarmlingArmorSteal;
    public static BuffDef SwarmlingNeedlerDebuff;
    public static void AddCustomSkills()
    {
        Log.Debug("Adding SnowtimeToybox Custom Skills...");

        ArtiPassiveFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/ArtificerHiddenPassiveFamily.asset");
        ArtiNoTurretSkill = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/ArtificerNoTurretling.asset");
        ArtiNoTurretSkill.activationState = new SerializableEntityStateType(typeof(Idle));
        ArtiNoTurretSkill.activationStateMachineName = "gorp";
        ArtiTurretSkill = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/ArtificerTurretling.asset");
        ArtiTurretSkill.activationState = new SerializableEntityStateType(typeof(Idle));
        ArtiTurretSkill.activationStateMachineName = "gorp";
        PassiveDemoTurretSkill = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/ArtificerDemoTurretling.asset");
        PassiveDemoTurretSkill.activationState = new SerializableEntityStateType(typeof(Idle));
        PassiveDemoTurretSkill.activationStateMachineName = "gorp";
        ContentAddition.AddSkillFamily(ArtiPassiveFamily);
        ContentAddition.AddSkillDef(ArtiNoTurretSkill);
        ContentAddition.AddSkillDef(ArtiTurretSkill);
        ContentAddition.AddSkillDef(PassiveDemoTurretSkill);

        if (SnowtimeToyboxMod.TurretlingArtificerPassive.Value)
        {
            string[] bodyNames = SnowtimeToyboxMod.TurretlingPassives.Value.Split(";");

            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += ContentManagerOnonContentPacksAssigned;
            void ContentManagerOnonContentPacksAssigned(ReadOnlyArray<ReadOnlyContentPack> obj)
            {
                foreach (var readOnly in obj)
                {
                    foreach (var bodyPrefab in readOnly.bodyPrefabs)
                    {
                        try
                        {
                            foreach (string bodyNameAndTurretlingName in bodyNames)
                            {
                                string bodyName = bodyNameAndTurretlingName.Split(",")[0];
                                if (bodyName != bodyPrefab.name) continue;

                                string turretlingName = bodyNameAndTurretlingName.Split(",")[1];
                                Log.Debug($"{bodyName} has turretling friend .,,. Its fragile so be careful!");

                                if (bodyPrefab.name == bodyName)
                                {
                                    GenericSkill Turretling = bodyPrefab.gameObject.AddComponent<GenericSkill>();

                                    var newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                                    (newFamily as ScriptableObject).name = bodyPrefab.name.Replace(" (UnityEngine.GameObject)", "").ToString() + "TurretlingFamily";
                                    newFamily.variants = [];
                                    foreach (var variant in ArtiPassiveFamily.variants)
                                    {
                                        Array.Resize(ref newFamily.variants, newFamily.variants.Length + 1);

                                        newFamily.variants[^1] = new SkillFamily.Variant
                                        {
                                            skillDef = variant.skillDef,
                                            unlockableDef = variant.unlockableDef,
                                            viewableNode = new ViewablesCatalog.Node(variant.skillDef.skillNameToken, false, null)
                                        };
                                    }

                                    ContentAddition.AddSkillFamily(newFamily);
                                    Turretling._skillFamily = newFamily;
                                    Turretling.skillName = "Turretling";
                                    DroneTechRepairQueue RepairQueue = bodyPrefab.gameObject.AddComponent<DroneTechRepairQueue>();
                                    RepairQueue.healRate = 0.05f;

                                    Log.Debug($" body prefab name {bodyPrefab.name}");
                                    LanguageAPI.Add($"TURRETLING_{bodyPrefab.name.ToUpper()}_NAME", turretlingName);
                                }
                                else
                                {
                                    Log.Warning($"unables to find body {bodyName} !!!");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Warning(e);
                        }
                    }
                }
            }
        }

        GameObject DroneTechBodyPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.DroneTechBody_prefab).WaitForCompletion();
        SkillLocator skillLocator = DroneTechBodyPrefab.GetComponent<SkillLocator>();
        SkillFamily skillFamily = skillLocator.primary.skillFamily;
        SnowtimePlasmaRifleSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/entskilldefFirePlasmaRifle.asset");

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

        // Turretlings!
        SkillDef DroneTechTurretlingSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/DroneTechTurretling.asset");
        DroneTechTurretlingSkillDef.activationState = new SerializableEntityStateType(typeof(Idle));
        DroneTechTurretlingSkillDef.activationStateMachineName = "gorp";
        SkillDef DroneTechTurretlingDemoSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/DroneTechTurretlingDemo.asset");
        DroneTechTurretlingDemoSkillDef.activationState = new SerializableEntityStateType(typeof(Idle));
        DroneTechTurretlingDemoSkillDef.activationStateMachineName = "gorp";
        foreach (GenericSkill genericSkill in DroneTechBodyPrefab.GetComponents<GenericSkill>())
        {
            if (genericSkill.skillName == "Drone1")
            {
                Log.Debug("Found Operator Passive SkillFamily 1!");
                Array.Resize(ref genericSkill.skillFamily.variants, genericSkill.skillFamily.variants.Length + 2);
                genericSkill.skillFamily.variants[^1] = new SkillFamily.Variant
                {
                    skillDef = DroneTechTurretlingSkillDef,
                    viewableNode = new ViewablesCatalog.Node(DroneTechTurretlingSkillDef.skillNameToken, false)
                };
                genericSkill.skillFamily.variants[^2] = new SkillFamily.Variant
                {
                    skillDef = DroneTechTurretlingDemoSkillDef,
                    viewableNode = new ViewablesCatalog.Node(DroneTechTurretlingDemoSkillDef.skillNameToken, false)
                };
            }
            else if (genericSkill.skillName == "Drone2")
            {
                Log.Debug("Found Operator Passive SkillFamily 2!");
                Array.Resize(ref genericSkill.skillFamily.variants, genericSkill.skillFamily.variants.Length + 2);
                genericSkill.skillFamily.variants[^1] = new SkillFamily.Variant
                {
                    skillDef = DroneTechTurretlingSkillDef,
                    viewableNode = new ViewablesCatalog.Node(DroneTechTurretlingSkillDef.skillNameToken, false)
                };
                genericSkill.skillFamily.variants[^2] = new SkillFamily.Variant
                {
                    skillDef = DroneTechTurretlingDemoSkillDef,
                    viewableNode = new ViewablesCatalog.Node(DroneTechTurretlingDemoSkillDef.skillNameToken, false)
                };
            }
        }
        ContentAddition.AddSkillDef(DroneTechTurretlingSkillDef);
        ContentAddition.AddSkillDef(DroneTechTurretlingDemoSkillDef);

        GameObject PlayerMaster = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Core.PlayerMaster_prefab).WaitForCompletion();
        PlayerMaster.AddComponent<TurretlingRainbow>();
    }
    public static void AddCustomAllies()
    {
        SnowtimeToyboxMod.bodyFlags = new CharacterBody.BodyFlags();
        if (SnowtimeToyboxMod.FriendlyTurretImmuneVoidDeath.Value)
        {
            SnowtimeToyboxMod.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE;
        }
        if (SnowtimeToyboxMod.FriendlyTurretFallImmunity.Value)
        {
            SnowtimeToyboxMod.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
        }
        if (SnowtimeToyboxMod.FriendlyTurretDrone.Value)
        {
            SnowtimeToyboxMod.bodyFlags |= CharacterBody.BodyFlags.Drone;
        }

        IEnumerable<Type> friendlyTurrets = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(FriendlyTurretBase)));
        foreach (Type friendlyTurret in friendlyTurrets)
        {
            FriendlyTurretBase turret = (FriendlyTurretBase)System.Activator.CreateInstance(friendlyTurret);
            turret.Initalization();
            turret.ContentAdditionFuncs();
            turret.StageInteractableFuncs();
            turret.PostInit();
        }

        BlasterScepterColor1 = ColorsAPI.RegisterDamageColor(new Color(1f, 0.1f, 0.1f));
        BlasterScepterColor2 = ColorsAPI.RegisterDamageColor(new Color(0.1f, 1f, 0.1f));
        BlasterScepterColor3 = ColorsAPI.RegisterDamageColor(new Color(0.1f, 0.7f, 1f));
        NeedlerColor = ColorsAPI.RegisterDamageColor(new Color(1f, 0.2f, 1f));

        // add turretling
        Log.Debug("Defining Turretling(s)...");
        string turretlingPath = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/";

        FriendlyTurretTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "_TurretlingBody.prefab");
        FriendlyTurretTurretlingBodyRemoteOp = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "_TurretlingBodyRemoteOp.prefab");
        // update stats and components
        FriendlyTurretTurretlingBody.GetComponent<CharacterBody>().baseDamage = SnowtimeToyboxMod.TurretlingBaseDamage.Value;
        FriendlyTurretTurretlingBody.GetComponent<CharacterBody>().levelDamage = SnowtimeToyboxMod.TurretlingBaseDamagePerLevel.Value;
        FriendlyTurretTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
        FriendlyTurretTurretlingBodyRemoteOp.GetComponent<CharacterBody>().baseDamage = SnowtimeToyboxMod.TurretlingBaseDamage.Value;
        FriendlyTurretTurretlingBodyRemoteOp.GetComponent<CharacterBody>().levelDamage = SnowtimeToyboxMod.TurretlingBaseDamagePerLevel.Value;
        FriendlyTurretTurretlingBodyRemoteOp.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
        FriendlyTurretTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "_TurretlingMaster.prefab");
        FriendlyTurretTurretlingPrimarySkillFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingPrimaryFamily.asset");
        FriendlyTurretTurretlingPrimaryMinionSkillFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingPrimaryFamilyMinion.asset");
        FriendlyTurretTurretlingPrimarySkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Primary.asset");
        FriendlyTurretTurretlingPrimaryMinionSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Primary_Minion.asset");
        FriendlyTurretTurretlingPrimarySkillDef.activationState = new SerializableEntityStateType(typeof(TurretlingBlaster));
        FriendlyTurretTurretlingPrimaryMinionSkillDef.activationState = new SerializableEntityStateType(typeof(TurretlingBlaster));
        FriendlyTurretTurretlingSecondarySkillFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingSecondaryFamily.asset");
        FriendlyTurretTurretlingSecondaryAltSkillFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingSecondaryFamilyAlt.asset");
        FriendlyTurretTurretlingSecondaryPlayerSkillFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingSecondaryFamilyPlayer.asset");
        FriendlyTurretTurretlingSecondarySkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Secondary.asset");
        FriendlyTurretTurretlingSecondarySkillDef.activationState = new SerializableEntityStateType(typeof(TurretlingMissile));
        FriendlyTurretTurretlingSecondaryAltSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_SecondaryAlt.asset");
        FriendlyTurretTurretlingSecondaryAltSkillDef.activationState = new SerializableEntityStateType(typeof(TurretlingNeedler));
        FriendlyTurretTurretlingUtilSkillFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingUtilityFamily.asset");
        FriendlyTurretTurretlingUtilSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/TurretlingShenanigans.asset");
        FriendlyTurretTurretlingUtilSkillDef.activationState = new SerializableEntityStateType(typeof(Shenanigans));
        FriendlyTurretTurretlingDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(turretlingPath + "_Turretling.asset");
        FriendlyTurretTurretlingMaster.AddComponent<TurretlingRainbow>();
        ContentAddition.AddDroneDef(FriendlyTurretTurretlingDef);
        ContentAddition.AddEntityState(typeof(TurretlingDeath), out _);
        ContentAddition.AddEntityState(typeof(TurretlingBlaster), out _);
        ContentAddition.AddEntityState(typeof(TurretlingMissile), out _);
        ContentAddition.AddEntityState(typeof(TurretlingNeedler), out _);
        ContentAddition.AddBody(FriendlyTurretTurretlingBody);
        ContentAddition.AddBody(FriendlyTurretTurretlingBodyRemoteOp);

        // erm
        ContentAddition.AddMaster(FriendlyTurretTurretlingMaster);
        ContentAddition.AddSkillFamily(FriendlyTurretTurretlingPrimarySkillFamily);
        ContentAddition.AddSkillFamily(FriendlyTurretTurretlingPrimaryMinionSkillFamily);
        ContentAddition.AddSkillDef(FriendlyTurretTurretlingPrimarySkillDef);
        ContentAddition.AddSkillDef(FriendlyTurretTurretlingPrimaryMinionSkillDef);
        ContentAddition.AddSkillFamily(FriendlyTurretTurretlingSecondarySkillFamily);
        ContentAddition.AddSkillFamily(FriendlyTurretTurretlingSecondaryAltSkillFamily);
        ContentAddition.AddSkillFamily(FriendlyTurretTurretlingSecondaryPlayerSkillFamily);
        ContentAddition.AddSkillDef(FriendlyTurretTurretlingSecondarySkillDef);
        ContentAddition.AddSkillDef(FriendlyTurretTurretlingSecondaryAltSkillDef);

        
        FriendlyTurretTurretlingPrimaryMeleeSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Primary_Melee.asset");
        FriendlyTurretTurretlingPrimaryMeleeSkillDef.activationState = new SerializableEntityStateType(typeof(FireTurretlingMeleeBeam));
        ContentAddition.AddSkillDef(FriendlyTurretTurretlingPrimaryMeleeSkillDef);
        ContentAddition.AddEntityState(typeof(FireTurretlingMeleeBeam), out _);
        FriendlyTurretTurretlingPrimaryMeleeMinionSkillFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingPrimaryFamilyMeleeMinion.asset");
        FriendlyTurretTurretlingPrimaryMeleeMinionSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Primary_Melee_Minion.asset");
        FriendlyTurretTurretlingPrimaryMeleeMinionSkillDef.activationState = new SerializableEntityStateType(typeof(FireTurretlingMeleeBeam));
        ContentAddition.AddSkillFamily(FriendlyTurretTurretlingPrimaryMeleeMinionSkillFamily);
        ContentAddition.AddSkillDef(FriendlyTurretTurretlingPrimaryMeleeMinionSkillDef);
        SwarmlingMeleeArmorStrip = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(turretlingPath + "Buff/TurretlingMeleeArmorDebuff.asset");
        SwarmlingMeleeBarrierHandler = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(turretlingPath + "Buff/TurretlingMeleeBarrierHandler.asset");
        SwarmlingMeleeBarrierDecayDelayHandler = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(turretlingPath + "Buff/TurretlingMeleeBarrierDecayDelayHandler.asset");
        SwarmlingArmorSteal = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(turretlingPath + "Buff/TurretlingMeleeArmorBuff.asset");
        SwarmlingNeedlerDebuff = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(turretlingPath + "Buff/TurretlingNeedlerDebuff.asset");

        // add turretling variants (spawned with a friendly turret)
        AcanthiTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Acanthi.prefab");
        AcanthiTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
        AcanthiTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Acanthi.prefab");
        BorboTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Borbo.prefab");
        BorboTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
        BorboTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Borbo.prefab");
        BreadTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Bread.prefab");
        BreadTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
        BreadTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Bread.prefab");
        ShortcakeTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Shortcake.prefab");
        ShortcakeTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
        ShortcakeTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Shortcake.prefab");
        SnowtimeTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Snowtime.prefab");
        SnowtimeTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
        SnowtimeTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Snowtime.prefab");
        ContentAddition.AddBody(AcanthiTurretlingBody);
        ContentAddition.AddMaster(AcanthiTurretlingMaster);
        ContentAddition.AddBody(BorboTurretlingBody);
        ContentAddition.AddMaster(BorboTurretlingMaster);
        ContentAddition.AddBody(BreadTurretlingBody);
        ContentAddition.AddMaster(BreadTurretlingMaster);
        ContentAddition.AddBody(ShortcakeTurretlingBody);
        ContentAddition.AddMaster(ShortcakeTurretlingMaster);
        ContentAddition.AddBody(SnowtimeTurretlingBody);
        ContentAddition.AddMaster(SnowtimeTurretlingMaster);
        // Arti really quickly 
        ArtiTurretlingDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_HolyTurretling.asset");
        ArtiTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_HolyTurretlingBody.prefab");
        ArtiTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(DTTurretlingDeath));
        ArtiTurretlingBody.AddComponent<PassiveTurretlingUpdateNamePerCharacter>();
        ArtiTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_HolyTurretlingMaster.prefab");
        ArtiTurretlingMaster.AddComponent<TurretlingRainbow>();
        ArtiTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
        ArtiTurretlingBroken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_HolyTurretlingBroken.prefab");
        ArtiTurretlingBroken.AddComponent<PassiveTurretlingUpdateNamePerCharacter>();
        ArtiTurretlingBody.GetComponent<CharacterBody>().baseDamage = (SnowtimeToyboxMod.TurretlingBaseDamage.Value / 1.5f);
        ArtiTurretlingBody.GetComponent<CharacterBody>().levelDamage = (SnowtimeToyboxMod.TurretlingBaseDamagePerLevel.Value / 1.5f);
        ContentAddition.AddDroneDef(ArtiTurretlingDef);
        ContentAddition.AddBody(ArtiTurretlingBody);
        ContentAddition.AddMaster(ArtiTurretlingMaster);
        ContentAddition.AddBody(ArtiTurretlingBroken);
        // Operator
        DTTurretlingDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretling.asset");
        DTTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretlingBody.prefab");
        DTTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(DTTurretlingDeath));
        DTTurretlingBody.GetComponent<DroneCommandReceiver>().droneState = DroneCommandReceiver.DroneState.Idle;
        DTTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretlingMaster.prefab");
        DTTurretlingMaster.AddComponent<TurretlingRainbow>();
        DTTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
        DTTurretlingBroken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretlingBroken.prefab");
        DTTurretlingSkillFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/DTTurretlingSpecialFamily.asset");
        DTTurretlingSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/DTTurretling_Special.asset");
        DTTurretlingSkillDef.activationState = new SerializableEntityStateType(typeof(DTTurretlingRainbowize));
        ContentAddition.AddDroneDef(DTTurretlingDef);
        ContentAddition.AddBody(DTTurretlingBody);
        ContentAddition.AddMaster(DTTurretlingMaster);
        ContentAddition.AddBody(DTTurretlingBroken);
        ContentAddition.AddSkillFamily(DTTurretlingSkillFamily);
        ContentAddition.AddSkillDef(DTTurretlingSkillDef);
        ContentAddition.AddEntityState(typeof(DTTurretlingDeath), out _);
        ContentAddition.AddEntityState(typeof(DTTurretlingRainbowize), out _);
        // Operator Turretling Alt
        DTDemoTurretlingDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretling_Demo.asset");
        DTDemoTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretling_DemoBody.prefab");
        DTDemoTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(DTTurretlingDeath));
        DTDemoTurretlingBody.GetComponent<DroneCommandReceiver>().droneState = DroneCommandReceiver.DroneState.Idle;
        DTDemoTurretlingBody.AddComponent<TurretlingDrunkenRamblingHandler>();
        DTDemoTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretling_DemoMaster.prefab");
        DTDemoTurretlingMaster.AddComponent<TurretlingRainbow>();
        DTDemoTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
        DTDemoTurretlingBroken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretling_DemoBroken.prefab");
        ContentAddition.AddDroneDef(DTDemoTurretlingDef);
        ContentAddition.AddBody(DTDemoTurretlingBody);
        ContentAddition.AddMaster(DTDemoTurretlingMaster);
        ContentAddition.AddBody(DTDemoTurretlingBroken);
        // Passive DemolingTurret really quickly 
        PassiveDemoTurretlingDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DManTurretling.asset");
        PassiveDemoTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DManTurretlingBody.prefab");
        PassiveDemoTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(DTTurretlingDeath));
        PassiveDemoTurretlingBody.AddComponent<PassiveTurretlingUpdateNamePerCharacter>();
        PassiveDemoTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DManTurretlingMaster.prefab");
        PassiveDemoTurretlingMaster.AddComponent<TurretlingRainbow>();
        PassiveDemoTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
        PassiveDemoTurretlingBroken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DManTurretlingBroken.prefab");
        PassiveDemoTurretlingBroken.AddComponent<PassiveTurretlingUpdateNamePerCharacter>();
        PassiveDemoTurretlingBody.GetComponent<CharacterBody>().baseDamage = (SnowtimeToyboxMod.TurretlingBaseDamage.Value / 1.5f);
        PassiveDemoTurretlingBody.GetComponent<CharacterBody>().levelDamage = (SnowtimeToyboxMod.TurretlingBaseDamagePerLevel.Value / 1.5f);
        PassiveDemoTurretlingBody.AddComponent<TurretlingDrunkenRamblingHandler>();
        ContentAddition.AddDroneDef(PassiveDemoTurretlingDef);
        ContentAddition.AddBody(PassiveDemoTurretlingBody);
        ContentAddition.AddMaster(PassiveDemoTurretlingMaster);
        ContentAddition.AddBody(PassiveDemoTurretlingBroken);

        string swarmlingPath = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/";
        SwarmlingDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SurvivorDef>(swarmlingPath + "Swarmling.asset");
        SwarmlingMinionDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(swarmlingPath + "_SwarmTurretling.asset");
        SwarmlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_TurretlingSurvivorBody.prefab");
        SwarmlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_TurretlingSurvivorMonsterMaster.prefab");
        SwarmlingBody.GetComponent<CharacterBody>().hasOneShotProtection = SnowtimeToyboxMod.SwarmlingOSP.Value;
        SwarmlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
        SwarmlingBody.AddComponent<TurretlingMissileTracker>();
        SwarmlingBody.AddComponent<SwarmPlayerOSPHandler>();
        SwarmlingBody.AddComponent<SwarmPlayerSwarmlingTracker>();
        SwarmlingBody.GetComponent<CharacterBody>().baseDamage = SnowtimeToyboxMod.SwarmlingBaseDamage.Value;
        SwarmlingBody.GetComponent<CharacterBody>().levelDamage = SnowtimeToyboxMod.SwarmlingDamagePerLevel.Value;
        SwarmlingBody.GetComponent<CharacterBody>().maxHealth = SnowtimeToyboxMod.SwarmlingBaseMaxHealth.Value;
        SwarmlingBody.GetComponent<CharacterBody>().levelMaxHealth = SnowtimeToyboxMod.SwarmlingMaxHealthPerLevel.Value;
        SwarmlingBody.GetComponent<CharacterBody>().baseRegen = SnowtimeToyboxMod.SwarmlingBaseRegen.Value;
        SwarmlingBody.GetComponent<CharacterBody>().levelRegen = SnowtimeToyboxMod.SwarmlingRegenPerLevel.Value;
        SwarmlingBody.GetComponent<CharacterBody>().baseArmor = SnowtimeToyboxMod.SwarmlingBaseArmor.Value;
        DroneTechRepairQueue repairQueueSwarmling = SwarmlingBody.AddComponent<DroneTechRepairQueue>();
        repairQueueSwarmling.healRate = 0.05f;

        SwarmlingMinionBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretlingBody.prefab");
        SwarmlingMinionBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(DTTurretlingDeath));
        SwarmlingMinionBody.AddComponent<TurretlingMissileTracker>();
        SwarmlingMinionBody.AddComponent<SwarmMinionSwarmlingTeleportHandler>();
        SwarmlingMinionBody.AddComponent<EquipmentSlot>();
        SwarmlingMinionBody.GetComponent<CharacterBody>().baseDamage = (SnowtimeToyboxMod.SwarmlingBaseDamage.Value / SnowtimeToyboxMod.SwarmlingMinionOffenseStatMult.Value);
        SwarmlingMinionBody.GetComponent<CharacterBody>().levelDamage = (SnowtimeToyboxMod.SwarmlingDamagePerLevel.Value / SnowtimeToyboxMod.SwarmlingMinionOffenseStatMult.Value);
        SwarmlingMinionBody.GetComponent<CharacterBody>().maxHealth = (SnowtimeToyboxMod.SwarmlingBaseMaxHealth.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingMinionBody.GetComponent<CharacterBody>().levelMaxHealth = (SnowtimeToyboxMod.SwarmlingMaxHealthPerLevel.Value);
        SwarmlingMinionBody.GetComponent<CharacterBody>().baseRegen = (SnowtimeToyboxMod.SwarmlingBaseRegen.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingMinionBody.GetComponent<CharacterBody>().levelRegen = (SnowtimeToyboxMod.SwarmlingRegenPerLevel.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingMinionBody.GetComponent<CharacterBody>().baseArmor = (SnowtimeToyboxMod.SwarmlingBaseArmor.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingMinionBroken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretlingBroken.prefab");
        SwarmlingMinionMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretlingMaster.prefab");
        SwarmlingMinionMaster.AddComponent<TurretlingRainbow>();
        SwarmlingMinionMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
        Swarmling_PassiveFamily1 = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily1.asset");
        Swarmling_PassiveFamily2 = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily2.asset");
        Swarmling_PassiveFamily3 = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily3.asset");
        Swarmling_PassiveFamily4 = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily4.asset");
        Swarmling_PassiveFamily5 = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily5.asset");
        Swarmling_PassiveFamily6 = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily6.asset");
        SwarmlingSpecialFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/TurretlingSpecialFamilySurvivor.asset");
        SwarmlingUtilityFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/TurretlingUtilityFamilySurvivor.asset");
        SwarmlingPassiveMinion = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(swarmlingPath + "Skills/Swarmling.asset");
        SwarmlingPassiveMinion.activationState = new SerializableEntityStateType(typeof(Idle));
        SwarmlingPassiveMinion.activationStateMachineName = "gorp";
        SwarmlingSpecialSkill = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(swarmlingPath + "Skills/Turretling_SpecialSurvivor.asset");
        SwarmlingSpecialSkill.activationState = new SerializableEntityStateType(typeof(TurretlingEnergyNova));
        SwarmlingUtilitySkill = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(swarmlingPath + "Skills/Turretling_UtilitySurvivor.asset");
        SwarmlingUtilitySkill.activationState = new SerializableEntityStateType(typeof(TurretlingMiniBlinkState));
        SwarmlingUtilityAltSkill = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(swarmlingPath + "Skills/Turretling_UtilityAltSurvivor.asset");
        SwarmlingUtilityAltSkill.activationState = new SerializableEntityStateType(typeof(TurretlingMiniBlinkState));
        ContentAddition.AddSurvivorDef(SwarmlingDef);
        ContentAddition.AddDroneDef(SwarmlingMinionDef);
        ContentAddition.AddBody(SwarmlingBody);
        ContentAddition.AddMaster(SwarmlingMaster);
        ContentAddition.AddBody(SwarmlingMinionBody);
        ContentAddition.AddBody(SwarmlingMinionBroken);
        ContentAddition.AddMaster(SwarmlingMinionMaster);
        ContentAddition.AddSkillFamily(Swarmling_PassiveFamily1);
        ContentAddition.AddSkillFamily(Swarmling_PassiveFamily2);
        ContentAddition.AddSkillFamily(Swarmling_PassiveFamily3);
        ContentAddition.AddSkillFamily(Swarmling_PassiveFamily4);
        ContentAddition.AddSkillFamily(Swarmling_PassiveFamily5);
        ContentAddition.AddSkillFamily(Swarmling_PassiveFamily6);
        ContentAddition.AddSkillFamily(SwarmlingSpecialFamily);
        ContentAddition.AddSkillFamily(SwarmlingUtilityFamily);
        ContentAddition.AddSkillDef(SwarmlingPassiveMinion);
        ContentAddition.AddSkillDef(SwarmlingSpecialSkill);
        ContentAddition.AddSkillDef(SwarmlingUtilitySkill);
        ContentAddition.AddSkillDef(SwarmlingUtilityAltSkill);
        ContentAddition.AddEntityState(typeof(TurretlingEnergyNova), out _);
        ContentAddition.AddEntityState(typeof(TurretlingMiniBlinkState), out _);

        SwarmlingDemoMinionDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(swarmlingPath + "_SwarmTurretling_Demo.asset");
        SwarmlingDemoMinionBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretling_DemoBody.prefab");
        SwarmlingDemoMinionBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(DTTurretlingDeath));
        SwarmlingDemoMinionBody.AddComponent<TurretlingMissileTracker>();
        SwarmlingDemoMinionBody.AddComponent<SwarmMinionSwarmlingTeleportHandler>();
        SwarmlingDemoMinionBody.AddComponent<EquipmentSlot>();
        SwarmlingDemoMinionBody.GetComponent<CharacterBody>().baseDamage = (SnowtimeToyboxMod.SwarmlingBaseDamage.Value / SnowtimeToyboxMod.SwarmlingMinionOffenseStatMult.Value);
        SwarmlingDemoMinionBody.GetComponent<CharacterBody>().levelDamage = (SnowtimeToyboxMod.SwarmlingDamagePerLevel.Value / SnowtimeToyboxMod.SwarmlingMinionOffenseStatMult.Value);
        SwarmlingDemoMinionBody.GetComponent<CharacterBody>().maxHealth = (SnowtimeToyboxMod.SwarmlingBaseMaxHealth.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingDemoMinionBody.GetComponent<CharacterBody>().levelMaxHealth = (SnowtimeToyboxMod.SwarmlingMaxHealthPerLevel.Value);
        SwarmlingDemoMinionBody.GetComponent<CharacterBody>().baseRegen = (SnowtimeToyboxMod.SwarmlingBaseRegen.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingDemoMinionBody.GetComponent<CharacterBody>().levelRegen = (SnowtimeToyboxMod.SwarmlingRegenPerLevel.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingDemoMinionBody.GetComponent<CharacterBody>().baseArmor = (SnowtimeToyboxMod.SwarmlingBaseArmor.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingDemoMinionBody.AddComponent<TurretlingDrunkenRamblingHandler>();
        SwarmlingDemoMinionBroken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretling_DemoBroken.prefab");
        SwarmlingDemoMinionMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretling_DemoMaster.prefab");
        SwarmlingDemoMinionMaster.AddComponent<TurretlingRainbow>();
        SwarmlingDemoMinionMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
        SwarmlingDemoPassiveMinion = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(swarmlingPath + "Skills/Swarmling_Demo.asset");
        SwarmlingDemoPassiveMinion.activationState = new SerializableEntityStateType(typeof(Idle));
        SwarmlingDemoPassiveMinion.activationStateMachineName = "gorp";
        ContentAddition.AddDroneDef(SwarmlingDemoMinionDef);
        ContentAddition.AddBody(SwarmlingDemoMinionBody);
        ContentAddition.AddBody(SwarmlingDemoMinionBroken);
        ContentAddition.AddMaster(SwarmlingDemoMinionMaster);
        ContentAddition.AddSkillDef(SwarmlingDemoPassiveMinion);


        SwarmlingMeleeMinionDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(swarmlingPath + "_SwarmTurretling_Melee.asset");
        SwarmlingMeleeMinionBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretling_MeleeBody.prefab");
        SwarmlingMeleeMinionBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(DTTurretlingDeath));
        SwarmlingMeleeMinionBody.AddComponent<TurretlingMissileTracker>();
        SwarmlingMeleeMinionBody.AddComponent<SwarmMinionSwarmlingTeleportHandler>();
        SwarmlingMeleeMinionBody.AddComponent<EquipmentSlot>();
        SwarmlingMeleeMinionBody.GetComponent<CharacterBody>().baseDamage = (SnowtimeToyboxMod.SwarmlingBaseDamage.Value / SnowtimeToyboxMod.SwarmlingMinionOffenseStatMult.Value);
        SwarmlingMeleeMinionBody.GetComponent<CharacterBody>().levelDamage = (SnowtimeToyboxMod.SwarmlingDamagePerLevel.Value / SnowtimeToyboxMod.SwarmlingMinionOffenseStatMult.Value);
        SwarmlingMeleeMinionBody.GetComponent<CharacterBody>().maxHealth = (SnowtimeToyboxMod.SwarmlingBaseMaxHealth.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingMeleeMinionBody.GetComponent<CharacterBody>().levelMaxHealth = (SnowtimeToyboxMod.SwarmlingMaxHealthPerLevel.Value);
        SwarmlingMeleeMinionBody.GetComponent<CharacterBody>().baseRegen = (SnowtimeToyboxMod.SwarmlingBaseRegen.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingMeleeMinionBody.GetComponent<CharacterBody>().levelRegen = (SnowtimeToyboxMod.SwarmlingRegenPerLevel.Value / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingMeleeMinionBody.GetComponent<CharacterBody>().baseArmor = ((SnowtimeToyboxMod.SwarmlingBaseArmor.Value * 2f) / SnowtimeToyboxMod.SwarmlingMinionDefenseStatMult.Value);
        SwarmlingMeleeMinionBroken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretling_MeleeBroken.prefab");
        SwarmlingMeleeMinionMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretling_MeleeMaster.prefab");
        SwarmlingMeleeMinionMaster.AddComponent<TurretlingRainbow>();
        SwarmlingMeleeMinionMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
        SwarmlingMeleePassiveMinion = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(swarmlingPath + "Skills/Swarmling_Melee.asset");
        SwarmlingMeleePassiveMinion.activationState = new SerializableEntityStateType(typeof(Idle));
        SwarmlingMeleePassiveMinion.activationStateMachineName = "gorp";
        ContentAddition.AddDroneDef(SwarmlingMeleeMinionDef);
        ContentAddition.AddBody(SwarmlingMeleeMinionBody);
        ContentAddition.AddBody(SwarmlingMeleeMinionBroken);
        ContentAddition.AddMaster(SwarmlingMeleeMinionMaster);
        ContentAddition.AddSkillDef(SwarmlingMeleePassiveMinion);

        DemoTurretlingDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(turretlingPath + "_DemoTurretling.asset");
        DemoTurretlingBody = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "_DemoTurretlingBody.prefab");
        // update stats and components
        DemoTurretlingBody.GetComponent<CharacterBody>().baseDamage = SnowtimeToyboxMod.TurretlingBaseDamage.Value;
        DemoTurretlingBody.GetComponent<CharacterBody>().levelDamage = SnowtimeToyboxMod.TurretlingBaseDamagePerLevel.Value;
        DemoTurretlingBody.AddComponent<TurretlingDrunkenRamblingHandler>();
        DemoTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
        DemoTurretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "_DemoTurretlingMaster.prefab");
        DemoTurretlingMaster.AddComponent<TurretlingRainbow>();
        DemoTurretlingPrimaryFamily = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingPrimaryFamilyAlt.asset");
        DemoTurretlingPrimarySkill = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Primary_GL.asset");
        DemoTurretlingPrimarySkill.activationState = new SerializableEntityStateType(typeof(TurretlingGrenadeLauncher));
        ContentAddition.AddEntityState(typeof(TurretlingGrenadeLauncher), out _);
        ContentAddition.AddSkillDef(DemoTurretlingPrimarySkill);
        ContentAddition.AddSkillFamily(DemoTurretlingPrimaryFamily);
        ContentAddition.AddBody(DemoTurretlingBody);
        ContentAddition.AddMaster(DemoTurretlingMaster);
        ContentAddition.AddDroneDef(DemoTurretlingDef);

        ContentAddition.AddEntityState(typeof(Shenanigans), out _);

        // Fix Camera for playable turretlings
        SwarmlingBody.GetComponent<CameraTargetParams>().dontRaycastToPivot = true;
        FriendlyTurretTurretlingBodyRemoteOp.GetComponent<CameraTargetParams>().dontRaycastToPivot = true;
        // Add the Turretling to stages interactable spawncards, as it is a standard walking turret and NOT a Friendly Turret, as its internal name may imply
        FriendlyTurretTurretlingBroken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/_mdlTurretlingBroken.prefab");
        FriendlyTurretTurretlingBroken.AddComponent<TurretlingKillNormalTurrets>();
        FriendlyTurretTurretlingIsc = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<InteractableSpawnCard>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/_iscBrokenTurretling.asset");
        DemoTurretlingBroken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/_DemoTurretlingBroken.prefab");
        DemoTurretlingIsc = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<InteractableSpawnCard>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/_iscBrokenDemoTurretling.asset");
        ContentAddition.AddNetworkedObject(FriendlyTurretTurretlingBroken);
        ContentAddition.AddNetworkedObject(DemoTurretlingBroken);
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

        var directorCardDemoTurretling = new DirectorCard // Borbo Turret Interactable
        {
            spawnCard = DemoTurretlingIsc,
            selectionWeight = 67, // the higher it is, the more common it is
            spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
            minimumStageCompletions = 69420,
            preventOverhead = false
        };

        var directorCardHolderDemoTurretling = new DirectorAPI.DirectorCardHolder
        {
            Card = directorCardDemoTurretling,
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
        turretlingCustomStageList.Add("sunkentombs_wormsworms");
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
            Log.Debug("Adding Turretlings to stage: " + stage);
            DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderFriendlyTurretTurretling, stage);
            DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderDemoTurretling, stage);
        }
        foreach (string stage in turretlingCustomStageList)
        {
            Log.Debug("Adding Turretlings to custom stage (if present, will log regardless): " + stage);
            DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderFriendlyTurretTurretling, DirectorAPI.Stage.Custom, stage);
            DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderDemoTurretling, DirectorAPI.Stage.Custom, stage);
        }

        List<GameObject> turretlingBodies =
        [
            FriendlyTurretTurretlingBody,
            AcanthiTurretlingBody,
            BorboTurretlingBody,
            ShortcakeTurretlingBody,
            SnowtimeTurretlingBody,
            BreadTurretlingBody,
            DTTurretlingBody,
            DTDemoTurretlingBody,
            FriendlyTurretTurretlingBodyRemoteOp,
            ArtiTurretlingBody,
            DemoTurretlingBody,
            PassiveDemoTurretlingBody,
        ];
        foreach (var turretling in turretlingBodies)
        {
            turretling.AddComponent<TurretlingMissileTracker>();
            if (turretling.gameObject.name.Contains("RemoteOp")) continue;
            turretling.AddComponent<EquipmentSlot>();
            if (SnowtimeToyboxMod.TurretlingImmuneVoidDeath.Value)
            {
                turretling.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE;
            }
        }

        FriendlyTurretTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
        BorboTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Borbo_Whitelist";
        SnowtimeTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Snowtime_Whitelist";
        AcanthiTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Acanthi_Whitelist";
        BreadTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Bread_Whitelist";
        ShortcakeTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Shortcake_Whitelist";
        ArtiTurretlingBody.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
        DemoTurretlingBody.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
        PassiveDemoTurretlingBody.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";

        foreach (var turretling in turretlingBodies)
        {
            turretling.GetComponent<CharacterBody>().bodyFlags |= SnowtimeToyboxMod.bodyFlags;
        }

        if (SnowtimeToyboxMod.TurretlingImmuneVoidDeath.Value)
        {
            SwarmlingMinionBody.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE;
            SwarmlingDemoMinionBody.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE;
            SwarmlingMeleeMinionBody.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE;
        }
        SwarmlingMinionBody.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
        SwarmlingDemoMinionBody.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
        SwarmlingMeleeMinionBody.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

        if (SnowtimeToyboxMod.scepterLoaded) return;
        DemoTurretlingPrimarySkill.keywordTokens = new string[1] { "TURRETLING_SKILL7_KEYWORD" };
        FriendlyTurretTurretlingPrimaryMeleeSkillDef.keywordTokens = new string[1] { "TURRETLING_SKILL8_KEYWORD" };
    }
    public static void AddCustomEffects()
    {
        //bwaa.,.,.,,,,,
        string variantPath = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/";
        orbShortcakeRetaliateObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcakeretaliate_orbeffect.prefab");
        orbShortcakeRetaliateImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcakeretaliate_impacteffect.prefab");
        orbShortcakeRetaliateFriendlyObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcakeretaliatefriendly_orbeffect.prefab");
        orbShortcakeRetaliateFriendlyImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcakeretaliatefriendly_impacteffect.prefab");
        orbShortcakeTauntObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcaketaunt_orbeffect.prefab");
        orbShortcakeTauntImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcaketaunt_impacteffect.prefab");
        orbTurretlingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_orbeffect.prefab");
        orbTurretlingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_impacteffect.prefab");
        orbAcanthilingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Orb_Acanthiling.prefab");
        orbAcanthilingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Impact_Acanthiling.prefab");
        orbBorbolingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Orb_Borboling.prefab");
        orbBorbolingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Impact_Borboling.prefab");
        orbBreadlingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Orb_Breadling.prefab");
        orbBreadlingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Impact_Breadling.prefab");
        orbShortcakelingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Orb_Shortcakeling.prefab");
        orbShortcakelingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Impact_Shortcakeling.prefab");
        orbSnowtimelingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Orb_Snowtimeling.prefab");
        orbSnowtimelingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Impact_Snowtimeling.prefab");
        orbRainbowMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Orb__Rainbow.prefab");
        orbRainbowMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Impact__Rainbow.prefab");
        orbPlayerMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Orb__Player.prefab");
        orbPlayerMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx__Missile_Impact__Player.prefab");
        muzzlefx_acanthi = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Muzzleflash_Acanthiling.prefab");
        hitfx_acanthi = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Hitspark_Acanthiling.prefab");
        tracerfx_acanthi = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Tracer_Acanthiling.prefab");
        muzzlefx_borbo = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Muzzleflash_Borboling.prefab");
        hitfx_borbo = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Hitspark_Borboling.prefab");
        tracerfx_borbo = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Tracer_Borboling.prefab");
        hitfx_bread = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Hitspark_Breadling.prefab");
        muzzlefx_shortcake = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Muzzleflash_Shortcakeling.prefab");
        hitfx_shortcake = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Hitspark_Shortcakeling.prefab");
        tracerfx_shortcake = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Tracer_Shortcakeling.prefab");
        muzzlefx_snowtime = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Muzzleflash_Snowtimeling.prefab");
        hitfx_snowtime = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Hitspark_Snowtimeling.prefab");
        tracerfx_snowtime = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Tracer_Snowtimeling.prefab");
        muzzlefx_rainbow = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Muzzleflash__Rainbow.prefab");
        hitfx_rainbow = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Hitspark__Rainbow.prefab");
        tracerfx_rainbow = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(variantPath + "vfx_Tracer__Rainbow.prefab");
        novafx = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/Skills/turretling_novaeffect.prefab");
        deathfx = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_deatheffect.prefab");
        grenadeObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/TurretlingDemoGrenadeProjectile.prefab");
        grenadePlayerObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/TurretlingDemoGrenadeProjectile_Player.prefab");
        grenadeGhostObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/DemoGrenadeGhost.prefab");
        grenadeImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/GrenadeImpact.prefab");
        grenadeImpactRainbowObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/GrenadeImpact_Rainbow.prefab");
        effectPrefabObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Skills/MuzzleflashBorbo.prefab");
        hitEffectPrefabObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Skills/ExplosionBorbo.prefab");
        tracerEffectPrefabObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Skills/TracerBorbo.prefab");
        muzzleflashEffectObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Snowtime/Skills/CryoGaussMuzzleFlash.prefab");
        projectileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Snowtime/Skills/CryoGaussProjectile.prefab");
        projectileGhostObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Snowtime/Skills/CryoGaussProjectileGhost.prefab");
        projectileExplosionObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Snowtime/Skills/CryoGaussExplosion.prefab");
        HaloMuzzleFlashObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/PlasmaRifleMuzzleFlashVFX.prefab");
        HaloTracerObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/TracerPlasmaRifle.prefab");
        HaloHitObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/PlasmaRifleImpactVFX.prefab");
        HaloorbEffectObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/PlasmaRifleOrbEffect.prefab");
        SwarmNeedlerOrb = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_needler_orb.prefab");
        SwarmNeedlerImpact = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_needler_impact.prefab");
        SwarmNeedlerExpire = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_needler_expire.prefab");
        SwarmNeedlerMuzzle = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_needler_muzzle.prefab");
        SwarmNeedlerSuperCombine = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_needler_supercombine.prefab");

        ContentAddition.AddEffect(SwarmNeedlerOrb);
        ContentAddition.AddEffect(SwarmNeedlerImpact);
        ContentAddition.AddEffect(SwarmNeedlerExpire);
        ContentAddition.AddEffect(SwarmNeedlerMuzzle);
        ContentAddition.AddEffect(SwarmNeedlerSuperCombine);

        ContentAddition.AddEffect(muzzleflashEffectObject);
        ContentAddition.AddProjectile(projectileObject);
        ContentAddition.AddEffect(projectileGhostObject);
        ContentAddition.AddEffect(projectileExplosionObject);
        ContentAddition.AddEffect(effectPrefabObject);
        ContentAddition.AddEffect(hitEffectPrefabObject);
        ContentAddition.AddEffect(tracerEffectPrefabObject);
        ContentAddition.AddEffect(HaloMuzzleFlashObject);
        ContentAddition.AddEffect(HaloTracerObject);
        ContentAddition.AddEffect(HaloHitObject);
        ContentAddition.AddEffect(HaloorbEffectObject);
        ContentAddition.AddEffect(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/PlasmaRifleImpactVFXRico.prefab"));
        ContentAddition.AddEffect(BorboCheck.turretUseEffect);
        ContentAddition.AddEffect(deathfx);
        ContentAddition.AddEffect(orbShortcakeRetaliateObject);
        ContentAddition.AddEffect(orbShortcakeRetaliateFriendlyObject);
        ContentAddition.AddEffect(orbShortcakeTauntObject);
        ContentAddition.AddEffect(orbShortcakeRetaliateImpactObject);
        ContentAddition.AddEffect(orbShortcakeRetaliateFriendlyImpactObject);
        ContentAddition.AddEffect(orbShortcakeTauntImpactObject);
        ContentAddition.AddEffect(orbTurretlingMissileObject);
        ContentAddition.AddEffect(orbTurretlingMissileImpactObject);
        ContentAddition.AddEffect(muzzlefx_acanthi);
        ContentAddition.AddEffect(hitfx_acanthi);
        ContentAddition.AddEffect(tracerfx_acanthi);
        ContentAddition.AddEffect(orbAcanthilingMissileObject);
        ContentAddition.AddEffect(orbAcanthilingMissileImpactObject);
        ContentAddition.AddEffect(muzzlefx_borbo);
        ContentAddition.AddEffect(hitfx_borbo);
        ContentAddition.AddEffect(tracerfx_borbo);
        ContentAddition.AddEffect(orbBorbolingMissileObject);
        ContentAddition.AddEffect(orbBorbolingMissileImpactObject);
        ContentAddition.AddEffect(hitfx_bread);
        ContentAddition.AddEffect(orbBreadlingMissileObject);
        ContentAddition.AddEffect(orbBreadlingMissileImpactObject);
        ContentAddition.AddEffect(muzzlefx_shortcake);
        ContentAddition.AddEffect(hitfx_shortcake);
        ContentAddition.AddEffect(tracerfx_shortcake);
        ContentAddition.AddEffect(orbShortcakelingMissileObject);
        ContentAddition.AddEffect(orbShortcakelingMissileImpactObject);
        ContentAddition.AddEffect(muzzlefx_snowtime);
        ContentAddition.AddEffect(hitfx_snowtime);
        ContentAddition.AddEffect(tracerfx_snowtime);
        ContentAddition.AddEffect(orbSnowtimelingMissileObject);
        ContentAddition.AddEffect(orbSnowtimelingMissileImpactObject);
        ContentAddition.AddEffect(muzzlefx_rainbow);
        ContentAddition.AddEffect(hitfx_rainbow);
        ContentAddition.AddEffect(tracerfx_rainbow);
        ContentAddition.AddEffect(orbRainbowMissileObject);
        ContentAddition.AddEffect(orbRainbowMissileImpactObject);
        ContentAddition.AddEffect(orbPlayerMissileObject);
        ContentAddition.AddEffect(orbPlayerMissileImpactObject);
        ContentAddition.AddEffect(novafx);
        ContentAddition.AddEffect(grenadeGhostObject);
        ContentAddition.AddEffect(grenadeImpactObject);
        ContentAddition.AddEffect(grenadeImpactRainbowObject);
        ContentAddition.AddProjectile(grenadeObject);
        ContentAddition.AddProjectile(grenadePlayerObject);
    }
    public static void AddScepterSkills()
    {
        string turretlingPath = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/";
        if (SnowtimeToyboxMod.scepterLoaded)
        {
            FriendlyTurretTurretlingPrimaryScepterSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Primary_Scepter.asset");
            FriendlyTurretTurretlingPrimaryScepterSkillDef.activationState = new SerializableEntityStateType(typeof(TurretlingBlasterScepter));
            FriendlyTurretTurretlingPrimarySkillDef.keywordTokens = new string[1] { "TURRETLING_SKILL1_KEYWORD" };
            DemoTurretlingPrimarySkill.keywordTokens = new string[1] { "TURRETLING_SKILL7_KEYWORD_SCEPTER" };
            FriendlyTurretTurretlingPrimaryMeleeSkillDef.keywordTokens = new string[1] { "TURRETLING_SKILL8_KEYWORD_SCEPTER" };
            FriendlyTurretTurretlingPrimaryScepterMinionSkillDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Primary_Scepter.asset");
            FriendlyTurretTurretlingPrimaryScepterMinionSkillDef.activationState = new SerializableEntityStateType(typeof(TurretlingBlasterScepter));

            ContentAddition.AddSkillDef(FriendlyTurretTurretlingPrimaryScepterSkillDef);
            ContentAddition.AddSkillDef(FriendlyTurretTurretlingPrimaryScepterMinionSkillDef);
            ContentAddition.AddEffect(TurretlingBlasterScepter.muzzlefx_kinetic);
            ContentAddition.AddEffect(TurretlingBlasterScepter.muzzlefx_corrosive);
            ContentAddition.AddEffect(TurretlingBlasterScepter.muzzlefx_energy);
            ContentAddition.AddEffect(TurretlingBlasterScepter.hitfx_kinetic);
            ContentAddition.AddEffect(TurretlingBlasterScepter.hitfx_corrosive);
            ContentAddition.AddEffect(TurretlingBlasterScepter.hitfx_energy);
            ContentAddition.AddEffect(TurretlingBlasterScepter.tracerfx_kinetic);
            ContentAddition.AddEffect(TurretlingBlasterScepter.tracerfx_corrosive);
            ContentAddition.AddEffect(TurretlingBlasterScepter.tracerfx_energy);
            ContentAddition.AddEntityState(typeof(TurretlingBlasterScepter), out _);
            // waow
            AncientScepter.AncientScepterItem.instance?.RegisterScepterSkill(FriendlyTurretTurretlingPrimaryScepterSkillDef, "_TurretlingSurvivorBody", SkillSlot.Primary, 0);
            // for some reason, ancient scepter really does not like skills using the same scepter replace ability. might be related to identical skilldefs; test with duplicate skilldefs.
            AncientScepter.AncientScepterItem.instance?.RegisterScepterSkill(FriendlyTurretTurretlingPrimaryScepterMinionSkillDef, "_SwarmTurretlingBody", SkillSlot.Primary, 0);
        }
    }
}