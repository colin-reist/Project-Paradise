using UnityEngine;
using UnityEditor;
using static UnityEngine.Mathf;
using System.Globalization;
using UnityEngine.U2D.IK;
using Unity.VisualScripting;
namespace LolopupkaAnimations2D
{

[CustomEditor(typeof(proceduralAnimation))]
public class ProceduralAnimationEditor : Editor
{

    private SerializedProperty characterType;
    private SerializedProperty stepHeight;
    private SerializedProperty stepSpeed;
    private SerializedProperty stepForwardOffset;
    private SerializedProperty animationSpeed;
    private SerializedProperty stepsInterval;
    private SerializedProperty customStepTimings;
    private SerializedProperty manualStepTimings;
    private SerializedProperty layerMask;
    private SerializedProperty verticalLegMovement;
    private SerializedProperty easingFunction;
    private SerializedProperty legsIkTargets;
    private SerializedProperty minDistanceToMakeStep;
    private SerializedProperty maxDistanceToMakeStep;
    private SerializedProperty debugVisualisation;
    private SerializedProperty groundCheckOffset;
    private SerializedProperty groundCheckRange;
    private SerializedProperty groundCheckRadius;
    private SerializedProperty body;
    private SerializedProperty bodyOffset ;
    private SerializedProperty bodyPositionSmoothing;
    private SerializedProperty legsImpact;
    private SerializedProperty bodyPositionTarget;
    private SerializedProperty bodyOrientation;
    private SerializedProperty bodyRotationTarget;
    private SerializedProperty bodyRotationSmoothing;
    private SerializedProperty bodyTilt;
    private SerializedProperty tiltDirection;
    private SerializedProperty maxTilt;
    private SerializedProperty tiltSpeed; 
    private SerializedProperty frequency;
    private SerializedProperty damping;
    private SerializedProperty response;

    private SerializedProperty OnStepFinishedUnityEvent;
    private SerializedProperty playFootstepSound;
    private SerializedProperty spawnParticles;
    private SerializedProperty stepSVX;
    private SerializedProperty stepVFX;


    private SerializedProperty currentInspectorTab;    
    private Vector2 startMousePosition, endMousePosition;
    private Texture2D scriptIcon;
    private Texture2D linkingButtonIcon;
    private Texture2D youtubeIcon;
    private Texture2D discordIcon;
    private Texture2D mailIcon;
    private Texture2D documentationIcon;
    private Texture2D legsSettingsIcon;
    private Texture2D bodySettingsIcon;
    private Texture2D stepEffectsIcon;
    private Texture2D HelpIcon;

    private SerializedProperty linkedLegsId;

    private int seekingButtonIdex = -1;
    private int groupCount;

    private bool seekingForDependesies = false;
    private bool drawLine = false;
    private bool showLegsList = true;
    private bool showAdvancedSettings = false;

