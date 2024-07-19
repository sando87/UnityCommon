#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

// 유니티 에디터 상태에서 Asset들을 일괄적으로 수정하는 등과 같은 작업을 수행

public class AssetEditor : MonoBehaviour
{
    [MenuItem("MyMenu/AddListener")]
    static void AddListener()
    {
        LOG.trace("Start AddListener");

        // ParticleBase pb = Resources.Load<ParticleBase>("VFX/FX_ExplosionSmall");

        // foreach (GameObject selObj in Selection.gameObjects)
        // {
        //     ShootingTwinkle st = selObj.GetComponentInChildren<ShootingTwinkle>();
        //     ShootingGameHealthManager sghm = selObj.GetComponent<ShootingGameHealthManager>();
        //     if (sghm != null)
        //     {
        //         UnityEditor.Events.UnityEventTools.AddPersistentListener(sghm.OnDamaged, st.StartTwinkle);
        //         UnityEditor.Events.UnityEventTools.AddObjectPersistentListener(sghm.OnDead, pb.InstantiateVFX, selObj.transform);
        //     }
        //     EditorUtility.SetDirty(selObj.gameObject);
        // }
        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();

        LOG.trace("End AddListener");
    }


    [MenuItem("MyMenu/EditPrefabAndApply")]
    static void EditPrefabAndApply()
    {
        LOG.trace("Start EditPrefabAndApply");

        GameObject prefab = Resources.Load<GameObject>("MapObjects/Props/Interact/AirMine");
        GameObject prefab2 = Resources.Load<GameObject>("MapObjects/Props/Interact/Bridge");
        if (prefab != null)
        {
            GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (obj != null)
            {
                GameObject newObj = PrefabUtility.InstantiatePrefab(prefab2) as GameObject;
                newObj.transform.parent = obj.transform;
                PrefabUtility.ApplyPrefabInstance(obj, InteractionMode.AutomatedAction);
            }
        }

        LOG.trace("End EditPrefabAndApply");
    }

    [MenuItem("MyMenu/FindRefScriptInAllAsset")]
    static void FindRefScriptInAllAsset<T>() where T : MonoBehaviour
    {
        LOG.trace("Start FindRefScriptInAllAsset");

        List<string> files = new List<string>();
        files.AddRange(MyUtils.FindAllFiles("Assets/00_MetaSuit/Resources/Suits"));
        files.AddRange(MyUtils.FindAllFiles("Assets/00_MetaSuit/Resources/MapObjects/Enemies"));
        files.AddRange(MyUtils.FindAllFiles("Assets/00_MetaSuit/09_Prefabs/Projectiles"));

        List<T> scripts = new List<T>();
        foreach (string file in files)
        {
            if (file.Contains("meta"))
                continue;

            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(file);
            if (obj == null)
                continue;

            scripts.Clear();
            obj.transform.ExFindChildAll(scripts, true);
            foreach(T script in scripts)
            {
                LOG.trace(script.gameObject.name);
            }
        }

        LOG.trace("End FindRefScriptInAllAsset");
    }

    [MenuItem("MyMenu/FindRefScriptInResources")]
    static void FindRefScriptInResources<T>() where T : MonoBehaviour
    {
        GameObject[] objs = Resources.LoadAll<GameObject>("/");
        foreach (GameObject obj in objs)
        {
            T[] monos = obj.GetComponentsInChildren<T>(true);
            foreach(T mono in monos)
            {
                LOG.trace(mono.gameObject.name);
            }
        }
        LOG.trace("End FindRefScriptInResources");
    }



