using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARFoundationHelper.Scripts
{
    public class NyImageTrackerManager : MonoBehaviour
    {
        public XRReferenceImageLibrary targetLib;
        public Transform[] trackerObjs;
        private TrackingState[] _trackerStates;

        private ARTrackedImageManager _foundationTrackerManager;
        private Dictionary<string, int> _trackerImageNameDict;

        public UnityEngine.UI.Text debugText;

        private int _imgIndex = 0;

        // setting tracked image limit
        private Dictionary<int, NyImageTracker> _trackedImgs = new();
        private int _maxTrackImgCount = 0;

        // Start is called before the first frame update
        private void Start()
        {
            // setup reference
            _trackerStates = new TrackingState[targetLib.count];
            _trackerImageNameDict = new Dictionary<string, int>();
            for(var i=0; i< trackerObjs.Length; i++)
            {
                _trackerImageNameDict.Add(trackerObjs[i].name, i);
                _trackerStates[i] = TrackingState.None;
            }


            // init foundation ar image manager
            _foundationTrackerManager = FindObjectOfType<ARTrackedImageManager>();
            _maxTrackImgCount = _foundationTrackerManager.currentMaxNumberOfMovingImages;

            Debug.Log("Get Manager: " + _foundationTrackerManager.gameObject.name);
            //_foundationTrackerManager = gameObject.AddComponent<ARTrackedImageManager>();
            _foundationTrackerManager.enabled = false;
            _foundationTrackerManager.referenceLibrary = targetLib;
            _foundationTrackerManager.enabled = true;

            Debug.Log("Start AR Manager");
        }


        // Update is called once per frame
        private void Update()
        {
            UpdateTrackerStatus();

            if (debugText != null) LogTrackerStatus();
        }

        private void UpdateTrackerStatus ()
        {
            foreach (ARTrackedImage arImg in _foundationTrackerManager.trackables)
            {
                _imgIndex = _trackerImageNameDict[arImg.referenceImage.name];

                // state has change!
                if (arImg.trackingState != _trackerStates[_imgIndex])
                {
                    _trackerStates[_imgIndex] = arImg.trackingState;

                    if (arImg.trackingState == TrackingState.Tracking)
                    {
                        if (_maxTrackImgCount > 0)
                        {
                            if (_trackedImgs.Count < _maxTrackImgCount)
                            {
                                _trackedImgs.Add(_imgIndex,trackerObjs[_imgIndex].GetComponent<NyImageTracker>());
                                trackerObjs[_imgIndex].GetComponent<NyImageTracker>().OnTrackingFound();
                            }
                        }
                        else
                        {
                            trackerObjs[_imgIndex].GetComponent<NyImageTracker>().OnTrackingFound();
                        }
                    }
                    else if (arImg.trackingState == TrackingState.Limited)
                    {
                        if(_maxTrackImgCount > 0) _trackedImgs.Remove(_imgIndex);

                        trackerObjs[_imgIndex].GetComponent<NyImageTracker>().OnTrackingLost();
                    }
                    else if (arImg.trackingState == TrackingState.None)
                    {
                        // error?
                        Debug.LogError("Object [" + arImg.referenceImage.name + "] become NONE");
                    }
                }

                // update data if tracked
                if (arImg.trackingState == TrackingState.Tracking)
                {
                    if(_maxTrackImgCount > 0)
                    {
                        if (_trackedImgs.ContainsKey(_imgIndex))
                            UpdateTargetObjStatus(arImg, _imgIndex);
                    }
                    else
                    {
                        UpdateTargetObjStatus(arImg, _imgIndex);
                    }
                
                }
            }
        }

        private void UpdateTargetObjStatus(ARTrackedImage targetImg, int targetIndex)
        {
            Transform targetObj = trackerObjs[targetIndex];

            //targetObj.position = targetImg.transform.position;
            //targetObj.rotation = targetImg.transform.rotation;
            //targetObj.transform.localScale = new Vector3(targetImg.extents.x, 1.0f, targetImg.extents.y);

            targetObj.GetComponent<NyImageTracker>().UpdateTransform(targetImg.transform.position, targetImg.transform.rotation);
        }

        private void LogTrackerStatus ()
        {
            var debugMessage = "";
            debugMessage += "trackers: " + _foundationTrackerManager.trackables.count + "\n";

            foreach (ARTrackedImage arImg in _foundationTrackerManager.trackables)
            {
                debugMessage += arImg.name + "\n";
                debugMessage += "[" + arImg.referenceImage.name + "]: " + arImg.trackingState.ToString() + "\n";
                debugMessage += "extends: " + arImg.extents.x + "  " + arImg.extents.y + "\n";
            }

            debugText.text = debugMessage;
        }

    }
}