    private void OnEnable() 
    {   
        #region  finding variables
        //legs variables
        characterType = serializedObject.FindProperty("characterType");
        stepHeight = serializedObject.FindProperty("stepHeight");
        stepSpeed = serializedObject.FindProperty("stepSpeed");
        stepForwardOffset = serializedObject.FindProperty("stepForwardOffset");
        animationSpeed = serializedObject.FindProperty("animationSpeed");
        stepsInterval = serializedObject.FindProperty("stepsInterval");
        customStepTimings = serializedObject.FindProperty("customStepTimings");
        manualStepTimings = serializedObject.FindProperty("manualStepTimings");
        layerMask = serializedObject.FindProperty("layerMask");
        verticalLegMovement = serializedObject.FindProperty("verticalLegMovement");
        easingFunction = serializedObject.FindProperty("easingFunction");
        legsIkTargets = serializedObject.FindProperty("legsIkTargets");
        minDistanceToMakeStep = serializedObject.FindProperty("minDistanceToMakeStep");
        maxDistanceToMakeStep = serializedObject.FindProperty("maxDistanceToMakeStep");
        debugVisualisation = serializedObject.FindProperty("debugVisualisation");
        groundCheckOffset = serializedObject.FindProperty("groundCheckOffset");
        groundCheckRange = serializedObject.FindProperty("groundCheckRange");
        groundCheckRadius = serializedObject.FindProperty("groundCheckRadius");


        linkedLegsId = serializedObject.FindProperty("linkedLegsId");
        // body variables
        body = serializedObject.FindProperty("body");
        bodyOffset = serializedObject.FindProperty("bodyOffset") ;
        bodyPositionSmoothing = serializedObject.FindProperty("bodyPositionSmoothing");
        legsImpact = serializedObject.FindProperty("legsImpact");
        bodyPositionTarget = serializedObject.FindProperty("bodyPositionTarget");

        bodyOrientation = serializedObject.FindProperty("bodyOrientation");
        bodyRotationTarget = serializedObject.FindProperty("bodyRotationTarget");
        bodyRotationSmoothing = serializedObject.FindProperty("bodyRotationSmoothing");

        tiltDirection = serializedObject.FindProperty("tiltDirection");
        bodyTilt = serializedObject.FindProperty("bodyTilt");
        maxTilt = serializedObject.FindProperty("maxTilt");
        tiltSpeed = serializedObject.FindProperty("tiltSpeed");
        frequency = serializedObject.FindProperty("frequency");
        damping = serializedObject.FindProperty("damping");
        response = serializedObject.FindProperty("response");
        #endregion

        OnStepFinishedUnityEvent = serializedObject.FindProperty("OnStepFinishedUnityEvent");
        playFootstepSound = serializedObject.FindProperty("playFootstepSound");
        spawnParticles = serializedObject.FindProperty("spawnParticles");
        stepSVX = serializedObject.FindProperty("stepSVX");
        stepVFX = serializedObject.FindProperty("stepVFX");

        //custom icons
        scriptIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Procedural Walk Animation/Scripts/procedural animation/Editor/icons/script icon.png");
        linkingButtonIcon = EditorGUIUtility.Load("Assets/Procedural Walk Animation/Scripts/procedural animation/Editor/icons/link icon.png") as Texture2D;
        youtubeIcon = EditorGUIUtility.Load("Assets/Procedural Walk Animation/Scripts/procedural animation/Editor/icons/youtube_1384060.png") as Texture2D;
        discordIcon = EditorGUIUtility.Load("Assets/Procedural Walk Animation/Scripts/procedural animation/Editor/icons/Discord_Icon.png") as Texture2D;
        mailIcon = EditorGUIUtility.Load("Assets/Procedural Walk Animation/Scripts/procedural animation/Editor/icons/Gmail_Icon.png") as Texture2D;
        documentationIcon = EditorGUIUtility.Load("Assets/Procedural Walk Animation/Scripts/procedural animation/Editor/icons/documentationIconIcon.png") as Texture2D;
        legsSettingsIcon = EditorGUIUtility.Load("Assets/Procedural Walk Animation/Scripts/procedural animation/Editor/icons/legs setting icon.png") as Texture2D;
        bodySettingsIcon = EditorGUIUtility.Load("Assets/Procedural Walk Animation/Scripts/procedural animation/Editor/icons/body settings icon.png") as Texture2D;
        stepEffectsIcon = EditorGUIUtility.Load("Assets/Procedural Walk Animation/Scripts/procedural animation/Editor/icons/step effects icon.png") as Texture2D;
        HelpIcon = EditorGUIUtility.Load("Assets/Procedural Walk Animation/Scripts/procedural animation/Editor/icons/Help_Icon.png") as Texture2D;

        currentInspectorTab = serializedObject.FindProperty("currentInspectorTab");
        
        if (scriptIcon != null)
        {
            // Apply the icon to the script
            EditorGUIUtility.SetIconForObject(target, scriptIcon);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        // tabs settings
        // GUIStyle customButtonStyle = new GUIStyle(GUI.skin.button);
// 
        // customButtonStyle.fontSize = 10; 
// 
        // customButtonStyle.normal.textColor = Color.white; 
        // customButtonStyle.hover.textColor = new Color(0.788f, 0.980f, 0.755f); 

        GUIStyle customButtonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleLeft,
            fixedHeight = 45f, 
            fixedWidth = 120f, 
            padding = new RectOffset(10, 10, 5, 5) 
        };

        customButtonStyle.fontSize = 11; 
        customButtonStyle.normal.textColor = Color.white; 
        customButtonStyle.hover.textColor = new Color(0.788f, 0.980f, 0.755f); 

        GUIContent[] toolbarIcons = new GUIContent[]
        {
            new GUIContent("  Legs Settings", legsSettingsIcon),
            new GUIContent("  Body Settings", bodySettingsIcon),
            new GUIContent("  Step Effects", stepEffectsIcon),
            new GUIContent("  Help", HelpIcon)
        };

        GUILayout.Space(8f);
        currentInspectorTab.intValue = GUILayout.SelectionGrid(currentInspectorTab.intValue, toolbarIcons, 4, customButtonStyle, GUILayout.Height(45f));
        GUILayout.Space(15f);

        switch(currentInspectorTab.intValue)
        {
            case 0:
            DrawLegSettingsVariables();
            DrawLegListAndLinkingUI();
            drawConnectingLine();
            DrawAdvancedLegsSettingsSection();
            break;

            case 1:
            DrawBodySettingsVariables();
            break;

            case 2:
            DrawStepEffectsProperties();
            break;

            case 3:
            DrawHelpOptions();
            break;
        }
        EditorGUILayout.Space(20f);

        serializedObject.ApplyModifiedProperties();
    }


