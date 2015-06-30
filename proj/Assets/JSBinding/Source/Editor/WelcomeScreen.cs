
    using System;
    using UnityEditor;
    using UnityEngine;

    public class WelcomeScreen : EditorWindow
    {
        private Rect mContactDescriptionRect = new Rect(70f, 344f, 250f, 30f);
        private Rect mContactHeaderRect = new Rect(70f, 324f, 250f, 20f);
        private Texture mContactImage;
        private Rect mContactImageRect = new Rect(15f, 322f, 50f, 50f);
        private Rect mDocDescriptionRect = new Rect(70f, 143f, 250f, 30f);
        private Rect mDocHeaderRect = new Rect(70f, 123f, 250f, 20f);
        private Texture mDocImage;
        private Rect mDocImageRect = new Rect(15f, 124f, 53f, 50f);
        private Rect mForumDescriptionRect = new Rect(70f, 278f, 250f, 30f);
        private Rect mForumHeaderRect = new Rect(70f, 258f, 250f, 20f);
        private Texture mForumImage;
        private Rect mForumImageRect = new Rect(15f, 256f, 50f, 50f);
        private Rect mSamplesDescriptionRect = new Rect(70f, 77f, 250f, 30f);
        private Rect mSamplesHeaderRect = new Rect(70f, 57f, 250f, 20f);
        private Texture mSamplesImage;
        private Rect mSamplesImageRect = new Rect(15f, 58f, 50f, 50f);
        private Rect mToggleButtonRect = new Rect(220f, 385f, 125f, 20f);
        private Rect mVersionRect = new Rect(5f, 385f, 125f, 20f);
        private Rect mVideoDescriptionRect = new Rect(70f, 209f, 250f, 30f);
        private Rect mVideoHeaderRect = new Rect(70f, 189f, 250f, 20f);
        private Texture mVideoImage;
        private Rect mVideoImageRect = new Rect(15f, 190f, 50f, 50f);
        private Rect mWelcomeIntroRect = new Rect(46f, 12f, 306f, 40f);
        private Texture mWelcomeScreenImage;
        private Rect mWelcomeScreenImageRect = new Rect(0f, 0f, 340f, 44f);

        public void OnEnable()
        {
            this.mWelcomeScreenImage =
                EditorGUIUtility.Load("WelcomeScreenHeader.png") as Texture;
                //BehaviorDesignerUtility.LoadTexture("WelcomeScreenHeader.png", false, this);
            this.mSamplesImage = EditorGUIUtility.Load("WelcomeScreenSamplesIcon.png") as Texture;
            this.mDocImage = EditorGUIUtility.Load("WelcomeScreenDocumentationIcon.png") as Texture;
            this.mVideoImage = EditorGUIUtility.Load("WelcomeScreenVideosIcon.png") as Texture;
            this.mForumImage = EditorGUIUtility.Load("WelcomeScreenForumIcon.png") as Texture;
            this.mContactImage = EditorGUIUtility.Load("WelcomeScreenContactIcon.png") as Texture;
        }

        public void OnGUI()
        {
            GUI.DrawTexture(this.mWelcomeScreenImageRect, this.mWelcomeScreenImage);
            GUI.Label(this.mWelcomeIntroRect, "Welcome To JSBinding");
            GUI.DrawTexture(this.mSamplesImageRect, this.mSamplesImage);
            GUI.Label(this.mSamplesHeaderRect, "Samples" );
            GUI.Label(this.mSamplesDescriptionRect, "Download sample projects to get a feel for Behavior Designer.");
            GUI.DrawTexture(this.mDocImageRect, this.mDocImage);
            GUI.Label(this.mDocHeaderRect, "Documentation");
            GUI.Label(this.mDocDescriptionRect, "Browser our extensive online documentation.");
            GUI.DrawTexture(this.mVideoImageRect, this.mVideoImage);
            GUI.Label(this.mVideoHeaderRect, "Videos");
            GUI.Label(this.mVideoDescriptionRect, "Watch our tutorial videos which cover a wide variety of topics.");
            GUI.DrawTexture(this.mForumImageRect, this.mForumImage);
            GUI.Label(this.mForumHeaderRect, "Forums");
            GUI.Label(this.mForumDescriptionRect, "Join the forums!");
            GUI.DrawTexture(this.mContactImageRect, this.mContactImage);
            GUI.Label(this.mContactHeaderRect, "Contact");
            GUI.Label(this.mContactDescriptionRect, "We are here to help.");
            GUI.Label(this.mVersionRect, "Version : " );
            //bool flag = GUI.Toggle(this.mToggleButtonRect, BehaviorDesignerPreferences.GetBool(BDPreferneces.ShowWelcomeScreen), "Show at Startup");
            //if (flag != BehaviorDesignerPreferences.GetBool(BDPreferneces.ShowWelcomeScreen))
            //{
            //    BehaviorDesignerPreferences.SetBool(BDPreferneces.ShowWelcomeScreen, flag);
            //}
            EditorGUIUtility.AddCursorRect(this.mSamplesImageRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mSamplesHeaderRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mSamplesDescriptionRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mDocImageRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mDocHeaderRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mDocDescriptionRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mVideoImageRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mVideoHeaderRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mVideoDescriptionRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mForumImageRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mForumHeaderRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mForumDescriptionRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mContactImageRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mContactHeaderRect, MouseCursor.Link);
            EditorGUIUtility.AddCursorRect(this.mContactDescriptionRect, MouseCursor.Link);
            if (Event.current.type == EventType.MouseUp)
            {
                Vector2 mousePosition = Event.current.mousePosition;
                if ((this.mSamplesImageRect.Contains(mousePosition) || this.mSamplesHeaderRect.Contains(mousePosition)) || this.mSamplesDescriptionRect.Contains(mousePosition))
                {
                    Application.OpenURL("http://www.opsive.com/assets/BehaviorDesigner/samples.php");
                }
                else if ((this.mDocImageRect.Contains(mousePosition) || this.mDocHeaderRect.Contains(mousePosition)) || this.mDocDescriptionRect.Contains(mousePosition))
                {
                    Application.OpenURL("http://www.cnblogs.com/answerwinner/p/4469021.html");
                }
                else if ((this.mVideoImageRect.Contains(mousePosition) || this.mVideoHeaderRect.Contains(mousePosition)) || this.mVideoDescriptionRect.Contains(mousePosition))
                {
                    Application.OpenURL("http://www.opsive.com/assets/BehaviorDesigner/videos.php");
                }
                else if ((this.mForumImageRect.Contains(mousePosition) || this.mForumHeaderRect.Contains(mousePosition)) || this.mForumDescriptionRect.Contains(mousePosition))
                {
                    Application.OpenURL("http://www.opsive.com/forum");
                }
                else if ((this.mContactImageRect.Contains(mousePosition) || this.mContactHeaderRect.Contains(mousePosition)) || this.mContactDescriptionRect.Contains(mousePosition))
                {
                    Application.OpenURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=12");
                }
            }
        }

        [UnityEditor.MenuItem("JSB/Welcome Screen", false, 152)]
        public static void ShowWindow()
        {
            WelcomeScreen window = EditorWindow.GetWindow<WelcomeScreen>(true, "Welcome to JSBinding");
            window.minSize = window.maxSize = new Vector2(340f, 410f);
            UnityEngine.Object.DontDestroyOnLoad(window);
        }
    }


