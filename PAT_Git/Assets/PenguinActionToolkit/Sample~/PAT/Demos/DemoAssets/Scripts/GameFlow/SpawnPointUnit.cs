using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    public class SpawnPointUnit: Interactor
    {
        public int id = 0;
        public Transform spawnTransform;
        public Animator animator;
        
        public override void Awake()
        {
            base.Awake();
            if (!spawnTransform) spawnTransform = transform;
            if (!animator) animator = GetComponent<Animator>();
            
            onSelected.AddListener(OnSelected);
            onDeselected.AddListener(OnDeselected);
        }

        public override void Interact()
        {
            base.Interact();
            GameManager.Instance.saveDataContainer.data.spawnPointId = id;
            GameManager.Instance.saveDataContainer.SaveGame(); ;
        }

        void OnSelected()
        {
            if(!animator) return;

            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        void OnDeselected()
        {
            if(!animator) return;

            animator.updateMode = AnimatorUpdateMode.Normal;
        }
    }
}