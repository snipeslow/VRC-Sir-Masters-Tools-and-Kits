using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SirMasters;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
namespace SirMasters.DynamicsInventorySystem
{ 
    public class DynamicsInventorySystem : EditorWindow
    {
        AnimatorController TargetAnimator;

        AnimationClip RightGrabAnimation;
        AnimationClip LeftGrabAnimation;

        AnimationClip RightEquipAnimation;
        AnimationClip LeftEquipAnimation;

        AnimationClip RightForegripAnimation;
        AnimationClip LeftForegripAnimation;

        AnimationClip MainHandFiringAnimation;
        AnimationClip OffHandFiringAnimation;
        VRCExpressionParameters ParametersFile;
        string WeaponName = "";
        bool WeaponHasOffhand = false;
        [MenuItem("SMTaK/Dynamics Inventory System")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(DynamicsInventorySystem), false, "Dynamics Inventory System");
        }
        void AddParameter(AnimatorController animatorController, string parameterName, AnimatorControllerParameterType controllerParameterType)
        {
            if(animatorController)
            {
                foreach(AnimatorControllerParameter animParameters in animatorController.parameters)
                {
                    if(animParameters.name == parameterName)
                    {
                        Debug.Log("Animator Controller already has parameter called: " + parameterName, animatorController);
                        return;
                    }
                }
                animatorController.AddParameter(parameterName, controllerParameterType);
            }

        }
        void AddParameter(VRCExpressionParameters expressionParameters, AnimatorController animatorController, string parameterName, AnimatorControllerParameterType controllerParameterType, bool doSave = false)
        {
            if (animatorController && expressionParameters)
            {
                //bool foundValue = false;
                //var CurrentParamters = new List<VRCExpressionParameters.Parameter>();

                VRCExpressionParameters.Parameter foundParamter = expressionParameters.FindParameter(parameterName);
                if(foundParamter == null)
                {
                    List<VRCExpressionParameters.Parameter> paramList = expressionParameters.parameters.ToList();
                    VRCExpressionParameters.ValueType valueType = VRCExpressionParameters.ValueType.Bool;
                    switch (controllerParameterType)
                    {
                        case AnimatorControllerParameterType.Bool:
                            valueType = VRCExpressionParameters.ValueType.Bool;
                            break;
                        case AnimatorControllerParameterType.Int:
                            valueType = VRCExpressionParameters.ValueType.Int;
                            break;
                        case AnimatorControllerParameterType.Float:
                            valueType = VRCExpressionParameters.ValueType.Float;
                            break;
                    }
                    paramList.Add(
                        new VRCExpressionParameters.Parameter()
                        {
                            name = parameterName,
                            saved = doSave,
                            valueType = valueType,
                        }

                        );
                    expressionParameters.parameters = paramList.ToArray();
                    EditorUtility.SetDirty(expressionParameters);
                }
                //expressionParameters.parameters = CurrentParamters.ToArray();
                foreach (var animParameter in animatorController.parameters)
                {
                    if (animParameter.name == parameterName)
                    {
                        Debug.Log("Animator Controller already has parameter called: " + parameterName, animatorController);
                        return;
                    }
                }
                animatorController.AddParameter(parameterName, controllerParameterType);
            }

        }
        void OnGUI()
        {
            SMTaK.DrawExperimentalWarning();
            GUIStyle WrapTextStyle = new GUIStyle(EditorStyles.label);
            WrapTextStyle.wordWrap = true;
            EditorGUILayout.Space();
            ParametersFile = (VRCExpressionParameters)EditorGUILayout.ObjectField("Target Parameters file", ParametersFile, typeof(VRCExpressionParameters), false);

            TargetAnimator = (AnimatorController)EditorGUILayout.ObjectField("Target Animator", TargetAnimator, typeof(AnimatorController), false);

            RightGrabAnimation = (AnimationClip)EditorGUILayout.ObjectField("Right grab animation", RightGrabAnimation, typeof(AnimationClip), false);

            LeftGrabAnimation = (AnimationClip)EditorGUILayout.ObjectField("Left grab animation", LeftGrabAnimation, typeof(AnimationClip), false);

            RightEquipAnimation = (AnimationClip)EditorGUILayout.ObjectField("Right equip animation", RightEquipAnimation, typeof(AnimationClip), false);

            LeftEquipAnimation = (AnimationClip)EditorGUILayout.ObjectField("Left equip animation", LeftEquipAnimation, typeof(AnimationClip), false);

            RightForegripAnimation = (AnimationClip)EditorGUILayout.ObjectField("Right foregrip animation", RightForegripAnimation, typeof(AnimationClip), false);

            LeftForegripAnimation = (AnimationClip)EditorGUILayout.ObjectField("Left foregrip animation", LeftForegripAnimation, typeof(AnimationClip), false);

            MainHandFiringAnimation = (AnimationClip)EditorGUILayout.ObjectField("Main Hand attack animation", MainHandFiringAnimation, typeof(AnimationClip), false);

            OffHandFiringAnimation = (AnimationClip)EditorGUILayout.ObjectField("Off Hand attack animation", OffHandFiringAnimation, typeof(AnimationClip), false);

            WeaponName = EditorGUILayout.TextField("Weapon Name", WeaponName);
            if (GUILayout.Button("Generate") && WeaponName.Length > 0)
            {
                if (TargetAnimator && ParametersFile)
                {
                    Undo.RecordObject(TargetAnimator, "Dynamics Inventory System (" + WeaponName + ")");
                    AddParameter(TargetAnimator, "GestureRight", AnimatorControllerParameterType.Int);
                    AddParameter(TargetAnimator, "GestureRightWeight", AnimatorControllerParameterType.Float);
                    AddParameter(TargetAnimator, "GestureLeft", AnimatorControllerParameterType.Int);
                    AddParameter(TargetAnimator, "GestureLeftWeight", AnimatorControllerParameterType.Float);
                    AddParameter(TargetAnimator, WeaponName + "_GrabR", AnimatorControllerParameterType.Float);
                    AddParameter(TargetAnimator, WeaponName + "_GrabL", AnimatorControllerParameterType.Float);
                    AddParameter(ParametersFile, TargetAnimator, WeaponName + "_Equipped", AnimatorControllerParameterType.Bool);
                    AddParameter(ParametersFile, TargetAnimator, WeaponName + "_Southpaw", AnimatorControllerParameterType.Bool, true);
                    if (RightForegripAnimation && LeftForegripAnimation)
                    {
                        AddParameter(TargetAnimator, WeaponName + "_ForegripGrabR", AnimatorControllerParameterType.Float);
                        AddParameter(TargetAnimator, WeaponName + "_ForegripGrabL", AnimatorControllerParameterType.Float);
                        AddParameter(ParametersFile, TargetAnimator, WeaponName + "_ForegripHeld", AnimatorControllerParameterType.Bool);

                    }
                    AddParameter(TargetAnimator, "HasItemInRightHand", AnimatorControllerParameterType.Bool);
                    AddParameter(TargetAnimator, "HasItemInLeftHand", AnimatorControllerParameterType.Bool);

                    AnimatorStateTransition animatorStateTransition;
                    if(RightGrabAnimation)
                    {
                        var rightGrabLayer = new AnimatorControllerLayer();
                        rightGrabLayer.name = TargetAnimator.MakeUniqueLayerName("HandGrab_Right");
                        if (rightGrabLayer.name != "HandGrab_Right")
                        {
                            Debug.LogWarning("Animator Controller already contains layers for grabbing! Please make sure to cleanup the animator when you're done!", TargetAnimator);
                        }
                        rightGrabLayer.defaultWeight = 1f;
                        rightGrabLayer.stateMachine = new AnimatorStateMachine();
                        TargetAnimator.AddLayer(rightGrabLayer);
                        AssetDatabase.AddObjectToAsset(rightGrabLayer.stateMachine, AssetDatabase.GetAssetPath(TargetAnimator));
                        var GrabWait = rightGrabLayer.stateMachine.AddState("Wait");
                        var GrabAction = rightGrabLayer.stateMachine.AddState("GrabAction");
                        GrabAction.motion = RightGrabAnimation;
                        var GrabPost = rightGrabLayer.stateMachine.AddState("GrabPost");
                        animatorStateTransition = GrabWait.AddTransition(GrabAction);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 1, "GestureRight");

                        animatorStateTransition = GrabWait.AddTransition(GrabAction);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 3, "GestureRight");

                        animatorStateTransition = GrabAction.AddTransition(GrabPost);
                        animatorStateTransition.hasExitTime = true;
                        animatorStateTransition.exitTime = 1f;
                        animatorStateTransition.duration = 0;

                        animatorStateTransition = GrabPost.AddTransition(GrabWait);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 2, "GestureRight");

                        animatorStateTransition = GrabPost.AddTransition(GrabWait);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 0, "GestureRight");
                        //RightGrabAnimation.
                    }

                    if (RightGrabAnimation)
                    {
                        var leftGrabLayer = new AnimatorControllerLayer();
                        leftGrabLayer.name = TargetAnimator.MakeUniqueLayerName("HandGrab_Left");
                        if (leftGrabLayer.name != "HandGrab_Left")
                        {
                            Debug.LogWarning("Animator Controller already contains layers for grabbing! Please make sure to cleanup the animator when you're done!", TargetAnimator);
                        }
                        leftGrabLayer.defaultWeight = 1f;
                        leftGrabLayer.stateMachine = new AnimatorStateMachine();
                        TargetAnimator.AddLayer(leftGrabLayer);
                        AssetDatabase.AddObjectToAsset(leftGrabLayer.stateMachine, AssetDatabase.GetAssetPath(TargetAnimator));
                        var GrabWait = leftGrabLayer.stateMachine.AddState("Wait");
                        var GrabAction = leftGrabLayer.stateMachine.AddState("GrabAction");
                        GrabAction.motion = RightGrabAnimation;
                        var GrabPost = leftGrabLayer.stateMachine.AddState("GrabPost");
                        animatorStateTransition = GrabWait.AddTransition(GrabAction);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 1, "GestureLeft");

                        animatorStateTransition = GrabWait.AddTransition(GrabAction);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 3, "GestureLeft");

                        animatorStateTransition = GrabAction.AddTransition(GrabPost);
                        animatorStateTransition.hasExitTime = true;
                        animatorStateTransition.exitTime = 1f;
                        animatorStateTransition.duration = 0;

                        animatorStateTransition = GrabPost.AddTransition(GrabWait);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 2, "GestureLeft");

                        animatorStateTransition = GrabPost.AddTransition(GrabWait);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 0, "GestureLeft");

                    }

                    var grabLayer = new AnimatorControllerLayer();
                    grabLayer.name = TargetAnimator.MakeUniqueLayerName(WeaponName + "_Grab");
                    if(grabLayer.name != WeaponName + "_Grab")
                    {
                        Debug.LogWarning("Animator Controller already contains layers for grabbing! Please make sure to cleanup the animator when you're done!", TargetAnimator);
                    }
                    grabLayer.defaultWeight = 1f;
                    grabLayer.stateMachine = new AnimatorStateMachine();
                    TargetAnimator.AddLayer(grabLayer);
                    //Apparently this is needed. For some reason state machines are saved seperatedly from the controller and needs to be associated with it in the database
                    AssetDatabase.AddObjectToAsset(grabLayer.stateMachine, AssetDatabase.GetAssetPath(TargetAnimator));


                    var unequippedState = grabLayer.stateMachine.AddState(WeaponName + "_Unequipped");

                    if(RightEquipAnimation)
                    {
                        var GrabState = grabLayer.stateMachine.AddState(WeaponName + "_GrabRight");
                        animatorStateTransition = unequippedState.AddTransition(GrabState);
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, "HasItemInRightHand");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, WeaponName + "_GrabR");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, WeaponName + "_Equipped");
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0;

                        var stateBehaviour = GrabState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                        var paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                        paramter.name = "HasItemInRightHand";
                        paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                        paramter.value = 1;
                        stateBehaviour.parameters.Add(paramter);

                        paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                        paramter.name = WeaponName + "_Equipped";
                        paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                        paramter.value = 1;
                        stateBehaviour.parameters.Add(paramter);

                        paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                        paramter.name = WeaponName + "_Southpaw";
                        paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                        paramter.value = 0;
                        stateBehaviour.parameters.Add(paramter);


                        var EquippedState = grabLayer.stateMachine.AddState(WeaponName + "_EquippedRight");
                        EquippedState.motion = RightEquipAnimation;
                        animatorStateTransition = GrabState.AddTransition(EquippedState);
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 0.5f, WeaponName + "_Equipped");
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.1f;

                        var UnequipState = grabLayer.stateMachine.AddState(WeaponName + "_UnequipRight");
                        animatorStateTransition = EquippedState.AddTransition(UnequipState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.1f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 2, "GestureRight");

                        stateBehaviour = UnequipState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                        paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                        paramter.name = "HasItemInRightHand";
                        paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                        paramter.value = 0;
                        stateBehaviour.parameters.Add(paramter);

                        paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                        paramter.name = WeaponName + "_Equipped";
                        paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                        paramter.value = 0;
                        stateBehaviour.parameters.Add(paramter);


                        animatorStateTransition = UnequipState.AddTransition(unequippedState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 2, WeaponName + "_Equipped");

                        animatorStateTransition = unequippedState.AddTransition(EquippedState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 2, WeaponName + "_Equipped");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 2, WeaponName + "_Southpaw");
                    }
                    //
                    if (LeftEquipAnimation)
                    {
                        var GrabState = grabLayer.stateMachine.AddState(WeaponName + "_GrabLeft");
                        animatorStateTransition = unequippedState.AddTransition(GrabState);
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, "HasItemInLeftHand");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, WeaponName + "_GrabL");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, WeaponName + "_Equipped");
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0;

                        var stateBehaviour = GrabState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                        var paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                        paramter.name = "HasItemInLeftHand";
                        paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                        paramter.value = 1;
                        stateBehaviour.parameters.Add(paramter);

                        paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                        paramter.name = WeaponName + "_Equipped";
                        paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                        paramter.value = 1;
                        stateBehaviour.parameters.Add(paramter);

                        paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                        paramter.name = WeaponName + "_Southpaw";
                        paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                        paramter.value = 1;
                        stateBehaviour.parameters.Add(paramter);

                        var EquippedState = grabLayer.stateMachine.AddState(WeaponName + "_EquippedLeft");
                        EquippedState.motion = LeftEquipAnimation;
                        animatorStateTransition = GrabState.AddTransition(EquippedState);
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 0.5f, WeaponName + "_Equipped");
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.1f;

                        var UnequipState = grabLayer.stateMachine.AddState(WeaponName + "_UnequipLeft");
                        animatorStateTransition = EquippedState.AddTransition(UnequipState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.1f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 2, "GestureLeft");

                        animatorStateTransition = UnequipState.AddTransition(unequippedState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 2, WeaponName + "_Equipped");

                        stateBehaviour = UnequipState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                        paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                        paramter.name = "HasItemInLeftHand";
                        paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                        paramter.value = 0;
                        stateBehaviour.parameters.Add(paramter);

                        paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                        paramter.name = WeaponName + "_Equipped";
                        paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                        paramter.value = 0;
                        stateBehaviour.parameters.Add(paramter);

                        animatorStateTransition = unequippedState.AddTransition(EquippedState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 2, WeaponName + "_Equipped");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 2, WeaponName + "_Southpaw");
                    }

                    if (RightForegripAnimation || LeftForegripAnimation)
                    {
                        var foregripLayer = new AnimatorControllerLayer();
                        foregripLayer.name = TargetAnimator.MakeUniqueLayerName(WeaponName + "_Foregrip");
                        if (foregripLayer.name != WeaponName + "_Foregrip")
                        {
                            Debug.LogWarning("Animator Controller already contains layers for foregrip! Please make sure to cleanup the animator when you're done!", TargetAnimator);
                        }
                        foregripLayer.defaultWeight = 1f;
                        foregripLayer.stateMachine = new AnimatorStateMachine();
                        TargetAnimator.AddLayer(foregripLayer);
                        AssetDatabase.AddObjectToAsset(foregripLayer.stateMachine, AssetDatabase.GetAssetPath(TargetAnimator));

                        var foregripUnequippedState = foregripLayer.stateMachine.AddState(WeaponName + "_Unequipped");
                        if (RightForegripAnimation)
                        {
                            var GrabState = foregripLayer.stateMachine.AddState(WeaponName + "_GrabRight");
                            animatorStateTransition = foregripUnequippedState.AddTransition(GrabState);
                            animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, "HasItemInRightHand");
                            animatorStateTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, WeaponName + "_ForegripGrabR");
                            animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, WeaponName + "_ForegripHeld");
                            animatorStateTransition.AddCondition(AnimatorConditionMode.If, 0.5f, WeaponName + "_Southpaw");
                            animatorStateTransition.hasExitTime = false;
                            animatorStateTransition.duration = 0;

                            var stateBehaviour = GrabState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                            var paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                            paramter.name = "HasItemInRightHand";
                            paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                            paramter.value = 1;
                            stateBehaviour.parameters.Add(paramter);

                            paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                            paramter.name = WeaponName + "_ForegripHeld";
                            paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                            paramter.value = 1;
                            stateBehaviour.parameters.Add(paramter);

                            var EquippedState = foregripLayer.stateMachine.AddState(WeaponName + "_EquippedRight");
                            EquippedState.motion = RightForegripAnimation;
                            animatorStateTransition = GrabState.AddTransition(EquippedState);
                            animatorStateTransition.AddCondition(AnimatorConditionMode.If, 0.5f, WeaponName + "_ForegripHeld");
                            animatorStateTransition.hasExitTime = false;
                            animatorStateTransition.duration = 0.1f;

                            var UnequipState = foregripLayer.stateMachine.AddState(WeaponName + "_UnequipRight");
                            animatorStateTransition = EquippedState.AddTransition(UnequipState);
                            animatorStateTransition.hasExitTime = false;
                            animatorStateTransition.duration = 0.1f;
                            animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 2, "GestureRight");

                            stateBehaviour = UnequipState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                            paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                            paramter.name = "HasItemInRightHand";
                            paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                            paramter.value = 0;
                            stateBehaviour.parameters.Add(paramter);

                            paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                            paramter.name = WeaponName + "_ForegripHeld";
                            paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                            paramter.value = 0;
                            stateBehaviour.parameters.Add(paramter);

                            animatorStateTransition = UnequipState.AddTransition(foregripUnequippedState);
                            animatorStateTransition.hasExitTime = false;
                            animatorStateTransition.duration = 0.0f;
                            animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 2, WeaponName + "_ForegripHeld");

                            animatorStateTransition = foregripUnequippedState.AddTransition(EquippedState);
                            animatorStateTransition.hasExitTime = false;
                            animatorStateTransition.duration = 0.0f;
                            animatorStateTransition.AddCondition(AnimatorConditionMode.If, 2, WeaponName + "_ForegripHeld");
                            animatorStateTransition.AddCondition(AnimatorConditionMode.If, 2, WeaponName + "_Southpaw");
                        }
                        if (LeftForegripAnimation)
                        {
                            var GrabState = foregripLayer.stateMachine.AddState(WeaponName + "_GrabLeft");
                            animatorStateTransition = foregripUnequippedState.AddTransition(GrabState);
                            animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, "HasItemInLeftHand");
                            animatorStateTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, WeaponName + "_ForegripGrabR");
                            animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, WeaponName + "_ForegripHeld");
                            animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 0.5f, WeaponName + "_Southpaw");
                            animatorStateTransition.hasExitTime = false;
                            animatorStateTransition.duration = 0;

                            var stateBehaviour = GrabState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                            var paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                            paramter.name = "HasItemInLeftHand";
                            paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                            paramter.value = 1;
                            stateBehaviour.parameters.Add(paramter);

                            paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                            paramter.name = WeaponName + "_ForegripHeld";
                            paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                            paramter.value = 1;
                            stateBehaviour.parameters.Add(paramter);

                            var EquippedState = foregripLayer.stateMachine.AddState(WeaponName + "_EquippedLeft");
                            EquippedState.motion = LeftForegripAnimation;
                            animatorStateTransition = GrabState.AddTransition(EquippedState);
                            animatorStateTransition.AddCondition(AnimatorConditionMode.If, 0.5f, WeaponName + "_ForegripHeld");
                            animatorStateTransition.hasExitTime = false;
                            animatorStateTransition.duration = 0.1f;

                            AnimatorState UnequipState = foregripLayer.stateMachine.AddState(WeaponName + "_UnequipLeft");
                            animatorStateTransition = EquippedState.AddTransition(UnequipState);
                            animatorStateTransition.hasExitTime = false;
                            animatorStateTransition.duration = 0.1f;
                            animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 2, "GestureLeft");

                            stateBehaviour = UnequipState.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
                            paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                            paramter.name = "HasItemInLeftHand";
                            paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                            paramter.value = 0;
                            stateBehaviour.parameters.Add(paramter);

                            paramter = new VRC.SDKBase.VRC_AvatarParameterDriver.Parameter();
                            paramter.name = WeaponName + "_ForegripHeld";
                            paramter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
                            paramter.value = 0;
                            stateBehaviour.parameters.Add(paramter);

                            animatorStateTransition = UnequipState.AddTransition(foregripUnequippedState);
                            animatorStateTransition.hasExitTime = false;
                            animatorStateTransition.duration = 0.0f;
                            animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 2, WeaponName + "_ForegripHeld");

                            animatorStateTransition = foregripUnequippedState.AddTransition(EquippedState);
                            animatorStateTransition.hasExitTime = false;
                            animatorStateTransition.duration = 0.0f;
                            animatorStateTransition.AddCondition(AnimatorConditionMode.If, 2, WeaponName + "_ForegripHeld");
                            animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 2, WeaponName + "_Southpaw");
                        }
                    }
                    if(MainHandFiringAnimation)
                    {

                        var mainHandLayer = new AnimatorControllerLayer();
                        mainHandLayer.name = TargetAnimator.MakeUniqueLayerName(WeaponName + "_Mainhand");
                        if (mainHandLayer.name != WeaponName + "_Mainhand")
                        {
                            Debug.LogWarning("Animator Controller already contains layers for mainhand! Please make sure to cleanup the animator when you're done!", TargetAnimator);
                        }
                        mainHandLayer.defaultWeight = 1f;
                        mainHandLayer.stateMachine = new AnimatorStateMachine();
                        TargetAnimator.AddLayer(mainHandLayer);
                        AssetDatabase.AddObjectToAsset(mainHandLayer.stateMachine, AssetDatabase.GetAssetPath(TargetAnimator));

                        var waitState = mainHandLayer.stateMachine.AddState(WeaponName + "_Wait");
                        var FireState = mainHandLayer.stateMachine.AddState(WeaponName + "_Fire");
                        animatorStateTransition = waitState.AddTransition(FireState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 1, WeaponName + "_Equipped");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 1, WeaponName + "_Southpaw");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 1, "GestureLeft");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, "GestureLeftWeight");

                        animatorStateTransition = waitState.AddTransition(FireState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 1, WeaponName + "_Equipped");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 1, WeaponName + "_Southpaw");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 1, "GestureRight");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, "GestureRightWeight");

                        animatorStateTransition = FireState.AddTransition(waitState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 1, WeaponName + "_Equipped");

                        animatorStateTransition = FireState.AddTransition(waitState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.NotEqual, 1, "GestureRight");

                        animatorStateTransition = FireState.AddTransition(waitState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, "GestureRightWeight");

                        animatorStateTransition = FireState.AddTransition(waitState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.NotEqual, 1, "GestureLeft");

                        animatorStateTransition = FireState.AddTransition(waitState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, "GestureLeftWeight");
                    }
                    if (OffHandFiringAnimation)
                    {

                        var offHandLayer = new AnimatorControllerLayer();
                        offHandLayer.name = TargetAnimator.MakeUniqueLayerName(WeaponName + "_Offhand");
                        if (offHandLayer.name != WeaponName + "_Offhand")
                        {
                            Debug.LogWarning("Animator Controller already contains layers for offhand! Please make sure to cleanup the animator when you're done!", TargetAnimator);
                        }
                        offHandLayer.defaultWeight = 1f;
                        offHandLayer.stateMachine = new AnimatorStateMachine();
                        TargetAnimator.AddLayer(offHandLayer);
                        AssetDatabase.AddObjectToAsset(offHandLayer.stateMachine, AssetDatabase.GetAssetPath(TargetAnimator));

                        var waitState = offHandLayer.stateMachine.AddState(WeaponName + "_Wait");
                        var FireState = offHandLayer.stateMachine.AddState(WeaponName + "_Fire");
                        animatorStateTransition = waitState.AddTransition(FireState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 1, WeaponName + "_Foregrip");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 1, WeaponName + "_Southpaw");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 1, "GestureLeft");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, "GestureLeftWeight");

                        animatorStateTransition = waitState.AddTransition(FireState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 1, WeaponName + "_Foregrip");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.If, 1, WeaponName + "_Southpaw");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Equals, 1, "GestureRight");
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Greater, 0.5f, "GestureRightWeight");

                        animatorStateTransition = FireState.AddTransition(waitState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.IfNot, 1, WeaponName + "_Foregrip");

                        animatorStateTransition = FireState.AddTransition(waitState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.NotEqual, 1, "GestureRight");

                        animatorStateTransition = FireState.AddTransition(waitState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, "GestureRightWeight");

                        animatorStateTransition = FireState.AddTransition(waitState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.NotEqual, 1, "GestureLeft");

                        animatorStateTransition = FireState.AddTransition(waitState);
                        animatorStateTransition.hasExitTime = false;
                        animatorStateTransition.duration = 0.0f;
                        animatorStateTransition.AddCondition(AnimatorConditionMode.Less, 0.5f, "GestureLeftWeight");
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
