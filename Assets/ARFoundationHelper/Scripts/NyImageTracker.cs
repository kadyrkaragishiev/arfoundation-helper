using UnityEngine;
using UnityEngine.Serialization;

namespace ARFoundationHelper.Scripts
{
    public class NyImageTracker : MonoBehaviour
    {
        public Texture2D trackerImage;
        public Vector2 physicalSize = new(1.0f, 1.0f);
        public float editScaler = 1.0f;
        private float _playScaler = 1.0f;

        public bool hideTrackerWhenPlay = true;

        public Transform referenceGrouper;

        private Vector3 _originPos;
        private Quaternion _originRot;
        private Vector3 _originScale;

        private Vector3 _grouperPosLocal;
        private Quaternion _grouperRotDiff;
        private Vector3 _grouperOriginScale;

        // for Editor use
        [FormerlySerializedAs("_lastTexture")]
        [HideInInspector]
        public Texture2D lastTexture;

        [FormerlySerializedAs("_lastTrackerSize")] [HideInInspector]
        public Vector2 lastTrackerSize = new(1.0f, 1.0f);

        [FormerlySerializedAs("_sizeRatio")] [HideInInspector]
        public float sizeRatio = 1.0f;

        [HideInInspector]
        public int inspectorCounter = -1; // check if it first time selected

        [FormerlySerializedAs("_lastEditScaler")] [HideInInspector]
        public float lastEditScaler = 1.0f;

        public void Start()
        {
            if (hideTrackerWhenPlay) gameObject.GetComponent<MeshRenderer>().enabled = false;

            // grab reference data before scale
            _originPos = transform.position;
            _originRot = transform.rotation;
            _originScale = transform.localScale;


            if (referenceGrouper != null)
            {
                _grouperPosLocal = transform.InverseTransformPoint(referenceGrouper.position);
                _grouperRotDiff = referenceGrouper.rotation * Quaternion.Inverse(transform.rotation);
                _grouperOriginScale = referenceGrouper.localScale;
            }

            // scale after
            _playScaler = 1.0f / editScaler;

            if (_playScaler != 1.0f) transform.localScale = Vector3.one * _playScaler;
        }

        public void UpdateTransform(Vector3 newPosition, Quaternion newRotation)
        {
            transform.position = newPosition;
            transform.rotation = newRotation;

            if (referenceGrouper != null)
            {
                Vector3 posDiff = transform.position - _originPos;
                Quaternion rotDiff = transform.rotation * Quaternion.Inverse(_originRot);

                referenceGrouper.position = transform.TransformPoint(_grouperPosLocal);
                referenceGrouper.rotation = transform.rotation * _grouperRotDiff;
                referenceGrouper.localScale = _grouperOriginScale * _playScaler;
            }
        }

        public void OnTrackingFound()
        {
            Debug.Log("origin size: " + _originScale + "  img: " + transform.localScale);

            if (gameObject.GetComponent<NyImageTrackerEventHandler>()) gameObject.GetComponent<NyImageTrackerEventHandler>().OnTrackingFound();
        }

        public void OnTrackingLost()
        {
            if (gameObject.GetComponent<NyImageTrackerEventHandler>()) gameObject.GetComponent<NyImageTrackerEventHandler>().OnTrackingLost();
        }
    }
}
