using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ConstraintBinder : MonoBehaviour
{
    [MenuItem("CONTEXT/SkinnedMeshRenderer/Bind bones")]
    static void BindSMR(MenuCommand command)
    {
        SkinnedMeshRenderer smr = command.context as SkinnedMeshRenderer;
        if (smr)
        {
            List<Transform> boneList = new List<Transform>();
            if(smr.rootBone)
            {
                Transform bone = smr.rootBone.Find("Spine/Chest/Neck/Head");
                if(bone)
                {
                    boneList.Add(bone);
                }
                bone = null;
                bone = smr.rootBone.Find("Spine/Chest/Neck");
                if (bone)
                {
                    boneList.Add(bone);
                }
                bone = null;
                bone = smr.rootBone.Find("Spine/Chest/");
                if (bone)
                {
                    boneList.Add(bone);
                }
                bone = null;
                bone = smr.rootBone.Find("Spine/Chest/Right shoulder");
                if (bone)
                {
                    boneList.Add(bone);
                }
                bone = null;
                bone = smr.rootBone.Find("Spine/Chest/Left shoulder");
                if (bone)
                {
                    boneList.Add(bone);
                }
                bone = null;
                smr.bones = boneList.ToArray();
            }
        }
    }
    [MenuItem("CONTEXT/SkinnedMeshRenderer/Unbind bones")]
    static void UnbindSMR(MenuCommand command)
    {
        SkinnedMeshRenderer smr = command.context as SkinnedMeshRenderer;
        if (smr)
        {
            smr.bones = null;
        }
    }
}
/*
[CustomEditor(typeof(SkinnedMeshRenderer))]
public class ConstraintBinderEditor : Editor {

    public override void OnInspectorGUI()
    {
        SkinnedMeshRenderer constraintBinder = target as SkinnedMeshRenderer;
        if (constraintBinder)
        {
            foreach (Transform bone in constraintBinder.bones)
            {
                GUILayout.Label(bone.name);
            }
        }
        if (GUILayout.Button("Bind SMR"))
        {
        }
    }
}*/
