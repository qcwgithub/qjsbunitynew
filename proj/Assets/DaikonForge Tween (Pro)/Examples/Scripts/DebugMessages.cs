using SharpKit.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using DaikonForge.Tween;
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/DaikonForge Tween (Pro)/Examples/Scripts/DebugMessages.javascript")]
public class DebugMessages
{

	#region Static variables 

	private static List<Message> messages = new List<Message>();

	private const int startLeft = 5;
	private const int startTop = 20;
	private const int lineHeight = 20;

	#endregion 

	#region Public methods 

	public static void Add( string text )
	{

		for( int i = 0; i < messages.Count; i++ )
		{

			var item = messages[ i ];

			var position = item.guiText.pixelOffset;
			position.y += lineHeight;

			item.guiText.pixelOffset = position;

		}

		var message = Message.Obtain();
		message.guiText.pixelOffset = new Vector2( startLeft, startTop );
		message.guiText.text = text;
		message.tween.Play();

		messages.Add( message );

	}

	#endregion 

	#region Nested classes
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/DaikonForge Tween (Pro)/Examples/Scripts/Message.javascript")]
	protected class Message
	{

		public GUIText guiText;
		public TweenBase tween;

		#region Object pooling 

		private static List<Message> pool = new List<Message>();

		public static Message Obtain()
		{

			if( pool.Count > 0 )
			{

				var instance = pool[ 0 ];
				pool.RemoveAt( 0 );

				return instance;

			}

			var gameObject = new GameObject( "__Message__" ) { hideFlags = HideFlags.HideInHierarchy };

			var newInstance = new Message();
			newInstance.guiText = gameObject.AddComponent<GUIText>();

			newInstance.tween = newInstance.guiText
				.TweenAlpha()
				.SetDelay( 5 )
				.SetDuration( 0.5f )
				.SetStartValue( 1 )
				.SetEndValue( 0 )
				.SetIsTimeScaleIndependent( true )
				.OnCompleted( ( tween ) =>
				{
					newInstance.Release();
				} );

			return newInstance;

		}

		public void Release()
		{
			if( !pool.Contains( this ) )
			{
				messages.Remove( this );
				pool.Add( this );
			}
		}

		#endregion 

	}

	#endregion 

}