    // [MenuItem("CONTEXT/Animator/InitParameters")]
    [MenuItem("Assets/MetalSuits/InitParameters")]
    static void InitParameters()
    {
        foreach (string guid in Selection.assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            AnimatorController controller = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
            if (controller != null)
            {
                // Add parameters
                controller.AddParameter("DoActionTrigger", AnimatorControllerParameterType.Trigger);
                controller.AddParameter("ActionType", AnimatorControllerParameterType.Int);
                controller.AddParameter("ActionSubType", AnimatorControllerParameterType.Int);
                controller.AddParameter("DerivedType", AnimatorControllerParameterType.Int);
                controller.AddParameter("VelocityY", AnimatorControllerParameterType.Float);
                controller.AddParameter("IsRun", AnimatorControllerParameterType.Bool);
                controller.AddParameter("IsPhaseSecond", AnimatorControllerParameterType.Bool);
                controller.AddParameter("IsLeftSide", AnimatorControllerParameterType.Bool);
                controller.AddParameter("IsLanding", AnimatorControllerParameterType.Bool);

                // controller.AddParameter("NomalAttackSpeed", AnimatorControllerParameterType.Float);
                // controller.ExAddParameter("NomalAttackSpeed", AnimatorControllerParameterType.Float, 1);

                AnimatorControllerParameter pp = new AnimatorControllerParameter();
                pp.name = controller.MakeUniqueParameterName("NomalAttackSpeed");
                pp.type = AnimatorControllerParameterType.Float;
                pp.defaultFloat = 1.0f;
                controller.AddParameter(pp);

                // // Add StateMachines
                // var rootStateMachine = controller.layers[0].stateMachine;
                // var stateMachineA = rootStateMachine.AddStateMachine("smA");
                // var stateMachineB = rootStateMachine.AddStateMachine("smB");
                // var stateMachineC = stateMachineB.AddStateMachine("smC");

                // // Add States
                // var stateA1 = stateMachineA.AddState("stateA1");
                // var stateB1 = stateMachineB.AddState("stateB1");
                // var stateB2 = stateMachineB.AddState("stateB2");
                // stateMachineC.AddState("stateC1");
                // var stateC2 = stateMachineC.AddState("stateC2"); // don’t add an entry transition, should entry to state by default

                // // Add Transitions
                // var exitTransition = stateA1.AddExitTransition();
                // exitTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "TransitionNow");
                // exitTransition.duration = 0;

                // var resetTransition = rootStateMachine.AddAnyStateTransition(stateA1);
                // resetTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Reset");
                // resetTransition.duration = 0;

                // var transitionB1 = stateMachineB.AddEntryTransition(stateB1);
                // transitionB1.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "GotoB1");
                // stateMachineB.AddEntryTransition(stateB2);
                // stateMachineC.defaultState = stateC2;
                // var exitTransitionC2 = stateC2.AddExitTransition();
                // exitTransitionC2.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "TransitionNow");
                // exitTransitionC2.duration = 0;

                // var stateMachineTransition = rootStateMachine.AddStateMachineTransition(stateMachineA, stateMachineC);
                // stateMachineTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "GotoC");
                // rootStateMachine.AddStateMachineTransition(stateMachineA, stateMachineB);

            }
        }

    }


    [MenuItem("MyMenu/EditComponets")]
    static void EditComponets()
    {
        LOG.trace("Start");
        GameObject[] objs = Resources.LoadAll<GameObject>("VFX/");
        foreach (GameObject obj in objs)
        {
            Component[] comps = obj.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (Component comp in comps)
            {
                // Do Something...
            }
        }
        LOG.trace("End");
    }

    [MenuItem("MyMenu/RenameImages")]
    static void RenameImages()
    {
        foreach (string guid in Selection.assetGUIDs)
        {
            string oldPath = AssetDatabase.GUIDToAssetPath(guid);
            string[] pieces = oldPath.Split("/");
            string filename = pieces[pieces.Length - 1];
            LOG.trace(filename);
            filename = filename.Replace("03_01_", "04_01_");

            AssetDatabase.RenameAsset(oldPath, filename);
        }
        AssetDatabase.SaveAssets();
    }



    // [MenuItem("MyMenu/CreateChapterInfoDB")]
    // static void CreateChapterInfoDB()
    // {
    //     List<ChapterInfo> infos = new List<ChapterInfo>();

    //     {
    //         ChapterInfo newInfo = new ChapterInfo();
    //         newInfo.NameID = "NewEarth";
    //         newInfo.DisplayName = "New Earth";
    //         newInfo.Description = "NewEarth Desc";
    //         infos.Add(newInfo);
    //     }
    //     {
    //         ChapterInfo newInfo = new ChapterInfo();
    //         newInfo.NameID = "Estiel";
    //         newInfo.DisplayName = "Estiel";
    //         newInfo.Description = "Estiel Desc";
    //         infos.Add(newInfo);
    //     }

    //     string filename = typeof(ChapterInfo).Name;
    //     string fullname = "./Assets/00_MetaSuit/Resources/Database/" + filename + ".json";
    //     string jsonString = JsonHelpper.ToJsonArray<ChapterInfo>(infos.ToArray());
    //     System.IO.File.WriteAllText(fullname, jsonString);

    //     // string csvstring = CSVParser<ChapterInfo>.Serialize(',', infos);
    //     // File.WriteAllText("./Assets/00_MetaSuit/Resources/Database/PropsSpecInfo.csv", csvstring);
    // }

    // [MenuItem("MyMenu/TransCSVToJson")]
    // static void TransCSVToJson()
    // {
    //     EnemySpecInfo[] enemies = DataManager.GetAllEnemySpecInfos();
    //     string enemiesData = JsonHelpper.ToJsonArray(enemies);
    //     File.WriteAllText("./Assets/00_MetaSuit/Resources/Database/EnemySpecInfo.json", enemiesData);