    private void DrawBodySettingsVariables()
    {
        EditorGUILayout.PropertyField(body, new GUIContent("Root Bone"));
        EditorGUILayout.Space(10f);
        EditorGUILayout.PropertyField(bodyPositionTarget, new GUIContent("Body Position Target"));
        if(bodyPositionTarget.enumValueIndex == 0) EditorGUILayout.PropertyField(bodyPositionSmoothing, new GUIContent("Body Position Smoothing"));
        EditorGUILayout.PropertyField(bodyOffset, new GUIContent("Body Position Offset"));


        if(bodyPositionTarget.enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(frequency, new GUIContent("Frequency"));
            EditorGUILayout.PropertyField(damping, new GUIContent("Damping"));
            EditorGUILayout.PropertyField(response, new GUIContent("Response"));
            EditorGUILayout.Space(120);
            DrawBodyPositionGrhaph(new Vector3(25, 295, 0), new Vector3(400, 220, 0));

        }

        if(characterType.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(legsImpact, new GUIContent("Legs Impact"));
        }

        if(characterType.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(bodyOrientation, new GUIContent("Body Orientation"));
            if(bodyOrientation.boolValue)
            {
                EditorGUILayout.PropertyField(bodyRotationTarget, new GUIContent("body Rotation Target")); 
                EditorGUILayout.PropertyField(bodyRotationSmoothing, new GUIContent("Body Rotation Smoothing"));
            }
        }

        if(bodyRotationTarget.enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(bodyTilt, new GUIContent("Body Tilt"));
            if(bodyTilt.boolValue)
            {
                EditorGUILayout.PropertyField(tiltDirection, new GUIContent("Tilt Direction"));
                EditorGUILayout.PropertyField(maxTilt, new GUIContent("Max Tilt"));
                EditorGUILayout.PropertyField(tiltSpeed, new GUIContent("Tilt Speed"));
            }
        }
    }
    private void DrawLegSettingsVariables()
    {
        EditorGUILayout.PropertyField(characterType, new GUIContent("Character Type"));
        EditorGUILayout.Space(10f);
        EditorGUILayout.PropertyField(stepHeight, new GUIContent("Step Height"));
        EditorGUILayout.PropertyField(stepSpeed, new GUIContent("Step Speed"));
        EditorGUILayout.PropertyField(stepForwardOffset, new GUIContent("Step Forward Offset"));
        EditorGUILayout.PropertyField(animationSpeed, new GUIContent("Animation Speed"));
        EditorGUILayout.PropertyField(stepsInterval, new GUIContent("Steps Interval"));
        EditorGUILayout.PropertyField(customStepTimings, new GUIContent("Custom Step Timings"));
        EditorGUILayout.Space(10);

        if(customStepTimings.boolValue)
        {        
            EditorGUILayout.PropertyField(manualStepTimings, new GUIContent("Manual Step Timings"));
            EditorGUILayout.Space(10);
        }

        EditorGUILayout.PropertyField(layerMask, new GUIContent("Walkable Layers"));
        EditorGUILayout.PropertyField(verticalLegMovement, new GUIContent("Vertical Leg Movement"));
        EditorGUILayout.PropertyField(easingFunction, new GUIContent("Easing Function"));
    }
    
