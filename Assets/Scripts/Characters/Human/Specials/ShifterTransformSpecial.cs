using System.Collections;
using UnityEngine;

namespace Characters
{
    class ShifterTransformSpecial : ResetSpecial
    {
        public float LiveTime = 60f;
        protected string _shifter;
        protected override float ActiveTime => 0.8f;

        public ShifterTransformSpecial(BaseCharacter owner, string shifter): base(owner, shifter, 60f)
        {
            _shifter = shifter;
        }

        protected override void Activate()
        {
            ((Human)_owner).EmoteAnimation(HumanAnimations.SpecialShifter);
        }

        protected override void Deactivate()
        {
            var human = (Human)_owner;
            if (!human.Dead)
                human.TransformShifter(_shifter, LiveTime);
        }

        public override void SetInitialCooldown()
        {
            SetCooldownLeft(Cooldown);
        }
    }
}