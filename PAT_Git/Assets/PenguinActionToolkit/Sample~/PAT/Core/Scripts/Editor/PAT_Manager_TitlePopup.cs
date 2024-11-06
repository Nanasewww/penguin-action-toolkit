using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace PAT
{
    public class PAT_Manager_TitlePopup : PopupWindowContent
    {
        // Start is called before the first frame update
        private string newTitle;
        private System.Action<string> onTitleChanged;

        public PAT_Manager_TitlePopup(string currentTitle, System.Action<string> onTitleChanged)
        {
            this.newTitle = currentTitle;
            this.onTitleChanged = onTitleChanged;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 80);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Edit Node Title", EditorStyles.boldLabel);
            newTitle = EditorGUILayout.TextField("New Title", newTitle);

            if (GUILayout.Button("Apply"))
            {
                onTitleChanged?.Invoke(newTitle);
                editorWindow.Close();
            }
        }

        public override void OnOpen() { }

        public override void OnClose() { }
        }
}
