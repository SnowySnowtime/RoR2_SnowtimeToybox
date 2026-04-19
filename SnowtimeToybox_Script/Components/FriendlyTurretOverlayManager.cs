using System;
using RoR2;
using RoR2.EntityLogic;
using SnowtimeToybox;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using On.EntityStates.AffixVoid;
using SnowtimeToybox.FriendlyTurrets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace SnowtimeToybox.FriendlyTurretChecks
{
    public class FriendlyTurretOverlayManager : MonoBehaviour
    {
        private TemporaryOverlayInstance temporaryOverlay;
        public List<TemporaryOverlayInstance> Overlay = [];
        public RoR2.CharacterBody Body;
        
        public void Start()
        {
            
        }

        public bool hasOverlay(string matName)
        {
            foreach (TemporaryOverlayInstance overlayInstance in Overlay)
            {
                if (matName == overlayInstance.materialInstance.name)
                {
                    return true;
                }
            }

            return false;
        }

        public void FixedUpdate()
        {
            foreach (TemporaryOverlayInstance overlayInstance in Overlay.ToList())
            {
                switch (overlayInstance.originalMaterial.name)
                {
                    case "matBreadFortune":
                        if (!Body.HasBuff(BreadFriendlyTurret.BreadTurretBuffFortune))
                        {
                            Overlay.Remove(overlayInstance);
                            overlayInstance.Destroy();
                        }
                        break;

                    case "matBreadGraceOverlay":
                        if (!Body.HasBuff(BreadFriendlyTurret.BreadTurretBuffNearbyAllies))
                        {
                            Overlay.Remove(overlayInstance);
                            overlayInstance.Destroy();
                        }
                        break;

                    case "borboturretdebuffoverlay":
                        if (!Body.HasBuff(BorboFriendlyTurret.BorboTurretDebuff))
                        {
                            Overlay.Remove(overlayInstance);
                            overlayInstance.Destroy();
                        }
                        break;
                    
                    case "acanthidebuffoverlay":
                        if (!Body.HasBuff(AcanthiFriendlyTurret.AcanthiTurretDebuff))
                        {
                            Overlay.Remove(overlayInstance);
                            overlayInstance.Destroy();
                        }
                        break;
                }
            }
            if (Overlay.Count == 0)
            {
                Destroy(this);
                return;
            }
        }
    }
}