// SC Post Effects
// Staggart Creations
// http://staggart.xyz

using UnityEngine;
using System.IO;

//Make this entire class editor-only with requiring it to be in an "Editor" folder
#if UNITY_EDITOR
using UnityEditor;

#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

namespace SCPE
{
    public class SCPE_Core : Editor
    {
        public static string version = "0.3.0 BETA";
        public static string docURL = "http://staggart.xyz/unity/sc-post-effects/scpe-docs/";
        public static string forumURL = "https://forum.unity.com/threads/513191";

        public const string layerName = "PostProcessing";
        public const string correctBaseFolder = "PostProcessing";

        #region Auto setup
#if UNITY_POST_PROCESSING_STACK_V2
        public static void SetupCamera()
        {
            GameObject mainCamera;

            if (Camera.main) { mainCamera = Camera.main.gameObject; }
            else { mainCamera = FindObjectOfType<Camera>().gameObject; }

            PostProcessLayer ppLayer;

            //Add PostProcessLayer component if not already present
            if (mainCamera.GetComponent<PostProcessLayer>())
            {
                ppLayer = mainCamera.GetComponent<PostProcessLayer>();
            }
            else
            {
                ppLayer = mainCamera.AddComponent<PostProcessLayer>();

                //Enable AA by default
                ppLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
            }

            ppLayer.volumeLayer = LayerMask.GetMask(LayerMask.LayerToName(GetPPLayerID()));

            Selection.objects = new[] { mainCamera };
        }

        //Create a global post processing volume and assign the correct layer and default profile
        public static void SetupGlobalVolume(PostProcessVolume existingVolume = null)
        {
            PostProcessVolume volume = null;
            GameObject volumeObject;

            if (existingVolume)
            {
                volumeObject = volume.gameObject;
                volume = existingVolume;
            }
            else
            {
                volumeObject = new GameObject("Global Post-process Volume");
                volume = volumeObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
            }

            volumeObject.layer = GetPPLayerID();
            volume.isGlobal = true;

            //Find default profile
#if !UNITY_2018_1_OR_NEWER
            string[] assets = AssetDatabase.FindAssets("SC Default Profile t: PostProcessProfile");
#else
			string[] assets = AssetDatabase.FindAssets("SC Default Profile");
#endif
            if (assets.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);

                PostProcessProfile defaultProfile = (PostProcessProfile)AssetDatabase.LoadAssetAtPath(assetPath, typeof(PostProcessProfile));
                volume.sharedProfile = defaultProfile;
            }
            else
            {
                Debug.Log("The default \"SC Post Effects\" profile could not be found. Add a new profile to the volume to get started.");
            }

            Selection.objects = new[] { volumeObject };

        }
#endif

