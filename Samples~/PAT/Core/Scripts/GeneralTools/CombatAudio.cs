using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class CombatAudio : MonoBehaviour
    {
        [Serializable]
        public class auidoUnit
        {
            public AudioSource audio;
            public float baseVolume;
        }
        
        public List<auidoUnit> auidoUnits = new List<auidoUnit>();
        public float fadeTime = 0.3f;
        public float bpm = 132;

        private Attribute sp;
        public static bool beat = false;
        public static bool beat4 = false;
        private float accTime = 0f;
        private int accFour = 0;
        // Start is called before the first frame update
        void Start()
        {
            foreach (var unit in auidoUnits)
            {
                unit.baseVolume = unit.audio.volume;
            }

            auidoUnits[1].audio.volume = 0f;
            auidoUnits[2].audio.volume = 0f;

            sp = Player.Players[0].character.GetAttributeByTag(GamePlayTag.SpAttack);
        }

        // Update is called once per frame
        void Update()
        {
            accTime += Time.deltaTime;
            beat = false;
            beat4 = false;
            if (accTime >= (60f / bpm))
            {
                accTime -= 60f / bpm;
                beat = true;
                accFour++;
            }

            if (accFour >= 4)
            {
                beat4 = true;
                accFour = 0;
            }
            
            if (sp == null)
            {
                sp = Player.Players[0].character.GetAttributeByTag(GamePlayTag.SpAttack);
                return;
            }

            if (sp.currentAmount >= 1)
            {
                if(auidoUnits[1].audio.volume < auidoUnits[1].baseVolume) auidoUnits[1].audio.volume += (1/fadeTime) * Time.deltaTime;
            }
            else
            {
                auidoUnits[1].audio.volume -= (1/fadeTime) * Time.deltaTime;
            }
            
            if (sp.currentAmount >= 3)
            {
                if(auidoUnits[2].audio.volume < auidoUnits[2].baseVolume) auidoUnits[2].audio.volume += (1/fadeTime) * Time.deltaTime;
            }
            else
            {
                auidoUnits[2].audio.volume -= (1/fadeTime) * Time.deltaTime;
            }
        }
    }
}
