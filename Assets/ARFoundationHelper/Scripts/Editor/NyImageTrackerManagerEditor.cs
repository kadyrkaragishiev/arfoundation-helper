using UnityEditor;
using UnityEditor.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace ARFoundationHelper.Scripts.Editor
{
    [CustomEditor(typeof(NyImageTrackerManager))]
    public class NyImageTrackerManagerEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update Library"))
            {
                GenerateLibFromSceneSetting();
            }
        }

        public void GenerateLibFromSceneSetting()
        {
            NyImageTracker[] trackers = GameObject.FindObjectsOfType<NyImageTracker>();

            var objs = new Transform[trackers.Length];
            XRReferenceImageLibrary newLib = ScriptableObject.CreateInstance<XRReferenceImageLibrary>();

            var debugText = "Found " + trackers.Length + " Trackers in Scene:\n";
            for (var i = 0; i < trackers.Length; i++)
            {
                debugText += "  " + trackers[i].gameObject.name + "\n";
                objs[i] = trackers[i].transform;

                XRReferenceImageLibraryExtensions.Add(newLib);
                XRReferenceImageLibraryExtensions.SetName(newLib, i, trackers[i].name);
                XRReferenceImageLibraryExtensions.SetTexture(newLib, i, trackers[i].trackerImage, false);
                XRReferenceImageLibraryExtensions.SetSpecifySize(newLib, i, true);
                XRReferenceImageLibraryExtensions.SetSize(newLib, i, trackers[i].physicalSize);
            }

            var targetPath = Resources.Load<ARFoundationHelperSettings>("HelperSettings").generatedLibrarySavePath;
            AssetDatabase.CreateAsset(newLib, targetPath + "generated-lib.asset");

            ((NyImageTrackerManager)target).targetLib = newLib;
            ((NyImageTrackerManager)target).trackerObjs = objs;

            Debug.Log(debugText);
        }
    }
}
