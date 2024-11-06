using System;
using System.Collections;
using UnityEngine;

namespace PAT
{
    public class TextTools
    {
        public static IEnumerator TextSequence(string content, Action<string> result, Action onComplete)
        {
            string toReturn = "";
            foreach (char c in content)
            {
                toReturn += c;
                yield return new WaitForSecondsRealtime(0.05f);
                if(c is '.' or '!' or ',' or '?') yield return new WaitForSecondsRealtime(0.05f);
                result(toReturn);
            }

            onComplete?.Invoke();
        }
    }
}