    private void DrawLegListAndLinkingUI()
    {
        proceduralAnimation proceduralAnimationTarget = (proceduralAnimation)target;

        GUILayout.Space(15f);
        EditorGUILayout.BeginHorizontal();
        Rect foldoutRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.Width(50));

        showLegsList = EditorGUI.Foldout(foldoutRect, showLegsList, "Legs List", true);

        GUILayout.Space(10);

        if(GUILayout.Button("Find Legs", GUILayout.Width(90f)))
        {
            FindIkTargets();
        }

        if (GUILayout.Button("Extract Step Timings"))
        {
            float[] timings = GetStepTimingsOffset();
            string textToCopy = "new float[]{";

            for (int i = 0; i < legsIkTargets.arraySize; i++)
            {
                textToCopy += timings[i].ToString(CultureInfo.InvariantCulture) + "f, ";
            }
            textToCopy = textToCopy.Substring(0, textToCopy.Length - 2);
            textToCopy += "}";

            // Copy the text to the system's clipboard
            EditorGUIUtility.systemCopyBuffer = textToCopy;
            Debug.Log("New timings saved to clipboard: " + textToCopy);
        }

        EditorGUILayout.EndHorizontal();

        if(legsIkTargets.arraySize == 0 || !showLegsList) return;
        
        for (int i = 0; i < legsIkTargets.arraySize; ++i)
        {
            EditorGUILayout.BeginHorizontal();

            Rect legTransformRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.Width(250));
            EditorGUI.PropertyField(legTransformRect, legsIkTargets.GetArrayElementAtIndex(i), GUIContent.none);

            GUILayout.Space(10f);

            DrawMoveGroupButtons(i);
            
            GUILayout.Space(10f);

            // draw linking button
            if(seekingButtonIdex == i)
            {
                GUI.color = Color.gray;
            }
            else
            {
                GUI.color = Color.white;
            }

            Rect linkingButtonRect = GUILayoutUtility.GetRect(23f, 23f, GUILayout.ExpandWidth(false));   
            GUI.Box(linkingButtonRect, linkingButtonIcon);

            GUI.color = Color.white;
            
            GUIStyle boldButtonStyle = new GUIStyle(GUI.skin.button);
            boldButtonStyle.fontStyle = FontStyle.Bold;
            bool deleteLeg = GUILayout.Button("–", boldButtonStyle, GUILayout.Width(20f));

            if(deleteLeg)
            {
                legsIkTargets.DeleteArrayElementAtIndex(i);
                linkedLegsId.DeleteArrayElementAtIndex(i); 
                manualStepTimings.DeleteArrayElementAtIndex(i);
            }

            handleLinkingInput(i, legTransformRect, linkingButtonRect);

            EditorGUILayout.EndHorizontal();

            // groups visualisation
            DrawVerticalLine(legTransformRect, Color.cyan);

