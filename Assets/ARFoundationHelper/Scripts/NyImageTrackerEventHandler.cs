using UnityEngine;

namespace ARFoundationHelper.Scripts
{
    [RequireComponent(typeof(NyImageTracker))]
    public class NyImageTrackerEventHandler : MonoBehaviour
    {
        public virtual void OnTrackingFound () => Debug.Log("tracker [" + gameObject.name + "] Found!");

        public virtual void OnTrackingLost () => Debug.Log("tracker [" + gameObject.name + "] Lost!");
    }
}
