using UnityEditor;

namespace Editor
{
    public static class EditorToolsMenu
    {
        [MenuItem("Tools/DeletePlayerPrefs")]
        private static void DeleteAllPlayerPrefs()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
            UnityEngine.PlayerPrefs.Save();
        }
    }
}
