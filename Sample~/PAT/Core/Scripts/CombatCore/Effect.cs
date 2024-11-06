using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    /// <summary>
    /// When using scriptable object in assets, try always instantiate instead directly applying
    /// Or mutiple character might having same effect instance at same time which is undeisred
    /// </summary>
    [CreateAssetMenu(menuName = "PAT/Effect")]
    public class Effect : ScriptableObject
    {
        
        public List<GamePlayTag> mainTags = new();
        public List<GamePlayTag> infoTags = new();
        [SerializeField] protected float _appliedTime;
        [SerializeField] protected PATComponent _source;
        [SerializeField] protected PATComponent _target;
        public List<EffectComponent> components = new();
        
        private bool _removeFlag = false;
        private bool _componentInitialized = false;
        
        public float appliedTime {get {return _appliedTime;}}
        public PATComponent source {get {return _source;} set {_source = value;}}
        public PATComponent target {get {return  _target;} set {_target = value;}}
        public bool removeFlag {get {return _removeFlag;}}


        /// <summary>
        /// This is the most appropriate to initialize an Effect from script
        /// You can set up the components easily can keep track of it
        /// Else you might lose reference since there's a internal intialization that clone the component
        /// You can even set it to null for now, we just want to know this object is not from assets 
        /// </summary>
        /// <param name="mtags">Can be null if you don't want to set up</param>
        /// <param name="effectComponents">Can be null if you don's want to set up</param>
        /// <param name="source">Can be null if you don's want to set up</param>
        /// <returns></returns>
        public static Effect NewEffect(ICollection<GamePlayTag> mtags, ICollection<EffectComponent> effectComponents, PATComponent source)
        {
            Effect newEffect = CreateInstance<Effect>();
            if( mtags != null) newEffect.mainTags = mtags.ToList();
            if( effectComponents != null) newEffect.components = effectComponents.ToList();
            newEffect._componentInitialized = true;//This is required or you might lost the reference of components
            newEffect._source = source;
            newEffect.infoTags = new List<GamePlayTag>();
            
            return newEffect;
        }
        
        public void OnApply(PATComponent controller)
        {
            if (controller == null) return;
            
            //We use this to make sure effect don't share component object
            if(!_componentInitialized) InitializeComponents();
            
            _target= controller;
            _appliedTime = 0f;
            _removeFlag = false;
            
            foreach (GamePlayTag mainTag in mainTags){controller.tagContainer.effectTags.Add(mainTag);}
            
            foreach (EffectComponent component in components) { component.OnApply(this); }
        }

        void InitializeComponents()
        {
            for (int i = 0; i < components.Count; i++)
            {
                EffectComponent component = Instantiate(components[i]);
                components[i] = component;
            }
            _componentInitialized = true;
        }

        public void OnTick()
        {
            if (_target == null) return;

            _appliedTime += Time.deltaTime;

            
            foreach (EffectComponent effectComponent in components)
            {
                effectComponent.OnTick(this);
            }

            if (_removeFlag) { MarkAsRemove(); }
        }

        public void OnRemove()
        {
            foreach (GamePlayTag t in mainTags) { _target.tagContainer.effectTags.Remove(t); }
            
            foreach (EffectComponent effectComponent in components)
            {
                effectComponent.OnRemove(this);
            }
        }

        public bool HasTag(GamePlayTag tag)
        {
            foreach (GamePlayTag t in mainTags)
            {
                if (t == tag) return true;
            }

            return false;
        }

        /// <summary>
        /// This is a proper way to remove an Effect
        /// Since it will only execute the remove at the end of Tick
        /// </summary>
        public void MarkAsRemove()
        {
            _removeFlag = true;
        }

        public T GetComponent<T>() where T : EffectComponent
        {
            return components.OfType<T>().FirstOrDefault();
        }
        
        public List<T> GetComponents<T>() where T : EffectComponent
        {
            return components.OfType<T>().ToList();
        }

        public static T GetComponentFromList<T>(List<Effect> effects) where T : EffectComponent
        {
            foreach (var effect in effects)
            {
                T component = effect.GetComponent<T>();
                if(component != null) return component;
            }
            return null;
        }
        
        public static List<T> GetComponentsFromList<T>(List<Effect> effects) where T : EffectComponent
        {
            List<T> components = new List<T>();
            foreach (var effect in effects)
            {
                List<T> component = effect.GetComponents<T>();
                if(component != null) components.AddRange(component);
            }
            return components;
        }
    }
}
