using System.Collections.Generic;
using System.Linq;
using Mono.Collections.Generic;
using R2API;
using RoR2;
using Ror2AggroTools;
using SnowtimeToybox.Components;
using SnowtimeToybox.FriendlyTurrets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SceneDirector = On.RoR2.SceneDirector;

namespace SnowtimeToybox;

public class Hooks
{
    public static void Hook()
    {
        Run.onRunStartGlobal += (Run run) =>
        {
            SnowtimeToyboxMod.Legendary = false;
            if (run.selectedDifficulty == SnowtimeToyboxMod.SnowtimeLegendaryDiffIndex)
            {
                SnowtimeToyboxMod.Legendary = true;
                CharacterMaster.onStartGlobal += CharacterMaster_OnStartGlobal;
            }
        };

        Run.onRunDestroyGlobal += (Run run) =>
        {
            SnowtimeToyboxMod.Legendary = false;
            CharacterMaster.onStartGlobal -= CharacterMaster_OnStartGlobal;
        };

        On.RoR2.GlobalEventManager.OnHitEnemy += JustLetMeDamageMyFoesPleaseThankYou;
            
        On.RoR2.SceneDirector.Start += SceneDirectorOnStart;
        On.RoR2.CharacterBody.FixedUpdate += ShortcakeTurretHandler;
        
        Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Turret1Broken.prefab").Completed += handle =>
        {
            handle.Result.AddComponent<TurretlingKillNormalTurrets>();
        };
    }

