using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Game
{
    [InitializeOnLoad]
    public static class UndirtyOnSave
    {
        const string WORLD_HOLDER = "---WORLD---";

        static UndirtyOnSave()
        {
            EditorSceneManager.sceneSaving += HandleSceneSaving;
        }

        static void HandleSceneSaving(Scene scene, string path)
        {
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.name == WORLD_HOLDER)
                {
                    var holder = root.GetComponentInChildren<RuntimeWorldHolder>();
                    if (holder != null)
                    {
                        holder.ClearEverything();
                    }
                    return;
                }
            }
        }
    }
}
