using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARFoundationHelper.Scripts.Editor
{
    [CustomEditor(typeof(NyImageTracker))]
    public class NyImageTrackerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var targetScript = (NyImageTracker)target;
            GameObject targetObj = targetScript.gameObject;

            // check component
            if(!targetObj.GetComponent<MeshRenderer>())
            {
                targetObj.AddComponent<MeshRenderer>();

                targetObj.GetComponent<MeshRenderer>().material = new Material(Resources.Load<ARFoundationHelperSettings>("HelperSettings").defaultImageTrackerMaterial);
            }

            if(!targetObj.GetComponent<MeshFilter>())
            {
                targetObj.AddComponent<MeshFilter>();

                targetObj.GetComponent<MeshFilter>().mesh = CreateTrackerMesh();
            }

            return base.CreateInspectorGUI();
        }

        private Mesh CreateTrackerMesh ()
        {
            var mesh = new Mesh();

            var verts = new Vector3[4];
            verts[0] = new Vector3(-0.5f, 0.0f, 0.5f);
            verts[1] = new Vector3(0.5f, 0.0f, 0.5f);
            verts[2] = new Vector3(-0.5f, 0.0f, -0.5f);
            verts[3] = new Vector3(0.5f, 0.0f, -0.5f);

            var uvs = new Vector2[4];
            uvs[0] = new Vector2(0.0f, 1.0f);
            uvs[1] = new Vector2(1.0f, 1.0f);
            uvs[2] = new Vector2(0.0f, 0.0f);
            uvs[3] = new Vector2(1.0f, 0.0f);

            var tris = new int[6];
            tris[0] = 0;
            tris[1] = 1;
            tris[2] = 2;
            tris[3] = 1;
            tris[4] = 3;
            tris[5] = 2;

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.name = "generated_tracker_mesh";

            return mesh;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var nyTrackerTarget = target as NyImageTracker;

            if (nyTrackerTarget.trackerImage != nyTrackerTarget.lastTexture)
            {
                OnTextureChange();
                UpdateMeshShape(nyTrackerTarget.physicalSize, nyTrackerTarget.editScaler);
            }
            else if (nyTrackerTarget.physicalSize != nyTrackerTarget.lastTrackerSize)
            {
                if (nyTrackerTarget.physicalSize.x != nyTrackerTarget.lastTrackerSize.x)
                    nyTrackerTarget.physicalSize.y = nyTrackerTarget.physicalSize.x * nyTrackerTarget.sizeRatio;
                else if (nyTrackerTarget.physicalSize.y != nyTrackerTarget.lastTrackerSize.y) nyTrackerTarget.physicalSize.x = nyTrackerTarget.physicalSize.y / nyTrackerTarget.sizeRatio;

                nyTrackerTarget.lastTrackerSize = nyTrackerTarget.physicalSize;
                UpdateMeshShape(nyTrackerTarget.physicalSize, nyTrackerTarget.editScaler);
            }
            else if(nyTrackerTarget.editScaler != nyTrackerTarget.lastEditScaler)
            {
                nyTrackerTarget.lastEditScaler = nyTrackerTarget.editScaler;
                UpdateMeshShape(nyTrackerTarget.physicalSize, nyTrackerTarget.editScaler);
            }
        }

        private void OnTextureChange()
        {
            var targetScript = (NyImageTracker)target;
            GameObject targetObj = targetScript.gameObject;

            if (targetScript.trackerImage != targetScript.lastTexture)
            {
                targetScript.lastTexture = targetScript.trackerImage;

                targetObj.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = targetScript.lastTexture;

                // change size ratio
                Vector2 textureSize = GetTextureSize(targetScript.lastTexture);

                Debug.Log("TEXTURE SIZE: " + textureSize);

                targetScript.sizeRatio = (float)textureSize.y / (float)textureSize.x;

                // landscape
                if(textureSize.x > textureSize.y)
                    targetScript.physicalSize = new Vector2(1.0f, targetScript.sizeRatio);
                else // portrait
                    targetScript.physicalSize = new Vector2(1.0f / targetScript.sizeRatio, 1.0f);

                targetObj.transform.localScale = Vector3.one;

                // update mesh
                targetScript.lastTrackerSize = targetScript.physicalSize;
            }
        }

        private void UpdateMeshShape(float meshWidth, float meshHeight)
        {
            // check if there's mesh
            GameObject targetObj = ((NyImageTracker)target).gameObject;

            if (targetObj.GetComponent<MeshFilter>().sharedMesh == null) targetObj.GetComponent<MeshFilter>().sharedMesh = CreateTrackerMesh();

            // update vertices
            Mesh targetMesh = targetObj.GetComponent<MeshFilter>().sharedMesh;

            var verts = new Vector3[4];
            verts[0] = new Vector3(-0.5f * meshWidth, 0.0f, 0.5f * meshHeight);
            verts[1] = new Vector3(0.5f * meshWidth, 0.0f, 0.5f * meshHeight);
            verts[2] = new Vector3(-0.5f * meshWidth, 0.0f, -0.5f * meshHeight);
            verts[3] = new Vector3(0.5f * meshWidth, 0.0f, -0.5f * meshHeight);

            targetMesh.vertices = verts;
            targetMesh.RecalculateBounds();
        }

        private void UpdateMeshShape(Vector2 meshSize, float scaler) => UpdateMeshShape(meshSize.x * scaler, meshSize.y * scaler);

        private Vector2 GetTextureSize(Texture2D targetTexture)
        {
            Vector2 returnValue = Vector2.zero;

            var assetPath = AssetDatabase.GetAssetPath(targetTexture);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                var args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);

                returnValue.x = (int)args[0];
                returnValue.y = (int)args[1];

                return returnValue;
            }

            return new Vector2(-1.0f, -1.0f);
        }
    }
}
