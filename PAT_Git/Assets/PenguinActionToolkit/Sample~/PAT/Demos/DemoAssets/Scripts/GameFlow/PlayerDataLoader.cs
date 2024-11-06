using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    //todo: make a better example for data loading
    public class PlayerDataLoader: MonoBehaviour
    {
        [FormerlySerializedAs("healthSoulLike")] [FormerlySerializedAs("_health")] [SerializeField] protected Health_PAT _healthPat;

        private void Awake()
        {
            if (!_healthPat) _healthPat = GetComponent<Health_PAT>();

            PlayerData data = GameManager.Instance.saveDataContainer.data;
            if (_healthPat) _healthPat.maxAmount = data.maxHp;
        }
    }
}