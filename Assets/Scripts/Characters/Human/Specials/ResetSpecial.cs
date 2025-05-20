using ExitGames.Client.Photon.StructWrapping;
using Settings;
using UnityEngine;

namespace Characters
{
    /// <summary>
    /// A useable that is protected from abuse by endless resets and performs logic over extended update frames.
    /// </summary>
    abstract class ResetSpecial : ExtendedUseable
    {
        public string SpecialName;

        public ResetSpecial(BaseCharacter owner, string specialName, float coolDown) : base(owner)
        {
            SpecialName = specialName;
            Cooldown = coolDown;

            if (SpecialName != null && _owner.GetComponent<Veteran>().AbilityCooldowns.TryGetValue(SpecialName, out float _cooldown)) {
                if (_cooldown > 0f) {
                    SetCooldownLeft(_cooldown);
                } else if (_cooldown <= 0f) {
                    RemoveFromDictionary();
                    SetInitialCooldown();
                }
            } else {
                SetInitialCooldown();
            }
        }

        protected override void OnUse()
        {
            base.OnUse();
            AddToDictionary();
        }
        protected void AddToDictionary()
        {
            if (_owner.GetComponent<Veteran>().AbilityCooldowns.ContainsKey(SpecialName))
               return;

            _owner.GetComponent<Veteran>().AbilityCooldowns.Add(SpecialName, GetCooldownLeft());
        }
        
        protected void RemoveFromDictionary()
        {
            if (!_owner.GetComponent<Veteran>().AbilityCooldowns.ContainsKey(SpecialName))
                return;
            
            _owner.GetComponent<Veteran>().AbilityCooldowns.Remove(SpecialName);
        }

        public virtual void SetInitialCooldown()
        {
        }
    }
}
