
#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "GamePlayTagHelper", menuName = "CreateGamePlayTagContainer", order = 1)]
public class GamePlayTagHelper: ScriptableObject
{
    public static GamePlayTagHelper instance;
    [SerializeField] protected GamePlayTag exampleTag;
    public List<EnumFolder> folders = new List<EnumFolder>();
    const string enumName = "GamePlayTag";
    const string extension = ".cs";
    const string path = "./";

    public void Awake()
    {
        instance = this;
    }

    public void OnEnable()
    {
        instance = this;
    }

    [System.Serializable]
    public class EnumEntry
    {
        public string name;
        public int id;
    }
    [System.Serializable]
    public class EnumFolder
    {
        public string name;
        public List<EnumEntry> enums;
    }

    // Method to load current enum entries into the ScriptableObject
    [ContextMenu("Initialize")]
    public void InitializeEnumEntries()
    {
        folders.Clear();
        folders.Add(new EnumFolder {name = "Basic", enums = new List<EnumEntry>()});
        folders.Add(new EnumFolder {name = "Combo", enums = new List<EnumEntry>() });
        folders.Add(new EnumFolder {name = "Input", enums = new List<EnumEntry>() });
        folders.Add(new EnumFolder {name = "Attributes", enums = new List<EnumEntry>() });
        folders.Add(new EnumFolder {name = "Other", enums = new List<EnumEntry>() });
        foreach (var value in System.Enum.GetValues(typeof(GamePlayTag)))
        {
            EnumEntry entry = new EnumEntry { name = value.ToString(), id = (int)value };
            switch ((int)value)
            {
                case < 100:
                    folders[0].enums.Add(entry);
                    break;
                case < 200:
                    folders[1].enums.Add(entry);
                    break;
                case < 500:
                    folders[2].enums.Add(entry);
                    break;
                case < 2000:
                    folders[3].enums.Add(entry);
                    break;
                default:
                    folders[4].enums.Add(entry);
                    break;
            }

        }
    }
    
    [ContextMenu("Refresh")]
    public void RefreshFolderList()
    {
        // Step 1: Get all enum values for GamePlayTag
        var enumValues = System.Enum.GetValues(typeof(GamePlayTag));

        // Step 2: Track found enum entries
        HashSet<int> foundEnumIds = new HashSet<int>();

        // Step 3: Iterate over all enum members
        foreach (var enumValue in enumValues)
        {
            int enumId = (int)enumValue;
            string enumName = enumValue.ToString();

            // Step 4: Try to find an existing EnumEntry with a matching ID
            EnumEntry matchingEntry = null;
            foreach (var folder in folders)
            {
                matchingEntry = folder.enums.Find(entry => entry.id == enumId);
                if (matchingEntry != null)
                {
                    // Step 5: Update the name if found
                    matchingEntry.name = enumName;
                    foundEnumIds.Add(enumId);  // Track the found ID
                    break;
                }
            }

            // Step 6: If no matching entry found, create a new one and add it to the last folder
            if (matchingEntry == null)
            {
                EnumEntry newEntry = new EnumEntry { id = enumId, name = enumName };
                folders[^1].enums.Add(newEntry);  // Add to the last folder
                foundEnumIds.Add(enumId);  // Track the new ID
            }
        }

        // (Optional) Step 7: Remove obsolete entries (entries that no longer match any enum member)
        foreach (var folder in folders)
        {
            folder.enums.RemoveAll(entry => !foundEnumIds.Contains(entry.id));
        }
    }

    public void SortEntriesByUserOrder()
    {
        // Sorting logic can be applied here based on user input
    }

    public void AddFolder(string folderName)
    {
        // Add new folder logic
    }

    public void DeleteFolder(string folderName)
    {
        // Delete folder logic
    }
}
#endif