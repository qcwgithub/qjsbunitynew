using UnityEngine;
//using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace RockAssets
{
    class TestOp
    {
        public TestOp()
        {
            UnityEngine.Debug.Log("Test op-------------");
            Card a = new Card(RANK.ACE, SUIT.DIAMONDS);
            Card b = new Card(RANK.ACE, SUIT.CLUBS);

            UnityEngine.Debug.Log(a > b);
            UnityEngine.Debug.Log(a < b);
            UnityEngine.Debug.Log(a == b);
            UnityEngine.Debug.Log(a != b);
        }
    }


    public enum RANK
    {
        TWO = 2, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING, ACE
    }
    public enum SUIT
    {
        DIAMONDS = 1,//钻石
        CLUBS,//美化
        HEARTS,//栏角
        SPADES//大魁
    }
    /// <summary>
    /// THIS IS THE CLASS THAT STARTED EVERYTHING AND MADE THIS GAME POSSIBLE
    /// </summary>
    public class Card
    {
        private int rank, suit;

        private bool faceUp;
        private bool highlight;
        public bool FaceUp
        {
            get { return faceUp; }
            set
            {
                faceUp = value;

            }
        }
        //default two of diamonds
        public Card()
        {
            rank = (int)RANK.TWO;
            suit = (int)SUIT.DIAMONDS;
            faceUp = false;
            highlight = false;
        }
        public Card(RANK rank, SUIT suit)
        {
            this.rank = (int)rank;
            this.suit = (int)suit;
            faceUp = false;
            highlight = false;
        }
        public Card(int rank, int suit)
        {
            if (rank < 1 || rank > 14 || suit < 1 || suit > 4)
                throw new ArgumentOutOfRangeException();
            this.rank = rank;
            this.suit = suit;
            faceUp = false;
            highlight = false;
        }
        public Card(RANK rank, SUIT suit, bool faceUp)
        {
            this.rank = (int)rank;
            this.suit = (int)suit;
            this.faceUp = faceUp;
            highlight = false;
        }
        public Card(int rank, int suit, bool faceUp)
        {
            if (rank < 1 || rank > 14 || suit < 1 || suit > 4)
                throw new ArgumentOutOfRangeException();
            this.rank = rank;
            this.suit = suit;
            this.faceUp = faceUp;
            highlight = false;
        }
        public Card(Card card)
        {
            this.rank = card.rank;
            this.suit = card.suit;
            this.faceUp = card.faceUp;
            highlight = false;
        }
        public static string rankToString(int rank)
        {
            switch (rank)
            {
                case 11:
                    return "Jack";
                case 12:
                    return "Queen";
                case 13:
                    return "King";
                case 14:
                    return "Ace";
                default:
                    return rank.ToString();
            }
        }
        public static string suitToString(int suit)
        {
            switch (suit)
            {
                case 1:
                    return "Diamonds";
                case 2:
                    return "Clubs";
                case 3:
                    return "Hearts";
                default:
                    return "Spades";
            }
        }
        public int getRank()
        {
            return rank;
        }
        public int getSuit()
        {
            return suit;
        }

        public void setRank(RANK rank)
        {
            this.rank = (int)rank;
        }
        public void setCard(RANK rank, SUIT suit)
        {
            this.rank = (int)rank;
            this.suit = (int)suit;
        }
        public void setCard(int rank, int suit)
        {
            if (rank < 1 || rank > 14 || suit < 1 || suit > 4)
                throw new ArgumentOutOfRangeException();
            this.rank = rank;
            this.suit = suit;
        }
        public override string ToString()
        {
            if (faceUp == true)
                return rankToString(rank) + " of " + suitToString(suit);
            return "The card is facedown, you cannot see it!";
        }

        //extract green channel from image to highlight image
        public void Highlight()
        {

        }
        //reload original image to unhighlight
        public void UnHighlight()
        {

        }
        public bool isHighlighted()
        {
            return this.highlight;
        }

        //compare rank of cards
        public static bool operator ==(Card a, Card b)
        {
            if (a.rank == b.rank)
                return true;
            else
                return false;
        }
        public static bool operator !=(Card a, Card b)
        {
            if (a.rank != b.rank)
                return true;
            else
                return false;
        }
        public static bool operator <(Card a, Card b)
        {
            if (a.rank < b.rank)
                return true;
            else
                return false;
        }
        public static bool operator >(Card a, Card b)
        {
            if (a.rank > b.rank)
                return true;
            else
                return false;
        }
        public static bool operator <=(Card a, Card b)
        {
            if (a.rank <= b.rank)
                return true;
            else
                return false;
        }
        public static bool operator >=(Card a, Card b)
        {
            if (a.rank >= b.rank)
                return true;
            else
                return false;
        }
    }
}


public class Kekoukele
{
    public delegate void MyFun(int v);
    public MyFun fun;
    public int instValue;
    public static int staticValue;
    public void emptyCall()
    {

    }
    public void callWithInt(int i)
    {

    }
    public void callWithDelegate(MyFun mf)
    {

    }
    public void getValue(out int v) { 
		v = 899; 
	}
    public int[] getArr()
    {
        return new int[] { 5,6,8};
    }
    public void inputIntArr(int[] a)
    {
        if (a == null)
        {
            Debug.Log("a is null");
        }
        else
        {
            string s = string.Empty;
            foreach (var v in a)
            {
                s += v.ToString() + " ";
            }
            Debug.Log(s);
        }
    }
    public void inputGameObjectArr(GameObject[] a)
    {
        if (a == null)
        {
            Debug.Log("a is null");
        }
        else
        {
            string s = string.Empty;
            foreach (var v in a)
            {
                s += v.ToString() + " ";
            }
            Debug.Log(s);
        }
    }
}

public class FanXing<T,U>
{
    public void Print<T,U>(T t)
    {
        Debug.Log(t.ToString());
    }
    public void ShowList(List<T> l)
    {


    }
}

