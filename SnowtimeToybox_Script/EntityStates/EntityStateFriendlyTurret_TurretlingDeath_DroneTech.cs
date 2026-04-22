using RoR2;
using SnowtimeToybox;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class DTTurretlingDeath : GenericCharacterDeath
    {
        [SerializeField]
        public float deathDuration = 1f;
        private GameObject OwnerObject;

        public static float deathDelay = 0f;

        public static GameObject deathEffectPrefab = TurretlingDeath.deathfx;

        private bool hasDied;

        public override void OnEnter()
        {
            base.OnEnter();
            if ((bool)base.characterBody.master?.minionOwnership?.ownerMaster)
            {
                CharacterBody body = base.characterBody.master.minionOwnership.ownerMaster.GetBody();
                OwnerObject = (body ? body.gameObject : null);
            }
            EffectManager.SimpleImpactEffect(deathEffectPrefab, base.characterBody.corePosition, Vector3.up, transmit: false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && base.fixedAge > deathDuration)
            {
                Explode();
            }
            DroneRepairMaster component2;
            if (!OwnerObject || !OwnerObject.TryGetComponent<DroneTechRepairQueue>(out var _))
            {
                EntityState.Destroy(base.gameObject);
                if ((bool)base.characterBody.master)
                {
                    EntityState.Destroy(base.characterBody.master.gameObject);
                }
            }
            else if ((bool)base.characterBody && (bool)base.characterBody.master && base.characterBody.master.TryGetComponent<DroneRepairMaster>(out component2))
            {
                ProcessDeath(component2, base.characterBody.master);
            }
        }

        public void Explode()
        {
            EntityState.Destroy(base.gameObject);
            if ((!OwnerObject || !OwnerObject.TryGetComponent<DroneTechRepairQueue>(out var _)) && (bool)base.characterBody.master)
            {
                EntityState.Destroy(base.characterBody.master.gameObject);
            }
        }

        private void ProcessDeath(DroneRepairMaster repairMaster, CharacterMaster droneMaster)
        {
            if (!repairMaster.DoNotRepair)
            {
                repairMaster.RespawnBrokenBody();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}