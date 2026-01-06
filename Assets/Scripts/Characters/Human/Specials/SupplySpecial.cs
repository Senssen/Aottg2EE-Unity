using System.Collections;
using ApplicationManagers;
using GameManagers;
using log4net.Util;
using Photon.Pun;
using Spawnables;
using UnityEngine;
using Utility;

namespace Characters
{
    class SupplySpecial : BaseEmoteSpecial
    {
        protected override float ActiveTime => 0.5f;

        public SupplySpecial(BaseCharacter owner): base(owner)
        {
            UsesLeft = -1;

            Cooldown = 1500f;
        }

        protected override void Activate()
        {
            _human.EmoteAnimation(HumanAnimations.EmoteWave);
        }

        protected override void Deactivate()
        {
            var rotation = _human.Cache.Transform.rotation.eulerAngles;
            Vector3 position = _human.Cache.Transform.position + (_human.Cache.Transform.forward * 4f) + new Vector3(0, 1.5f, 0);
            PhotonNetwork.Instantiate(ResourcePaths.Logistician + "/SpinningSupplyGasPrefabHalf", position, _human.Cache.Transform.rotation, 0);
            PhotonNetwork.Instantiate(ResourcePaths.Logistician + "/SpinningSupplyBladePrefab", position, _human.Cache.Transform.rotation, 0);
            UsesLeft = -1;
        }

        public override void Reset()
        {
            base.Reset();
            SetCooldownLeft(0f);
        }
    }
}
