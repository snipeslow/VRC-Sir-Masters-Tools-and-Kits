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
using SirMasters;

public class ScaleAvatarToolWindow : EditorWindow
{
    GameObject TargetObject;
    float Scale = 1;
    float Factor = 1;
    [MenuItem("CONTEXT/Animator/(DEBUG)Print Json")]
    static public void PrintScaleOfHumanoid(MenuCommand command)
    {
        Animator animator = (Animator)command.context;
        if (animator)
        {
            if (animator.isHuman)
            {
                Debug.Log("Human scale " + animator.humanScale);
            }
        }
    }
    [MenuItem("CONTEXT/Animator/Scale to around 160cm (Not exact)")]
    static public void ScaleHumanoid(MenuCommand command)
    {
        Animator animator = (Animator)command.context;
        if (animator)
        {
            if (animator.isHuman)
            {
                animator.transform.localScale = Vector3.one * (Mathf.Floor(0.985f / animator.humanScale * 10000f) / 10000f);
            }
            else
            {
                Debug.Log("Animator does not have human avatar set!");
            }
        }
    }
    [MenuItem("CONTEXT/VRC_AvatarDescriptor/Guess viewpoint based on human avatar")]
    static public void SetViewPoint(MenuCommand command)
    {

        VRC_AvatarDescriptor vrcAD = (VRC_AvatarDescriptor)command.context;//animator.GetComponent<VRC_AvatarDescriptor>();

        if (vrcAD)
        {
            Animator animator = vrcAD.transform.GetComponent<Animator>();
            if (animator)
            {
                if (animator.isHuman)
                {
                        Transform leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
                        Transform rightEye = animator.GetBoneTransform(HumanBodyBones.RightEye);
                    if(leftEye && rightEye)
                    {
                        Vector3 averageEyePos = (leftEye.position + rightEye.position) / 2f;

                        vrcAD.ViewPosition = vrcAD.transform.InverseTransformPoint(averageEyePos);

                    }
                    else
                    {
                        Debug.Log("No eye bones! Guessing based on humanoid scale!");
                        vrcAD.ViewPosition = Vector3.up * (animator.humanScale);
                        vrcAD.ViewPosition += Vector3.forward * (animator.humanScale);
                        //vrcAD.ViewPosition.x *= vrcAD.transform.localScale.x * 1.5f;
                        vrcAD.ViewPosition.y *= vrcAD.transform.localScale.y * 1.5f;
                        vrcAD.ViewPosition.z *= vrcAD.transform.localScale.z * 0.07f;
                    }

                }
                else
                {
                    Debug.Log("Animator does not have human avatar set!");
                }
            }
        }

    }
    [MenuItem("SMTaK/Avatar Scaling Tools")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ScaleAvatarToolWindow), false, "Avatar Scaling Tools");

    }
    bool FoldoutScaling = true;
    bool FoldoutGuessViewpoint = true;
    bool FoldoutScaleTo160CM = true;
    void OnGUI()
    {
        SMTaK.DrawExperimentalWarning();
        GUIStyle WrapTextStyle = new GUIStyle(EditorStyles.label);
        WrapTextStyle.wordWrap = true;
        TargetObject = (GameObject)EditorGUILayout.ObjectField("Target", TargetObject, typeof(GameObject), true);
        if (FoldoutScaling = EditorGUILayout.Foldout(FoldoutScaling, "Scale avatar"))
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("This tool will scale both GameObject and avatar viewpoint at the same value, using existing values.", WrapTextStyle);
            EditorGUILayout.Space();
            Scale = EditorGUILayout.FloatField("Value", Scale);
            if (GUILayout.Button("Scale Avatar!") && TargetObject)
            {
                Undo.RecordObject(TargetObject.transform, "Scale Avatar");
                TargetObject.transform.localScale *= Scale;
                VRC_AvatarDescriptor avatarDescriptor = TargetObject.GetComponent<VRC_AvatarDescriptor>();
                if (avatarDescriptor)
                {
                    Undo.RecordObject(avatarDescriptor, "Scale Avatar");
                    avatarDescriptor.ViewPosition *= Scale;
                }
            }

        }
        SMTaK.DrawHorizontalLine();
        if (FoldoutGuessViewpoint = EditorGUILayout.Foldout(FoldoutGuessViewpoint, "Guess ViewPoint"))
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("This tool will guess your viewpoint based on eye bone positions or humanoid scale, then factors in scale.", WrapTextStyle);
            EditorGUILayout.Space();
            Factor = EditorGUILayout.FloatField("Factor", Factor);
            if (GUILayout.Button("Guess ViewPoint!") && TargetObject)
            {
                //Undo.RecordObject(TargetObject, "Scale Avatar");
                //TargetObject.transform.localScale *= Scale;
                Animator animator = TargetObject.GetComponent<Animator>();
                VRC_AvatarDescriptor avatarDescriptor = TargetObject.GetComponent<VRC_AvatarDescriptor>();
                if (avatarDescriptor && animator)
                {
                    //avatarDescriptor.ViewPosition *= Scale;
                    if (animator.isHuman)
                    {
                        Transform leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
                        Transform rightEye = animator.GetBoneTransform(HumanBodyBones.RightEye);
                        Undo.RecordObject(avatarDescriptor, "Guess ViewPoint");
                        if (leftEye && rightEye)
                        {
                            Vector3 averageEyePos = (leftEye.position + rightEye.position) / 2f;

                            avatarDescriptor.ViewPosition = avatarDescriptor.transform.InverseTransformPoint(averageEyePos);

                        }
                        else
                        {
                            Debug.Log("No eye bones! Guessing based on humanoid scale!");
                            avatarDescriptor.ViewPosition = Vector3.up * (animator.humanScale);
                            avatarDescriptor.ViewPosition += Vector3.forward * (animator.humanScale);
                            avatarDescriptor.ViewPosition.y *= avatarDescriptor.transform.localScale.y * 1.5f;
                            
                            avatarDescriptor.ViewPosition.y = SMTaK.TrunicateFloat(avatarDescriptor.ViewPosition.y, EditorPrefs.GetInt("SMTaKFloatTrunication", 3));
                            avatarDescriptor.ViewPosition.z *= avatarDescriptor.transform.localScale.z * 0.07f;
                            avatarDescriptor.ViewPosition.z = SMTaK.TrunicateFloat(avatarDescriptor.ViewPosition.z, EditorPrefs.GetInt("SMTaKFloatTrunication", 3));
                        }

                    }
                    else
                    {
                        Debug.Log("Animator does not have human avatar set, unable to calculate!");
                    }
                }
            }

        }
        SMTaK.DrawHorizontalLine();
        if (FoldoutScaleTo160CM = EditorGUILayout.Foldout(FoldoutScaleTo160CM, "Scale to 160cm (not accurate)"))
        {
            if (GUILayout.Button("Scale to 160cm (not accurate)!") && TargetObject)
            {
                Animator animator = TargetObject.GetComponent<Animator>();
                if (animator)
                {
                    if (animator.isHuman)
                    {
                        Undo.RecordObject(animator.transform, "Scale to 160cm (not accurate)");
                        //animator.transform.localScale = Vector3.one * (Mathf.Floor(0.985f / animator.humanScale * 10000f) / 10000f);
                        animator.transform.localScale = Vector3.one * SMTaK.TrunicateFloat(1f / animator.humanScale, EditorPrefs.GetInt("SMTaKFloatTrunication", 3));
                    }
                    else
                    {
                        Debug.Log("Animator does not have human avatar set!");
                    }
                }

            }
            
        }
    }
}
