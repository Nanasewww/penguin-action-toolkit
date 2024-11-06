using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PAT
{
    public class PlayerInteraction: MonoBehaviour
    {
        public static PlayerInteraction Instance;
        public InputActionReference inputAction;
        public Interactor currentSelection;

        public static event Action<Interactor> onSelect;


        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            //out of range
            CheckIfOutOfRange();
            UpdateCurrentSelection();
            
            if(!currentSelection)return;
            if(inputAction.action.WasPressedThisFrame()) currentSelection.Interact();
        }

        public void ChangeSelection(Interactor newSelection)
        {
            if(currentSelection) currentSelection.onDeselected.Invoke();
            currentSelection = newSelection;
            if(currentSelection) currentSelection.onSelected.Invoke();
            onSelect?.Invoke(currentSelection);
        }
        private void CheckIfOutOfRange()
        {
            if (!currentSelection) return;
            
            float dis = Vector3.Distance(currentSelection.transform.position, transform.position);
            if (currentSelection.interactionDistance < dis)ChangeSelection(null);
        }

        private void UpdateCurrentSelection()
        {
            if(Interactor.interactors == null) return;
            
            foreach (Interactor interactor in Interactor.interactors)
            {
                //Check if within range
                float dis = Vector3.Distance(interactor.transform.position, transform.position);
                if(interactor.interactionDistance < dis) continue;

                //Case: no selection yet
                if (!currentSelection) { ChangeSelection(interactor); continue; }
                
                //Case: different priority
                if(currentSelection.priority > interactor.priority) continue;
                if(currentSelection.priority < interactor.priority)  { ChangeSelection(interactor); continue; }
                
                //Case: pick the closer one
                float currentDist = Vector3.Distance(currentSelection.transform.position, transform.position);
                if(currentDist > dis) { ChangeSelection(interactor); continue; }
            }
        }
        
    }
}