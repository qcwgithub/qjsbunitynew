/* Copyright 2014 Daikon Forge */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DaikonForge.Tween
{

	/// <summary>
	/// Manages currently playing tweens
	/// </summary>
	public class TweenManager : MonoBehaviour
	{

		#region Singleton implementation

		private static TweenManager instance;

		/// <summary>
		/// Gets or creates the tween manager instance
		/// </summary>
		public static TweenManager Instance
		{
			get
			{

				lock( typeof( TweenManager ) )
				{

					if( instance == null )
					{
						instance = new GameObject( "_TweenManager_" ) { hideFlags = HideFlags.HideInHierarchy }.AddComponent<TweenManager>();
					}

					return instance;

				}

			}
		}

		#endregion

		#region Static fields

		/// <summary>
		/// Keeps time-scale-independent track of the time between the current frame and the previous frame
		/// </summary>
		internal static float realDeltaTime = 0f;

		private static float lastFrameTime = 0f;

		#endregion

		#region Public fields

		/// <summary>
		/// Returns the real time since startup. Note that this property returns a 0 until application 
		/// startup is complete, unlike Time.realTimeSinceStartup, which does not return a useful value
		/// until all Monobehaviour.Awake() methods have been called.
		/// http://forum.unity3d.com/threads/205773-realtimeSinceStartup-is-not-0-in-first-Awake()-call?p=1390295&viewfull=1#post1390295
		/// </summary>
		internal float realTimeSinceStartup = 0f;

		#endregion

		#region Private runtime variables

		private List<ITweenUpdatable> playingTweens = new List<ITweenUpdatable>();
		private Queue<ITweenUpdatable> addTweenQueue = new Queue<ITweenUpdatable>();
		private Queue<ITweenUpdatable> removeTweenQueue = new Queue<ITweenUpdatable>();

		#endregion

		#region Constructor

		static TweenManager()
		{
			lastFrameTime = 0f;
			realDeltaTime = 0f;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Register a tween to be updated
		/// </summary>
		internal void RegisterTween( ITweenUpdatable tween )
		{

			lock( playingTweens )
			{

				if( playingTweens.Contains( tween ) && !removeTweenQueue.Contains( tween ) )
					return;

				lock( addTweenQueue )
				{
					addTweenQueue.Enqueue( tween );
				}

			}

		}

		/// <summary>
		/// Unregister a tween
		/// </summary>
		internal void UnregisterTween( ITweenUpdatable tween )
		{

			lock( removeTweenQueue )
			{

				if( !playingTweens.Contains( tween ) || removeTweenQueue.Contains( tween ) )
					return;

				removeTweenQueue.Enqueue( tween );

			}

		}

		/// <summary>
		/// Removes all tweens from the scheduler. This will effectively stop all tweens from playing,
		/// but will not set the TweenBase.State property, which should be done manually
		/// </summary>
		public void Clear()
		{

			lock( playingTweens )
			{
				playingTweens.Clear();
				removeTweenQueue.Clear();
			}

		}

		/// <summary>
		/// Stops all currently playing tweens
		/// </summary>
		public void Stop()
		{
			lock( playingTweens )
			{

				var list = new List<ITweenUpdatable>( playingTweens );
				list.AddRange( addTweenQueue );

				playingTweens.Clear();
				addTweenQueue.Clear();
				removeTweenQueue.Clear();

				for( int i = 0; i < list.Count; i++ )
				{
					var tween = list[ i ];
					if( tween is TweenBase )
					{
						( (TweenBase)tween ).Stop();
					}
				}

			}
		}

		/// <summary>
		/// Stops all currently playing tweens
		/// </summary>
		public void Pause()
		{
			lock( playingTweens )
			{

				var list = new List<ITweenUpdatable>( playingTweens );
				list.AddRange( addTweenQueue );

				for( int i = 0; i < list.Count; i++ )
				{
					var tween = list[ i ];
					if( tween is TweenBase )
					{
						( (TweenBase)tween ).Pause();
					}
				}

			}
		}

		/// <summary>
		/// Stops all currently playing tweens
		/// </summary>
		public void Resume()
		{
			lock( playingTweens )
			{

				var list = new List<ITweenUpdatable>( playingTweens );
				list.AddRange( addTweenQueue );

				for( int i = 0; i < list.Count; i++ )
				{
					var tween = list[ i ];
					if( tween is TweenBase )
					{
						( (TweenBase)tween ).Resume();
					}
				}

			}
		}

		#endregion

		#region Monobehaviour event handlers

		public virtual void OnDestroy()
		{
			instance = null;
		}

		public virtual void Update()
		{

			realTimeSinceStartup = Time.realtimeSinceStartup;
			realDeltaTime = realTimeSinceStartup - lastFrameTime;
			lastFrameTime = realTimeSinceStartup;

			lock( playingTweens )
			{

				lock( addTweenQueue )
				{
					while( addTweenQueue.Count > 0 )
					{
						playingTweens.Add( addTweenQueue.Dequeue() );
					}
				}

				lock( removeTweenQueue )
				{
					while( removeTweenQueue.Count > 0 )
					{
						playingTweens.Remove( removeTweenQueue.Dequeue() );
					}
				}

				var count = playingTweens.Count;
				for( int i = 0; i < count; i++ )
				{
					var tween = playingTweens[ i ];
					var state = tween.State;
					if( state == TweenState.Playing || state == TweenState.Started )
					{
						tween.Update();
					}
				}

			}

		}

		#endregion

	}

}
