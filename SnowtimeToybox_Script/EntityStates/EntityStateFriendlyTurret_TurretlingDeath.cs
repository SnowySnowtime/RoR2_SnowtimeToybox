using RoR2;
using SnowtimeToybox;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingDeath : GenericCharacterDeath
    {
        public static float deathDelay = 0f;

        public static GameObject deathfx = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_deatheffect.prefab");
        public static GameObject deathEffectPrefab = deathfx;

        private bool hasDied;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > deathDelay && !hasDied)
            {
                hasDied = true;
                DestroyModel();
                EffectManager.SimpleImpactEffect(deathEffectPrefab, base.characterBody.corePosition, Vector3.up, transmit: false);
                if (NetworkServer.active)
                {
                    DestroyBodyAsapServer();
                }
            }
        }

        public override void OnExit()
        {
            DestroyModel();
            base.OnExit();
        }
    }
}