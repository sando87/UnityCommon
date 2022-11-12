#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// 유니티 에디터 상태에서 Asset들을 일괄적으로 수정하는 등과 같은 작업을 수행

public class AssetEditor : MonoBehaviour
{
    public GameObject RefPrefab = null;

    [MenuItem("MyMenu/CreateSuitDB")]
    static void CreateSuitDB()
    {
        List<SuitRawInfo> infos = new List<SuitRawInfo>();
        infos.Add(new SuitRawInfo());
        infos.Add(new SuitRawInfo());
        infos.Add(new SuitRawInfo());
        infos.Add(new SuitRawInfo());
        infos.Add(new SuitRawInfo());
        infos.Add(new SuitRawInfo());
        string csvstring = CSVParser<SuitRawInfo>.Serialize(',', infos);
        File.WriteAllText("./Assets/00_MetaSuit/Resources/Database/SuitRawInfo.csv", csvstring);

        // SuitRawInfo info = DatabaseCSV<SuitRawInfo>.Instance.GetInfo(2);
        // LOG.trace(info.name);
    }

    [MenuItem("CONTEXT/AssetEditor/AddNewExteriors")]
    static void AddNewExteriors(MenuCommand command)
    {
        AssetEditor inst = (AssetEditor)command.context;
        if(inst == null)
            return;
        
        foreach(string guid in Selection.assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
            
            string localPath = "Assets/00_MetaSuit/Resources/MapObjects/Props/Exterior/Exterior_" + sprite.name + ".prefab";
            
            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab and log whether Prefab was saved successfully.
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(inst.RefPrefab, localPath);

            SpriteRenderer sr = newPrefab.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = sprite;

            newPrefab.GetComponentInChildren<BoxCollider>().size = sr.bounds.size;
            newPrefab.GetComponentInChildren<BoxCollider>().center = sr.bounds.center;

            string newPrefabGUID = AssetDatabase.AssetPathToGUID(localPath);
            newPrefab.GetComponentInChildren<MapEditorObject>().resourceID = MyUtils.GUIDToLong(newPrefabGUID);

            LOG.trace(newPrefab.name);
        }
        
        AssetDatabase.SaveAssets();
    }





    [MenuItem("MyMenu/Edit Renderers")]
    static void EditRenderers()
    {
        Debug.Log("EditRenderers...");
        
        Material mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/00_MetaSuit/06_Materials/FlashSprite.mat", typeof(Material));

        foreach(GameObject obj in Selection.gameObjects)
        {
            List<Transform> childs = new List<Transform>();
            obj.transform.FindChildAll("key (2)", childs);
            foreach(Transform target in childs)
                ChangeToFlashRenderer(target.gameObject, mat);
        }
        
        AssetDatabase.SaveAssets();

        // string path = AssetDatabase.GetAssetPath(target);
        // GameObject targetInst = PrefabUtility.InstantiatePrefab(target) as GameObject;
        // targetInst.GetComponentInChildren<SpriteRenderer>().enabled = false;
        // PrefabUtility.SaveAsPrefabAsset(targetInst, path, out bool success);
        // LOG.trace(success);
    }
    [MenuItem("MyMenu/Edit Renderers", true)]
    static bool EditRenderersValide()
    {
        return Selection.activeObject != null && EditorUtility.IsPersistent(Selection.activeGameObject);
    }
    static void ChangeToFlashRenderer(GameObject target, Material mat)
    {
        SpriteRenderer sr = target.GetComponentInChildren<SpriteRenderer>();
        DestroyImmediate(sr.GetComponent<AllIn1SpriteShader.AllIn1Shader>(), true);
        DestroyImmediate(sr.GetComponent<RendererAllInOne>(), true);
        sr.material = mat;
        sr.gameObject.AddComponent<RendererSpriteFlash>();
    }










