using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PAT
{
    public class GrantTagMod : StateModifier
    {
        [SerializeField] protected List<GamePlayTag> _grantTags = new List<GamePlayTag>();

        public List<GamePlayTag> grantTags { get { return _grantTags; } set { _grantTags = value; } }

        private void Awake()
        {
            if (_grantTags == null) { _grantTags = new List<GamePlayTag>(); }
        }

        public override void BeginEvent()
        {
            base.BeginEvent();
            foreach (GamePlayTag grantTag in _grantTags) characterController.tagContainer.stateTags.Add(grantTag);
        }

        public override void EndEvent()
        {
            base.EndEvent();
            foreach (GamePlayTag t in _grantTags) { characterController.tagContainer.stateTags.Remove(t); }
        }
    }
}
