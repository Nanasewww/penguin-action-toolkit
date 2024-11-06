using System;
using UnityEngine;

namespace PAT
{
    public class Posture: Attribute
    {
        public enum PostureStat
        {
            Active,
            Down
        }

        [SerializeField] protected PostureStat _currentStats;
        [SerializeField] protected ActionState _downState;
        [SerializeField] protected ActionState _recoverState;

        public PostureStat currentStats { get { return _currentStats; } }

        protected Effect breakEffect;

        private Character _character;
        private void Reset()
        {
            _resourceTag = GamePlayTag.Posture;
            _maxAmount = 10;
            _startRecoverSpeed = 2;
        }

        public override void Awake()
        {
            base.Awake();
            _currentStats = PostureStat.Active;
            
            EffectModValue mod = ScriptableObject.CreateInstance<EffectModValue>();
            mod.resourceTag = GamePlayTag.Resistance;
            mod.modTarget = EffectModValue.ModTarget.Amount;
            mod.modPattern = EffectModValue.ModPattern.Override;
            mod.value = 0;
            
            breakEffect = Effect.NewEffect(null, null, null);
            breakEffect.components.Add(mod);
            
            if(owner is Character) {_character = (Character)owner;}
            else {Debug.LogWarning("Posture must work with Character");}
        }

        public override void Update()
        {
            if(!_character) return;
            
            switch (_currentStats)
            {
                case PostureStat.Active:
                    break;
                case PostureStat.Down:
                    base.Update();
                    break;
            }
            
            if (_currentStats == PostureStat.Down && _baseAmount >= _maxAmount)
            {
                _currentStats = PostureStat.Active;
                owner.RemoveEffect(breakEffect);
                if (_recoverState) _character.ChangeState(_recoverState);
            }
        }

        public override void ModBaseAttribute(float amount)
        {
            if(_currentStats == PostureStat.Down) return;
            
            base.ModBaseAttribute(amount);
            
            if(!_character) return;
            
            if (_currentStats == PostureStat.Active && _baseAmount <= 0)
            {
                _currentStats = PostureStat.Down;
                owner.AddEffect(breakEffect);
                if (_downState) _character.ChangeState(_downState);
            }
        }
    }
}