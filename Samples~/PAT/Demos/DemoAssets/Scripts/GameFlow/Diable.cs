using System;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class Diable: Interactor
    {
        [Serializable]
        public struct DiaContent
        {
            public bool spearkerIsPlayer;
            public string speakerName;
            public GameObject speakerObj;
            [TextArea]public string content;
        }
        
        public float hintDistance = 10f;
        public Transform camPos;
        [TextArea] public string hintText;
        public Vector3 hintOffset = new Vector3(0, 1, 0);
        public List<DiaContent> dias; 


        public static event Action<Diable> onDiableSpawn;
        public static event Action<Diable> onDiaStart;
        public event Action onHintRangeIn;
        public event Action onHintRangeOut;

        protected bool inRange = false;
        
        private void Reset()
        {
            info = "Talk";
        }

        public override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, hintDistance);
        }

        private void Start()
        {
            onDiableSpawn?.Invoke(this);
        }

        private void Update()
        {
            float dis = Vector3.Distance(transform.position, PlayerInteraction.Instance.transform.position);
            if(inRange && dis > hintDistance) {inRange = false; onHintRangeOut?.Invoke();}
            if(!inRange && dis < hintDistance) {inRange = true; onHintRangeIn?.Invoke();}
        }

        public override void Interact()
        {
            base.Interact();
            onDiaStart?.Invoke(this);
        }
    }
    
    
}