        //Taken from this post: https://forum.unity.com/threads/adding-layer-by-script.41970/#post-2904413
        //Unity community <3
        public static void CreatePPLayer()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            bool ExistLayer = false;

            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);

                if (layerSP.stringValue == layerName)
                {
                    ExistLayer = true;
                    return;
                }

            }
            for (int j = 8; j < layers.arraySize; j++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(j);
                if (layerSP.stringValue == "" && !ExistLayer)
                {
                    layerSP.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();

                    return;
                }
            }

            //Failed
            Debug.LogError("The layer \"PostProcessing\" could not be added, the maximum number of layers (32) has been exceeded");
            EditorApplication.ExecuteMenuItem("Edit/Project Settings/Tags and Layers");

        }

        public static int GetPPLayerID()
        {
            return LayerMask.NameToLayer(layerName);
        }
        #endregion
    }

    //Opens the AboutWindow when this script file is imported
    public class OpenOnImport : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                if (str.Contains("SCPE_Core.cs"))
                {
                    SCPE_Window.ShowWindow();
                }
            }

        }
    }

    public class SCPE_Window : EditorWindow
    {
        //Window properties
        private static int width = 440;
        private static int height = 500;

        private static Texture2D _headerImg;
        public static Texture2D headerImg
        {
            get
            {
                if (_headerImg == null)
                {
                    _headerImg = (Texture2D)AssetDatabase.LoadAssetAtPath(SessionState.GetString("SCPE_HEADERIMG_PATH", ""), typeof(Texture2D));
                }
                return _headerImg;
            }
        }

        //Tabs
        private bool isTabInstallation = true;
        private bool isTabSetup = false;
        private bool isTabGettingStarted = false;
        private bool isTabSupport = false;

        [MenuItem("Help/SC Post Effects", false, 0)]
        public static void ShowWindow()
        {

#if UNITY_5_6_OR_NEWER
            SessionState.SetBool("SCPE_COMPATIBLE_VERSION", true);

#else
            SessionState.SetBool("SCPE_COMPATIBLE_VERSION", false);
#endif

            EditorWindow editorWindow = EditorWindow.GetWindow<SCPE_Window>(false, "SC Post Effects", true);
            editorWindow.titleContent = new GUIContent("SC Post Effects");
            editorWindow.autoRepaintOnSceneChange = true;

            //Open somewhat in the center of the screen
            editorWindow.position = new Rect((Screen.width) / 2f, (Screen.height) / 2f, width, height);

            //Fixed size
            editorWindow.maxSize = new Vector2(width, height);
            editorWindow.minSize = new Vector2(width, 200);

            Init();

            editorWindow.Show();

        }

        private void SetWindowHeight(float height)
        {
            this.maxSize = new Vector2(width, height);
            this.minSize = new Vector2(width, height);
        }

        //Store values in the volatile SessionState
        static void Init()
        {
            //Check PPSv2 installation
#if UNITY_POST_PROCESSING_STACK_V2
            SessionState.SetBool("SCPE_PPS_INSTALLED", true);
#else
            SessionState.SetBool("SCPE_PPS_INSTALLED", false);
#endif

            //Get script path
            string[] res = System.IO.Directory.GetFiles("Assets", "SCPE_Core.cs", SearchOption.AllDirectories);
            string scriptFilePath = res[0];

            //Truncate to get relative path
            SessionState.SetString("SCPE_PATH", scriptFilePath.Replace("\\Core\\SCPE_Core.cs", string.Empty));

            //Debug.Log(SessionState.GetString("SCPE_PATH", ""));

            //Check if SC Post Effects folder is placed inside PostProcessing folder
            bool correctFolder = false;

            //Slashes are handled differently between Windows and macOS apparently
#if UNITY_EDITOR_WIN
            correctFolder = SessionState.GetString("SCPE_PATH", "").Contains(SCPE_Core.correctBaseFolder + "\\SC Post Effects");
#endif

#if UNITY_EDITOR_OSX
            correctFolder = SessionState.GetString("SCPE_PATH", "").Contains(SCPE_Core.correctBaseFolder + "/SC Post Effects");
#endif

            SessionState.SetBool("SCPE_CORRECT_FOLDER", correctFolder);

            //Load banner image
            SessionState.SetString("SCPE_HEADERIMG_PATH", SessionState.GetString("SCPE_PATH", "") + "/Core/Images/SCPE_Banner.png");

            //Check if Linear space lighting
            SessionState.SetBool("SCPE_IS_LINEAR_SPACE", (PlayerSettings.colorSpace == ColorSpace.Linear) ? true : false);

        }

        void OnGUI()
        {

            DrawHeader();

            GUILayout.Space(5);
            DrawTabs();
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (isTabInstallation) DrawInstallation();

            if (isTabSetup) DrawSetup();

            if (isTabGettingStarted) DrawGettingStarted();

            if (isTabSupport) DrawSupport();

            //DrawActionButtons();

            EditorGUILayout.EndVertical();

            DrawFooter();

        }

        void DrawHeader()
        {
            Rect headerRect = new Rect(0, 0, width, height / 6.5f);
            if (headerImg)
            {
                GUI.DrawTexture(headerRect, headerImg, ScaleMode.ScaleToFit);
                GUILayout.Space(headerImg.height / 4 + 60);
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b><size=24>SC Post Effects</size></b>\n<size=16>For Post Processing Stack</size>", Header);
            }

            GUILayout.Label("Version: " + SCPE_Core.version, Footer);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Toggle(isTabInstallation, new GUIContent("Installation", (SessionState.GetBool("SCPE_CORRECT_FOLDER", true) && SessionState.GetBool("SCPE_PPS_INSTALLED", true)) ? smallGreenDot : smallRedDot), Tab))
            {
                isTabInstallation = true;
                isTabSetup = false;
                isTabGettingStarted = false;
                isTabSupport = false;
            }

            EditorGUI.BeginDisabledGroup(!SessionState.GetBool("SCPE_PPS_INSTALLED", true) || !SessionState.GetBool("SCPE_CORRECT_FOLDER", true));
            if (GUILayout.Toggle(isTabSetup, "Setup", Tab))
            {
                isTabInstallation = false;
                isTabSetup = true;
                isTabGettingStarted = false;
                isTabSupport = false;
            }

            if (GUILayout.Toggle(isTabGettingStarted, "Getting started", Tab))
            {
                isTabInstallation = false;
                isTabSetup = false;
                isTabGettingStarted = true;
                isTabSupport = false;
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Toggle(isTabSupport, "Support", Tab))
            {
                isTabInstallation = false;
                isTabSetup = false;
                isTabGettingStarted = false;
                isTabSupport = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        void DrawInstallation()
        {
            if (SessionState.GetBool("SCPE_PPS_INSTALLED", true)) { SetWindowHeight(315f); }
            else { SetWindowHeight(415f); }

            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent(" Compiling scripts...", EditorGUIUtility.FindTexture("cs Script Icon")), Header);

                EditorGUILayout.Space();
                return;
            }

            if (SessionState.GetBool("SCPE_COMPATIBLE_VERSION", false) == false)
            {
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField("This version of Unity is not supported.", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Please upgrade to at least Unity 5.6.1");
                return;
            }

            //Folder
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("SC Post Effects Folder");

            Color defaultColor = GUI.contentColor;
            if (SessionState.GetBool("SCPE_CORRECT_FOLDER", true))
            {
                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField("Inside \"PostProcessing\" folder");
                GUI.contentColor = defaultColor;
            }
            else
            {
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField("Outside \"PostProcessing\" folder", EditorStyles.boldLabel);
                GUI.contentColor = defaultColor;
            }

            EditorGUILayout.EndHorizontal();
            if (!SessionState.GetBool("SCPE_CORRECT_FOLDER", true))
            {
                EditorGUILayout.LabelField("Please move the \"SC Post Effects\" folder back into the \"PostProcessing\" folder!");
                EditorGUILayout.Space();
            }

            //PPSv2
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
#if !UNITY_2018_1_OR_NEWER
            EditorGUILayout.LabelField("Post Processing Stack v2:");
#else
            EditorGUILayout.LabelField("Post Processing");
#endif

            if (SessionState.GetBool("SCPE_PPS_INSTALLED", true))
            {
                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField("Installed");
                GUI.contentColor = defaultColor;
            }
            else
            {
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField("NOT INSTALLED", EditorStyles.boldLabel);
                GUI.contentColor = defaultColor;
            }
            EditorGUILayout.EndHorizontal();

            //Color space
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Color space:");

            if (SessionState.GetBool("SCPE_IS_LINEAR_SPACE", true))
            {
                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField("Linear");
                GUI.contentColor = defaultColor;
            }
            else
            {
                GUI.contentColor = Color.red;
                EditorGUILayout.LabelField("Gamma", EditorStyles.boldLabel);
                GUI.contentColor = defaultColor;
            }
            EditorGUILayout.EndHorizontal();

            if (!SessionState.GetBool("SCPE_IS_LINEAR_SPACE", true))
            {
                EditorGUILayout.LabelField("Linear space is recommend for HDR");
                EditorGUILayout.Space();
            }

            //PPS not installed, display instructions
            if (!SessionState.GetBool("SCPE_PPS_INSTALLED", true))
            {
#if !UNITY_2018_1_OR_NEWER
                EditorGUILayout.LabelField("This package requires the Post Processing Stack v2 to be installed");
#else
                EditorGUILayout.LabelField("This package requires Post Processing package to be installed");
#endif
                EditorGUILayout.Space();


#if !UNITY_2018_1_OR_NEWER
                string buttonLabel = "<b><size=16>Installation instructions</size></b>\n<i>Opens documentation</i>";
#else
                string buttonLabel = "<b><size=16>Install package</size></b>\n<i>Opens package manager</i>";
#endif
                if (GUILayout.Button(buttonLabel, Button))
                {
#if !UNITY_2018_1_OR_NEWER
                    Application.OpenURL(SCPE_Core.docURL + "#getting-started-5");
#else
                    EditorApplication.ExecuteMenuItem("Window/Package Manager");
#endif
                    this.Close();
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Once installed, reopen this window through Help->SC Post Effects");
            }


        }

        void DrawSetup()
        {
            SetWindowHeight(450f);

            bool layerPresent = (SCPE_Core.GetPPLayerID() > 0) ? true : false;

            EditorGUILayout.HelpBox("\nThese actions will automatically configure your project and scene for use with the Post Processing Stack.\n\nThe manual set up steps are outlined in the documentation.\n", MessageType.Info);

            EditorGUILayout.Space();

            //Layer setup
            EditorGUI.BeginDisabledGroup(layerPresent);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Add \"PostProcessing\" layer to project");
            if (GUILayout.Button("Execute")) SCPE_Core.CreatePPLayer();
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            if (layerPresent)
            {
                EditorGUILayout.LabelField("The layer \"PostProcessing\" is present in your project", EditorStyles.helpBox);
            }

            EditorGUILayout.Space();

            //Camera setup
            EditorGUI.BeginDisabledGroup(!layerPresent);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Setup component on Main Camera");
#if UNITY_POST_PROCESSING_STACK_V2
            if (GUILayout.Button("Execute")) SCPE_Core.SetupCamera();
#endif
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            //Volume setup
            EditorGUI.BeginDisabledGroup(!layerPresent);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Setup a new Global Post Processing Volume");
#if UNITY_POST_PROCESSING_STACK_V2
            if (GUILayout.Button("Execute")) SCPE_Core.SetupGlobalVolume();
#endif
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

        }

        void DrawGettingStarted()
        {
            SetWindowHeight(335);

            EditorGUILayout.HelpBox("Please view the documentation for further details about this package and its workings.", MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (SessionState.GetBool("SCPE_PPS_INSTALLED", true))
            {

                if (GUILayout.Button("<b><size=12>Documentation</size></b>\n<i>Usage instructions</i>", Button))
                {
                    Application.OpenURL(SCPE_Core.docURL);
                }
                if (GUILayout.Button("<b><size=12>Effect details</size></b>\n<i>View effect examples</i>", Button))
                {
                    Application.OpenURL(SCPE_Core.docURL + "#effects");
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("This package requires the Post Processing Stack v2 to be installed");
                EditorGUILayout.Space();

                if (GUILayout.Button("<b><size=16>Installation instructions</size></b>\n<i>Opens documentation</i>", Button))
                {
                    Application.OpenURL(SCPE_Core.docURL + "#getting-started-5");
                }
            }
        }

        void DrawSupport()
        {
            SetWindowHeight(350f);

            EditorGUILayout.BeginVertical(); //Support box

            EditorGUILayout.HelpBox("If you have any questions, or ran into issues, please get in touch.\n\nThis package is still in its Beta stage, feedback is greatly appreciated and will help improve it!", MessageType.Info);

            EditorGUILayout.Space();

            //Buttons box
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("<b><size=12>Email</size></b>\n<i>Contact</i>", Button))
            {
                Application.OpenURL("mailto:contact@staggart.xyz");
            }
            if (GUILayout.Button("<b><size=12>Twitter</size></b>\n<i>Follow developments</i>", Button))
            {
                Application.OpenURL("https://twitter.com/search?q=staggart%20creations");
            }
            if (GUILayout.Button("<b><size=12>Forum</size></b>\n<i>Join the discussion</i>", Button))
            {
                Application.OpenURL(SCPE_Core.forumURL);
            }
            EditorGUILayout.EndHorizontal();//Buttons box

            EditorGUILayout.EndVertical(); //Support box
        }

        //TODO: Implement after Beta
        private void DrawActionButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();


            if (GUILayout.Button("<size=12>Rate</size>", Button))
                Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/account/downloads/search=");

            if (GUILayout.Button("<size=12>Review</size>", Button))
                Application.OpenURL("");


            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void DrawFooter()
        {
            //EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
            GUILayout.Label("- Staggart Creations -", Footer);
        }

#region Styles
        private static Texture2D _smallGreenDot;
        public static Texture2D smallGreenDot
        {
            get
            {
                if (_smallGreenDot == null)
                {
                    _smallGreenDot = EditorGUIUtility.FindTexture("d_winbtn_mac_max");
                }

                return _smallGreenDot;
            }
        }

        private static Texture2D _smallRedDot;
        public static Texture2D smallRedDot
        {
            get
            {
                if (_smallRedDot == null)
                {
                    _smallRedDot = EditorGUIUtility.FindTexture("d_winbtn_mac_close_h");
                }

                return _smallRedDot;
            }
        }

        private static GUIStyle _Footer;
        public static GUIStyle Footer
        {
            get
            {
                if (_Footer == null)
                {
                    _Footer = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        wordWrap = true,
                        fontSize = 12
                    };
                }

                return _Footer;
            }
        }

        private static GUIStyle _Button;
        public static GUIStyle Button
        {
            get
            {
                if (_Button == null)
                {
                    _Button = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = true,
                        padding = new RectOffset()
                        {
                            left = 14,
                            right = 14,
                            top = 8,
                            bottom = 8
                        }
                    };
                }

                return _Button;
            }
        }

        private static GUIStyle _Header;
        public static GUIStyle Header
        {
            get
            {
                if (_Header == null)
                {
                    _Header = new GUIStyle(GUI.skin.label)
                    {
                        richText = true,
                        alignment = TextAnchor.MiddleCenter,
                        wordWrap = true,
                        fontSize = 18,
                        fontStyle = FontStyle.Bold
                    };
                }

                return _Header;
            }
        }

        private static Texture _HelpIcon;
        public static Texture HelpIcon
        {
            get
            {
                if (_HelpIcon == null)
                {
                    _HelpIcon = EditorGUIUtility.FindTexture("d_UnityEditor.InspectorWindow");
                }
                return _HelpIcon;
            }
        }


        private static GUIStyle _Tab;
        public static GUIStyle Tab
        {
            get
            {
                if (_Tab == null)
                {
                    _Tab = new GUIStyle(EditorStyles.miniButtonMid)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        stretchWidth = true,
                        richText = true,
                        wordWrap = true,
                        fontSize = 12,
                        fontStyle = FontStyle.Bold,
                        padding = new RectOffset()
                        {
                            left = 14,
                            right = 14,
                            top = 8,
                            bottom = 8
                        }
                    };
                }

                return _Tab;
            }
        }

#endregion //Stylies
    }//SCPE_Window Class
}//namespace
#endif //If Unity Editor