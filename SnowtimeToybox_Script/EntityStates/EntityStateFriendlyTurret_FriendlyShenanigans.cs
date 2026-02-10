using EntityStates;
using RoR2;
using UnityEngine;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class Shenanigans : BaseSkillState
    {
        public static float hopPower = 7f;

        public static string playtimeSoundString = "Play_Item_FriendUnit_VO_Whistle";

        public static string stopSoundString = "Play_Item_FriendUnit_VO_Happy";

        public static float baseDuration = 2f;

        public static float timeBetweenHops = 0.2f;

        private float hopTimer;

        private uint soundID;

        public override void OnEnter()
        {
            base.OnEnter();
            hopTimer = 0f;
            soundID = Util.PlaySound(playtimeSoundString, outer.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (hopTimer > 0f)
            {
                hopTimer -= Time.deltaTime;
            }
            if (base.characterMotor.isGrounded && hopTimer <= 0f)
            {
                SmallHop(base.characterMotor, hopPower);
                hopTimer += timeBetweenHops;
            }
            if (base.isAuthority && !IsKeyDownAuthority())
            {
                AkSoundEngine.StopPlayingID(soundID);
                Util.PlaySound(stopSoundString, outer.gameObject);
                outer.SetNextStateToMain();
            }
        }
    }
}