public class XueBi
{
    public delegate bool A();
    public static void hA(A a) {
        Debug.Log("a = " + a().ToString());
    }

    public delegate string B();
    public static void hB(B b)
    {
        Debug.Log("b = " + b().ToString());
    }

    public delegate GameObject C();
    public static void hC(C c)
    {
        Debug.Log("c = " + c().name);
    }

    public delegate Vector3 D();
    public static void hD(D d)
    {
        Debug.Log("d = " + d().ToString());
    }


    public delegate float E();
    public static void hE(E e)
    {
        Debug.Log("e = " + e().ToString());
    }


    public delegate int[] F();
    public static void hF(F f)
    {
        string s = "[";
        int[] arr = f();
        foreach (var v in arr)
        {
            s += v.ToString()+",";
        }
        s += "]";
        Debug.Log("f = " + s);
    }

    public delegate string G(string s);
    public static void hG(G g, string s)
    {
        string str = g(s);
        Debug.Log("g = " + str);
    }

    public static void hH(string s)
    {

    }
}

public class JSBindingSettings
{
    public static Type[] enums = new Type[]
    {
        /*
         * Nothing to export for demo
         * Add enums here as you need.
         */

	    /*
        
        typeof(UnityEngine.FilterMode),
        typeof(UnityEngine.TextureWrapMode),
        typeof(UnityEngine.NPOTSupport),
        typeof(UnityEngine.TextureFormat),
        typeof(UnityEngine.CubemapFace),
        typeof(UnityEngine.RenderTextureFormat),
        typeof(UnityEngine.RenderTextureReadWrite),
        typeof(UnityEngine.Rendering.BlendMode),
        typeof(UnityEngine.Rendering.BlendOp),
        typeof(UnityEngine.Rendering.CompareFunction),
        typeof(UnityEngine.Rendering.CullMode),
        typeof(UnityEngine.Rendering.ColorWriteMask),
        typeof(UnityEngine.Rendering.StencilOp),
        typeof(UnityEngine.SocialPlatforms.UserState),
        typeof(UnityEngine.SocialPlatforms.UserScope),
        typeof(UnityEngine.SocialPlatforms.TimeScope),
        typeof(UnityEngineInternal.TypeInferenceRules),
        typeof(UnityEngine.ForceMode),
        typeof(UnityEngine.RigidbodyConstraints),
        typeof(UnityEngine.RigidbodyInterpolation),
        typeof(UnityEngine.JointDriveMode),
        typeof(UnityEngine.JointProjectionMode),
        typeof(UnityEngine.ConfigurableJointMotion),
        typeof(UnityEngine.RotationDriveMode),
        typeof(UnityEngine.CollisionDetectionMode),
        typeof(UnityEngine.PhysicMaterialCombine),
        typeof(UnityEngine.CollisionFlags),
        typeof(UnityEngine.RigidbodyInterpolation2D),
        typeof(UnityEngine.RigidbodySleepMode2D),
        typeof(UnityEngine.CollisionDetectionMode2D),
        typeof(UnityEngine.ForceMode2D),
        typeof(UnityEngine.JointLimitState2D),
        typeof(UnityEngine.ObstacleAvoidanceType),
        typeof(UnityEngine.OffMeshLinkType),
        typeof(UnityEngine.NavMeshPathStatus),
        typeof(UnityEngine.AudioSpeakerMode),
        typeof(UnityEngine.AudioType),
        typeof(UnityEngine.AudioVelocityUpdateMode),
        typeof(UnityEngine.FFTWindow),
        typeof(UnityEngine.AudioRolloffMode),
        typeof(UnityEngine.AudioReverbPreset),
        typeof(UnityEngine.WebCamFlags),
        typeof(UnityEngine.WrapMode),
        typeof(UnityEngine.PlayMode),
        typeof(UnityEngine.QueueMode),
        typeof(UnityEngine.AnimationBlendMode),
        typeof(UnityEngine.AnimationPlayMode),
        typeof(UnityEngine.AnimationCullingType),
        typeof(UnityEngine.AvatarTarget),
        typeof(UnityEngine.AvatarIKGoal),
        typeof(UnityEngine.AnimatorCullingMode),
        typeof(UnityEngine.AnimatorUpdateMode),
        typeof(UnityEngine.HumanBodyBones),
        typeof(UnityEngine.DetailRenderMode),
        typeof(UnityEngine.TerrainRenderFlags),
        typeof(UnityEngine.HideFlags),
        typeof(UnityEngine.SendMessageOptions),
        typeof(UnityEngine.PrimitiveType),
        typeof(UnityEngine.Space),
        typeof(UnityEngine.RuntimePlatform),
        typeof(UnityEngine.SystemLanguage),
        typeof(UnityEngine.LogType),
        typeof(UnityEngine.DeviceType),
        typeof(UnityEngine.ThreadPriority),
        typeof(UnityEngine.CursorMode),
        typeof(UnityEngine.LightType),
        typeof(UnityEngine.LightRenderMode),
        typeof(UnityEngine.LightShadows),
        typeof(UnityEngine.FogMode),
        
        typeof(UnityEngine.ShadowProjection),
        typeof(UnityEngine.CameraClearFlags),
        typeof(UnityEngine.DepthTextureMode),
        typeof(UnityEngine.TexGenMode),
        typeof(UnityEngine.AnisotropicFiltering),
        typeof(UnityEngine.BlendWeights),
        //typeof(UnityEngine.TextureCompressionQuality),
        typeof(UnityEngine.MeshTopology),
        typeof(UnityEngine.SkinQuality),
        typeof(UnityEngine.ParticleRenderMode),
        typeof(UnityEngine.LightmapsMode),
        typeof(UnityEngine.ColorSpace),
        typeof(UnityEngine.ScreenOrientation),
        typeof(UnityEngine.TextAlignment),
        typeof(UnityEngine.TextAnchor),
        typeof(UnityEngine.ScaleMode),
        typeof(UnityEngine.FocusType),
        typeof(UnityEngine.FontStyle),
        typeof(UnityEngine.TextWrapMode),
        typeof(UnityEngine.ImagePosition),
        typeof(UnityEngine.TextClipping),
        //typeof(UnityEngine.FullScreenMovieControlMode),
        //typeof(UnityEngine.FullScreenMovieScalingMode),
        //typeof(UnityEngine.iOSActivityIndicatorStyle),
        //typeof(UnityEngine.AndroidActivityIndicatorStyle),
        //typeof(UnityEngine.TouchScreenKeyboardType),
        //typeof(UnityEngine.iPhoneGeneration),
        typeof(UnityEngine.KeyCode),
        typeof(UnityEngine.EventType),
        typeof(UnityEngine.EventModifiers),
        //typeof(UnityEngine.iPhoneTouchPhase),
        //typeof(UnityEngine.iPhoneOrientation),
        //typeof(UnityEngine.iPhoneScreenOrientation),
        //typeof(UnityEngine.iPhoneKeyboardType),
        //typeof(UnityEngine.iPhoneMovieControlMode),
        //typeof(UnityEngine.iPhoneMovieScalingMode),
        //typeof(UnityEngine.iPhoneNetworkReachability),
        //typeof(UnityEngine.CalendarIdentifier),
        //typeof(UnityEngine.CalendarUnit),
        //typeof(UnityEngine.RemoteNotificationType),
        typeof(UnityEngine.RPCMode),
        typeof(UnityEngine.ConnectionTesterStatus),
        typeof(UnityEngine.NetworkConnectionError),
        typeof(UnityEngine.NetworkDisconnection),
        typeof(UnityEngine.MasterServerEvent),
        typeof(UnityEngine.NetworkStateSynchronization),
        typeof(UnityEngine.NetworkPeerType),
        typeof(UnityEngine.NetworkLogLevel),
        typeof(UnityEngine.ParticleSystemRenderMode),
        typeof(UnityEngine.ParticleSystemSimulationSpace),
        typeof(UnityEngine.ProceduralProcessorUsage),
        typeof(UnityEngine.ProceduralCacheSize),
        typeof(UnityEngine.ProceduralLoadingBehavior),
        typeof(UnityEngine.ProceduralPropertyType),
        typeof(UnityEngine.ProceduralOutputType),
        typeof(UnityEngine.SpriteAlignment),
        typeof(UnityEngine.SpritePackingMode),
        typeof(UnityEngine.SpritePackingRotation),
        typeof(UnityEngine.SpriteMeshType),
        typeof(UnityEngine.NetworkReachability),
        typeof(UnityEngine.UserAuthorization),
        typeof(UnityEngine.RenderingPath),
        typeof(UnityEngine.TransparencySortMode),
        typeof(UnityEngine.ComputeBufferType),
        typeof(UnityEngine.TouchPhase),
        typeof(UnityEngine.IMECompositionMode),
        typeof(UnityEngine.DeviceOrientation),
        typeof(UnityEngine.LocationServiceStatus),

        // Obsolete
        // typeof(UnityEngine.QualityLevel),
         
         
        typeof(SeekOrigin), 		
		*/       
    };

