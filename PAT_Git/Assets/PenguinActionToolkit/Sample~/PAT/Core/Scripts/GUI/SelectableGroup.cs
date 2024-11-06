using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PAT
{
    public class SelectableGroup: MonoBehaviour
    {
        public static List<SelectableGroup> groups;

        public int priority = 0;
        public bool state = false;
        public Selectable defaultOption;
        public List<Selectable> options;
        public GameObject selectionIndicator;

        public UnityEvent onCancel;

        private GameObject lastSelection;
        private void Awake()
        {
            if (groups == null) groups = new List<SelectableGroup>();
            groups.Add(this);
            
            Selectable[] childOptions = GetComponentsInChildren<Selectable>();
            foreach (Selectable s in childOptions)
            {
                if(options.Contains(s)) continue;
                options.Add(s);
            }
        }

        private void OnDisable() { groups.Remove(this); ChangeState(false);}

        private void OnEnable() { if(!groups.Contains(this))groups.Add(this); }

        private void OnDestroy() { groups.Remove(this); }


        public void ChangeState(bool target)
        {
            state = target;
            foreach (Selectable selectable in options)
            {
                selectable.interactable = target;
            }

            if (state)
            {
                EventSystem.current.SetSelectedGameObject(defaultOption.gameObject);
                if(lastSelection)EventSystem.current.SetSelectedGameObject(lastSelection);
                if (selectionIndicator)
                {
                    selectionIndicator.transform.position =
                        EventSystem.current.currentSelectedGameObject.transform.position;
                }
            }
            
            if(selectionIndicator) selectionIndicator.SetActive(state);
        }
        
        private void Update()
        {
            //if there's higher priority, turn off
            foreach (SelectableGroup s in groups)
            {
                if(s.priority <= priority) continue;
                if (state == true) ChangeState(false);
                return;
            }
            
            //Return in the first update to avoid input issue
            if(state == false) {ChangeState(true); return;} 

            if (!EventSystem.current.currentSelectedGameObject)
            {
                EventSystem.current.SetSelectedGameObject(lastSelection);
            }
            if (!EventSystem.current.currentSelectedGameObject)
            {
                EventSystem.current.SetSelectedGameObject(defaultOption.gameObject);
            }
            
            if (selectionIndicator)
            {
                selectionIndicator.transform.position =
                    EventSystem.current.currentSelectedGameObject.transform.position;
            }
            
            //todo: make this work again with player?
            //if(Player.Players[0].WasPressedThisFrame()) onCancel.Invoke();

            lastSelection = EventSystem.current.currentSelectedGameObject;
        }
        
    }
}