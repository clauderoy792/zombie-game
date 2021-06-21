using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Reflection;

namespace HCUtils
{
	[Serializable]
	public class Timer : IDisposable,ISerializable
	{
		const string HANDLER_NULL_VALUE = "-1";

		#region PRIVATE_MEMBERS
		
		protected Action		 			mHandler 		= null;
		protected float 					mDuration		= 0;
		protected float 					mCurrentTime	= 0;
		protected bool						mLoop			= false;
		protected bool 						mIsRunning		= false;
		protected bool						mIsFinised		= false;
		protected string					mInvokerName	= null;
		protected string					mInvokerType	= null;
		protected string					mMethodName		= null;

		//Used only for monobehaviour handler.
		protected MonoBehaviour				mOwner			= null;
		#endregion
		
		#region CONSTRUCTORS/DESTRUCTORS
		
		/// <summary>
		/// Initializes a new instance of the <see cref="HCUtils.Timer"/> class.
		/// Must be used when the class you use it on can be serialized
		/// </summary>
		/// <param name='duration'>
		/// Duration.
		/// </param>
		/// <param name='onTimerEndHandler'>
		/// On timer end handler.
		/// </param>
		/// <param name='aLoop'>
		/// A loop.
		/// </param>
		public Timer(float duration, Action onTimerEndHandler,bool aLoop = false)
		{
			mDuration = duration;
			mLoop = aLoop;
			mHandler = onTimerEndHandler;
			
			//
			TimerManager.Instance.AddTimer(this);
		}
		
		/// <summary>
		/// Use it with any monobehavior class Initializes a new instance of the <see cref="HCUtils.Timer"/> class.
		/// </summary>
		/// <param name='duration'>
		/// Duration.
		/// </param>
		/// <param name='onTimerEndHandler'>
		/// On timer end handler.
		/// </param>
		/// <param name='aLoop'>
		/// A loop.
		/// </param>
		public Timer(MonoBehaviour aOwner, float duration, Action onTimerEndHandler,bool aLoop = false)
		{
			mOwner = aOwner;
			mInvokerType = aOwner.GetType().ToString();
			mDuration = duration;
			mLoop = aLoop;
			mHandler = onTimerEndHandler;
			
			//
			TimerManager.Instance.AddTimer(this);
		}
		
		/*~Timer()
		{
			Dispose();
		}*/
		
		#endregion
		
		#region ACCESSORS
		
		public float Duration
		{
			get{return mDuration;}
		}

		public float CurrentTime
		{
			get{return mCurrentTime;}
		}

		public bool IsRunning
		{
			get{return mIsRunning;}
		}

		public Action Handler
		{
			get{return mHandler;}
		}

		#endregion
		
		#region PUBLIC_METHODS
		
		public void Dispose()
		{
			//
			Stop ();

			//
			TimerManager.Instance.RemoveTimer(this);
			
			//
			mHandler = null;
		}
		
		public void Start()	
		{
			if (mIsRunning)
			{
				Debug.LogWarning("Timer with handler : "+(mHandler != null ? mHandler.Method.Name : "none")+" is already running."); 
			}

			if (mIsFinised)
			{
				mCurrentTime = 0;
			}

			mIsRunning = true;
			mIsFinised = false;
		}
		
		public void Stop()
		{
			mIsRunning = false;
		}

		public void Restart()
		{
			mCurrentTime = 0;
			Start();
		}
		
		public void SetDuration(float aDuration)
		{
			mDuration = aDuration;
		}

		public float GetPercentage01()
		{
			return mCurrentTime/mDuration;
		}
		
		#endregion
		
		#region INTERNAL_METHODS
		
