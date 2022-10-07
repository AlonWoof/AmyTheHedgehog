using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
//using Artngame.PDM;
//#if UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9 || UNITY_2017 || UNITY_2018 || UNITY_2017_OR_NEWER
using UnityEngine.SceneManagement; //vSM3.4.6 Unity 5.4.2f2 //v2.1.19
//#endif

namespace Artngame.INfiniDy {

// Code from http://unitygems.com/threads/, MIT license.

public class LoomINfiniDyGRASS : MonoBehaviour
{
	void OnApplicationQuit()
	{
		_quitting = true;
	}
	static bool _quitting;
		private static LoomINfiniDyGRASS _current;
	private int _count;
	
		public static LoomINfiniDyGRASS Current
	{
		get
		{
			Initialize();
			return _current;
		}
	}
	
	static bool _initialized;
	static int _threadId;
	
	public static void Initialize()
	{
		if (!Application.isPlaying || _quitting)
			return;
		var go = !_initialized;
		if (!go && _threadId == Thread.CurrentThread.ManagedThreadId && _current == null)
			go = true;
		
		if (go)
		{
				foreach (var LoomINfiniDyGRASS in Resources.FindObjectsOfTypeAll(typeof(LoomINfiniDyGRASS)).Cast<LoomINfiniDyGRASS>())
					DestroyImmediate(LoomINfiniDyGRASS.gameObject);
				var g = new GameObject("LoomINfiniDyGRASS");
				_current = g.AddComponent<LoomINfiniDyGRASS>();
			_initialized = true;
			_threadId = Thread.CurrentThread.ManagedThreadId;
		}
		
	}

        //v2.1.19
//#if UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9 || UNITY_2017 || UNITY_2018 || UNITY_2017_OR_NEWER
	void Awake()
	{//SMv3.4.6
	SceneManager.sceneLoaded += OnSceneLoaded;
	}
//#endif

        void OnDestroy()
	{
		_actions.Clear();
		_delayed.Clear();
		if (_current == this)
		{
			_initialized = false;
		}
	}
	
	private readonly List<Action> _actions = new List<Action>();
	public class DelayedQueueItem
	{
		public float time;
		public Action action;
		public string name;
	}
	private readonly List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();
	
	public static void QueueOnMainThread(Action action, string name)
	{
		QueueOnMainThread(action, 0, name);
	}
	
	public static void QueueOnMainThread(Action action, float time, string name)
	{
		if (!Application.isPlaying)
			return;
		if (Math.Abs(time - 0) > float.Epsilon || !string.IsNullOrEmpty(name))
		{
			lock (Current._delayed)
			{
				DelayedQueueItem existing = null;
				if (!string.IsNullOrEmpty(name))
					existing = Current._delayed.FirstOrDefault(d => d.name == name);
				if (existing != null)
				{
					existing.time = Time.time + time;
					return;
				}
				var queueItem = new DelayedQueueItem();
				queueItem.name = name;
				queueItem.time = Time.time + time;
				queueItem.action = action;
				Current._delayed.Add(queueItem);
			}
		}
		else
		{
			lock (Current._actions)
			{
				Current._actions.Add(action);
			}
		}
		
	}
	
	/// <summary>
	/// Queues an action on the main thread
	/// </summary>
	/// <param name='action'>
	/// The action to execute
	/// </param>
	public static void QueueOnMainThread(Action action)
	{
		QueueOnMainThread(action, 0f);
	}
	/// <summary>
	/// Queues an action on the main thread after a delay
	/// </summary>
	/// <param name='action'>
	/// The action to run
	/// </param>
	/// <param name='time'>
	/// The amount of time to delay
	/// </param>
	public static void QueueOnMainThread(Action action, float time)
	{
		QueueOnMainThread(action, time, null);
	}
	
	/// <summary>
	/// Runs an action on another thread
	/// </summary>
	/// <param name='action'>
	/// The action to execute on another thread
	/// </param>
	public static void RunAsync(Action action)
	{
		var t = new Thread(RunAction)
		{
			Priority = System.Threading.ThreadPriority.Normal
		};
		t.Start(action);
	}
	
	private static void RunAction(object action)
	{
		((Action)action)();
	}
	
	readonly Action[] _toRun = new Action[4000];
	
	// Update is called once per frame
	void Update()
	{
		if (Current != this)
		{
			if (Application.isPlaying)
				DestroyImmediate(gameObject);
			return;
		}
		if (!Application.isPlaying)
		{
			_actions.Clear();
			_delayed.Clear();
			return;
		}
		var count = Mathf.Min(_actions.Count, 4000);
		lock (_actions)
		{
			_actions.CopyTo(0, _toRun, 0, count);
			if (count == _actions.Count)
				_actions.Clear();
			else
				_actions.RemoveRange(0, count);
		}
		for (var i = 0; i < count; i++)
		{
			try
			{
				_toRun[i]();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		
		lock (_delayed)
		{
			count = 0;
			for (var i = _delayed.Count - 1; i >= 0 && count < 3999; i--)
			{
				if (!(_delayed[i].time <= Time.time))
				{
					continue;
				}
				_toRun[count++] = _delayed[i].action;
				_delayed.RemoveAt(i);
			}
		}
		
		for (var i = 0; i < count; i++)
		{
			try
			{
				_toRun[i]();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		
		
	}

        //v2.1.19
//#if UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9 || UNITY_2017 || UNITY_2018 || UNITY_2017_OR_NEWER
	void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{ //v3.4.6 //SMv3.4.9
	_actions.Clear();
	_delayed.Clear();
	}
//#else
//        void OnLevelWasLoaded()
//	{
//		_actions.Clear();
//		_delayed.Clear();
//	}
//	#endif

}

}