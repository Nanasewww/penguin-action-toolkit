using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace PAT
{
    public class PATComponent: MonoBehaviour
    {
        public static List<PATComponent> allInstance;
        
        public int team = 0;
        [Tooltip("This attribute determine if this component die or not. Death happens when the base value reach min value")]
        [SerializeField] protected Attribute _healthAttribute;
        [SerializeField] protected List<Effect> _effects;
        [Tooltip("The reference to all attributes, helpful to find the object")]
        [SerializeField] protected List<Attribute> _attributes;
        [SerializeField] protected List<AttributeLaw> _attributeLawsSource;
        [SerializeField] protected List<AttributeLaw> _attributeLawsTarget;
        
        
        protected TagContainer _tagContainer;

        #region Events

        public static event Action<PATComponent> OnAnySpawn;
        public event Action<Effect> onEffectAdd;
        public event Action<Effect> onEffectRemove;
        public event Action<EffectPackage> onEffectPackageSent;
        public event Action<EffectPackage> onEffectPackageReceived;
        public static event Action<EffectPackage> onAnyEffectPackageEvent;
        #endregion

        #region Getter Setter
        public Attribute healthAttribute {get{return _healthAttribute;}}
        public TagContainer tagContainer { get { return _tagContainer; } set { _tagContainer = value; } }
        public List<Effect> effects { get { return _effects; } }
        public List<Attribute> attributes { get { return _attributes; } }
        public List<AttributeLaw> attributeLawsSource { get { return _attributeLawsSource; } }
        public List<AttributeLaw> attributeLawsTarget { get { return _attributeLawsTarget; } }
        #endregion
        
        protected virtual void Awake()
        {
            if (allInstance == null) allInstance = new List<PATComponent>();
            allInstance.Add(this);
            
            if (!_tagContainer) { tagContainer = GetComponent<TagContainer>(); }
            if (!_tagContainer) { tagContainer = gameObject.AddComponent<TagContainer>(); }
            
            if(_healthAttribute == null){ Debug.LogWarning(gameObject.name + " has no health attribute assigned, would not die");}
            else{ _healthAttribute.OnBaseValueReachMin += ProcessDeath;}
        }

        protected virtual void Start()
        {
            OnAnySpawn?.Invoke(this);
        }

        public virtual void OnDestroy()
        {
            allInstance.Remove(this);
        }

        public virtual void OnDisable()
        {
            allInstance.Remove(this);
        }

        public void OnEnable()
        {
            if(!allInstance.Contains(this))allInstance.Add(this);
        }
        
        protected virtual void Update()
        {
            //===Process Effect===//
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                Effect effect = effects[i];
                effect.OnTick();
                if(effect.removeFlag) ActualRemoveEffect(effect);
            }
        }
        
        public void AddEffect(Effect effect) { if(!enabled) return;_effects.Add(effect); effect.OnApply(this); onEffectAdd?.Invoke(effect); }
        
        /// <summary>
        /// Don't use this in most cases
        /// The proper way to remove effect is to call Effect.Removeself()
        /// </summary>
        /// <param name="effect"></param>
        public void RemoveEffect(Effect effect)
        {
            if(effects.Contains(effect))effect.MarkAsRemove();
        }

        void ActualRemoveEffect(Effect effect)
        {
            effect.OnRemove(); onEffectRemove?.Invoke(effect); _effects.Remove(effect); 
        }
        
        public Attribute GetAttributeByTag(GamePlayTag t)
        {
            if(t == GamePlayTag.None) return null;
            
            foreach (Attribute r in attributes)
            {
                if(r.resourceTag != t) continue;

                return r;
            }
            return null;
        }

        /// <summary>
        /// Use this to do something like "Attack" or "Heal" toward other character
        /// This method will create a clone of effect in list and pass to receiver
        /// That could avoid mutiple receiver sharing the same effect
        /// And many other thing can listen event invoked
        /// </summary>
        /// <param name="target">Must be valid and enabled</param>
        /// <param name="toSend"></param>
        public void SendEffectPackage(PATComponent target, List<Effect> toSend)
        {
            if(target == null) return;
            if(!target.enabled) return;
            
            List<Effect> duplicate = new List<Effect>();
            foreach (var effect in toSend)
            {
                Effect newEffect = Instantiate(effect);
                newEffect.source = this;
                newEffect.target = target;
                duplicate.Add(newEffect);
            }
            
            EffectPackage package = new EffectPackage { target = target, source = this, effects = duplicate };
            target.ReceiveEffectPackage(package);
        }

        private void ReceiveEffectPackage(EffectPackage package)
        {
            foreach (var effect in package.effects)
            {
                AddEffect(effect);
            }
            
            package.source.onEffectPackageSent?.Invoke(package);
            this.onEffectPackageReceived?.Invoke(package);
            onAnyEffectPackageEvent?.Invoke(package);
        }
        
        public struct EffectPackage
        {
            public PATComponent target;
            public PATComponent source;
            public List<Effect> effects;
        }
        
        public virtual void ProcessDeath()
        {
            this.enabled = false;
            allInstance.Remove(this);
        }

    }
}