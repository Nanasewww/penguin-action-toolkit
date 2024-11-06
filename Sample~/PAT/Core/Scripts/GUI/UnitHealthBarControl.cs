using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class UnitHealthBarControl : MonoBehaviour
    {
        public int playerID =-1;
        [SerializeField] protected GameObject unitHealthBarPrefab;
        [SerializeField] protected GameObject unitHealthBarFriendPrefab;

        protected Player _player;
        private void Awake()
        {
            PATComponent.OnAnySpawn += SpawnUnitHealthBar;
        }

        private void OnDestroy()
        {
            PATComponent.OnAnySpawn -= SpawnUnitHealthBar;
        }

        public void SpawnUnitHealthBar(PATComponent pat)
        {
            if (_player == null) _player = Player.GetPlayerByID(playerID);
            if (_player == null) return;
            
            if(pat == _player.character) return;
            
            //make sure space is matching
            RectTransform rectThis = ((RectTransform)transform);
            RectTransform rectParent = ((RectTransform)transform.parent);
            rectThis.sizeDelta = rectParent.sizeDelta;
            
            if (pat.team != _player.character.team)
            {
                GameObject obj = Instantiate(unitHealthBarPrefab, transform);
                obj.GetComponent<UnitHealthBar>().Initialization(pat, _player);
            }
            else
            {
                GameObject obj = Instantiate(unitHealthBarFriendPrefab, transform);
                obj.GetComponent<UnitHealthBar>().Initialization(pat, _player); 
            }
        }
    }
}