    public static Type[] classes2 = new Type[]
    {
        typeof(List<>), 
    };
    public static Type[] classes = new Type[]
    {
        /*
         * Classes to export for demo
         * Add classes here to export
         */

         //interface

//         typeof(UnityEngine.SocialPlatforms.ISocialPlatform), 
//         typeof(UnityEngine.SocialPlatforms.ILocalUser),
//         typeof(UnityEngine.SocialPlatforms.IUserProfile),
//         typeof(UnityEngine.SocialPlatforms.IAchievement),
//         typeof(UnityEngine.SocialPlatforms.IAchievementDescription),
//         typeof(UnityEngine.SocialPlatforms.IScore),
//         typeof(UnityEngine.SocialPlatforms.ILeaderboard),
//         typeof(UnityEngine.ISerializationCallbackReceiver),

         //class

 //       typeof(UnityEngine.SocialPlatforms.Impl.LocalUser),
 //       typeof(UnityEngine.SocialPlatforms.Impl.UserProfile),
 //       typeof(UnityEngine.SocialPlatforms.Impl.Achievement),
 //       typeof(UnityEngine.SocialPlatforms.Impl.AchievementDescription),
 //       typeof(UnityEngine.SocialPlatforms.Impl.Score),
 //       typeof(UnityEngine.SocialPlatforms.Impl.Leaderboard),
 //       typeof(UnityEngine.SocialPlatforms.Local),

        typeof(UnityEngine.Security),

//        typeof(UnityEngine.StackTraceUtility),

        typeof(UnityEngine.UnityException),
        typeof(UnityEngine.MissingComponentException),
        typeof(UnityEngine.UnassignedReferenceException),
        typeof(UnityEngine.MissingReferenceException),
        typeof(UnityEngine.TextEditor),

        typeof(UnityEngine.TextGenerator),
        typeof(UnityEngine.TrackedReference),
        typeof(UnityEngine.WWW),


        typeof(UnityEngine.Serialization.UnitySurrogateSelector),
//        typeof(UnityEngineInternal.GenericStack),
        typeof(UnityEngine.Physics),
        typeof(UnityEngine.Rigidbody),
        typeof(UnityEngine.Joint),
        typeof(UnityEngine.HingeJoint),
        typeof(UnityEngine.SpringJoint),
        typeof(UnityEngine.FixedJoint),
        typeof(UnityEngine.CharacterJoint),
        typeof(UnityEngine.ConfigurableJoint),
        typeof(UnityEngine.ConstantForce),
        typeof(UnityEngine.Collider),
        typeof(UnityEngine.BoxCollider),
        typeof(UnityEngine.SphereCollider),
        typeof(UnityEngine.MeshCollider),
        typeof(UnityEngine.CapsuleCollider),
        
        typeof(UnityEngine.WheelCollider),
        typeof(UnityEngine.PhysicMaterial),
        typeof(UnityEngine.Collision),
        typeof(UnityEngine.ControllerColliderHit),
        typeof(UnityEngine.CharacterController),
        typeof(UnityEngine.Cloth),
        typeof(UnityEngine.InteractiveCloth),
        typeof(UnityEngine.SkinnedCloth),
        typeof(UnityEngine.ClothRenderer),
        typeof(UnityEngine.TerrainCollider),
        typeof(UnityEngine.Physics2D),
        typeof(UnityEngine.Rigidbody2D),
        typeof(UnityEngine.Collider2D),
        typeof(UnityEngine.CircleCollider2D),
        typeof(UnityEngine.BoxCollider2D),
        typeof(UnityEngine.EdgeCollider2D),
        typeof(UnityEngine.PolygonCollider2D),
        typeof(UnityEngine.Collision2D),
        typeof(UnityEngine.Joint2D),
        typeof(UnityEngine.AnchoredJoint2D),
        typeof(UnityEngine.SpringJoint2D),
        typeof(UnityEngine.DistanceJoint2D),
        typeof(UnityEngine.HingeJoint2D),

        typeof(UnityEngine.SliderJoint2D),
        typeof(UnityEngine.WheelJoint2D),
        typeof(UnityEngine.PhysicsMaterial2D),
        typeof(UnityEngine.NavMeshAgent),
        typeof(UnityEngine.NavMesh),
        typeof(UnityEngine.OffMeshLink),
        typeof(UnityEngine.NavMeshPath),
        typeof(UnityEngine.NavMeshObstacle),
        typeof(UnityEngine.AudioSettings),

        typeof(UnityEngine.AudioClip),
        typeof(UnityEngine.AudioListener),
        typeof(UnityEngine.AudioSource),
        typeof(UnityEngine.AudioReverbZone),
        typeof(UnityEngine.AudioLowPassFilter),

        typeof(UnityEngine.AudioHighPassFilter),
        typeof(UnityEngine.AudioDistortionFilter),
        typeof(UnityEngine.AudioEchoFilter),
        typeof(UnityEngine.AudioChorusFilter),
        typeof(UnityEngine.AudioReverbFilter),
        typeof(UnityEngine.Microphone),
        typeof(UnityEngine.WebCamTexture),
        typeof(UnityEngine.AnimationClipPair),
//        typeof(UnityEngine.AnimatorOverrideController),
        typeof(UnityEngine.AnimationEvent),
        typeof(UnityEngine.AnimationClip),
        typeof(UnityEngine.AnimationCurve),
        typeof(UnityEngine.Animation),
        typeof(UnityEngine.AnimationState),
        typeof(UnityEngine.GameObject),
        typeof(UnityEngine.Animator),
        typeof(UnityEngine.AvatarBuilder),
        typeof(UnityEngine.RuntimeAnimatorController),
        typeof(UnityEngine.Avatar),
        typeof(UnityEngine.HumanTrait),
        typeof(UnityEngine.TreePrototype),
        typeof(UnityEngine.DetailPrototype),
        typeof(UnityEngine.SplatPrototype),
//        typeof(UnityEngine.TerrainData),
//        typeof(UnityEngine.Terrain),
//        typeof(UnityEngine.Tree),                                              
        typeof(UnityEngine.AssetBundleCreateRequest),                          
        typeof(UnityEngine.AssetBundleRequest),                                
        typeof(UnityEngine.AssetBundle),                                       
        typeof(UnityEngine.SystemInfo),                                        
        typeof(UnityEngine.WaitForSeconds),                                    
        typeof(UnityEngine.WaitForFixedUpdate),                                
        typeof(UnityEngine.WaitForEndOfFrame),                                 
        typeof(UnityEngine.Coroutine),     
        // attributes                            
        // typeof(UnityEngine.DisallowMultipleComponent),                         
        //typeof(UnityEngine.RequireComponent),                                  
        //typeof(UnityEngine.AddComponentMenu),                                  
        //typeof(UnityEngine.ContextMenu),                                       
        //typeof(UnityEngine.ExecuteInEditMode),                                 
        //typeof(UnityEngine.HideInInspector),                                   
        typeof(UnityEngine.ScriptableObject),                                  
        typeof(UnityEngine.Resources),                                         
        typeof(UnityEngine.Profiler),                                          
        typeof(UnityEngineInternal.Reproduction),                              
        typeof(UnityEngine.CrashReport),                                       
        typeof(UnityEngine.Cursor),                                            
        typeof(UnityEngine.OcclusionArea),                                     
        typeof(UnityEngine.OcclusionPortal),                                   
        typeof(UnityEngine.RenderSettings),                                    
        typeof(UnityEngine.QualitySettings),                                   
        typeof(UnityEngine.MeshFilter),                                        
        typeof(UnityEngine.Mesh),                                              
        typeof(UnityEngine.SkinnedMeshRenderer),                               
        typeof(UnityEngine.Flare),                                             
        typeof(UnityEngine.LensFlare),                                         
        typeof(UnityEngine.Renderer),                                          
        typeof(UnityEngine.Projector),                                         
        typeof(UnityEngine.Skybox),                                            
        typeof(UnityEngine.TextMesh),                                          
        typeof(UnityEngine.ParticleEmitter),                                   
        typeof(UnityEngine.ParticleAnimator),                                  
        typeof(UnityEngine.TrailRenderer),                                     
        typeof(UnityEngine.ParticleRenderer),                                  
        typeof(UnityEngine.LineRenderer),                                      
        typeof(UnityEngine.MaterialPropertyBlock),                             
        typeof(UnityEngine.Graphics),                                          
        typeof(UnityEngine.LightmapData),                                      
        typeof(UnityEngine.LightProbes),                                       
        typeof(UnityEngine.LightmapSettings),                                  
        typeof(UnityEngine.GeometryUtility),                                   
        typeof(UnityEngine.Screen),                                            
        typeof(UnityEngine.SleepTimeout),                                      
        typeof(UnityEngine.GL),                                                
        typeof(UnityEngine.MeshRenderer),                                      
        typeof(UnityEngine.StaticBatchingUtility),                             
        //typeof(UnityEngine.ImageEffectTransformsToLDR),                        
        //typeof(UnityEngine.ImageEffectOpaque),                                 
        typeof(UnityEngine.Texture),                                           
//        typeof(UnityEngine.Texture2D),                                         
        typeof(UnityEngine.Cubemap),                                           
        typeof(UnityEngine.Texture3D),                                         
        typeof(UnityEngine.SparseTexture),                                     
        typeof(UnityEngine.RenderTexture),                                     
        typeof(UnityEngine.GUIElement),                                        
        typeof(UnityEngine.GUITexture),                                        
        typeof(UnityEngine.GUIText),                                           
        typeof(UnityEngine.Font),                                              
        typeof(UnityEngine.GUILayer),                                          
        typeof(UnityEngine.LODGroup),                                          
        typeof(UnityEngine.Gradient),                                          
        typeof(UnityEngine.GUI),
        typeof(UnityEngine.GUILayout),
        typeof(UnityEngine.GUILayoutUtility),                                    
        typeof(UnityEngine.GUILayoutOption),                                     
        typeof(UnityEngine.ExitGUIException),
        typeof(UnityEngine.GUIUtility),
        typeof(UnityEngine.GUISettings),
        typeof(UnityEngine.GUISkin),
        typeof(UnityEngine.GUIContent),
        typeof(UnityEngine.GUIStyleState),
        typeof(UnityEngine.RectOffset),
        typeof(UnityEngine.GUIStyle),                     
        typeof(UnityEngine.Event),                                   
        typeof(UnityEngine.Gizmos),                    
        typeof(UnityEngine.LightProbeGroup),                         
        typeof(UnityEngine.Ping),                                    
        typeof(UnityEngine.NetworkView),                             
        typeof(UnityEngine.Network),                                 
        typeof(UnityEngine.BitStream),                               
        //typeof(UnityEngine.RPC),                                     
        typeof(UnityEngine.HostData),                                
        typeof(UnityEngine.MasterServer),                            
        typeof(UnityEngine.ParticleSystem),                          
        typeof(UnityEngine.ParticleSystemRenderer),                  
        typeof(UnityEngine.TextAsset),                               
                    
        //typeof(UnityEngine.SerializeField),                          
        typeof(UnityEngine.Shader),                                  
        typeof(UnityEngine.Material),                                
        typeof(UnityEngine.ProceduralPropertyDescription),           
        typeof(UnityEngine.ProceduralMaterial),                      
        typeof(UnityEngine.ProceduralTexture),                       
        typeof(UnityEngine.Sprite),                                  
        typeof(UnityEngine.SpriteRenderer),                          
        typeof(UnityEngine.Sprites.DataUtility),                     
        typeof(UnityEngine.WWWForm),                                 
        typeof(UnityEngine.Caching),                                 
        typeof(UnityEngine.AsyncOperation),                          
        typeof(UnityEngine.Application),                             
        typeof(UnityEngine.Behaviour),                               
        typeof(UnityEngine.Camera),                                  
        typeof(UnityEngine.ComputeShader),                           
        typeof(UnityEngine.ComputeBuffer),                           
        typeof(UnityEngine.Debug),                                   
        typeof(UnityEngine.Display),
        typeof(UnityEngine.Flash.ActionScript),                      
        typeof(UnityEngine.Flash.FlashPlayer),                       
        typeof(UnityEngine.MonoBehaviour),                           
        typeof(UnityEngine.Gyroscope),                               
        typeof(UnityEngine.LocationService),                         
        typeof(UnityEngine.Compass),                                 
        typeof(UnityEngine.Input),                                   
        typeof(UnityEngine.Object),
        typeof(UnityEngine.Component),                              
        typeof(UnityEngine.Light),                                  
        typeof(UnityEngine.Transform),                              
        typeof(UnityEngine.Time),                                   
        typeof(UnityEngine.Random),
        typeof(UnityEngine.YieldInstruction),
        typeof(UnityEngine.PlayerPrefsException),
        typeof(UnityEngine.PlayerPrefs),                               
        
         //ValueType

        typeof(UnityEngine.SocialPlatforms.Range),             
        typeof(UnityEngine.TextGenerationSettings),            
        typeof(UnityEngine.JointMotor),                        
        typeof(UnityEngine.JointSpring),                       
        typeof(UnityEngine.JointLimits),                       
        typeof(UnityEngine.SoftJointLimit),                    
        typeof(UnityEngine.JointDrive),                        
        typeof(UnityEngine.WheelFrictionCurve),                
        typeof(UnityEngine.WheelHit),                          
        typeof(UnityEngine.RaycastHit),                        
        typeof(UnityEngine.ContactPoint),                      
        typeof(UnityEngine.ClothSkinningCoefficient),          
        typeof(UnityEngine.RaycastHit2D),                      
        typeof(UnityEngine.ContactPoint2D),                    
        typeof(UnityEngine.JointAngleLimits2D),                
        typeof(UnityEngine.JointTranslationLimits2D),          
        typeof(UnityEngine.JointMotor2D),                      
        typeof(UnityEngine.JointSuspension2D),                 
        typeof(UnityEngine.OffMeshLinkData),                
        typeof(UnityEngine.NavMeshHit),                     
        typeof(UnityEngine.NavMeshTriangulation),           
        typeof(UnityEngine.WebCamDevice),                   
        typeof(UnityEngine.Keyframe),                       
        typeof(UnityEngine.AnimationInfo),                  
        typeof(UnityEngine.AnimatorStateInfo),              
        typeof(UnityEngine.AnimatorTransitionInfo),         
        typeof(UnityEngine.MatchTargetWeightMask),          
        typeof(UnityEngine.SkeletonBone),                   
        typeof(UnityEngine.HumanLimit),                     
        typeof(UnityEngine.HumanBone),                      
        typeof(UnityEngine.HumanDescription),               
        typeof(UnityEngine.TreeInstance),                   
        typeof(UnityEngine.UIVertex),                       
        typeof(UnityEngine.LayerMask),                      
        typeof(UnityEngine.CombineInstance),                
        typeof(UnityEngine.BoneWeight),                     
        typeof(UnityEngine.Particle),                       
        typeof(UnityEngine.RenderBuffer),                   
        typeof(UnityEngine.Resolution),                     
        typeof(UnityEngine.CharacterInfo),                  
        typeof(UnityEngine.UICharInfo),                     
        typeof(UnityEngine.UILineInfo),                     
        typeof(UnityEngine.LOD),                            
        typeof(UnityEngine.GradientColorKey),               
        typeof(UnityEngine.GradientAlphaKey),                 
        typeof(UnityEngine.Vector2),                        
        typeof(UnityEngine.Vector3),                    
        typeof(UnityEngine.Color),                       
        typeof(UnityEngine.Color32),                     
        typeof(UnityEngine.Quaternion),                  
        typeof(UnityEngine.Rect),                        
        typeof(UnityEngine.Matrix4x4),                   
        typeof(UnityEngine.Bounds),                      
        typeof(UnityEngine.Vector4),                     
        typeof(UnityEngine.Ray),                         
        typeof(UnityEngine.Ray2D),                       
        typeof(UnityEngine.Plane),                       
        typeof(UnityEngine.Mathf),                       
        typeof(UnityEngine.NetworkPlayer),               
        typeof(UnityEngine.NetworkViewID),               
        typeof(UnityEngine.NetworkMessageInfo),          
        
        typeof(UnityEngine.Touch),                       
        typeof(UnityEngine.AccelerationEvent),           
        typeof(UnityEngine.LocationInfo), 
   
        //////////////////////////////////////////////////////
        // types not from UnityEngine
        typeof(System.Diagnostics.Stopwatch),

        typeof(Kekoukele),

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN) && (!UNITY_IPHONE && !UNITY_ANDROID)
        typeof(UnityEngine.MovieTexture),
#endif

