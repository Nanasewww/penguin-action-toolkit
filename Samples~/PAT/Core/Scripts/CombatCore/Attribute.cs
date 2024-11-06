using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    public class Attribute: MonoBehaviour
    {
        public enum RecoverTimeMode
        {
            Update = 0 ,
            FixedUpdate = 1,
            RealTime = 2
        }

        public PATComponent owner;
        [SerializeField] protected GamePlayTag _resourceTag;
        [SerializeField] protected float _maxAmount = 999;
        [SerializeField] protected float _minAmount = 0;
        [SerializeField] protected float _startAmount;
        [FormerlySerializedAs("_recoverSpeed")] [SerializeField] protected float _startRecoverSpeed;
        [SerializeField] protected RecoverTimeMode recoverMode;

        protected float _baseAmount;
        protected float _baseRecoverSpeed;

        protected List<EffectModValue> amountEffects = new ();
        protected List<EffectModValue> recoverEffects = new ();

        public event Action<EffectModValue> OnModApplied;
        public event Action OnBaseValueReachMax;
        public event Action OnBaseValueReachMin;
        
        public float currentAmount { get { return ProcessCurrentData(_baseAmount, amountEffects); } }
        public GamePlayTag resourceTag { get { return _resourceTag; } set{ _resourceTag = value;} }
        public float maxAmount { get { return _maxAmount; } set{ _maxAmount = value;}}

        protected List<AttributeLaw> globalLaw;
        private void Reset()
        {
            _resourceTag = GamePlayTag.None;
        }
        
        public virtual void Awake()
        {
            if (!owner) owner = GetComponentInParent<PATComponent>();
            if (owner && !owner.attributes.Contains(this)) owner.attributes.Add(this);

            _baseAmount = _startAmount;
            _baseRecoverSpeed = _startRecoverSpeed;

            globalLaw = AttributeLaw.GetGlobalLaw(resourceTag);
        }

        #region EffectLoadTools
        public void AddEffectModValue(EffectModValue mod)
        {
            mod = ApplyLawToMod(mod);

            if (mod.modPattern == EffectModValue.ModPattern.ToBaseValue)
            {
                EffectOneTimeMod(mod);
                OnModApplied?.Invoke(mod);
                return;
            }

            AddAsSorted(mod.modTarget == EffectModValue.ModTarget.Amount ? amountEffects : recoverEffects);
            return;

            void AddAsSorted(List<EffectModValue> list)
            {
                if(list.Count <= 0)
                {
                    list.Add(mod);
                    return;
                }

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].order > mod.order)
                    {
                        list.Insert(i, mod);
                        return;
                    }

                    if (i == list.Count - 1)
                    {
                        list.Add(mod);
                        return;
                    }

                    if (list[i + 1].order > mod.order)
                    {
                        list.Insert(i+1, mod);
                        return;
                    }
                }
            }
        }

        private EffectModValue ApplyLawToMod(EffectModValue mod)
        {
            if (mod.ignoreLaw) return mod;
            List<AttributeLaw> laws = new List<AttributeLaw>();
            
            //todo: maybe optimize this one day to add as sorted
            if(globalLaw != null) laws.AddRange(globalLaw);
            if(mod.ownerEffect.source) laws.AddRange(mod.ownerEffect.source.attributeLawsSource);
            if(mod.ownerEffect.target) laws.AddRange(mod.ownerEffect.target.attributeLawsTarget);
            EffectHoldLaw lawHolder = mod.ownerEffect.GetComponent<EffectHoldLaw>();
            if (lawHolder) laws.AddRange(lawHolder.attributeLaws);
            
            laws.Sort();

            foreach (var law in laws)
            {
                //todo: applying at this stage seems in-effecient
                if (!law.CanApplyLaw(this,mod)) continue; 
                mod = law.ApplyLawToMod(this,mod);
            }

            return mod;
        }

        public virtual void RemoveEffectModValue(EffectModValue effect)
        {
            if(effect.modPattern == EffectModValue.ModPattern.ToBaseValue) return;
            if (effect.modTarget == EffectModValue.ModTarget.Amount) amountEffects.Remove(effect);
            else recoverEffects.Remove(effect);
        }

        #endregion
        
        public virtual void OnDestroy()
        {
            if (owner) owner.attributes.Remove(this); 
        }
        
        public virtual void Update()
        {
            ProcessRecovery();
        }

        protected virtual float ProcessCurrentData(float baseValue, List<EffectModValue> list)
        {
            float toReturn = baseValue;

            float addedMuti = 0;
            for (int i = 0; i < list.Count; i++)
            {
                EffectModValue effect = list[i];
                switch (effect.modPattern)
                {
                    case EffectModValue.ModPattern.Add:
                        toReturn += effect.value;
                        break;
                    
                    case EffectModValue.ModPattern.FinalMulti:
                        toReturn *= effect.value;
                        break;
                    
                    case EffectModValue.ModPattern.AddMulti:
                        
                        addedMuti += effect.value;
                        if(list[i+1].order <= effect.order) continue;
                        if(i != list.Count -1) continue;
                        
                        toReturn *= addedMuti;
                        addedMuti = 0;
                        break;
                    
                    case EffectModValue.ModPattern.Override:
                        toReturn = effect.value;
                        break;
                }
            }
            
            toReturn = Mathf.Min(toReturn, _maxAmount);
            toReturn = Mathf.Max(toReturn, _minAmount);

            return toReturn;
        }

        protected virtual void ProcessRecovery()
        {
            float recoverSpeed = ProcessCurrentData(_baseRecoverSpeed, recoverEffects);
            
            if(recoverSpeed == 0 ) return;

            float deltaTime;
            
            switch (recoverMode)
            {
                case RecoverTimeMode.Update:
                    deltaTime = Time.deltaTime;
                    break;
                case RecoverTimeMode.FixedUpdate:
                    deltaTime = Time.fixedDeltaTime;
                    break;
                case RecoverTimeMode.RealTime:
                    deltaTime = Time.unscaledDeltaTime;
                    break;
                default:
                    deltaTime = 0;
                    break;
            }
            
            if (recoverSpeed > 0 && _baseAmount  < _maxAmount)
            {
                _baseAmount += deltaTime * recoverSpeed;
                _baseAmount  = Mathf.Min(_baseAmount , _maxAmount);
            }
            
            if (recoverSpeed < 0 && _baseAmount  > 0)
            {
                _baseAmount  += deltaTime * recoverSpeed;
                _baseAmount  = Mathf.Max(_baseAmount , 0);
            }
        }

        
        /// <summary>
        /// The 
        /// </summary>
        /// <param name="mod"></param>
        protected virtual void EffectOneTimeMod(EffectModValue mod)
        {
            ModBaseAttribute(mod.value);
        }
        
        /// <summary>
        /// A direct modification to the base value
        /// Shall be treated carefully since this might skipped effect checks
        /// </summary>
        /// <param name="amount"></param>
        public virtual void ModBaseAttribute(float amount)
        {
            _baseAmount += amount;
            _baseAmount = Mathf.Min(_baseAmount, _maxAmount);
            _baseAmount = Mathf.Max(_baseAmount, _minAmount);
            
            if(_baseAmount >= _maxAmount) OnBaseValueReachMax?.Invoke();
            if(_baseAmount <= _minAmount) OnBaseValueReachMin?.Invoke();
        }

        public virtual void SetBaseValue(float value)
        {
            _baseAmount = value;
        }
        
        public float GetBaseValue(){return _baseAmount;}
        
        [ContextMenu("Log Value")]
        public void LogCurrentValue(){Debug.Log(currentAmount);}
    }
}