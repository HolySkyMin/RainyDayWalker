using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hard Edge를 가진 모델은 같은 위치의 vertex라도 다른 Normal을 가지고 있습니다.
// 따라서, Normal 방향으로 이동시키는 Inverted Hull 방식의 Outline이 끊겨 보이는 문제가 발생합니다.
// 이에, 같은 위치의 vertex의 평균 normal을 tangent에 넣어주는 방식으로 평균 normal을 전달합니다.
// 이후, normal 대신 tangent를 사용하는 OutlineByTangent Shader에 의해 외곽선이 정상적으로 표현됩니다.

// 개선할 점들:
// 1. 모든 Visible Object에 Script를 적용하는 것은 번거로움
// 1.1. GetComponentsInChild로 해도 되겠으나 Awake에서 너무 많은 일을 하는 게 아닐까
// 1.2. 모델을 미리 수정해두는 것도 방법 (개발 도중에는 Awake에서 적용하다가 나중에 바꿔도 될 것 같음)
// 2. Script를 적용하지 않은 Object들은 정확한 Tangent값에 의해 이상한 Outline이 그려짐.
// 2.1. Layer Mask를 조정하면 될 것 같다. (조금 더 알아보고 수정)
// 2.2. 사실 모든 Object에 외곽선을 그릴 거라면 상관없음

namespace RainyDay
{
  public class NormalAverager : MonoBehaviour
  {
    private MeshFilter meshFilter;

    private void Awake() {
      // Mesh Filter 혹은 SkinnedMeshRenderer의 mesh를 패치합니다.
      if (GetComponent<MeshFilter>()) {
        setTangentByNormalAverage(GetComponent<MeshFilter>().mesh);
      } else if (GetComponent<SkinnedMeshRenderer>()) {
        var mesh = Instantiate(GetComponent<SkinnedMeshRenderer>().sharedMesh);
        setTangentByNormalAverage(mesh);
        GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
      }
    }

    private void setTangentByNormalAverage(Mesh mesh) {
      // vertex 위치를 key로 하는 dictionary를 통해 같은 위치의 vertex id를 정리합니다.
      Dictionary<Vector3, List<int>> vertexMap = new Dictionary<Vector3, List<int>>();
      for (int i = 0; i < mesh.vertices.Length; i++)
      {
        Vector3 vertex = mesh.vertices[i];
        if (!vertexMap.ContainsKey(vertex))
          vertexMap.Add(vertex, new List<int>());
        vertexMap[vertex].Add(i);
      }

      // mesh의 normal과 새로운 tangent를 준비합니다.
      var normals = mesh.normals;
      var newTangent = new Vector4[normals.Length];

      // 각 vertex에 대해, 모든 vertex id의 normal을 평균낸 뒤 각 tangent에 넣습니다.
      foreach (var vertex in vertexMap)
      {
        Vector3 averageNormal = Vector3.zero;
        foreach (var i in vertex.Value)
        {
          averageNormal += normals[i];
        }
        averageNormal /= vertex.Value.Count;

        foreach (var i in vertex.Value)
        {
          newTangent[i] = averageNormal;
        }
      }

      // 이후, mesh의 tangent를 덮어씌웁니다.
      mesh.tangents = newTangent;
    }
  }
}