        //////////////////////////////////////////////////////
        // iPhone only
#if UNITY_IPHONE
                         
                                          
        typeof(UnityEngine.iPhoneInput),                             
        typeof(UnityEngine.iPhoneSettings),                          
                                  
        typeof(UnityEngine.iPhoneUtils),                             
        typeof(UnityEngine.LocalNotification),                       
        typeof(UnityEngine.RemoteNotification),                      
        typeof(UnityEngine.NotificationServices),
        typeof(UnityEngine.iPhone),
        typeof(UnityEngine.ADBannerView),
        typeof(UnityEngine.ADInterstitialAd),
#endif

        //////////////////////////////////////////////////////
        // Android only
#if UNITY_ANDROID
        typeof(UnityEngine.AndroidJNIHelper),                         
        typeof(UnityEngine.AndroidJNI),                               
        typeof(UnityEngine.AndroidInput),
        typeof(UnityEngine.AndroidJavaException), 
        typeof(UnityEngine.AndroidJavaProxy),
        typeof(UnityEngine.AndroidJavaObject),
        typeof(UnityEngine.AndroidJavaClass),
#endif
        //////////////////////////////////////////////////////
        // iPhone and Android
#if UNITY_ANDROID || UNITY_IPHONE
        typeof(UnityEngine.TouchScreenKeyboard),
#endif
        //////////////////////////////////////////////////////
        //
        // Obsolete!!
        //
//         typeof(UnityEngine.RaycastCollider),
//         typeof(UnityEngine.SerializePrivateVariables),   
//         typeof(UnityEngine.CacheIndex),                  
//         typeof(UnityEngine.iPhoneTouch),   
//         typeof(UnityEngine.iPhoneAccelerationEvent), 
//         typeof(UnityEngine.iPhoneKeyboard),



