using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IA.ScriptableEvent.Listener
{
	public class AudioClipChannelListener : GenericScriptableEventChannelListener<AudioClip>
	{
		// Add your specific functionality here
#if UNITY_EDITOR

		[MenuItem("GameObject/IA/Event Listener/ -> AudioClip Channel Listener", false, 3)]
		public static void AddListenerToHierarchy()
		{
			GameObject listenerObj = new GameObject("AudioClip Channel Listener");
			listenerObj.AddComponent<AudioClipChannelListener>();
			Selection.activeGameObject = listenerObj;
		}
#endif
	}
}
