using System.Collections.Generic;
using EntityStates;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace SnowtimeToybox.FriendlyTurrets;

public class AcanthiFriendlyTurret : FriendlyTurretBase<AcanthiFriendlyTurret>
{
    public override string turretWhitelistString => "FriendTurret_Acanthi_Whitelist";
    public override string turretName => "canthi turret"; // only used for logging atm .,.
    public override string[] riskierStats => ["baseDamage", "5", "baseRegen", "7", "baseArmor", "40", "levelDamage", "1.25", "levelRegen", "1.5", "levelArmor", "4"];

    public static BuffDef AcanthiTurretBuff;
    public static BuffDef AcanthiTurretDebuff;
    
    public override void Initalization()
    {
        base.Initalization();
        string friendDir = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Acanthi/";
        
        broken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_mdlFriendlyTurretAcanthiBroken.prefab");
        body = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretAcanthiBody.prefab");
        bodyRemoteOp = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretAcanthiBodyRemoteOp.prefab");
        master = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretAcanthiMaster.prefab");
        skillFamilies.Add(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(friendDir + "Skills/AcanthiPrimaryFamily.asset"));
        skillFamilies.Add(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(friendDir + "Skills/AcanthiUtilityFamily.asset"));
        
        SkillDef beam = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(friendDir + "Skills/AcanthiLaser.asset");
        beam.activationState = new SerializableEntityStateType(typeof(FireAcanthiBeam));
        skillDefs.Add(beam);
        
        SkillDef silly = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(friendDir + "Skills/AcanthiShenanigans.asset");
        silly.activationState = new SerializableEntityStateType(typeof(Shenanigans));
        skillDefs.Add(silly);

        AcanthiTurretBuff = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(friendDir + "Buff/VampiricDesires.asset");
        AcanthiTurretDebuff = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(friendDir + "Buff/VampiricBleeding.asset");
       
        droneDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(friendDir + "_FriendlyTurretAcanthi.asset");
        interactableSpawnCard = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<InteractableSpawnCard>(friendDir + "_iscBrokenFriendlyTurretAcanthi.asset");
        turretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/_TurretlingMaster_Acanthi.prefab");
    }

    public override void ContentAdditionFuncs()
    {
        base.ContentAdditionFuncs();
        ContentAddition.AddEntityState(typeof(FireAcanthiBeam), out _);
    }
    public override void StageInteractableFuncs()
    {
        base.StageInteractableFuncs();
    }
}