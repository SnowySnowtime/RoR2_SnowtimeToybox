using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace SnowtimeToybox.Items;

public class RainbowizerPowerup : ItemBase<RainbowizerPowerup>
{
    public static ConfigEntry<float> AttackSpeedMod;
    public static ConfigEntry<float> CritDMGMod;
    public static ConfigEntry<float> DamageMod;
    public static ConfigEntry<float> CoolDownMod;
    public static ConfigEntry<float> LuckMod;
    public static ConfigEntry<bool> AdditionalMissiles;
    public static ConfigEntry<bool> PreventBarrierDecay;
    public override ItemDef ItemDef => SnowtimeToyboxMod._stitemAssetBundle.LoadAsset<ItemDef>(@"Assets/SnowtimeMod/Assets/Items/Hidden/RainbowizerPowerUp.asset");

    public override ItemDisplayRuleDict CreateItemDisplayRules()
    {
        return new ItemDisplayRuleDict();
    }

    public override void Init(ConfigFile config)
    {
        CreateConfig(config);
        CreateLang();
        CreateItem();
        Hooks();
    }

    public override void CreateConfig(ConfigFile config)
    {
        AttackSpeedMod = config.Bind<float>("Operator Turretling: Rainbowizer Powerup", "Attack Speed Modifier (Mult)", 7.5f, "Attack speed modifier when Rainbowizer is active");
        CritDMGMod = config.Bind<float>("Operator Turretling: Rainbowizer Powerup", "Crit Damage Modifier (Mult)", 1f, "Critical Strike damage modifier when Rainbowizer is active");
        DamageMod = config.Bind<float>("Operator Turretling: Rainbowizer Powerup", "Damage Modifier (Mult)", 1f, "Damage modifier when Rainbowizer is active");
        CoolDownMod = config.Bind<float>("Operator Turretling: Rainbowizer Powerup", "Pixi Launcher Cooldown Modifier (Mult)", 0.25f, "Pixi Launcher Cooldown modifier when Rainbowizer is active. Excludes Rainbowizer");
        LuckMod = config.Bind<float>("Operator Turretling: Rainbowizer Powerup", "Luck Modifier (Add)", 1f, "Luck modifier when Rainbowizer is active");
        AdditionalMissiles = config.Bind<bool>("Operator Turretling: Rainbowizer Powerup", "Additional Missiles", true, "If true, fire additional missiles with Pixi Launcher without the need of a Pocket I.C.B.M");
        PreventBarrierDecay = config.Bind<bool>("Operator Turretling: Rainbowizer Powerup", "Barrier Decay", true, "If true, barrier no longer decays naturally");
    }

    public override void Hooks()
    {
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStats;
    }
    private void RecalculateStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
        if (sender)
        {
            var itemCount = GetCount(sender);
            if (itemCount != 0 && sender.healthComponent != null)
            {
                args.attackSpeedMultAdd += AttackSpeedMod.Value;
                args.damageMultAdd += DamageMod.Value;
                args.critDamageMultAdd *= CritDMGMod.Value;
                args.shouldFreezeBarrier = PreventBarrierDecay.Value;
                args.luckAdd += LuckMod.Value;
                args.secondarySkill.cooldownReductionMultAdd += CoolDownMod.Value;
            }
        }
    }
}