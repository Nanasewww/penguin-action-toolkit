using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    public class AttackWithHitboxMod : StateModifier
    {
        [SerializeReference, SubclassPicker]     
        public EffectFactory _factory;
        public List<Effect> _effects = new List<Effect>();
        [SerializeField] protected Hitbox _owningHitbox;
        //[SerializeField] protected EffectFactory_PAT.PATDmg _hitInfo;

        protected List<Effect> _dmgEffects;

        #region Getter

        public Hitbox owningHitbox
        {
            get { return _owningHitbox; }
            set { _owningHitbox = value; }
        }

        /*public EffectFactory_PAT.PATDmg hitInfo
        {
            get { return _hitInfo; }
            set { _hitInfo = value; }
        }*/

        #endregion

        public override void OnEnter(Character controller)
        {
            base.OnEnter(controller);

            if (_owningHitbox == null)
            {
                _owningHitbox = controller.GetComponentInChildren<Hitbox>();
            }
        }
        
        void UpdateDmgEffects()
        {
            if(_factory != null)_dmgEffects = _factory.GenerateEffect(characterController);
            else _dmgEffects = new List<Effect>();
            _dmgEffects.AddRange(_effects);
        }


        public override void BeginEvent()
        {
            base.BeginEvent();
            if (_owningHitbox == null)
            {
                Debug.LogWarning("No Hitbox to be controlled");
                return;
            }

            UpdateDmgEffects();
            _owningHitbox.Activate();
            _owningHitbox.UpdateEffectList(_dmgEffects);
        }
        
        public override void EndEvent()
        {
            base.EndEvent();
            if (_owningHitbox == null) { return; }

            _owningHitbox.DeActivate();
        }
    }
}