    private static void JustLetMeDamageMyFoesPleaseThankYou(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
    {
        orig(self, damageInfo, victim);

        if (victim == null) return;
        if (damageInfo.HasModdedDamageType(SnowtimeToyboxMod.HaloRicochetOnHit))
        {
            //Log.Debug("OnHitEnemy DamageInfo: " + damageInfo + " Attacker: " + damageInfo.attacker + " Attacker Team: " + damageInfo.attacker.GetComponent<TeamComponent>().teamIndex + " Victim Body: " + victim.GetComponent<CharacterBody>());
            SnowtimeHaloRicochetOrb.CreateHaloRicochetOrb(damageInfo, damageInfo.attacker.GetComponent<TeamComponent>().teamIndex, victim.GetComponent<CharacterBody>());
        }
        if (damageInfo.HasModdedDamageType(SnowtimeToyboxMod.BorboSuperDebuffOnHit))
        {
            victim.GetComponent<CharacterBody>().AddTimedBuff(BorboFriendlyTurret.BorboTurretDebuff, 3);
        }
    }

    private static void ShortcakeTurretHandler(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
    {
        orig(self);

        if (self.teamComponent.teamIndex == TeamIndex.Player)
        {
            if (!self.baseNameToken.Contains("FRIENDLYTURRET_")) return;

            if (self.baseNameToken.Contains("FRIENDLYTURRET_SHORTCAKE"))
            {
                if (!self.HasBuff(ShortcakeFriendlyTurret.ShortcakeTurretBuff))
                {
                    self.AddBuff(ShortcakeFriendlyTurret.ShortcakeTurretBuff);
                    if(SnowtimeToyboxMod.FriendlyTurretShortcakeAggroType.Value)
                    {
                        self.AddBuff(AggroToolsPlugin.priorityAggro);
                    }
                }
            }

            if (self.baseNameToken.Contains("FRIENDLYTURRET_ACANTHI"))
            {
                if (!self.HasBuff(AcanthiFriendlyTurret.AcanthiTurretBuff))
                {
                    self.AddBuff(AcanthiFriendlyTurret.AcanthiTurretBuff);
                }
            }

            if (self.baseNameToken.Contains("FRIENDLYTURRET_BREAD"))
            {
                if (!self.HasBuff(BreadFriendlyTurret.BreadTurretBuffPassive))
                {
                    self.AddBuff(BreadFriendlyTurret.BreadTurretBuffPassive);
                }
            }
        }
    }

    private static void SceneDirectorOnStart(SceneDirector.orig_Start orig, RoR2.SceneDirector self)
    {
        orig(self);
        
        if (!NetworkServer.active) return;
            
        #region turretSpawns
        Dictionary<Vector3, Quaternion> stagePositions = new Dictionary<Vector3, Quaternion>();
        string currStage = SceneManager.GetActiveScene().name;
        switch (currStage)
        {
            // Stage 1
            case "golemplains":
                stagePositions.Add(new Vector3(190.3899f, -86.776f, -135.183f), Quaternion.Euler(0f, 59.69361f, 0f));
                stagePositions.Add(new Vector3(-91.03493f, -158.2584f, 120.482f), Quaternion.Euler(0f, 205.1253f, 0f));
                break;
            case "golemplains2":
                stagePositions.Add(new Vector3(31.92904f, 82.58593f, -64.25964f), Quaternion.Euler(0f, 0.5191317f, 0f));
                stagePositions.Add(new Vector3(316.7331f, 44.01719f, -29.57135f), Quaternion.Euler(0f, 336.1666f, 0f));
                break;
            case "blackbeach":
                stagePositions.Add(new Vector3(-24.12934f, -174.6557f, -387.6982f), Quaternion.Euler(0f, 182.4907f, 0f));
                stagePositions.Add(new Vector3(-4.805637f, -204.4452f, -12.48376f), Quaternion.Euler(0f, 152.4669f, 0f));
                break;
            case "blackbeach2":
                stagePositions.Add(new Vector3(13.17906f, 87.47905f, -133.2737f), Quaternion.Euler(0f, 323.4963f, 0f));
                stagePositions.Add(new Vector3(-174.5754f, 17.0354f, -96.01575f), Quaternion.Euler(0f, 23.97439f, 0f));
                break;
            case "lakes":
                stagePositions.Add(new Vector3(86.66148f, 15.45605f, 118.2119f), Quaternion.Euler(0f, 195.2255f, 0f));
                stagePositions.Add(new Vector3(-160.3739f, 0.8921404f, -86.5373f), Quaternion.Euler(0f, 21.13958f, 0f));
                break;
            case "lakesnight":
                stagePositions.Add(new Vector3(86.66148f, 15.45605f, 118.2119f), Quaternion.Euler(0f, 195.2255f, 0f));
                stagePositions.Add(new Vector3(-160.3739f, 0.8921404f, -86.5373f), Quaternion.Euler(0f, 21.13958f, 0f));
                break;
            case "snowyforest":
                stagePositions.Add(new Vector3(106.6341f, 68.99653f, 214.1347f), Quaternion.Euler(0f, 241.8315f, 0f));
                stagePositions.Add(new Vector3(101.7054f, 36.81925f, 28.63689f), Quaternion.Euler(0f, 273.5906f, 0f));
                break;
            case "village":
                stagePositions.Add(new Vector3(-116.2186f, -4.684125f, -248.0959f), Quaternion.Euler(0f, 47.27631f, 0f));
                stagePositions.Add(new Vector3(234.6463f, 48.88169f, 22.69479f), Quaternion.Euler(0f, 232.9216f, 0f));
                break;
            case "villagenight":
                stagePositions.Add(new Vector3(-116.2186f, -4.684125f, -248.0959f), Quaternion.Euler(0f, 47.27631f, 0f));
                stagePositions.Add(new Vector3(234.6463f, 48.88169f, 22.69479f), Quaternion.Euler(0f, 232.9216f, 0f));
                break;
            // Stage 2
            case "goolake":
                stagePositions.Add(new Vector3(116.9908f, -91.04021f, -7.358109f), Quaternion.Euler(0f, 170.321f, 0f));
                stagePositions.Add(new Vector3(193.6468f, -122.2093f, 81.47984f), Quaternion.Euler(0f, 134.2626f, 0f));
                break;
            case "foggyswamp":
                stagePositions.Add(new Vector3(21.54059f, -97.81608f, -48.92618f), Quaternion.Euler(0f, 294.0464f, 0f));
                stagePositions.Add(new Vector3(321.5318f, -160.4051f, -323.4489f), Quaternion.Euler(0f, 78.9093f, 0f));
                break;
            case "ancientloft":
                stagePositions.Add(new Vector3(80.94776f, 30.03779f, -130.4797f), Quaternion.Euler(0f, 125.4776f, 0f));
                stagePositions.Add(new Vector3(-71.81654f, 27.19449f, 83.8267f), Quaternion.Euler(0f, 93.52406f, 0f));
                break;
            case "nest":
                stagePositions.Add(new Vector3(-188.9024f, 10.64726f, 30.56748f), Quaternion.Euler(0f, 201.877f, 0f));
                stagePositions.Add(new Vector3(-129.1165f, 128.4458f, -108.6409f), Quaternion.Euler(0f, 185.5041f, 0f));
                stagePositions.Add(new Vector3(-85.97453f, 173.3482f, 99.71858f), Quaternion.Euler(0f, 78.47749f, 0f));
                break;
            case "lemuriantemple":
                stagePositions.Add(new Vector3(-4.580923f, 27.38782f, 3.959016f), Quaternion.Euler(0f, 9.96745f, 0f));
                stagePositions.Add(new Vector3(101.6959f, -27.22142f, 163.6068f), Quaternion.Euler(0f, 232.0676f, 0f));
                break;
            // Stage 3
            case "frozenwall":
                stagePositions.Add(new Vector3(82.25099f, 3.083081f, 64.41711f), Quaternion.Euler(0f, 330.366f, 0f));
                stagePositions.Add(new Vector3(96.06144f, 115.7271f, 156.4782f), Quaternion.Euler(0f, 199.4881f, 0f));
                break;
            case "wispgraveyard":
                stagePositions.Add(new Vector3(-133.0515f, -31.09213f, -118.661f), Quaternion.Euler(0f, 24.76236f, 0f));
                stagePositions.Add(new Vector3(79.80575f, 30.06047f, 56.93044f), Quaternion.Euler(0f, 52.20761f, 0f));
                break;
            case "sulfurpools":
                stagePositions.Add(new Vector3(-183.9088f, 10.77933f, 32.21773f), Quaternion.Euler(0f, 181.6929f, 0f));
                stagePositions.Add(new Vector3(174.8141f, 28.67295f, 33.7763f), Quaternion.Euler(0f, 308.1877f, 0f));
                break;
            case "ironalluvium":
                stagePositions.Add(new Vector3(13.42801f, 125.7309f, -188.0156f), Quaternion.Euler(0f, 43.0607f, 0f));
                stagePositions.Add(new Vector3(167.1385f, 149.9529f, 95.30859f), Quaternion.Euler(0f, 205.8401f, 0f));
                break;
            case "ironalluvium2":
                stagePositions.Add(new Vector3(13.42801f, 125.7309f, -188.0156f), Quaternion.Euler(0f, 43.0607f, 0f));
                stagePositions.Add(new Vector3(167.1385f, 149.9529f, 95.30859f), Quaternion.Euler(0f, 205.8401f, 0f));
                break;
            case "habitat":
                stagePositions.Add(new Vector3(62.35349f, 0.6543378f, 143.4429f), Quaternion.Euler(0f, 292.0843f, 0f));
                stagePositions.Add(new Vector3(-13.10078f, 21.30807f, -104.7179f), Quaternion.Euler(0f, 205.8312f, 0f));
                break;
            case "habitatfall":
                stagePositions.Add(new Vector3(62.35349f, 0.6543378f, 143.4429f), Quaternion.Euler(0f, 292.0843f, 0f));
                stagePositions.Add(new Vector3(-13.10078f, 21.30807f, -104.7179f), Quaternion.Euler(0f, 205.8312f, 0f));
                break;
            // Stage 4
            case "dampcavesimple":
                stagePositions.Add(new Vector3(74.77532f, -87.4948f, -196.4726f), Quaternion.Euler(0f, 200.3068f, 0f));
                stagePositions.Add(new Vector3(-156.3687f, -122.5679f, -91.23058f), Quaternion.Euler(0f, 111.9192f, 0f));
                break;
            case "shipgraveyard":
                stagePositions.Add(new Vector3(-93.99381f, 11.12058f, -31.31874f), Quaternion.Euler(0f, 218.6378f, 0f));
                stagePositions.Add(new Vector3(186.8673f, 77.27081f, 26.32287f), Quaternion.Euler(0f, 346.0909f, 0f));
                break;
            case "rootjungle":
                stagePositions.Add(new Vector3(77.77145f, -60.95433f, -8.285771f), Quaternion.Euler(0f, 359.0302f, 0f));
                stagePositions.Add(new Vector3(-202.9731f, 92.59643f, -166.5592f), Quaternion.Euler(0f, 71.41415f, 0f));
                break;
            case "repurposedcrater":
                stagePositions.Add(new Vector3(-253.8853f, 121.0383f, 11.31156f), Quaternion.Euler(0f, 68.99377f, 0f));
                stagePositions.Add(new Vector3(9.018836f, 53.40439f, -134.2828f), Quaternion.Euler(0f, 305.9838f, 0f));
                break;
            case "conduitcanyon":
                stagePositions.Add(new Vector3(-420.648f, 43.71975f, -332.4908f), Quaternion.Euler(0f, 61.96296f, 0f));
                stagePositions.Add(new Vector3(-131.2769f, 32.62687f, -154.4974f), Quaternion.Euler(0f, 173.366f, 0f));
                stagePositions.Add(new Vector3(135.7131f, 40.00665f, 304.5394f), Quaternion.Euler(0f, 127.5887f, 0f));
                stagePositions.Add(new Vector3(120.3498f, 113.6439f, 605.4644f), Quaternion.Euler(0f, 181.6343f, 0f));
                break;
            // Stage 5
            case "skymeadow":
                stagePositions.Add(new Vector3(-122.741f, -87.85542f, -117.1602f), Quaternion.Euler(0f, 79.03946f, 0f));
                stagePositions.Add(new Vector3(-197.5687f, 26.55142f, 30.68633f), Quaternion.Euler(0f, 135.1837f, 0f));
                break;
            case "helminthroost":
                stagePositions.Add(new Vector3(-569.6815f, -148.7018f, 337.3586f), Quaternion.Euler(0f, 269.8808f, 0f));
                stagePositions.Add(new Vector3(-397.3075f, 118.8055f, -55.87037f), Quaternion.Euler(0f, 123.3015f, 0f));
                break;
            // Stage - SPECIAL
            case "goldshores":
                stagePositions.Add(new Vector3(49.76122f, -7.326298f, -37.93284f), Quaternion.Euler(0f, 180.3956f, 0f));
                break;
            case "moon":
                stagePositions.Add(new Vector3(274.709f, 329.6422f, 110.5199f), Quaternion.Euler(0f, 145.1531f, 0f));
                break;
            case "moon2":
                stagePositions.Add(new Vector3(705.5821f, -157.1221f, 858.8436f), Quaternion.Euler(0f, 79.45797f, 0f));
                break;
            case "meridian":
                stagePositions.Add(new Vector3(101.4198f, 102.1784f, -54.02138f), Quaternion.Euler(0f, 209.4111f, 0f));
                break;
            case "solutionalhaunt":
                stagePositions.Add(new Vector3(96.6469f, -555.6259f, -107.9996f), Quaternion.Euler(0f, 245.2375f, 0f));
                break;
            // Custom Stages
            // Fogbound Lagoon - Jace
            case "FBLScene":
                stagePositions.Add(new Vector3(300.2629f, 229.4142f, -126.4303f), Quaternion.Euler(0f, 125.6396f, 0f));
                stagePositions.Add(new Vector3(169.5165f, 255.284f, 410.8152f), Quaternion.Euler(0f, 20.25785f, 0f));   
                break;
            // wormsworms Stages
            case "observatory_wormsworms":
                stagePositions.Add(new Vector3(176.1036f, -2.617247f, -34.78374f), Quaternion.Euler(0f, 210.1473f, 0f));
                stagePositions.Add(new Vector3(-141.1253f, 77.90317f, 221.5992f), Quaternion.Euler(0f, 13.25202f, 0f));
                break;
            case "foggyswampdownpour":
                stagePositions.Add(new Vector3(-749.5313f, 91.3914f, -2025.714f), Quaternion.Euler(0f, 227.2804f, 0f));
                stagePositions.Add(new Vector3(-1018.735f, 76.65155f, -2313.54f), Quaternion.Euler(0f, 334.4469f, 0f));
                break;
            case "broadcastperch_wormsworms":
                stagePositions.Add(new Vector3(50.9431f, 280.1794f, 136.6628f), Quaternion.Euler(0f, 176.2385f, 0f));
                stagePositions.Add(new Vector3(-126.8886f, 270.8356f, 53.03748f), Quaternion.Euler(0f, 109.5885f, 0f));
                stagePositions.Add(new Vector3(12.31855f, 305.7722f, -138.1022f), Quaternion.Euler(0f, 285.201f, 0f));
                break;
            case "tropics_wormsworms":
                stagePositions.Add(new Vector3(-199.3958f, -31.72103f, -170.7593f), Quaternion.Euler(0f, 338.8611f, 0f));
                stagePositions.Add(new Vector3(127.7805f, 38.15013f, 158.8848f), Quaternion.Euler(0f, 121.6468f, 0f));
                break;
            case "tropicsnight_wormsworms":
                stagePositions.Add(new Vector3(-199.3958f, -31.72103f, -170.7593f), Quaternion.Euler(0f, 338.8611f, 0f));
                stagePositions.Add(new Vector3(127.7805f, 38.15013f, 158.8848f), Quaternion.Euler(0f, 121.6468f, 0f));
                break;
            case "hollowsummit_wormsworms":
                stagePositions.Add(new Vector3(-37.58895f, 106.0261f, 108.1745f), Quaternion.Euler(0f, 24.11862f, 0f));
                stagePositions.Add(new Vector3(160.7567f, 55.26473f, -70.89723f), Quaternion.Euler(0f, 87.57731f, 0f));
                break;
            case "hollowsummitnight_wormsworms":
                stagePositions.Add(new Vector3(-37.58895f, 106.0261f, 108.1745f), Quaternion.Euler(0f, 24.11862f, 0f));
                stagePositions.Add(new Vector3(160.7567f, 55.26473f, -70.89723f), Quaternion.Euler(0f, 87.57731f, 0f));
                break;
            // Bobomb Battlefield - viliger
            case "sm64_bbf_SM64_BBF":
                stagePositions.Add(new Vector3(162.9347f, 38.44512f, 74.83717f), Quaternion.Euler(0f, 189.5465f, 0f));
                stagePositions.Add(new Vector3(-97.6097f, 15.58791f, 97.34737f), Quaternion.Euler(0f, 10.41307f, 0f));
                break;
            // Catacombs - viliger
            case "catacombs_DS1_Catacombs":
                stagePositions.Add(new Vector3(-49.93897f, 218.098f, -439.0562f), Quaternion.Euler(0f, 125.6632f, 0f));
                stagePositions.Add(new Vector3(-29.51958f, 190.1485f, -174.524f), Quaternion.Euler(0f, 135.824f, 0f));
                break;
            // SnowtimeStages
            case "snowtime_bloodgulch":
                stagePositions.Add(new Vector3(-380.7556f, 7.749712f, 374.3361f), Quaternion.Euler(0f, 53.47242f, 0f));
                stagePositions.Add(new Vector3(-59.86384f, 47.40347f, 365.2287f), Quaternion.Euler(0f, 288.4397f, 0f));
                break;
            case "snowtime_deathisland":
                stagePositions.Add(new Vector3(101.2409f, 49.95498f, -98.19534f), Quaternion.Euler(0f, 166.7226f, 0f));
                stagePositions.Add(new Vector3(119.7398f, 36.45596f, -237.7042f), Quaternion.Euler(0f, 306.9397f, 0f));
                break;
            case "snowtime_deltahalo":
                stagePositions.Add(new Vector3(35.93007f, 82.32765f, 981.3515f), Quaternion.Euler(0f, 175.9509f, 0f));
                stagePositions.Add(new Vector3(31.13048f, 65.11033f, 641.2076f), Quaternion.Euler(0f, 15.0037f, 0f));
                break;
            case "snowtime_gephyrophobia":
                stagePositions.Add(new Vector3(-93.77584f, -17.08484f, 51.58313f), Quaternion.Euler(0f, 359.9742f, 0f));
                stagePositions.Add(new Vector3(-93.86058f, -17.27335f, 453.9898f), Quaternion.Euler(0f, 179.6247f, 0f));
                stagePositions.Add(new Vector3(-93.63819f, -59.47458f, 253.0157f), Quaternion.Euler(0f, 359.8916f, 0f));
                break;
            case "snowtime_gmconstruct":
                stagePositions.Add(new Vector3(125.6074f, -24.182f, 41.94416f), Quaternion.Euler(0f, 224.6205f, 0f));
                stagePositions.Add(new Vector3(191.7613f, -4.52605f, -229.1491f), Quaternion.Euler(0f, 134.3182f, 0f));
                stagePositions.Add(new Vector3(-35.97609f, -0.746001f, 50.99677f), Quaternion.Euler(0f, 134.9052f, 0f));
                stagePositions.Add(new Vector3(-36.71188f, -6.793999f, 58.53889f), Quaternion.Euler(0f, 277.8377f, 0f));
                break;
            case "snowtime_gmflatgrass":
                stagePositions.Add(new Vector3(10.22947f, 15.37018f, 9.359962f), Quaternion.Euler(0f, 256.2039f, 0f));
                break;
            case "snowtime_halo":
                stagePositions.Add(new Vector3(-30.71955f, 229.6823f, -144.8316f), Quaternion.Euler(0f, 209.9076f, 0f));
                stagePositions.Add(new Vector3(-208.1239f, 202.1529f, 170.1005f), Quaternion.Euler(0f, 134.7629f, 0f));
                break;
            case "snowtime_halo2":
                stagePositions.Add(new Vector3(1867.69f, 238.3529f, -642.8783f), Quaternion.Euler(0f, 44.7936f, 0f));
                stagePositions.Add(new Vector3(1293.771f, 263.4328f, -554.2413f), Quaternion.Euler(0f, 107.449f, 0f));
                stagePositions.Add(new Vector3(1342.334f, 164.51f, -135.1692f), Quaternion.Euler(0f, 125.0683f, 0f));
                stagePositions.Add(new Vector3(1840.748f, 213.0739f, -200.0254f), Quaternion.Euler(0f, 337.9286f, 0f));
                break;
            case "snowtime_highcharity":
                stagePositions.Add(new Vector3(60.14213f, 11.577f, -483.8502f), Quaternion.Euler(0f, 241.12f, 0f));
                stagePositions.Add(new Vector3(-52.33072f, 11.577f, -487.8752f), Quaternion.Euler(0f, 129.98f, 0f));
                stagePositions.Add(new Vector3(-61.07679f, 11.57699f, -546.1075f), Quaternion.Euler(0f, 59.45599f, 0f));
                stagePositions.Add(new Vector3(57.52615f, 11.577f, -544.13f), Quaternion.Euler(0f, 297.445f, 0f));
                stagePositions.Add(new Vector3(21.75992f, 2.126976f, -484.5297f), Quaternion.Euler(0f, 35.51595f, 0f));
                stagePositions.Add(new Vector3(37.98527f, 2.126978f, -514.8828f), Quaternion.Euler(0f, 89.67403f, 0f));
                stagePositions.Add(new Vector3(22.01921f, 2.126978f, -545.0482f), Quaternion.Euler(0f, 145.1344f, 0f));
                stagePositions.Add(new Vector3(-21.56447f, 2.12697f, -544.3843f), Quaternion.Euler(0f, 217.2564f, 0f));
                stagePositions.Add(new Vector3(-37.30863f, 2.126962f, -514.6474f), Quaternion.Euler(0f, 271.6194f, 0f));
                stagePositions.Add(new Vector3(-21.56099f, 2.12697f, -485.05f), Quaternion.Euler(0f, 324.465f, 0f));
                stagePositions.Add(new Vector3(-17.53811f, 9.018085f, -621.3613f), Quaternion.Euler(0f, 123.2604f, 0f));
                stagePositions.Add(new Vector3(17.22844f, 9.0188f, -408.7164f), Quaternion.Euler(0f, 313.6848f, 0f));
                break;
            case "snowtime_icefields":
                stagePositions.Add(new Vector3(272.5726f, 7.359958f, -303.162f), Quaternion.Euler(0f, 357.3307f, 0f));
                stagePositions.Add(new Vector3(-86.97459f, 7.360003f, 77.48335f), Quaternion.Euler(0f, 178.8512f, 0f));
                break;
            case "snowtime_mvmmannhattan":
                stagePositions.Add(new Vector3(97.37387f, -3.909989f, 39.39054f), Quaternion.Euler(0f, 243.5768f, 0f));
                stagePositions.Add(new Vector3(11.37051f, -10.19141f, 163.9846f), Quaternion.Euler(0f, 138.7431f, 0f));
                stagePositions.Add(new Vector3(-30.7068f, 1.629176f, -88.67094f), Quaternion.Euler(0f, 44.10945f, 0f));
                break;
            case "snowtime_newmombasabridge":
                stagePositions.Add(new Vector3(-856.0861f, 1.803394f, -65.45497f), Quaternion.Euler(0f, 91.96432f, 0f));
                stagePositions.Add(new Vector3(79.94922f, 48.21177f, 8.020412f), Quaternion.Euler(0f, 255.742f, 0f));
                stagePositions.Add(new Vector3(836.3431f, 5.02102f, 28.03459f), Quaternion.Euler(0f, 0.5525074f, 0f));
                break;
            case "snowtime_odstmombasa":
                stagePositions.Add(new Vector3(-600.8646f, 19.12913f, 4.601078f), Quaternion.Euler(0f, 128.6979f, 0f));
                stagePositions.Add(new Vector3(-41.66861f, 17.09908f, -53.34187f), Quaternion.Euler(0f, 269.7134f, 0f));
                stagePositions.Add(new Vector3(-413.2506f, 12.5159f, 210.6578f), Quaternion.Euler(0f, 135.8528f, 0f));
                break;
            case "snowtime_plrhightower":
                stagePositions.Add(new Vector3(188.9554f, 46.25977f, -153.9811f), Quaternion.Euler(0f, 304.0394f, 0f));
                stagePositions.Add(new Vector3(71.34775f, 31.16273f, -141.1281f), Quaternion.Euler(0f, 91.36182f, 0f));
                stagePositions.Add(new Vector3(23.99743f, 23.75547f, -210.7883f), Quaternion.Euler(0f, 12.54428f, 0f));
                stagePositions.Add(new Vector3(26.78335f, 24.51611f, -58.37008f), Quaternion.Euler(0f, 194.0668f, 0f));
                break;
            case "snowtime_sandtrap":
                stagePositions.Add(new Vector3(37.73601f, -77.13338f, 176.5529f), Quaternion.Euler(0f, 350.9677f, 0f));
                stagePositions.Add(new Vector3(-0.09483957f, -78.48047f, 20.26742f), Quaternion.Euler(0f, 178.6303f, 0f));
                stagePositions.Add(new Vector3(-133.8743f, -71.43472f, 2.363332f), Quaternion.Euler(0f, 276.9908f, 0f));
                break;
            case "snowtime_sidewinder":
                stagePositions.Add(new Vector3(-7.530613f, -7.065588f, -197.7645f), Quaternion.Euler(0f, 2.06729f, 0f));
                stagePositions.Add(new Vector3(116.0248f, -3.413208f, 46.1681f), Quaternion.Euler(0f, 83.94437f, 0f));
                stagePositions.Add(new Vector3(117.4637f, -7.276778f, 112.5169f), Quaternion.Euler(0f, 204.6324f, 0f));
                stagePositions.Add(new Vector3(-102.5031f, -8.326768f, 126.306f), Quaternion.Euler(0f, 181.4125f, 0f));
                break;
            default:
                Log.Debug("no custom pos !!! too bad ,..");
                return;
        }
        #endregion
        
        //make a sister array to friendly turret list that has the count of each type of turret .,,. 
        int[] turretCounts = new int[SnowtimeToyboxMod.friendlyTurretList.Count];
        var characterMaster = CharacterMaster.readOnlyInstancesList;
        foreach (CharacterMaster minion in characterMaster)
        {
            if(minion.minionOwnership == null) continue;
            if (!minion.name.Contains("FriendlyTurret")) continue;
            
            string cleanedName = minion.name.Replace("(Clone)", "");
            for (int i = 0; i < SnowtimeToyboxMod.friendlyTurretList.Count; i++)
            {
                //Log.Debug($"comaprings {cleanedName} tp {friendlyTurretListNames[i]} !!!");;
                if (cleanedName != SnowtimeToyboxMod.friendlyTurretList[i].master.name) continue;
                    
                //Log.Debug($"found turret {cleanedName} !!! adding to list of current turrets .,,. ");
                turretCounts[i]++;
            }
        }
        
        //add turrets that arent maxed out to the list of available turrets to spawn ,..
        List<GameObject> availableTurrets = [];
        for (int i = 0; i < SnowtimeToyboxMod.friendlyTurretList.Count; i++)
        {
            // skip if max amount of turrets reached ,.,. .,...
            //Log.Debug("turret[i] count = " + turretCounts[i]);
            if (turretCounts[i] == PlayerCharacterMasterController.playerCount) continue;
            
            //Log.Debug($"adding {friendlyTurretList[i]} to available turret list !!!");
            availableTurrets.Add(SnowtimeToyboxMod.friendlyTurretList[i].broken);
        }
        if (availableTurrets.Count == 0) return;
        
        //Log.Debug("Friendly Turret Count: " + friendlyTurretList.Count);
        GameObject turret = availableTurrets[Run.instance.runRNG.RangeInt(0, availableTurrets.Count)];
        KeyValuePair<Vector3, Quaternion> stagePos = stagePositions.ElementAt(Run.instance.runRNG.RangeInt(0, stagePositions.Count));
        GameObject term = Object.Instantiate(turret, stagePos.Key, stagePos.Value);
        //Log.Debug($"turret name = {turret.name} !!!!");

        NetworkServer.Spawn(term);
    }
    
    private static void CharacterMaster_OnStartGlobal(CharacterMaster obj)
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
}