            if(i != linkedLegsId.arraySize - 1 && !deleteLeg)
            {
                int currentElement = linkedLegsId.GetArrayElementAtIndex(i).intValue;
                int nextElement = linkedLegsId.GetArrayElementAtIndex(i + 1).intValue;
                if(currentElement != nextElement || currentElement == 0)
                {
                    DrawGroupSeparationLine(Color.cyan, legTransformRect);
                }
            }
        }

        DrawAddLegButton();
    }

    private void DrawAdvancedLegsSettingsSection()
    {
        showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings", true);
        if(showAdvancedSettings)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = new Color(0.920f, 0.598f, 0.684f); 

            EditorGUILayout.LabelField("Movement Thresholds", style);
            EditorGUILayout.PropertyField(minDistanceToMakeStep, new GUIContent("Min Distance To Make Step"));
            EditorGUILayout.PropertyField(maxDistanceToMakeStep, new GUIContent("Max Distance To Make Step"));

            EditorGUILayout.Space(15f);

            EditorGUILayout.LabelField("Ground Detection", style);
            EditorGUILayout.PropertyField(debugVisualisation, new GUIContent("Debug Visualisation"));
            EditorGUILayout.PropertyField(groundCheckOffset, new GUIContent("Ground Check Offset"));
            EditorGUILayout.PropertyField(groundCheckRange, new GUIContent("Ground Check Range"));
            EditorGUILayout.PropertyField(groundCheckRadius, new GUIContent("Ground Check Radius"));
        }
    }
    private void DrawStepEffectsProperties()
    {
        EditorGUILayout.PropertyField(OnStepFinishedUnityEvent, new GUIContent("On StepFinished"));
        EditorGUILayout.PropertyField(playFootstepSound, new GUIContent("Play Footstep Sound"));

        if(playFootstepSound.boolValue)
        {
            EditorGUILayout.PropertyField(stepSVX, new GUIContent("Footsetps sounds"));
        }

        EditorGUILayout.PropertyField(spawnParticles, new GUIContent("Spawn Particles"));
        if(spawnParticles.boolValue)
        {
            EditorGUILayout.PropertyField(stepVFX, new GUIContent("stepVFX"));
        }
    }

    private void DrawHelpOptions()
    {
        GUIContent mailButton = new GUIContent("   Email me", mailIcon);
        GUIContent discordButton = new GUIContent("   Join Discord", discordIcon);
        GUIContent YouTubeButton = new GUIContent("   Watch Tutorial", youtubeIcon);
        GUIContent documentationButton = new GUIContent("   Read Documentation", documentationIcon);
        
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleLeft,
            fixedHeight = 35f, 
            fixedWidth = 200f, 
            padding = new RectOffset(10, 10, 5, 5) 
        };

        if(GUILayout.Button(mailButton, buttonStyle))
        {
            Application.OpenURL("https://mail.google.com/mail/?view=cm&fs=1&to=karen.magik.user@gmail.com"); 
        }
        if(GUILayout.Button(discordButton, buttonStyle))
        {
            Application.OpenURL("https://discord.gg/7GXeUtvGgY"); 
        }
        if (GUILayout.Button(YouTubeButton, buttonStyle))
        {
            Application.OpenURL("https://www.youtube.com/@lolopupka"); 
        }            
        if (GUILayout.Button(documentationButton, buttonStyle))
        {
            Application.OpenURL("https://drive.google.com/file/d/1QHM5Xt8FWHU4N4HHlOFWLgYBaRMyveOw/view?usp=drive_link"); 
        }
    }

    private void DrawMoveGroupButtons(int index)
    {
        if(index == 0)
        {
            if(GUILayout.Button("↓", GUILayout.Width(25f))) MoveLegGroupDown(index);
            if(GUILayout.Button("↑", GUILayout.Width(25f))) MoveLegGroupUp(index);
            return;
        }
        else if(linkedLegsId.GetArrayElementAtIndex(index).intValue == linkedLegsId.GetArrayElementAtIndex(index - 1).intValue && linkedLegsId.GetArrayElementAtIndex(index).intValue != 0)
        {
            GUILayout.Space(56f);
            return;
        }
        else
        {
            if(GUILayout.Button("↓", GUILayout.Width(25f))) MoveLegGroupDown(index);
            if(GUILayout.Button("↑", GUILayout.Width(25f))) MoveLegGroupUp(index);
        }
    }

    private void MoveLegGroupUp(int index)
    {
        if(index == 0)
        {
            return;
        }

        int nbLegs = linkedLegsId.arraySize;
        int el = 0;
        int previousGroupId = linkedLegsId.GetArrayElementAtIndex(index - 1).intValue;
        
        int currentGroupId = linkedLegsId.GetArrayElementAtIndex(index).intValue;

        if(previousGroupId == 0)
        {
            MoveLegIkTargetsElement(index - 1, GetLastElementOfGroup(index));
            return;
        }

        for (int i = 0; i < nbLegs; i++)
        {
            //find first el of upper group 
            if(linkedLegsId.GetArrayElementAtIndex(i).intValue == previousGroupId)
            {
                el = i;
                break;
            }
        }

        for (int i = 0; i < nbLegs; i++)
        {
            //find every el of upper group 
            if(linkedLegsId.GetArrayElementAtIndex(i).intValue == currentGroupId)
            {
                // move every el of upper group to last el of current group  
                MoveLegIkTargetsElement(i, el);
            }
        }
    }
    private void MoveLegGroupDown(int index)
    {
        if(GetLastElementOfGroup(index) == linkedLegsId.arraySize - 1)
        {
            return;
        }

        int nbLegs = linkedLegsId.arraySize;
        int nextGroupId = linkedLegsId.GetArrayElementAtIndex(GetLastElementOfGroup(index) + 1).intValue;
        for (int i = 0; i < nbLegs; i++)
        {
            if(linkedLegsId.GetArrayElementAtIndex(i).intValue == nextGroupId)
            {
                MoveLegIkTargetsElement(i, index);
            }
        }
    }

    private void MoveLegIkTargetsElement(int srcIndex, int dstIndex)
    {
        legsIkTargets.MoveArrayElement(srcIndex, dstIndex);
        linkedLegsId.MoveArrayElement(srcIndex, dstIndex);
    }

    private int GetLastElementOfGroup(int groupPosition)
    {
        for (int i = groupPosition + 1; i < linkedLegsId.arraySize; i++)
        {
            if(i == linkedLegsId.arraySize) return i;

            if(linkedLegsId.GetArrayElementAtIndex(i).intValue != linkedLegsId.GetArrayElementAtIndex(i - 1).intValue || linkedLegsId.GetArrayElementAtIndex(i).intValue == 0)
            {
                return i - 1;
            }
        }

        return linkedLegsId.arraySize - 1;
    }

    private void DrawAddLegButton()
    {
        //make new transform field for a leg
        GUILayout.BeginHorizontal();

        GUILayout.Space(353);

        if (GUILayout.Button("+", GUILayout.Width(25f)))
        {
            legsIkTargets.arraySize++;
            linkedLegsId.arraySize++;
            manualStepTimings.arraySize++;
            linkedLegsId.GetArrayElementAtIndex(linkedLegsId.arraySize - 1).intValue = 0;
        }
        GUILayout.EndHorizontal();
    }
    private void handleLinkingInput(int index, Rect LegIkTargetRect, Rect linkingButtonRect)
    {

        // highlight the leg that we are crrently trying to link  
        if (LegIkTargetRect.Contains(Event.current.mousePosition) && seekingForDependesies)
        {
            if(seekingButtonIdex != index)
            {
                DrawOutline(LegIkTargetRect, 2, new Color(0.625f, 0.830f, 0.614f)); 
            }
            else
            {
                DrawOutline(LegIkTargetRect, 2, new Color(0.790f, 0.0869f, 0.0869f)); 
            }
        }

        if(Event.current.type == EventType.MouseDown && linkingButtonRect.Contains(Event.current.mousePosition) && !seekingForDependesies)
        {
            startMousePosition = linkingButtonRect.center;
            drawLine = true;
            seekingForDependesies = true;
            seekingButtonIdex = index;
        }

        if (Event.current.type == EventType.MouseUp && seekingForDependesies && LegIkTargetRect.Contains(Event.current.mousePosition))
        {
            seekingForDependesies = false;
            drawLine = false;
            if(seekingButtonIdex == index)
            {
                //delete element from the group and make it separate
                int targetIndex = GetLastElementOfGroup(index);
                linkedLegsId.GetArrayElementAtIndex(seekingButtonIdex).intValue = 0;
                MoveLegIkTargetsElement(index, targetIndex);
                seekingButtonIdex = -1;
                return;
            }

            if(linkedLegsId.GetArrayElementAtIndex(seekingButtonIdex).intValue == 0)
            {
                linkedLegsId.GetArrayElementAtIndex(seekingButtonIdex).intValue = groupCount + 1;
                linkedLegsId.GetArrayElementAtIndex(index).intValue = groupCount + 1;

                LinkArrayElements(seekingButtonIdex, index);
                groupCount ++;
            }
            else
            {
                int newGroupIndex = linkedLegsId.GetArrayElementAtIndex(seekingButtonIdex).intValue;
                linkedLegsId.GetArrayElementAtIndex(index).intValue = newGroupIndex;

                LinkArrayElements(seekingButtonIdex, index);
            }
            seekingButtonIdex = -1;
        }
    }

    private void LinkArrayElements(int SeekingLegIndex, int targetLegIndex)
    {
        if(SeekingLegIndex < targetLegIndex)
        {
            legsIkTargets.MoveArrayElement(targetLegIndex, SeekingLegIndex + 1);
            linkedLegsId.MoveArrayElement(targetLegIndex, SeekingLegIndex + 1);
        }
        else
        {
            legsIkTargets.MoveArrayElement(targetLegIndex, SeekingLegIndex);
            linkedLegsId.MoveArrayElement(targetLegIndex, SeekingLegIndex);
        }
    }

    private void DrawGroupSeparationLine(Color color, Rect rect)
    {
        Handles.color = color;

        Handles.DrawLine(new Vector2(11, rect.y + rect.height + 8), new Vector2(20, rect.y + rect.height + 2));
        Handles.DrawLine(new Vector2(20, rect.y + rect.height + 2), new Vector2(392, rect.y + rect.height + 2));

        Handles.color = Color.white;
    }
    private void DrawVerticalLine(Rect transformRect, Color color)
    {
        Handles.color = color;
        
        Handles.DrawLine(new Vector2(10, transformRect.y), new Vector2(10, transformRect.y + transformRect.height + 5));

        Handles.color = Color.white;
    }
    private void DrawOutline(Rect rect, float thickness, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;

        // Draw top line
        EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Draw bottom line
        EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        // Draw left line
        EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Draw right line
        EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);

        GUI.color = previousColor;
    }
    private void drawConnectingLine()
    {
        if (Event.current.type == EventType.MouseUp && seekingForDependesies)
        {
            seekingForDependesies = false;
            drawLine = false;
            seekingButtonIdex = -1;
        }

        if (!drawLine) return;

        endMousePosition = Event.current.mousePosition;
        Repaint(); 
        Handles.color = new Color(0.00f, 0.930f, 0.915f);
        Handles.BeginGUI();
        Handles.DrawAAPolyLine(startMousePosition, endMousePosition);
        Handles.EndGUI();
        Handles.color = Color.white;
    }
    private void DrawBodyPositionGrhaph(Vector3 start, Vector3 end)
    {
        Handles.DrawLine(new Vector3(start.x, end.y, 0), new Vector3(end.x, end.y, 0), 0);
        Handles.DrawLine(new Vector3(start.x, start.y, 0), new Vector3(end.x, start.y, 0), 0);
        Handles.DrawLine(new Vector3(start.x, end.y - 25, 0), new Vector3(start.x, start.y, 0), 0);

        float k1 = damping.floatValue / (PI * frequency.floatValue);
        float k2 = 1 / ((2 * PI * frequency.floatValue) * (2 * PI * frequency.floatValue));
        float k3 = response.floatValue * damping.floatValue / (2 * PI * frequency.floatValue);

        Vector3 xp = new Vector3(start.x, start.y, 0);
        Vector3 y = new Vector3(start.x, start.y, 0);
        Vector3 yd = Vector3.zero;
        Vector3 yp = new Vector3(start.x, start.y, 0);


        for (float i = 0; i < 94; i += .5f)
        {
            float T = .025f; 
            Vector3 x = new Vector3(start.x + i * 4, end.y, 0); 
            Vector3 xd = Vector3.zero;

            if(xd == Vector3.zero)
            {
                xd = (x - xp) / T;
                xp = x;
            }
            float k2_stable = Max(k2, 1.1f * (T*T/4 + T*k1/2));
            y = y + T * yd;
            yd = yd + T * (x + k3*xd - y - k1*yd) / k2_stable;
            Handles.color = Color.cyan;
            Handles.DrawLine(yp, new Vector3(x.x, y.y, 0), 0); 
            
            yp = new Vector3(x.x, y.y, 0);
        }
    }

    private void FindIkTargets()
    {
        proceduralAnimation proceduralAnimationTarget = (proceduralAnimation)target;

        int nbLegs = proceduralAnimationTarget.GetLegCount();
        
        legsIkTargets.ClearArray();
        legsIkTargets.arraySize = nbLegs;
        linkedLegsId.arraySize = nbLegs;
        manualStepTimings.arraySize = nbLegs;

        for (int i = 0; i < nbLegs; i++)
        {
            Solver2D solver2D = proceduralAnimationTarget.GetIkManager().solvers[i];
            solver2D.transform.localPosition = new Vector3(0, 0, 0);
            solver2D.transform.rotation = Quaternion.Euler(0, 0, 0);
            legsIkTargets.GetArrayElementAtIndex(i).objectReferenceValue = solver2D.GetChain(0).target;
            proceduralAnimationTarget.GetIkManager().solvers[i].GetChain(i).target.name = "target " + i;
        }
    }

    public float[] GetStepTimingsOffset()
    {
        int nbLegs = legsIkTargets.arraySize;
        float[] timings = new float[nbLegs];
        if(customStepTimings.boolValue)
        {
            for (int i = 0; i < nbLegs; i++)
            {
                timings[i] = manualStepTimings.GetArrayElementAtIndex(i).floatValue;
            }   
            return timings;
        } 


        float counter = 1;
        for (int i = 0; i < nbLegs; i++)
        {
           if(i != 0)
           {
                if(linkedLegsId.GetArrayElementAtIndex(i).intValue != linkedLegsId.GetArrayElementAtIndex(i - 1).intValue || linkedLegsId.GetArrayElementAtIndex(i).intValue == 0)
                {
                    counter++;
                }
           }
        }

        float step = stepsInterval.floatValue / counter;
        float multiplyIndex = 1;

        for (int i = 0; i < nbLegs; i++)
        {
            if(i != 0)
            {
                if(linkedLegsId.GetArrayElementAtIndex(i).intValue != linkedLegsId.GetArrayElementAtIndex(i - 1).intValue || linkedLegsId.GetArrayElementAtIndex(i).intValue == 0)
                {
                    timings[i] = multiplyIndex * step;
                    multiplyIndex++;
                }
                else
                {
                    timings[i] = timings[i - 1];
                }
            }
        } 

        return timings;
    } 

}
}