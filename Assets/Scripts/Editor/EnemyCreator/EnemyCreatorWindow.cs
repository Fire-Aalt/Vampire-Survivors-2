using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Animations;
using System;

namespace Game
{
    public class EnemyCreatorWindow : OdinEditorWindow
    {
        [Title("Settings (Can be modified only in code)")]
        [ReadOnly] public readonly string DataPath = "Assets/ScriptableObjects/Enemies";
        [ReadOnly] public readonly string PrefabPath = "Assets/Prefabs/Enemies";
        [ReadOnly] public readonly string AnimationsPath = "Assets/Animations/Enemies";
        [ReadOnly] public readonly string EnemyBasePrefabPath = "Assets/Prefabs/Enemies/EnemyBase.prefab";

        [Button(ButtonSizes.Large)]
        public void CreateNewEnemy(string enemyName)
        {
            if (string.IsNullOrEmpty(enemyName)) return;

            // Data
            EnemyDataSO data = CreateInstance<EnemyDataSO>();
            data.entityGuid = GUID.Generate().ToString();

            string dataFilePath = Path.Combine(DataPath, ToDataFileName(enemyName));
            CreateDirectoryFromAssetPath(dataFilePath);
            AssetDatabase.CreateAsset(data, dataFilePath);

            // Create Prefab Instance
            UnityEngine.Object parentPrefab = AssetDatabase.LoadAssetAtPath(EnemyBasePrefabPath, typeof(GameObject));
            GameObject instance = PrefabUtility.InstantiatePrefab(parentPrefab) as GameObject;

            instance.name = enemyName;
            var enemyComp = instance.GetComponent<Enemy>();
            enemyComp.Data = data;

            // Animation Controller
            string animFilePath = Path.Combine(AnimationsPath, enemyName, ToControllerFileName(enemyName));
            CreateDirectoryFromAssetPath(animFilePath);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(animFilePath);

            enemyComp.Animator.runtimeAnimatorController = controller;

            // Prefab
            string prefabFilePath = Path.Combine(PrefabPath, enemyName, ToPrefabFileName(enemyName));
            CreateDirectoryFromAssetPath(prefabFilePath);
            PrefabUtility.SaveAsPrefabAssetAndConnect(instance, prefabFilePath, InteractionMode.AutomatedAction);
        }

        [Button(ButtonSizes.Large)]
        public void RenameEnemy(string oldName, string newName)
        {
            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName)) return;

            string renamed = "Renamed: ";
            if (RenameFile(DataPath, oldName, newName, false, ToDataFileName))
            {
                renamed += "Data";
            }
            if (RenameFile(AnimationsPath, oldName, newName, true, ToControllerFileName))
            {
                renamed += ", AnimationController";
            }
            if (RenameFile(PrefabPath, oldName, newName, true, ToPrefabFileName))
            {
                renamed += ", Prefab";
            }
            Debug.Log(renamed);
        }

        private bool RenameFile(string relativePath, string oldName, string newName, bool hasNamedDirectory, Func<string, string> GetFileName)
        {
            if (hasNamedDirectory)
            {
                string oldFolderPath = Path.Combine(relativePath, oldName);
                string oldFilePath = Path.Combine(oldFolderPath, GetFileName(oldName));

                if (File.Exists(oldFilePath))
                {
                    AssetDatabase.RenameAsset(oldFilePath, GetFileName(newName));

                    string newFolderPath = Path.Combine(relativePath, newName);
                    AssetDatabase.MoveAsset(oldFolderPath, newFolderPath);
                    return true;
                }
            }
            else
            {
                string oldFilePath = Path.Combine(relativePath, GetFileName(oldName));

                if (File.Exists(oldFilePath))
                {
                    AssetDatabase.RenameAsset(oldFilePath, GetFileName(newName));
                    return true;
                }
            }
            return false;
        }

        public static void CreateDirectoryFromAssetPath(string assetPath)
        {
            string directoryPath = Path.GetDirectoryName(assetPath);
            if (Directory.Exists(directoryPath))
                return;
            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
        }

        private string ToDataFileName(string name) => name + ".asset";
        private string ToPrefabFileName(string name) => name + ".prefab";
        private string ToControllerFileName(string name) => "_" + name + ".controller";

        [MenuItem("Game/Enemy Creator")]
        private static void OpenWindow()
        {
            GetWindow<EnemyCreatorWindow>().Show();
        }
    }
}
