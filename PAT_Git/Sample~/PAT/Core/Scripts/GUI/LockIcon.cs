using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PAT
{
    public class LockIcon : MonoBehaviour
    {
        public int playerID = -1;
        [SerializeField] protected Image img;

        protected PlayerVCamLock LockVCam;
        private Player _player;
        private void Awake()
        {
            if (img == null) img = GetComponent<Image>();
        }

        private void Start()
        {
            _player = Player.GetPlayerByID(playerID);
            LockVCam = _player.GetComponentInChildren<PlayerVCamLock>();
        }

        private void Update()
        {
            if(!LockVCam) return;
            
            //Case Not Locking;
            if (!LockVCam.IsActive() || !LockVCam.currentTarget) { img.enabled = false; return; }

            //Case Locking
            img.enabled = true;
            RectTransform rectThis = ((RectTransform)transform);
            RectTransform rectParent = ((RectTransform)transform.parent);
            rectThis.anchoredPosition = _player.WorldToPlayerViewPositionNormalized(LockVCam.currentTarget.transform.position);
            rectThis.anchoredPosition = new Vector2(rectThis.anchoredPosition.x * rectParent.sizeDelta.x, rectThis.anchoredPosition.y* rectParent.sizeDelta.y);
        }
    }
}
