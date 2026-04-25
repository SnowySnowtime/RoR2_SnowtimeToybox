using System.Collections.Generic;
using EntityStates;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace SnowtimeToybox.FriendlyTurrets;

public class BorboFriendlyTurret : FriendlyTurretBase<BorboFriendlyTurret>
{
    public override string turretWhitelistString => "FriendTurret_Borbo_Whitelist";
    public override string turretName => "borbno turret"; // only used for logging atm .,.
    public override string[] riskierStats => ["baseDamage", "25", "baseRegen", "20", "baseArmor", "35", "levelDamage", "5", "levelRegen", "4", "levelArmor", "3"];

    public static BuffDef BorboTurretDebuff;
    
    public override void Initalization()
    {
        base.Initalization();
        string friendDir = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/";
        
        broken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_mdlFriendlyTurretBorboBroken.prefab");
        body = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretBorboBody.prefab");
        bodyRemoteOp = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretBorboBodyRemoteOp.prefab");
        master = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretBorboMaster.prefab");
        skillFamilies.Add(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(friendDir + "Skills/BorboPrimaryFamily.asset"));
        skillFamilies.Add(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(friendDir + "Skills/BorboUtilityFamily.asset"));

        SkillDef borboBlast = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(friendDir + "Skills/BorboBlast.asset");
        borboBlast.activationState = new SerializableEntityStateType(typeof(ChargeBorboLaser));
        droneDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(friendDir + "_FriendlyTurretBorbo.asset");

        SkillDef silly = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(friendDir + "Skills/BorboShenanigans.asset");
        silly.activationState = new SerializableEntityStateType(typeof(Shenanigans));
        skillDefs.Add(silly);
        
        BorboTurretDebuff = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(friendDir + "Buff/BorboTurretDebuff.asset");
       
        droneDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(friendDir + "_FriendlyTurretBorbo.asset");
        interactableSpawnCard = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<InteractableSpawnCard>(friendDir + "_iscBrokenFriendlyTurretBorbo.asset");
        turretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/_TurretlingMaster_Borbo.prefab");
    }

    public override void ContentAdditionFuncs()
    {
        base.ContentAdditionFuncs();
        ContentAddition.AddEntityState(typeof(FireBorboLaser), out _);
        ContentAddition.AddEntityState(typeof(ChargeBorboLaser), out _);
        ContentAddition.AddEffect(FireBorboLaser.effectPrefabObject);
        ContentAddition.AddEffect(FireBorboLaser.hitEffectPrefabObject);
        ContentAddition.AddEffect(FireBorboLaser.tracerEffectPrefabObject);
    }
    public override void StageInteractableFuncs()
    {
        base.StageInteractableFuncs();
    }
}