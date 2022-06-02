using ARFoundationHelper.Scripts;
using UnityEngine;

namespace _Demo.Scripts
{
    public class PlayAnimationOnOffTracking : NyImageTrackerEventHandler
    {
        public Animator targetAnim;

        public string onFoundAnimName;
        public string onLostAnimName;

        public override void OnTrackingFound() => targetAnim.Play(onFoundAnimName);

        public override void OnTrackingLost() => targetAnim.Play(onLostAnimName);
    }
}
