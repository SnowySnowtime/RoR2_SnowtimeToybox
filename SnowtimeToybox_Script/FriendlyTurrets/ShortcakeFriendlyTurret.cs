using System.Collections.Generic;
using EntityStates;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace SnowtimeToybox.FriendlyTurrets;

public class ShortcakeFriendlyTurret : FriendlyTurretBase<ShortcakeFriendlyTurret>
{
    public override string turretWhitelistString => "FriendTurret_Shortcake_Whitelist";
    public override string turretName => "shortscake turret"; // only used for logging atm .,.
    public override string[] riskierStats => ["baseDamage", "20", "baseRegen", "25", "baseArmor", "60", "levelDamage", "4", "levelRegen", "5", "levelArmor", "15"];
    
    public static BuffDef ShortcakeTurretBuff;

    public override void Initalization()
    {
        base.Initalization();
        string friendDir = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/";
        
        broken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_mdlFriendlyTurretShortcakeBroken.prefab");
        body = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretShortcakeBody.prefab");
        bodyRemoteOp = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretShortcakeBodyRemoteOp.prefab");
        master = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretShortcakeMaster.prefab");
        skillFamilies.Add(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(friendDir + "Skills/ShortcakePrimaryFamily.asset"));
        skillFamilies.Add(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(friendDir + "Skills/ShortcakeUtilityFamily.asset"));
            
        SkillDef taunt = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(friendDir + "Skills/ShortcakeTaunt.asset");
        taunt.activationState = new SerializableEntityStateType(typeof(ShortcakeTaunt));
        skillDefs.Add(taunt);
        
        SkillDef silly = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(friendDir + "Skills/ShortcakeShenanigans.asset");
        silly.activationState = new SerializableEntityStateType(typeof(Shenanigans));
        skillDefs.Add(silly);
        
        ShortcakeTurretBuff = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(friendDir + "Buff/ShortcakeTurretBuff.asset");
       
        droneDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(friendDir + "_FriendlyTurretShortcake.asset");
        interactableSpawnCard = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<InteractableSpawnCard>(friendDir + "_iscBrokenFriendlyTurretShortcake.asset");
        turretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/_TurretlingMaster_Shortcake.prefab");
    }

    public override void ContentAdditionFuncs()
    {
        base.ContentAdditionFuncs();
        ContentAddition.AddEntityState(typeof(ShortcakeTaunt), out _);
        ContentAddition.AddEffect(SnowtimeOrbs.orbShortcakeRetaliateObject);
        ContentAddition.AddEffect(SnowtimeOrbs.orbShortcakeRetaliateFriendlyObject);
        ContentAddition.AddEffect(SnowtimeOrbs.orbShortcakeTauntObject);
        ContentAddition.AddEffect(SnowtimeOrbs.orbShortcakeRetaliateImpactObject);
        ContentAddition.AddEffect(SnowtimeOrbs.orbShortcakeRetaliateFriendlyImpactObject);
        ContentAddition.AddEffect(SnowtimeOrbs.orbShortcakeTauntImpactObject);
    }
    public override void StageInteractableFuncs()
    {
        base.StageInteractableFuncs();
    }
}