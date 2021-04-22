using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEngine.Animations;

using SirMasters;
#if VRC_SDK_VRCSDK2
using VRCSDK2;
#endif

#if VRC_SDK_VRCSDK3
using VRC.SDKBase;
#endif


public class CharacterPuppeteerSetupWindow : EditorWindow
{
    Animator Reference;
    Animator Target;
    [MenuItem("SMTaK/Character Puppeteer Tools")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CharacterPuppeteerSetupWindow), false, "Character Puppeteer Tools");

    }
    void OnGUI()
    {
        SMTaK.DrawExperimentalWarning();
        GUIStyle WrapTextStyle = new GUIStyle(EditorStyles.label);
        WrapTextStyle.wordWrap = true;
        Target = (Animator)EditorGUILayout.ObjectField("Target", Target, typeof(Animator), true);
        Reference = (Animator)EditorGUILayout.ObjectField("Reference", Reference, typeof(Animator), true);
        if (GUILayout.Button("Add rotation constraints with zero offets!"))
        {
            AssembleRotation();
        }
        if (GUILayout.Button("Add parent constraints with zero offets!"))
        {
            AssembleParent();
        }
    }
    public void AssembleRotation(bool useHipPosConstraint = false)
    {
        if (Reference)
        {
            if (Target)
            {
                //Undo.RecordObject(Reference, "Scale Avatar");
                //Undo.RecordObject(Target, "Scale Avatar");
                for (int i = 0; i < (int)HumanBodyBones.LastBone; i++)
                {
                    Transform ActiveParentBone = Reference.GetBoneTransform((HumanBodyBones)i);
                    Transform ActiveChildBone = Target.GetBoneTransform((HumanBodyBones)i);
                    if (ActiveChildBone && Reference)
                    {
                        RotationConstraint RotCont = Undo.AddComponent<RotationConstraint>(ActiveChildBone.gameObject);
                        ConstraintSource ConSou = new ConstraintSource()
                        {
                            sourceTransform = ActiveParentBone,
                            weight = 1
                        };
                        RotCont.AddSource(ConSou);
                        RotCont.locked = true;
                        RotCont.constraintActive = true;
                    }
                }
                if (useHipPosConstraint)
                {

                    Transform ActiveParentBone = Reference.GetBoneTransform(HumanBodyBones.Hips);
                    Transform ActiveChildBone = Target.GetBoneTransform(HumanBodyBones.Hips);
                    if (ActiveChildBone && ActiveParentBone)
                    {
                        PositionConstraint PosCont = Undo.AddComponent<PositionConstraint>(ActiveChildBone.gameObject);
                        ConstraintSource ConSou = new ConstraintSource()
                        {
                            sourceTransform = ActiveParentBone,
                            weight = 1
                        };
                        PosCont.AddSource(ConSou);
                        PosCont.locked = true;
                        PosCont.constraintActive = true;
                    }
                }
            }
        }
    }
    public void AssembleParent()
    {
        if (Reference)
        {
            if (Target)
            {
                //Undo.RecordObject(Reference, "Scale Avatar");
                //Undo.RecordObject(Target, "Scale Avatar");
                for (int i = 0; i < (int)HumanBodyBones.LastBone; i++)
                {
                    Transform ActiveParentBone = Reference.GetBoneTransform((HumanBodyBones)i);
                    Transform ActiveChildBone = Target.GetBoneTransform((HumanBodyBones)i);
                    if (ActiveChildBone && ActiveParentBone)
                    {
                        ParentConstraint RotCont = Undo.AddComponent<ParentConstraint>(ActiveChildBone.gameObject);
                        ConstraintSource ConSou = new ConstraintSource()
                        {
                            sourceTransform = ActiveParentBone,
                            weight = 1
                        };
                        RotCont.AddSource(ConSou);
                        RotCont.locked = true;
                        RotCont.constraintActive = true;
                    }
                }
            }
        }
    }
}

public class FixCharacterPoseWindow : EditorWindow
{
    Animator Reference;
    Animator Target;
    [MenuItem("SMTaK/Fix Character Pose")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FixCharacterPoseWindow), false, "Fix Character Pose");

    }
    void OnGUI()
    {
        SMTaK.DrawExperimentalWarning();
        GUIStyle WrapTextStyle = new GUIStyle(EditorStyles.label);
        WrapTextStyle.wordWrap = true;
        Target = (Animator)EditorGUILayout.ObjectField("Target", Target, typeof(Animator), true);
        Reference = (Animator)EditorGUILayout.ObjectField("Reference", Reference, typeof(Animator), true);
        if (GUILayout.Button("Fix Character Pose!"))
        {
            FixCharacterPose();
        }
    }
    public void FixCharacterPose()
    {
        if (Reference)
        {
            if (Target)
            {
                //Undo.RecordObject(Reference, "Scale Avatar");
                //Undo.RecordObject(Target, "Scale Avatar");
                for (int i = 0; i < (int)HumanBodyBones.LastBone; i++)
                {
                    Transform ActiveParentBone = Reference.GetBoneTransform((HumanBodyBones)i);
                    Transform ActiveChildBone = Target.GetBoneTransform((HumanBodyBones)i);
                    if (ActiveChildBone && ActiveParentBone)
                    {
                        ActiveChildBone.localPosition = ActiveParentBone.localPosition;
                        ActiveChildBone.localRotation = ActiveParentBone.localRotation;
                    }
                }
            }
        }
    }
}
