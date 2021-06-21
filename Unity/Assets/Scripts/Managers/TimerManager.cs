using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HCUtils
{
	public class TimerManager
	{
		#region CONSTANTS
	
		#endregion
	
		#region STATIC_MEMBERS
	
		private static TimerManager sInstance;
	
		#endregion
	
		#region PRIVATE_MEMBERS
		
		List<Timer> mTimers;

		//List of timers that could not be initialized yet. (handler could not be found in the scene)
		List<Timer> mNonInitializedTimers;

		#endregion
	
		#region ACCESSORS
	
		public static TimerManager Instance
		{
			get
			{
				if (sInstance == null)
				{
					sInstance = new TimerManager();
				}
	
				return sInstance;
			}
		}
		#endregion
	
		#region CONSTRUCTORS
	
		private TimerManager ()
		{
			mTimers = new List<Timer>();
			mNonInitializedTimers = new List<Timer>();
		}
	
		#endregion
	
		#region PUBLIC_METHODS
		
		public void AddTimer(Timer aTimer)
		{
			mTimers.Add(aTimer);
		}

		public void RemoveTimer(Timer aTimer)
		{
			mTimers.Remove(aTimer);
		}

		public void AddNonInitializedTimer(Timer aTimer)
		{
			mNonInitializedTimers.Add(aTimer);
		}
		
		public void Update(float aDeltaTime)
		{
			if (mNonInitializedTimers.Count > 0)
			{
				for(int i = 0;i<mNonInitializedTimers.Count;i++)
				{
					if (mNonInitializedTimers[i].HasNotBeenInitialized())
					{
						if (mNonInitializedTimers[i].InitializeMonobehaviourHandler())
						{
							//Initialization succeeded.
							mNonInitializedTimers.Remove(mNonInitializedTimers[i]);
						}
					}
				}
			}

			for(int i = 0;i<mTimers.Count;i++)
			{
				if (mTimers[i] != null)
				{
					mTimers[i].Update(aDeltaTime);
				}
				else
				{
					Debug.LogError("TIMER IS NULL");
				}
			}
		}
			
		#endregion
	
		#region PRIVATE_METHODS
	
		#endregion
	}
}