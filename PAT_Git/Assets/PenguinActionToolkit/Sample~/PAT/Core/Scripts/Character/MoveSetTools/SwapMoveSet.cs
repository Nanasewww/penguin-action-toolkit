using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    [Serializable]
    public struct WeaponMoveSet
    {
        public MoveSet moveSet;
        public GameObject model;
    }
    
    // Pure Hard Code, shouldn't be used for further purpose
    public class SwapMoveSet : MonoBehaviour
    {
        [SerializeField] private WeaponMoveSet[] _weaponMoveSet;
        [SerializeField] private Character _character;
        private int currentSet;
        private void Awake()
        {
            _character.LoadMoveSet(_weaponMoveSet[0].moveSet);
            _weaponMoveSet[0].model.gameObject.SetActive(true);

            for (int i = 1; i < _weaponMoveSet.Length; i++)
            {
                if (_weaponMoveSet[i].model != null)
                {
                    _weaponMoveSet[i].model.gameObject.SetActive(false);
                }
            }

            currentSet = 0;
        }

        private void Update()
        {
            //todo: make this works again
            /*if (Input.GetKeyDown(KeyCode.Alpha1) || InputManager.Input.playerInput.GamePlay.Set1.WasPerformedThisFrame())
            {
                SwapSet(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) || InputManager.Input.playerInput.GamePlay.Set2.WasPerformedThisFrame())
            {
                SwapSet(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) || InputManager.Input.playerInput.GamePlay.Set3.WasPerformedThisFrame())
            {
                SwapSet(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4) || InputManager.Input.playerInput.GamePlay.Set4.WasPerformedThisFrame())
            {
                SwapSet(3);
            }*/
        }

        private void SwapSet(int index)
        {
            if (index == currentSet) return;
            if (index >= _weaponMoveSet.Length) return;
            _character.UnLoadMoveSet(_weaponMoveSet[currentSet].moveSet);
            if (_weaponMoveSet[currentSet].model != null)
            {
                _weaponMoveSet[currentSet].model.gameObject.SetActive(false);
            }
            
            currentSet = index;
            _character.LoadMoveSet(_weaponMoveSet[currentSet].moveSet);
            if (_weaponMoveSet[currentSet].model != null)
            {
                _weaponMoveSet[currentSet].model.gameObject.SetActive(true);
            }
        }
    }
}
