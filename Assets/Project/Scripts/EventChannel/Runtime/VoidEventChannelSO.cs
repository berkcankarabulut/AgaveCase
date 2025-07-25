using UnityEngine;
using UnityEngine.Events;

namespace Project.EventChannel.Runtime
{
    [CreateAssetMenu(menuName = "Game ScriptableObjects/Events/Channels/Void Event Channel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        public UnityAction onEventRaised;

        public void RaiseEvent()
        {
            if (onEventRaised != null)
                onEventRaised.Invoke();
        }
    }
}
