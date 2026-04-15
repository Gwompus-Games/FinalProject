using DunGen.Versioning;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DunGen.Editor.Versioning
{
	[InitializeOnLoad]
	public static class AssetMigrationSystem
	{
		static AssetMigrationSystem()
		{
			EditorApplication.delayCall += CheckDunGenMigrationVersion;
		}

		private static void CheckDunGenMigrationVersion()
		{
			if (!DunGenSettings.Instance.IsMigrationRequired())
				return;

			AssetMigrationWindow.Open(true);
		}

		[MenuItem("Window/DunGen/Run Project Migration", priority = 10)]
		public static void OpenMigrationWindow()
		{
			AssetMigrationWindow.Open(false);
		}

		internal static void RunMigration(bool scanAssets, bool scanOpenScenes, string[] searchFolders = null)
		{
			var updatedScriptableObjectPaths = new List<string>();
			var updatedPrefabPaths = new List<string>();
			var updatedSceneObjectNames = new List<string>();

			try
			{
				AssetDatabase.StartAssetEditing();

				if (scanAssets)
				{
					EditorUtility.DisplayProgressBar("Migrating", "Scanning ScriptableObjects...", 0.0f);
					updatedScriptableObjectPaths = MigrateScriptableObjects(searchFolders);

					EditorUtility.DisplayProgressBar("Migrating", "Scanning Prefabs...", 0.33f);
					updatedPrefabPaths = MigratePrefabs(searchFolders);
				}

				if (scanOpenScenes)
				{
					EditorUtility.DisplayProgressBar("Migrating", "Scanning Scene Objects...", 0.66f);
					updatedSceneObjectNames = MigrateSceneObjects();
				}

				if (DunGenSettings.Instance.IsMigrationRequired())
				{
					DunGenSettings.Instance.UpdateMigrationVersion();
					EditorUtility.SetDirty(DunGenSettings.Instance);
				}
			}
			finally
			{
				EditorUtility.ClearProgressBar();

				AssetDatabase.StopAssetEditing();
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			string logMessage = "<b>[DunGen]</b> Migration Complete" +
				$"\nUpdated {updatedScriptableObjectPaths.Count} ScriptableObjects, {updatedPrefabPaths.Count} Prefabs, and {updatedSceneObjectNames.Count} Scene Objects";

			if (updatedScriptableObjectPaths.Count > 0)
			{
				logMessage += "\n\n--- ScriptableObjects ---";
				foreach (var path in updatedScriptableObjectPaths)
					logMessage += $"\n\t- {path}";
			}

			if (updatedPrefabPaths.Count > 0)
			{
				logMessage += "\n\n--- Prefabs ---";
				foreach (var path in updatedPrefabPaths)
					logMessage += $"\n\t- {path}";
			}

			if (updatedSceneObjectNames.Count > 0)
			{
				logMessage += "\n\n--- Scene Objects ---";
				foreach (var name in updatedSceneObjectNames)
					logMessage += $"\n\t- {name}";
			}

			Debug.Log(logMessage);
		}

		private static List<string> MigrateScriptableObjects(string[] searchFolders)
		{
			var updatedPaths = new List<string>();

			string[] guids = (searchFolders != null && searchFolders.Length > 0)
				? AssetDatabase.FindAssets("t:VersionedScriptableObject", searchFolders)
				: AssetDatabase.FindAssets("t:VersionedScriptableObject");

			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				var so = AssetDatabase.LoadAssetAtPath<VersionedScriptableObject>(path);

				if (so != null && so.RequiresMigration)
				{
					so.Migrate();
					EditorUtility.SetDirty(so);

					updatedPaths.Add(path);
				}
			}

			return updatedPaths;
		}

		private static List<string> MigratePrefabs(string[] searchFolders)
		{
			var updatedPaths = new List<string>();

			string[] guids = (searchFolders != null && searchFolders.Length > 0)
				? AssetDatabase.FindAssets("t:Prefab", searchFolders)
				: AssetDatabase.FindAssets("t:Prefab");

			var paths = new List<string>();
			foreach (var guid in guids)
				paths.Add(AssetDatabase.GUIDToAssetPath(guid));

			int current = 0;
			int total = paths.Count;

			foreach (string path in paths)
			{
				current++;
				EditorUtility.DisplayProgressBar("Migrating Prefabs", $"Processing {System.IO.Path.GetFileName(path)}", (float)current / total);

				GameObject contentsRoot = null;

				try
				{
					contentsRoot = PrefabUtility.LoadPrefabContents(path);
					var components = contentsRoot.GetComponentsInChildren<VersionedMonoBehaviour>(true);
					bool isDirty = false;

					foreach (var comp in components)
					{
						if (comp.RequiresMigration)
						{
							comp.Migrate();
							isDirty = true;
						}
					}

					if (isDirty)
					{
						PrefabUtility.SaveAsPrefabAsset(contentsRoot, path);
						updatedPaths.Add(path);
					}
				}
				catch (System.Exception e)
				{
					Debug.LogError($"Failed to migrate prefab at {path}: {e.Message}");
				}
				finally
				{
					if (contentsRoot != null)
						PrefabUtility.UnloadPrefabContents(contentsRoot);
				}
			}

			EditorUtility.ClearProgressBar();
			return updatedPaths;
		}

		private static List<string> MigrateSceneObjects()
		{
			var updatedObjectNames = new List<string>();

			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);

				foreach (var rootGo in scene.GetRootGameObjects())
				{
					var versionedComps = rootGo.GetComponentsInChildren<VersionedMonoBehaviour>(true);

					foreach (var comp in versionedComps)
					{
						if (comp.RequiresMigration)
						{
							comp.Migrate();
							EditorUtility.SetDirty(comp);
							updatedObjectNames.Add(comp.gameObject.name);
						}
					}
				}
			}

			return updatedObjectNames;
		}
	}
}