        //////////////////////////////////////////////////////
        // attributes
//         typeof(AOT.MonoPInvokeCallbackAttribute),
//         typeof(UnityEngine.PropertyAttribute),
//         typeof(UnityEngine.ContextMenuItemAttribute),
//         typeof(UnityEngine.TooltipAttribute),
//         typeof(UnityEngine.SpaceAttribute),
//         typeof(UnityEngine.HeaderAttribute),
//         typeof(UnityEngine.RangeAttribute),
//         typeof(UnityEngine.MultilineAttribute),
//         typeof(UnityEngine.TextAreaAttribute),
//         typeof(UnityEngine.ThreadSafeAttribute),
//         typeof(UnityEngine.ConstructorSafeAttribute),
//         typeof(UnityEngine.AssemblyIsEditorAssembly),        
//         typeof(UnityEngine.ImplementedInActionScriptAttribute),
//         typeof(UnityEngine.SelectionBaseAttribute),
//         typeof(UnityEngineInternal.TypeInferenceRuleAttribute),
//         typeof(UnityEngine.Internal.DefaultValueAttribute),
//         typeof(UnityEngine.Internal.ExcludeFromDocsAttribute),                   
//         typeof(UnityEngine.NotConvertedAttribute),                   
//         typeof(UnityEngine.NotFlashValidatedAttribute),              
//         typeof(UnityEngine.NotRenamedAttribute),  