    //     PropsSpecInfo[] props = DataManager.GetAllPropsSpecInfos();
    //     string propsData = JsonHelpper.ToJsonArray(props);
    //     File.WriteAllText("./Assets/00_MetaSuit/Resources/Database/PropsSpecInfo.json", propsData);

    //     SuitSpecInfo[] suits = DataManager.GetAllSuitSpecInfos();
    //     string suitsData = JsonHelpper.ToJsonArray(suits);
    //     File.WriteAllText("./Assets/00_MetaSuit/Resources/Database/SuitSpecInfo.json", suitsData);

    //     // EnemySpecInfo[] items = JsonHelpper.FromJsonArray<EnemySpecInfo>(enemiesData);
    //     // foreach(EnemySpecInfo item in items)
    //     //     LOG.trace(item.NameID);
    // }

    // [MenuItem("MyMenu/TransJsonToUnity")]
    // static void TransJsonToUnity()
    // {
    //     {
    //         TextAsset ta = Resources.Load<TextAsset>("Database/PropsSpecInfo_old");
    //         PropsSpecInfo[] oldProps = JsonHelpper.FromJsonArray<PropsSpecInfo>(ta.text);
    //         for (int i = 0; i < oldProps.Length; ++i)
    //         {
    //             oldProps[i].RowIndex = i;
    //         }

    //         DatabaseUnityProps.Instance.Save(oldProps);
    //     }
    //     {
    //         TextAsset ta = Resources.Load<TextAsset>("Database/EnemySpecInfo_old");
    //         EnemySpecInfo[] olds = JsonHelpper.FromJsonArray<EnemySpecInfo>(ta.text);
    //         for (int i = 0; i < olds.Length; ++i)
    //         {
    //             olds[i].RowIndex = i;
    //         }

    //         DatabaseUnityEnemies.Instance.Save(olds);
    //     }
    //     {
    //         TextAsset ta = Resources.Load<TextAsset>("Database/SuitSpecInfo_old");
    //         SuitSpecInfo[] olds = JsonHelpper.FromJsonArray<SuitSpecInfo>(ta.text);
    //         for (int i = 0; i < olds.Length; ++i)
    //         {
    //             olds[i].RowIndex = i;
    //         }

    //         DatabaseUnitySuits.Instance.Save(olds);
    //     }
    // }

    // 특정 프리팹과 동일한 프리팹을 생성후 sprite만 교체해주는 작업을 일괄적으로 한번에 처리
    [MenuItem("MyMenu/AddNewExteriors")]
    static void AddNewExteriors(MenuCommand command)
    {
        GameObject RefPrefab = Resources.Load<GameObject>("Temp/");
        if(RefPrefab == null)
            return;
        
        foreach(string guid in Selection.assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
            
            string localPath = "Assets/00_MetaSuit/Resources/MapObjects/Props/Exterior/Exterior_" + sprite.name + ".prefab";
            
            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab and log whether Prefab was saved successfully.
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(RefPrefab, localPath);

            SpriteRenderer sr = newPrefab.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = sprite;

            newPrefab.GetComponentInChildren<BoxCollider>().size = sr.bounds.size;
            newPrefab.GetComponentInChildren<BoxCollider>().center = sr.bounds.center;

            // string newPrefabGUID = AssetDatabase.AssetPathToGUID(localPath);
            // newPrefab.GetComponentInChildren<MapEditorObject>().resourceID = MyUtils.GUIDToLong(newPrefabGUID);
        }
        
        AssetDatabase.SaveAssets();
    }




    //특정 선택된 프리팹들을 일괄적으로 수정해줌
    [MenuItem("MyMenu/Edit Renderers")]
    static void EditRenderers()
    {
        Debug.Log("EditRenderers...");
        
        Material mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/00_MetaSuit/06_Materials/FlashSprite.mat", typeof(Material));

        foreach(GameObject obj in Selection.gameObjects)
        {
            List<Transform> childs = new List<Transform>();
            obj.transform.ExFindChildAll("key (2)", childs);
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
        //DestroyImmediate(sr.GetComponent<AllIn1SpriteShader.AllIn1Shader>(), true);
        // DestroyImmediate(sr.GetComponent<RendererAllInOne>(), true);
        // sr.material = mat;
        // sr.gameObject.AddComponent<RendererCustomLit>();
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

    [MenuItem("Assets/MyCategory/LogSelections")]
    static void LogSelections()
    {
        foreach (Object asset in Selection.GetFiltered<Object>(SelectionMode.Assets))
        {
            string path = AssetDatabase.GetAssetPath(asset);
            LOG.trace(path);
        }
        
        foreach(GameObject prefab in Selection.gameObjects)
            LOG.trace(prefab.name);
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