using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PAT
{
    public class Interactor: MonoBehaviour
    {
        public static List<Interactor> interactors;

        [Header("Attributes")]
        public float interactionDistance = 3f;
        public int priority = 0;
        public string info = "Interact";
        
        [Header("Events")]
        public UnityEvent onSelected;
        public UnityEvent onInteract;
        public UnityEvent onDeselected;

        public virtual void Awake()
        {
            if (interactors == null) interactors = new List<Interactor>();
            interactors.Add(this);
        }

        #region Enable & Disable

        public virtual void OnDisable()
        {
            interactors.Remove(this);
        }

        public virtual void OnDestroy()
        {
            interactors.Remove(this);
        }

        public virtual void OnEnable()
        {
            interactors.Add(this);
        }

        #endregion

        public virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }

        public virtual void Interact()
        {
            onInteract.Invoke();
        }
    }
}