        // typeof(UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform), //Action<>..... oh, manually add a namespace can solve this

        //////////////////////////////////////////////////////
        // delegates
        // typeof(UnityEngine.AndroidJavaRunnable),
        // typeof(UnityEngine.Events.UnityAction),
        // typeof(UnityEngineInternal.FastCallExceptionHandler),
        // typeof(UnityEngineInternal.GetMethodDelegate),

        //////////////////////////////////////////////////////
        // static classes
        // typeof(UnityEngine.Types),
        // typeof(UnityEngine.Social),

        //////////////////////////////////////////////////////
        // not exist (why? see IsDiscard)
        typeof(UnityEngine.Motion),

        //typeof(UnityEngine.SamsungTV),









        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Unity 4.6.2
        //
        //
        //

#if UNITY_4_6
        
        // interface

        typeof(UnityEngine.EventSystems.IEventSystemHandler),
        typeof(UnityEngine.EventSystems.IPointerEnterHandler),
        typeof(UnityEngine.EventSystems.IPointerExitHandler),
        typeof(UnityEngine.EventSystems.IPointerDownHandler),
        typeof(UnityEngine.EventSystems.IPointerUpHandler),
        typeof(UnityEngine.EventSystems.IPointerClickHandler),
        typeof(UnityEngine.EventSystems.IBeginDragHandler),
        typeof(UnityEngine.EventSystems.IInitializePotentialDragHandler),
        typeof(UnityEngine.EventSystems.IDragHandler),
        typeof(UnityEngine.EventSystems.IEndDragHandler),
        typeof(UnityEngine.EventSystems.IDropHandler),
        typeof(UnityEngine.EventSystems.IScrollHandler),
        typeof(UnityEngine.EventSystems.IUpdateSelectedHandler),
        typeof(UnityEngine.EventSystems.ISelectHandler),
        typeof(UnityEngine.EventSystems.IDeselectHandler),
        typeof(UnityEngine.EventSystems.IMoveHandler),
        typeof(UnityEngine.EventSystems.ISubmitHandler),
        typeof(UnityEngine.EventSystems.ICancelHandler),
        typeof(UnityEngine.UI.ICanvasElement),
        typeof(UnityEngine.UI.IMask),
        typeof(UnityEngine.UI.IMaskable),
        typeof(UnityEngine.UI.ILayoutElement),
        typeof(UnityEngine.UI.ILayoutController),
        typeof(UnityEngine.UI.ILayoutGroup),
        typeof(UnityEngine.UI.ILayoutSelfController),
        typeof(UnityEngine.UI.ILayoutIgnorer),
        typeof(UnityEngine.UI.IMaterialModifier),
        typeof(UnityEngine.UI.IVertexModifier),

