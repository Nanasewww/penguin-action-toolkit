using System;
using UnityEngine;

namespace PAT
{
    /// <summary>
    /// This is a typically class that add addtional functionality toward a moveset
    /// You subscribe to moveset's actions in awake and will get the informations
    /// </summary>
    public class MoveSetModelAttach: MonoBehaviour
    {
        public Transform model;
        public string socketName = "[MainWeaponSlot]";

        ModelHandler modelHandler;
        private void Awake()
        {
            MoveSet moveSet = GetComponent<MoveSet>();
            if (moveSet != null)
            {
                moveSet.onInitialized += Initialize;
                moveSet.onLoaded += OnLoad;
                moveSet.onUnloaded += OnUnload;
            }
        }

        private void OnDestroy()
        {
            Destroy(model.gameObject);
        }

        private void OnDisable()
        {
            model.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            model.gameObject.SetActive(true);
        }

        void Initialize(Character character)
        {
            modelHandler = character.modelHandler;
        }

        void OnLoad()
        {
            modelHandler.AttachTransformToSocket(model, socketName);
        }

        void OnUnload()
        {
            model.parent = transform;
            model.localPosition = Vector3.zero;
            model.localRotation = Quaternion.identity;
        }
    }
}