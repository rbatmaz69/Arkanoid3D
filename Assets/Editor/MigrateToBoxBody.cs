using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Einmaliges Werkzeug: ersetzt Rigidbody und Collider durch BoxBody (eigene Kollision).
// Menü: Tools > Arkanoid > Migrate Physics To BoxBody. Danach diese Datei löschen.
public static class MigrateToBoxBody
{
    private static readonly string[] WallNames = { "Wand_LInks", "Wand_rechts", "Wans_Oben" };

    [MenuItem("Tools/Arkanoid/Migrate Physics To BoxBody")]
    public static void Run()
    {
        if (!EditorUtility.DisplayDialog("Migration",
                "Rigidbody und Collider in Game.unity und allen Prefabs durch BoxBody ersetzen?\n" +
                "Vorher committen oder sichern.", "Ausführen", "Abbrechen"))
        {
            return;
        }

        int changed = 0;

        // Prefabs: Basis-Prefabs zuerst, Varianten danach (erben die Änderungen)
        List<string> bases = new List<string>();
        List<string> variants = new List<string>();
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (PrefabUtility.GetPrefabAssetType(asset) == PrefabAssetType.Variant) variants.Add(path);
            else bases.Add(path);
        }

        foreach (string path in bases) changed += MigratePrefab(path);
        foreach (string path in variants) changed += MigratePrefab(path);

        // Szene
        Scene scene = EditorSceneManager.OpenScene("Assets/Scenes/Game.unity", OpenSceneMode.Single);
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            changed += MigrateHierarchy(root);
        }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        AssetDatabase.SaveAssets();
        Debug.Log("Migration fertig, geänderte Objekte: " + changed);
    }

    private static int MigratePrefab(string path)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(path);
        int changed = MigrateHierarchy(root);
        if (changed > 0) PrefabUtility.SaveAsPrefabAsset(root, path);
        PrefabUtility.UnloadPrefabContents(root);
        return changed;
    }

    private static int MigrateHierarchy(GameObject root)
    {
        int changed = 0;

        // Collider zuerst, Rigidbody zuletzt
        foreach (Collider col in root.GetComponentsInChildren<Collider>(true))
        {
            if (PrefabUtility.IsPartOfPrefabInstance(col)) continue; // erbt von Basis-Prefab

            GameObject go = col.gameObject;
            if (col is BoxCollider box && NeedsBoxBody(go) && go.GetComponent<BoxBody>() == null)
            {
                BoxBody body = go.AddComponent<BoxBody>();
                body.Center = box.center;
                body.Size = box.size;
                body.Solid = IsSolid(go);
            }

            Object.DestroyImmediate(col, true);
            changed++;
        }

        foreach (Rigidbody rb in root.GetComponentsInChildren<Rigidbody>(true))
        {
            if (PrefabUtility.IsPartOfPrefabInstance(rb)) continue;
            Object.DestroyImmediate(rb, true);
            changed++;
        }

        return changed;
    }

    private static bool NeedsBoxBody(GameObject go)
    {
        if (go.GetComponent<Block>() != null) return true;
        if (go.GetComponent<StableBlock>() != null) return true;
        if (go.GetComponent<SpecialBlock>() != null) return true;
        if (go.GetComponent<BodensensorTrigger>() != null) return true;
        if (go.GetComponent<PowerUpPickup>() != null) return true;
        return System.Array.IndexOf(WallNames, go.name) >= 0;
    }

    // Nur Wände und Blöcke sind Hindernisse. Bodensensor und Power-ups sind Zonen.
    private static bool IsSolid(GameObject go)
    {
        return go.GetComponent<BodensensorTrigger>() == null && go.GetComponent<PowerUpPickup>() == null;
    }
}
