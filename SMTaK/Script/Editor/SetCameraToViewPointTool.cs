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

public class SetCameraToViewPointToolWindow : EditorWindow
{
    float Distance = 0.5f;
    float Factor = 0.28f;
    bool UseFactor = false;
    VRC_AvatarDescriptor avatarDescriptor;
    [MenuItem("SMTaK/Set Camera To ViewPoint Tool")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SetCameraToViewPointToolWindow), false, "Set Thumbnail Camera To ViewPoint Tool");

    }
    void OnGUI()
    {
        SMTaK.DrawExperimentalWarning();
        GUIStyle WrapTextStyle = new GUIStyle(EditorStyles.label);
        WrapTextStyle.wordWrap = true;
        EditorGUILayout.Space();
        if (SMTaK.IsAprilFools())
        {
            EditorGUILayout.LabelField("I see you want to see you", WrapTextStyle);

        }
        else
        {
            EditorGUILayout.LabelField("This tool will position VRCCam to your viewpoint and at the distance you want, either with a fixed value or a factor based on height.", WrapTextStyle);

        }
        EditorGUILayout.Space();
        avatarDescriptor = (VRC_AvatarDescriptor)EditorGUILayout.ObjectField("Target Avatar", avatarDescriptor, typeof(VRC_AvatarDescriptor), true);
        EditorGUILayout.Space();
        UseFactor = EditorGUILayout.Toggle("Use Factor", UseFactor);
        if (UseFactor)
        {
            Factor = EditorGUILayout.FloatField("Factor", Factor);

        }
        else
        {
            Distance = EditorGUILayout.FloatField("Distance", Distance);
        }
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Set thumbnail camera to viewpoint!"))
            {
                RunThis();
            }
        }
        else
        {
            GUILayout.Button("Please start the avatar upload process first.");
        }
    }

    void RunThis()
    {
        GameObject vrcCam = GameObject.Find("VRCCam");
        //PipelineSaver pipelineSaver = Component.FindObjectOfType<PipelineSaver>();
        if (vrcCam /*&& pipelineSaver*/)
        {
            //VRC_AvatarDescriptor avatarDescriptor = pipelineSaver.GetComponent<VRC_AvatarDescriptor>();
            if(avatarDescriptor)
            {
                vrcCam.transform.position = avatarDescriptor.transform.position;
                vrcCam.transform.position += avatarDescriptor.ViewPosition;
            }
            if (UseFactor)
            {
                vrcCam.transform.position += new Vector3(0, 0, vrcCam.transform.position.y * Factor);
            }
            else
            {
                vrcCam.transform.position += new Vector3(0, 0, Distance);
            }
        }
    }
}