    // =========================================================================================================
    // 기본 사용법 예시
    // =========================================================================================================

    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("MyMenu/Do Something")]
    static void DoSomething()
    {
        Debug.Log("Doing Something...");
    }

    // Validated menu item.
    // Add a menu item named "Log Selected Transform Name" to MyMenu in the menu bar.
    // We use a second function to validate the menu item
    // so it will only be enabled if we have a transform selected.
    [MenuItem("MyMenu/Log Selected Transform Name")]
    static void LogSelectedTransformName()
    {
        Debug.Log("Selected Transform is on " + Selection.activeTransform.gameObject.name + ".");
    }

    // Validate the menu item defined by the function above.
    // The menu item will be disabled if this function returns false.
    [MenuItem("MyMenu/Log Selected Transform Name", true)]
    static bool ValidateLogSelectedTransformName()
    {
        // Return false if no transform is selected.
        return Selection.activeTransform != null;
    }

    // Add a menu item named "Do Something with a Shortcut Key" to MyMenu in the menu bar
    // and give it a shortcut (ctrl-h on Windows, cmd-h on macOS).
    [MenuItem("MyMenu/Do Something with a Shortcut Key %h")]
    static void DoSomethingWithAShortcutKey()
    {
        Debug.Log("Doing something with a Shortcut Key...");
    }

    // Add a menu item called "Double Mass" to a Rigidbody's context menu.
    [MenuItem("CONTEXT/Rigidbody/Double Mass")]
    static void DoubleMass(MenuCommand command)
    {
        Rigidbody body = (Rigidbody)command.context;
        body.mass = body.mass * 2;
        Debug.Log("Doubled Rigidbody's Mass to " + body.mass + " from Context Menu.");
    }

    // Add a menu item to create custom GameObjects.
    // Priority 10 ensures it is grouped with the other menu items of the same kind
    // and propagated to the hierarchy dropdown and hierarchy context menus.
    [MenuItem("GameObject/MyCategory/Custom Game Object", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("Custom Game Object");
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    
    // Creates a new menu item 'Examples > Create Prefab' in the main menu.
    [MenuItem("Examples/Create Prefab")]
    static void CreatePrefab()
    {
        // Keep track of the currently selected GameObject(s)
        GameObject[] objectArray = Selection.gameObjects;

        // Loop through every GameObject in the array above
        foreach (GameObject gameObject in objectArray)
        {
            // Create folder Prefabs and set the path as within the Prefabs folder,
            // and name it as the GameObject's name with the .Prefab format
            if (!Directory.Exists("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            string localPath = "Assets/Prefabs/" + gameObject.name + ".prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab and log whether Prefab was saved successfully.
            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAsset(gameObject, localPath, out prefabSuccess);
            if (prefabSuccess == true)
                Debug.Log("Prefab was saved successfully");
            else
                Debug.Log("Prefab failed to save" + prefabSuccess);
        }
    }

    // Disable the menu item if no selection is in place.
    [MenuItem("Examples/Create Prefab", true)]
    static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
    }
    

    [MenuItem("MyMenu/LogPrefabState")]
    static void LogPrefabState()
    {
        Debug.Log("LogPrefabState...");
        
        // PrejoectView 창에서 선택된 prefab인 경우
        // Regular, true
         
        // Scene에 존재하는 복재된 prefab을 선택한 경우
        // Regular, false
         
        // Prefab모드에 존재하는 prefab을 선택한 경우
        // NotAPrefab, false

        // 실행중에 instaiate된 상태의 prefab을 선택한 경우
        // NotAPrefab, false
        LOG.trace(PrefabUtility.GetPrefabAssetType(Selection.activeGameObject));
        LOG.trace("IsPersistent : " + EditorUtility.IsPersistent(Selection.activeGameObject));

        GameObject prefab = Resources.Load<GameObject>("MapObjects/Props/AirMine");
        LOG.trace(PrefabUtility.GetPrefabAssetType(prefab)); // Regular
        LOG.trace("IsPersistent : " + EditorUtility.IsPersistent(prefab)); // true

        GameObject ppff = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/00_MetaSuit/Resources/MapObjects/Props/AirMine.prefab", typeof(GameObject));
        LOG.trace(PrefabUtility.GetPrefabAssetType(ppff)); // Regular
        LOG.trace("IsPersistent : " + EditorUtility.IsPersistent(ppff)); // true
    }
}

#endif