using UnityEditor;
using UnityEngine;

namespace RockStudio
{
    public class SupportWindow : EditorWindow
    {
        private GUILayoutOption bannerHeight;
        private GUIStyle centeredVersionLabel;
        private GUIStyle greyText;
        private GUIStyle publisherNameStyle;
        private GUIStyle reviewBanner;

        private bool stylesNotLoaded = true;
        private GUILayoutOption toolBarHeight;
        private int toolBarIndex;
        private GUIContent[] toolBarOptions;
        private GUIStyle toolBarStyle;

        string colorPublisherName = "666666";
        string colorFooter = "000000";
        string assetName = " RockStudio";

        [MenuItem("Tools/RockStudio/About and Support", false, 3)]
        public static void ShowWindow()
        {
            var myWindow = GetWindow<SupportWindow>("About");
            myWindow.minSize = new Vector2(300, 400);
            myWindow.maxSize = myWindow.minSize;
            myWindow.titleContent = new GUIContent("About");
            myWindow.Show();
        }

        private void OnEnable()
        {
            toolBarOptions = new GUIContent[2];
            toolBarOptions[0] =
                new GUIContent("<size=11><b> Support</b></size>\n <size=11>Get help and talk \n with others.</size>",
                    (Texture2D)Resources.Load("Icons/support"), "");
            toolBarOptions[1] =
                new GUIContent("<size=11><b> Contact</b></size>\n <size=11>Reach out and \n get help.</size>",
                    (Texture2D)Resources.Load("Icons/contact"), "");
            toolBarHeight = GUILayout.Height(100);
            bannerHeight = GUILayout.Height(30);
        }

        private void LoadStyles()
        {
            if (EditorGUIUtility.isProSkin)
            {
                colorPublisherName = "bfbfbf";
                colorFooter = "bfbfbf";
            }

            publisherNameStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            toolBarStyle = new GUIStyle("LargeButtonMid")
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            greyText = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleLeft
            };

            centeredVersionLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };

            reviewBanner = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };

            stylesNotLoaded = false;
        }

        private void OnGUI()
        {
            if (stylesNotLoaded) LoadStyles();

            EditorGUILayout.Space();
            GUILayout.Label(new GUIContent(string.Format("<size=20><b><color=#{0}>{1}</color></b></size>", colorPublisherName, assetName)), publisherNameStyle);

            EditorGUILayout.Space();
           /// toolBarIndex = GUILayout.Toolbar(toolBarIndex, toolBarOptions, toolBarStyle, toolBarHeight);

  
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Support Forum", EditorStyles.label))
                        Application.OpenURL(
                            "https://forum.unity.com/threads/rockstudio-3-procedural-rock-generator.624304/");
                    EditorGUILayout.LabelField("Talk with others.", greyText);

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Documentation", EditorStyles.label))
                        Application.OpenURL("https://alexanderameye.github.io/rockstudio/");
                    EditorGUILayout.LabelField("Detailed documentation and quick-start guides.", greyText);

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Email", EditorStyles.label))
                        Application.OpenURL("mailto:alexanderameye@gmail.com?");
                    EditorGUILayout.LabelField("Get in touch with me.", greyText);

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Twitter", EditorStyles.label))
                        Application.OpenURL("https://twitter.com/alexanderameye");
                    EditorGUILayout.LabelField("See what I'm working on.", greyText);

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("v3.0.0", centeredVersionLabel))
                Application.OpenURL("https://forum.unity.com/threads/rockstudio-3-procedural-rock-generator.624304/");
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent(string.Format("<size=11><color=#{0}> {1}</color></size>", colorFooter, "Please consider leaving us a review."),  (Texture2D)Resources.Load("Icons/award_star"), ""), reviewBanner, bannerHeight))
                Application.OpenURL(
                    "https://www.assetstore.unity3d.com/en/#!/account/downloads/search=RockStudio");
        }

    }
}