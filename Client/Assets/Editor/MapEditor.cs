using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

// 유니티 에디터에서만 사용 가능!
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/GenerateMap %#m")] // Ctrl + Shift + m
    private static void GenerateMap()
    {
        GenerateMap("Assets/Resources/Map");    // 클라이언트에서 쓸거
        GenerateMap("../Common/MapData");       // 서버에서 쓸꺼
    }

    private static List<List<int>> MakeAdj(int[,] map,int nodeCount, out int cnt)
    {
        List<List<int>> adj = new List<List<int>>();
        // c++ 처럼 한번에 초기화 하는방법 없나?
        for (int i = 0; i <= nodeCount; i++)
            adj.Add(new List<int>());
        cnt = 0;
        // TODO 간선 만들기

        // TEMP 출력확인
        cnt = 7;
        adj[1].Add(3);
        adj[1].Add(2);
        adj[1].Add(4);
        adj[2].Add(5);
        adj[4].Add(7);
        adj[4].Add(5);
        adj[5].Add(6);

        return adj;
    }

    private static void GenerateMap(string pathPrefix)
    {
        // 프리팹의 맵 찾기
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");
        Dictionary<string, int> nodeNumber = new Dictionary<string, int>();

        int[,] map;
        int nodeCount = 0;


        // 순회
        foreach (GameObject go in gameObjects)
        {
            Tilemap tmBase = go.FindChild<Tilemap>("Tilemap_Base", true);       // 맵전체 크기정보
            Tilemap tmColl = go.FindChild<Tilemap>("Tilemap_Collision", true);  // 충돌체크 정보가있는 콜리션

            // 추출한거 출력하기
            using (var writer = File.CreateText($"{pathPrefix}/{go.name}.txt"))
            {
                writer.WriteLine(tmBase.cellBounds.xMin);
                writer.WriteLine(tmBase.cellBounds.xMax);
                writer.WriteLine(tmBase.cellBounds.yMin);
                writer.WriteLine(tmBase.cellBounds.yMax);

                #region MakeMap
                int sizeX = tmBase.cellBounds.xMax - tmBase.cellBounds.xMin +1;
                int sizeY = tmBase.cellBounds.yMax - tmBase.cellBounds.yMin +1;

                map = new int[sizeY, sizeX];

                // 위에서 아래로
                for (int y = tmBase.cellBounds.yMax; y >= tmBase.cellBounds.yMin; y--)
                {
                    // 왼쪽에서 오른쪽으로
                    for (int x = tmBase.cellBounds.xMin; x <= tmBase.cellBounds.xMax; x++)
                    {
                        // 추출해서
                        TileBase tileColl = tmColl.GetTile(new Vector3Int(x, y, 0));
                        
                        if (tileColl != null)
                        {
                            Sprite t = tmColl.GetSprite(new Vector3Int(x, y, 0));
                            int n;
                            if (nodeNumber.TryGetValue(t.name, out n) == false)
                            {
                                nodeCount++;
                                n = nodeCount;
                                nodeNumber.Add(t.name, n);
                            }
                            writer.Write($"{ n} ");
                            map[tmBase.cellBounds.yMax - y, x - tmBase.cellBounds.xMin] = n;
                        }
                        // 없으면 0
                        else
                        {
                            writer.Write("0 ");
                            map[tmBase.cellBounds.yMax - y, x - tmBase.cellBounds.xMin] = 0;
                        }
                    }
                    writer.WriteLine();
                }
                #endregion
                #region adj
                // 간선출력
                int adjCount;
                List<List<int>> adj = MakeAdj(map, nodeCount, out adjCount);

                writer.WriteLine($"{nodeCount} {adjCount}");
                foreach(List<int> list in adj)
                    foreach(int i in list)
                        writer.WriteLine($"{adj.IndexOf(list)} {i}");
                #endregion
            }
        }
    }
#endif
}