		internal virtual void Update(float aDeltaTime)
		{
			if(mIsRunning)
			{
				mCurrentTime += aDeltaTime;
				
				if (mCurrentTime >= mDuration)
				{
					mCurrentTime = 0;
				
					if (!mLoop)
					{
						mIsFinised = true;
						Stop();
					}
					
					if (mHandler != null)
					{
						mHandler();
					}
					else
					{
						Debug.LogError("No handler on timer : "+GetType().ToString());
					}
				}
			}
		}
		
		#endregion
		
		#region INITIALIZATION

		public bool HasNotBeenInitialized()
		{
			return mInvokerName!= null && mInvokerType != null && mHandler == null;
		}

		public bool InitializeMonobehaviourHandler()
		{
			bool returnValue = false;

			//If we are handling a monobehavior
			if (mInvokerName!= null && mInvokerType != null)
			{
				//If we have no handler
				if (mMethodName == HANDLER_NULL_VALUE)
				{
					returnValue = true;
				}
				else
				{
					GameObject go 	= GameObject.Find(mInvokerName); 
					
					if (go != null)
					{
						MonoBehaviour target = go.GetComponent(mInvokerType) as MonoBehaviour;
						
						if (target != null)
						{
							mOwner = target;
							Type invokerType 		= Type.GetType(mInvokerType);
							
							//Creating the delegate
							MethodInfo method = invokerType.GetMethod(mMethodName,BindingFlags.NonPublic | BindingFlags.Instance);
							mHandler =Action.CreateDelegate(typeof(Action),target,method.Name) as Action;

							if (mHandler != null)
							{
								//Initialization succeeded.
								returnValue = true;
							}
						}
					}
				}
			}

			return returnValue;
		}
		
		#endregion

		#region OVERRIDEN METHODS

		public override string ToString ()
		{
			return string.Format ("[Timer: CurrentTime={0}, Duration={1}, IsRunning={2}]",mCurrentTime, Duration, IsRunning);
		}

		#endregion
		
		#region SERIALIZATION
	
		protected Timer(SerializationInfo info, StreamingContext context)
		{
			//Setting Handler
			mInvokerName 			= info.GetString("invokerName");
			mInvokerType 			= info.GetString("invokerType");

			//If we had a non-monobehavior (else don't forget to call method, InitializeMonobehaviorTimer, after
			// your gameobject's name has beens set.
			if (mInvokerName == null || mInvokerType == null)
			{
				mHandler 			= (Action)info.GetValue("handler",typeof(Action));
			}
			else
			{
				mMethodName			= info.GetString("handler");
			}

			//If we are handling a monobehavior
			if (mInvokerName!= null && mInvokerType != null)
			{
				if (!InitializeMonobehaviourHandler())
				{
					//Initialization of handler failed.
					TimerManager.Instance.AddNonInitializedTimer(this);
				}
			}
			
			//
			mDuration 				= (float)info.GetValue("duration",typeof(float));
			mCurrentTime 			= (float)info.GetValue("currenTime",typeof(float));
			
			//
			mLoop 					= info.GetBoolean("loop");
			mIsRunning 				= info.GetBoolean("running");
			mIsFinised				= info.GetBoolean("isFinished");
			
			//
			TimerManager.Instance.AddTimer(this);
		 }
		
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//Setting Handler
			if (mHandler != null)
			{
				//If we are handling a monobehavior
				if (mOwner != null)
				{
					mInvokerName = mOwner.name;

					if (mHandler != null)
					{
						info.AddValue("handler", mHandler.Method.Name);
					}
					else
					{
						info.AddValue("handler",HANDLER_NULL_VALUE);
					}
				}
				else
				{
					info.AddValue("handler", mHandler);
				}
			}

			//
			info.AddValue("duration",mDuration);
			info.AddValue("currenTime",mCurrentTime);
			info.AddValue("loop",mLoop);
			info.AddValue("running", mIsRunning);
			info.AddValue("isFinished",mIsFinised);
			info.AddValue("invokerName", mInvokerName);
			info.AddValue("invokerType", mInvokerType);
		}
		
		#endregion
	}
}