        // class

        typeof(UnityEngine.EventSystems.EventSystem),
//        typeof(UnityEngine.EventSystems.EventTrigger),
        typeof(UnityEngine.EventSystems.EventTrigger.TriggerEvent),
//        typeof(UnityEngine.EventSystems.EventTrigger.Entry),
//        typeof(UnityEngine.EventSystems.ExecuteEvents),
        typeof(UnityEngine.EventSystems.UIBehaviour),
        typeof(UnityEngine.EventSystems.AxisEventData),
        typeof(UnityEngine.EventSystems.BaseEventData),
        typeof(UnityEngine.EventSystems.PointerEventData),
        typeof(UnityEngine.EventSystems.BaseInputModule),
//        typeof(UnityEngine.EventSystems.PointerInputModule),
//        typeof(UnityEngine.EventSystems.PointerInputModule.MouseButtonEventData),
        typeof(UnityEngine.EventSystems.StandaloneInputModule),
        typeof(UnityEngine.EventSystems.TouchInputModule),
        typeof(UnityEngine.EventSystems.BaseRaycaster),
        typeof(UnityEngine.EventSystems.Physics2DRaycaster),
        typeof(UnityEngine.EventSystems.PhysicsRaycaster),
        //typeof(UnityEngine.UI.CoroutineTween.ColorTween.ColorTweenCallback),
        typeof(UnityEngine.UI.AnimationTriggers),
        typeof(UnityEngine.UI.Button),
        typeof(UnityEngine.UI.Button.ButtonClickedEvent),
        typeof(UnityEngine.UI.CanvasUpdateRegistry),
        typeof(UnityEngine.UI.FontUpdateTracker),
        typeof(UnityEngine.UI.Graphic),
//        typeof(UnityEngine.UI.GraphicRaycaster),
        typeof(UnityEngine.UI.GraphicRebuildTracker),
        typeof(UnityEngine.UI.GraphicRegistry),
        typeof(UnityEngine.UI.Image),
        typeof(UnityEngine.UI.InputField),
        typeof(UnityEngine.UI.InputField.SubmitEvent),
        typeof(UnityEngine.UI.InputField.OnChangeEvent),
        typeof(UnityEngine.UI.MaskableGraphic),
        typeof(UnityEngine.UI.RawImage),
        typeof(UnityEngine.UI.Scrollbar),
        typeof(UnityEngine.UI.Scrollbar.ScrollEvent),
        typeof(UnityEngine.UI.ScrollRect),
        typeof(UnityEngine.UI.ScrollRect.ScrollRectEvent),
        typeof(UnityEngine.UI.Selectable),
        typeof(UnityEngine.UI.Slider),
        typeof(UnityEngine.UI.Slider.SliderEvent),
        typeof(UnityEngine.UI.StencilMaterial),
        typeof(UnityEngine.UI.Text),
        typeof(UnityEngine.UI.Toggle),
        typeof(UnityEngine.UI.Toggle.ToggleEvent),
        typeof(UnityEngine.UI.ToggleGroup),
        typeof(UnityEngine.UI.AspectRatioFitter),
        typeof(UnityEngine.UI.CanvasScaler),
        typeof(UnityEngine.UI.ContentSizeFitter),
        typeof(UnityEngine.UI.GridLayoutGroup),
        typeof(UnityEngine.UI.HorizontalLayoutGroup),
        typeof(UnityEngine.UI.HorizontalOrVerticalLayoutGroup),
        typeof(UnityEngine.UI.LayoutElement),
        typeof(UnityEngine.UI.LayoutGroup),
        typeof(UnityEngine.UI.LayoutUtility),
        typeof(UnityEngine.UI.VerticalLayoutGroup),
        typeof(UnityEngine.UI.Mask),
        typeof(UnityEngine.UI.BaseVertexEffect),
        typeof(UnityEngine.UI.Outline),
        typeof(UnityEngine.UI.PositionAsUV1),
        typeof(UnityEngine.UI.Shadow),
        // typeof(UnityEngine.EventSystems.ExecuteEvents.EventFunction`1[T1]),
        

