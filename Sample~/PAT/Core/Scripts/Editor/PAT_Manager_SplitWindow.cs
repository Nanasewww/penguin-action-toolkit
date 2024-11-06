using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PAT
{
    public class PAT_Manager_SplitWindow : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<PAT_Manager_SplitWindow, TwoPaneSplitView.UxmlTraits>{}
    }
}
