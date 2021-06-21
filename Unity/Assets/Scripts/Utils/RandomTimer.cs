using UnityEngine;
using System;
using System.Collections;
using System.Runtime.Serialization;

using HCUtils;

namespace HCUtils
{
	[System.Serializable]
	public class RandomTimer : Timer {
	
		#region PRIVATE_MEMBERS
	
		protected int mSuccessRate;
		
		#endregion
		
		#region CONSTRUCTORS
		
		/// <summary>
		/// Initializes a new instance of the <see cref="RandomTimer"/> class.
		/// </summary>
		/// <param name='duration'>
		/// Duration.
		/// </param>
		/// <param name='onTimerEndHandler'>
		/// On timer end handler.
		/// </param>
		/// <param name='aSuccessRate'>
		/// A success rate (between 0 and 100).
		/// </param>
		/// <param name='aLoop'>
		/// A loop.
		/// </param>
		public RandomTimer(float duration, Action onTimerEndHandler,int aSuccessRate,bool aLoop = false) :base (duration,onTimerEndHandler,aLoop)
		{
			//
			mSuccessRate = aSuccessRate;
		}
		
		public RandomTimer(MonoBehaviour aOwner, float duration, Action onTimerEndHandler,int aSuccessRate,bool aLoop = false) :base (aOwner,duration,onTimerEndHandler,aLoop)
		{
			//
			mSuccessRate = aSuccessRate;
		}
		
		#endregion
		
		#region PUBLIC_METHODS

		/// <summary>
		/// Sets the sucress rate in percentage, must be between 0 and 100.
		/// </summary>
		/// <param name="aRate">A rate.</param>
		public void SetSucressRate(int aRate)
		{
			mSuccessRate = Mathf.Clamp(aRate,0,100);
		}
		
		#endregion
		
		#region OVERRIDEN_METHODS
		
		internal override void Update (float aDeltaTime)
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
						//If random "succeded"
						if (UnityEngine.Random.Range(0,100)<mSuccessRate)
						{
							mHandler();
						}
						else if (!mLoop)
						{
							mIsFinised = false;
							Start();
						}
					}
					else
					{
						Debug.LogWarning("No handler on timer.");
					}
				}
			}
		}

		public override string ToString ()
		{
			return string.Format ("[RandomTimer]");
		}

		#endregion
		
		#region SERIALIZATION
		
			protected RandomTimer(SerializationInfo info, StreamingContext context) : base(info,context)
			{
				//
				mSuccessRate			= info.GetInt32("successRate");
			 }
			
			public override void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				base.GetObjectData(info,context);
				info.AddValue("successRate", mSuccessRate);
			}
			
			#endregion
	}
}
