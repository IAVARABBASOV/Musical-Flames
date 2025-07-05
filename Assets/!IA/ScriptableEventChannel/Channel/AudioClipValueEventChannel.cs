using UnityEngine;

namespace IA.ScriptableEvent.Channel
{
	[CreateAssetMenu(fileName = "AudioClip Value Event Channel", menuName = "IA/Event Channel/ -> AudioClip Value Event Channel", order = 3)]
	public class AudioClipValueEventChannel : GenericScriptableEventChannel<AudioClip>
	{
		public override void LoadDefaultData() => value = null;
	}
}
