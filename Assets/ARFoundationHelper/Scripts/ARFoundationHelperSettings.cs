using UnityEngine;
using UnityEngine.Serialization;

namespace ARFoundationHelper.Scripts
{
    public class ARFoundationHelperSettings : ScriptableObject
    {
        [FormerlySerializedAs("GeneratedLibrarySavePath")] 
        public string generatedLibrarySavePath = "Assets/ARFoundationHelper/GeneratedLibrary/auto-image-lib.asset";
        [FormerlySerializedAs("DefaultImageTrackerMaterial")] 
        public Material defaultImageTrackerMaterial;
     }
}
