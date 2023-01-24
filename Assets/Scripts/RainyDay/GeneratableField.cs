using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainyDay
{
    public class GeneratableField : MonoBehaviour
    {
        [SerializeField] GameObject waterObject;
        [SerializeField] float tileSize = 10;
        [Header("Prebake Configuration")]
        [SerializeField] int prebakedWidth = 50;
        [SerializeField] int prebakedLength = 100;  // width => z, length => x
        [SerializeField, Range(0, 100)] int prebakedWaterLevel = 50;
        [SerializeField] float prebakedNoiseDensity = 20;
        [SerializeField] float prebakeSeedX, prebakeSeedZ;

        const float MAX_HEIGHT = 0.2f;

        Mesh _mesh;
        Vector3[] _vertices;
        int[] _triangles;
        Vector2[] _uvs;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.EditorApplication.update += OnValidateInternal;
        }

        void OnValidateInternal()
        {
            UnityEditor.EditorApplication.update -= OnValidateInternal;

            if(waterObject != null && waterObject.activeSelf)
                waterObject.transform.localPosition = new Vector3(
                    waterObject.transform.localPosition.x,
                    MAX_HEIGHT * prebakedWaterLevel / 100,
                    waterObject.transform.localPosition.z
                );
        }
#endif

        public void GenerateField()
        {
            GenerateField(prebakedWidth, prebakedLength, prebakedWaterLevel, prebakedNoiseDensity, prebakeSeedX, prebakeSeedZ);
        }

        public void GenerateField(int width, int length, int waterLevel, float noiseDensity, float seedX, float seedZ)
        {
            _mesh = new Mesh()
            {
                name = "Walking Terrain"
            };
            GetComponent<MeshFilter>().mesh = _mesh;

            // Creating Mesh Shape
            _vertices = new Vector3[(width + 1) * (length + 1)];
            _uvs = new Vector2[(width + 1) * (length + 1)];

            var minSize = Mathf.Min(width, length);
            for(int i = 0, z = 0; z <= width; z++)
            {
                for(int x = 0; x <= length; x++, i++)
                {
                    var y = MAX_HEIGHT * Mathf.Clamp01(Mathf.PerlinNoise(x * noiseDensity / minSize + seedX, z * noiseDensity / minSize + seedZ));
                    _vertices[i] = new Vector3(x, y, z);
                    _uvs[i] = new Vector2(x * tileSize / minSize, z * tileSize / minSize);
                }
            }
            
            _triangles = new int[width * length * 6];
            for(int z = 0, vert = 0, tris = 0; z < width; z++, vert++)
            {
                for(int x = 0; x < length; x++, vert++, tris += 6)
                {
                    _triangles[tris] = vert;
                    _triangles[tris + 1] = vert + length + 1;
                    _triangles[tris + 2] = vert + 1;
                    _triangles[tris + 3] = vert + 1;
                    _triangles[tris + 4] = vert + length + 1;
                    _triangles[tris + 5] = vert + length + 2;
                }
            }

            // Updating Mesh Data
            _mesh.Clear();

            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            _mesh.uv = _uvs;

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();

            GetComponent<MeshCollider>().sharedMesh = _mesh;

            // Configuring Water Object
            waterObject.transform.localPosition = new Vector3(length / 2f, MAX_HEIGHT * waterLevel / 100f, width / 2f);
            waterObject.transform.localScale = new Vector3(length / 10f, 0.0001f, width / 10f);
            waterObject.SetActive(true);
        }
    }
}
