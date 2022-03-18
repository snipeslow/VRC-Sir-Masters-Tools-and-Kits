using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Globalization;
#if VRC_SDK_VRCSDK2
using VRCSDK2;
#endif
#if VRC_SDK_VRCSDK3
using VRC.SDKBase;
#endif
using UnityEditor.Presets;
using SirMasters;
namespace SirMasters.DynamicBoneTools
{
    public class DynamicBoneToolsWindow : EditorWindow
    {
        [MenuItem("SMTaK/DynamicBone Copy Tool (Deprecated)")]
        public static void ShowWindow()
        {
            Debug.LogWarning("SMTak DynamicBone Copy Tool is deprecated, please wait for the Physbone version!");
        }
        /*
        [MenuItem("CONTEXT/DynamicBone/(DEBUG)Print Json")]
        static public void DynamicBonePrintData(MenuCommand command)
        {
            DynamicBone dynamicBone = (DynamicBone)command.context;
            if (dynamicBone)
            {
                Debug.Log(dynamicBone.m_Root);
                Debug.Log(EditorJsonUtility.ToJson(dynamicBone, true));
            }
        }

        [MenuItem("CONTEXT/DynamicBone/Set Dyanamic Bone Root")]
        static public void DynamicBoneFix(MenuCommand command)
        {
            DynamicBone dynamicBone = (DynamicBone)command.context;
            if (dynamicBone)
            {
                dynamicBone.m_Root = dynamicBone.transform;
            }
        }
        //private const DateTime AprilFools = new DateTime();
        bool AllToRoot = false;
        bool ClearExistingDynamicBones = false;
        public GameObject RefenceObject;
        public GameObject TargetObject;
        public GameObject TargetObjectMass;
        [MenuItem("SMTaK/DynamicBone Copy Tool")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(DynamicBoneToolsWindow), false, "DynamicBone Tools");

        }
        void OnInspectorUpdate()
        {
            Repaint();
        }
        bool FoldoutCopy = true;
        bool FoldoutScale = true;
        float Scale = 1.0f;
        bool FoldoutPreset = true;
        Preset presetPreset = null;
        GameObject presetGameObject = null;
        string prefix = "";
        bool TogglePresetMode = false;
        bool ToggleCaseSensitive = true;
        void OnGUI()
        {
            SMTaK.DrawExperimentalWarning();
            GUIStyle WrapTextStyle = new GUIStyle(EditorStyles.label);
            WrapTextStyle.wordWrap = true;
            EditorGUILayout.Space();
            TargetObject = (GameObject)EditorGUILayout.ObjectField("Target", TargetObject, typeof(GameObject), true);
            AllToRoot = EditorGUILayout.ToggleLeft("All to root", AllToRoot);

            SMTaK.DrawHorizontalLine();
            ClearExistingDynamicBones = EditorGUILayout.ToggleLeft("!!Clear existing Dynamic Bones!!", ClearExistingDynamicBones);
            if (ClearExistingDynamicBones)
            {
                if (GUILayout.Button("!!Clear existing Dynamic Bones and Colliders!!"))
                {
                    Undo.SetCurrentGroupName("Clear existing Dynamic Bones and Colliders");
                    ClearDynamicBone(TargetObject.transform);
                    Selection.activeGameObject = TargetObject;
                }

            }
            else
            {
                GUILayout.Button("To enable \"!!Clear existing Dynamic Bones!!\",\nplease check \"!!Clear existing Dynamic Bones!!\" checkbox first!");
            }
            SMTaK.DrawHorizontalLine();
            FoldoutCopy = EditorGUILayout.Foldout(FoldoutCopy, "Copy from model");

            if (FoldoutCopy)
            {
                EditorGUILayout.Space();
                if (SMTaK.IsAprilFools())
                {
                    EditorGUILayout.LabelField("UwU, notices your bulgy wulgy DynamicBone.", WrapTextStyle);

                }
                else
                {
                    EditorGUILayout.LabelField("This window will copy over DynamicBones located at refence to target, based on their assigned roots.\nWarning, duplicate bone names in hiearchy could lead to unintended behaviors", WrapTextStyle);

                }
                EditorGUILayout.Space();
                // The actual window code goes here
                RefenceObject = (GameObject)EditorGUILayout.ObjectField("Reference", RefenceObject, typeof(GameObject), true);
                //DrawDefaultInspector();
                if (GUILayout.Button("Copy Dynamic Bone over!"))
                {
                    if (TargetObject)
                    {
                        if (RefenceObject)
                        {
                            Undo.SetCurrentGroupName("Copy Dynamic Bone over");
                            Transform[] boneArray = TargetObject.GetComponentsInChildren<Transform>();

                            DynamicBoneCollider[] dynamicBoneCollders = RefenceObject.GetComponentsInChildren<DynamicBoneCollider>();
                            foreach (DynamicBoneCollider dynamicBoneCollder in dynamicBoneCollders)
                            {
                                foreach (Transform boneEntry in boneArray)
                                {
                                    if (boneEntry.name == dynamicBoneCollder.transform.name)
                                    {
                                        DynamicBoneCollider dynamicBoneColliderCopy = Undo.AddComponent<DynamicBoneCollider>(boneEntry.gameObject);

                                        Preset preset = new Preset(dynamicBoneCollder);
                                        preset.ApplyTo(dynamicBoneColliderCopy);
                                        break;
                                    }
                                }

                            }
                            //
                            DynamicBone[] dynamicBones = RefenceObject.GetComponentsInChildren<DynamicBone>();
                            foreach (DynamicBone dynamicBone in dynamicBones)
                            {
                                if (!dynamicBone.m_Root)
                                {
                                    continue;
                                }
                                Debug.Log(dynamicBone.m_Root.name);
                                Transform foundMatchingRootBone = null;

                                Transform foundMatchingRefernceBone = null;
                                foreach (Transform boneEntry in boneArray)
                                {
                                    if (boneEntry.name == dynamicBone.m_Root.name)
                                    {
                                        foundMatchingRootBone = boneEntry;
                                    }
                                    if (dynamicBone.m_ReferenceObject)
                                    {
                                        if (boneEntry.transform.name == dynamicBone.m_ReferenceObject.name)
                                        {
                                            foundMatchingRefernceBone = boneEntry;
                                        }
                                        if (foundMatchingRootBone && foundMatchingRefernceBone)
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (foundMatchingRootBone)
                                        {
                                            break;
                                        }

                                    }
                                }
                                if (foundMatchingRootBone)
                                {
                                    Preset preset = new Preset(dynamicBone);
                                    ApplyDynamicBone(foundMatchingRootBone, preset, TargetObject.transform, AllToRoot);
                                }

                            }
                            Selection.activeGameObject = TargetObject;
                        }
                    }
                }

            }
            SMTaK.DrawHorizontalLine();
            FoldoutScale = EditorGUILayout.Foldout(FoldoutScale, "Mass scale dynamic bones radius");
            if (FoldoutScale)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Allows you to mass scale DynamicBones in a GameObject hierarchy evenly.", WrapTextStyle);
                EditorGUILayout.Space();
                Scale = EditorGUILayout.FloatField("Scale factor", Scale);
                if(GUILayout.Button("Mass scale Dynamic Bone!"))
                {
                    DynamicBone[] dynamicBones = TargetObject.GetComponentsInChildren<DynamicBone>();
                    DynamicBoneCollider[] dynamicBoneColliders = TargetObject.GetComponentsInChildren<DynamicBoneCollider>();
                    //DynamicBonePlaneCollider[] dynamicBoneColliders = TargetObject.GetComponentsInChildren<DynamicBoneCollider>();
                    Undo.SetCurrentGroupName("Mass scale Dynamic Bone");
                    Undo.RecordObjects(dynamicBones, "Mass scale Dynamic Bone");
                    Undo.RecordObjects(dynamicBoneColliders, "Mass scale Dynamic Bone");
                    foreach (DynamicBone dynamicBone in dynamicBones)
                    {
                        dynamicBone.m_Radius *= Scale;
                        dynamicBone.m_EndOffset *= Scale;
                    }
                    foreach (DynamicBoneCollider dynamicBoneCollider in dynamicBoneColliders)
                    {
                        dynamicBoneCollider.m_Height *= Scale;
                        dynamicBoneCollider.m_Radius *= Scale;
                        dynamicBoneCollider.m_Center *= Scale;
                    }
                }
            }
            SMTaK.DrawHorizontalLine();
            FoldoutPreset = EditorGUILayout.Foldout(FoldoutPreset, "Mass set dynamic bones");
            if (FoldoutPreset)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Allows you to add DynamicBones to GameObjects with prefixed names from a preset.", WrapTextStyle);
                EditorGUILayout.Space();
                //TargetObjectMass = (GameObject)EditorGUILayout.ObjectField("Target root", TargetObjectMass, typeof(GameObject), true);
                prefix = EditorGUILayout.TextField("Prefix", prefix);
                TogglePresetMode = EditorGUILayout.ToggleLeft("Use Prefab as preset (only root object Dynamic Bone)", TogglePresetMode);
                ToggleCaseSensitive = EditorGUILayout.ToggleLeft("Case Sensitive", ToggleCaseSensitive);
                if (TogglePresetMode)
                {
                    presetGameObject = (GameObject)EditorGUILayout.ObjectField("Preset", presetGameObject, typeof(GameObject), true);
                }
                else
                {
                    presetPreset = (Preset)EditorGUILayout.ObjectField("Preset", presetPreset, typeof(Preset), true);
                }
                if(GUILayout.Button("Apply Dynamic Bone Preset!") && TargetObject)
                {
                    Transform[] boneArray = TargetObject.GetComponentsInChildren<Transform>();
                    DynamicBoneCollider[] dbColliderArray = TargetObject.GetComponentsInChildren<DynamicBoneCollider>();
                    foreach (Transform bone in boneArray)
                    {
                        if (bone.name.StartsWith(prefix, !ToggleCaseSensitive, CultureInfo.CurrentCulture) && prefix.Length > 0)
                        {
                            Undo.SetCurrentGroupName("Apply Dynamic Bone Preset");
                            if (TogglePresetMode)
                            {
                                if(presetGameObject)
                                {
                                    DynamicBone dynamicBone = presetGameObject.GetComponent<DynamicBone>();
                                    if(dynamicBone)
                                    {
                                        Preset preset = new Preset(dynamicBone);
                                        if (!AllToRoot)
                                        {
                                            ApplyDynamicBone(bone, preset, TargetObject.transform);

                                        }
                                        else
                                        {

                                            ApplyDynamicBone(bone, preset, TargetObject.transform, true);
                                        }

                                    }

                                }

                            }
                            else
                            {
                                if (!AllToRoot)
                                {
                                    ApplyDynamicBone(bone, presetPreset, TargetObject.transform);

                                }
                                else
                                {

                                    ApplyDynamicBone(bone, presetPreset,TargetObject.transform, true);
                                }
                            }
                        }
                    }
                    Selection.activeGameObject = TargetObject;

                }
                
            }
            SMTaK.DrawHorizontalLine();
        }
        public void ClearDynamicBone(Transform target)
        {
            DynamicBone[] dynamicBonesToClear = target.GetComponentsInChildren<DynamicBone>();
            foreach (DynamicBone dynamicBone in dynamicBonesToClear)
            {
                if (dynamicBone)
                {
                    Undo.DestroyObjectImmediate(dynamicBone);
                    
                }

            }
            DynamicBoneCollider[] dynamicBoneCollidersToClear = target.GetComponentsInChildren<DynamicBoneCollider>();
            foreach (DynamicBoneCollider dynamicBoneCollider in dynamicBoneCollidersToClear)
            {
                if (dynamicBoneCollider)
                {
                    Undo.DestroyObjectImmediate(dynamicBoneCollider);

                }

            }
        }
        public void ApplyDynamicBone(Transform target, Preset preset, Transform root, bool toRoot = false)
        {
            if(!target)
            {
                return;
            }
            if (!preset)
            {
                return;
            }
            if(!root)
            {
                return;
            }
            DynamicBone dynBone;
            if(toRoot)
            {
                dynBone = Undo.AddComponent<DynamicBone>(root.gameObject);

            }
            else
            {
                dynBone = Undo.AddComponent<DynamicBone>(target.gameObject);
            }
            preset.ApplyTo(dynBone);
            dynBone.m_Root = target;
            //DynamicBoneCollider dbCollider in dynBone.m_Colliders
            DynamicBoneCollider[] dbColliderArray = root.GetComponentsInChildren<DynamicBoneCollider>();
            for (int i = dynBone.m_Colliders.Count; i > 0; i--)
            {
                bool hasFoundCollider = false;
                foreach (DynamicBoneCollider dbCollider in dbColliderArray)
                {
                    if (dbCollider.name == dynBone.m_Colliders[i - 1].name)
                    {
                        dynBone.m_Colliders[i - 1] = dbCollider;
                        hasFoundCollider = true;
                        break;
                    }
                }
                if (!hasFoundCollider)
                {
                    dynBone.m_Colliders.RemoveAt(i - 1);
                }
            }
            Transform[] dbeTransformArray = root.GetComponentsInChildren<Transform>();
            for (int i = dynBone.m_Exclusions.Count; i > 0; i--)
            {
                bool hasFoundExclusion = false;
                foreach (Transform dbeTransform in dbeTransformArray)
                {
                    if (dbeTransform.name == dynBone.m_Exclusions[i - 1].name)
                    {
                        dynBone.m_Exclusions[i - 1] = dbeTransform;
                        hasFoundExclusion = true;
                        break;
                    }
                }
                if (!hasFoundExclusion)
                {
                    dynBone.m_Exclusions.RemoveAt(i - 1);
                }
            }

        }
        */
    }
}