        // ValueType

        typeof(UnityEngine.EventSystems.RaycastResult),
        typeof(UnityEngine.UI.ColorBlock),
        typeof(UnityEngine.UI.FontData),
        typeof(UnityEngine.UI.Navigation),
        typeof(UnityEngine.UI.SpriteState),
        typeof(UnityEngine.UI.LayoutRebuilder),

        //////////////////////////////////////////////////////
        // delegates
        // typeof(UnityEngine.UI.InputField.OnValidateInput),

#endif

        
        typeof(List<>), 

    };

    

    // some public class members can be used
    // some of them are only in editor mode
    // some are because of unknown reason
    //
    // they can't be distinguished by code, but can be known by checking unity docs
    public static bool IsDiscard(Type type, MemberInfo memberInfo)
    {
        string memberName = memberInfo.Name;


        if (memberName == "networkView" && (type == typeof(GameObject) || typeof(Component).IsAssignableFrom(type)))
        {
            return true;
        }

        if ((type == typeof(Motion)) ||
            (type == typeof(AnimationClip) && memberInfo.DeclaringType == typeof(Motion)) ||
            (type == typeof(AnimatorOverrideController) && memberName == "PerformOverrideClipListCleanup") ||
            (type == typeof(Caching) && (memberName == "ResetNoBackupFlag" || memberName == "SetNoBackupFlag")) ||
            (type == typeof(Light) && (memberName == "areaSize")) ||
            (type == typeof(Security) && memberName == "GetChainOfTrustValue") ||
            (type == typeof(Texture2D) && memberName == "alphaIsTransparency") ||
            (type == typeof(WebCamTexture) && (memberName == "isReadable" || memberName == "MarkNonReadable")) ||
            (type == typeof(StreamReader) && (memberName == "CreateObjRef" || memberName == "GetLifetimeService" || memberName == "InitializeLifetimeService")) ||
            (type == typeof(StreamWriter) && (memberName == "CreateObjRef" || memberName == "GetLifetimeService" || memberName == "InitializeLifetimeService")) ||
            (type == typeof(UnityEngine.Font) && memberName == "textureRebuildCallback")
#if UNITY_4_6
             || (type == typeof(UnityEngine.EventSystems.PointerEventData) && memberName == "lastPress") ||
            (type == typeof(UnityEngine.UI.InputField) && memberName == "onValidateInput") // property delegate FUCK
#endif
)
        {
            return true;
        }

#if UNITY_ANDROID || UNITY_IPHONE
        if (type == typeof(WWW) && (memberName == "movie"))
            return true;
#endif
        return false;
    }
    public static bool IsGeneratedDefaultConstructor(Type type)
    {
        return type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Length == 0;

//if (type == typeof(Coroutine)
//    || type == typeof(CrashReport)
//    || type == typeof(Display)
//    || type == typeof(GUILayoutOption)
//    || type == typeof(Gyroscope)
//
//    || type == typeof(TrackedReference)
//    || type == typeof(Transform)
//    #if UNITY_IPHONE
//|| type == typeof(RemoteNotification)
//    #endif
//#if UNITY_ANDROID
//||  type == typeof(AndroidInput)
//|| type == typeof(AndroidJNI)
//|| type == typeof(AndroidJNIHelper)
//|| type == typeof(AndroidJavaException)
//    #endif
//|| type == typeof(UnityEngine.EventSystems.EventSystem)
//|| type == typeof(UnityEngine.EventSystems.EventTrigger)
//|| type == typeof(UnityEngine.EventSystems.Physics2DRaycaster)
//|| type == typeof(UnityEngine.EventSystems.PhysicsRaycaster)
//|| type == typeof(UnityEngine.EventSystems.StandaloneInputModule)
//|| type == typeof(UnityEngine.EventSystems.TouchInputModule)
//|| type == typeof(UnityEngine.UI.AspectRatioFitter)
//|| type == typeof(UnityEngine.UI.Button)
//|| type == typeof(UnityEngine.UI.CanvasScaler)
//|| type == typeof(UnityEngine.UI.CanvasUpdateRegistry)
//|| type == typeof(UnityEngine.UI.ContentSizeFitter)
//|| type == typeof(UnityEngine.UI.GraphicRegistry)
//|| type == typeof(UnityEngine.UI.GridLayoutGroup)
//|| type == typeof(UnityEngine.UI.HorizontalLayoutGroup)
//|| type == typeof(UnityEngine.UI.Image)
//|| type == typeof(UnityEngine.UI.InputField)
//)
//return false;
//return true;
    }

    public static string jsDir = Application.streamingAssetsPath + "/JavaScript";
    public static string jsGeneratedDir = jsDir + "/Generated";
    public static string csDir = Application.dataPath + "/CSharp";
    public static string csGeneratedDir = csDir + "/Generated";
    public static string jsExtension = ".javascript";
}
