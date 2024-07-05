using UnityEditor;

namespace Editor
{
    public static class EditorToolsMenu
    {
        [MenuItem("DeletePlayerPrefs")]
        private static void DeleteAllPlayerPrefs()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
            UnityEngine.PlayerPrefs.Save();
        }
    }
}
