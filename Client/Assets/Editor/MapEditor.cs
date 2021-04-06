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

    private static int[,] MakeAdj(ref int[,] map, int nodeCount, int sizeY, int sizeX)
    {
        int[,] adj = new int[nodeCount + 1, nodeCount + 1];
        int[] dy = new int[] { 0, -1 };
        int[] dx = new int[] { -1, 0 };

        for (int i = 1; i <= nodeCount; i++)
            for (int j = 1; j <= nodeCount; j++)
            {
                adj[i, j] = 0x3f3f3f3f;
                if (i == j) adj[i, i] = 0;
            }
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                if (map[y, x] == 0) continue;
                for (int i = 0; i < 2; i++)
                {
                    if (y + dy[i] < 0 || x + dx[i] < 0 ||
                        y + dy[i] >= sizeY || x + dx[i] >= sizeX) continue;

                    if (map[y + dy[i], x + dx[i]] != 0 &&
                        map[y + dy[i], x + dx[i]] != map[y, x])
                    {
                        // TODO Cost 책정
                        adj[map[y + dy[i], x + dx[i]], map[y, x]] = 1;
                        adj[map[y, x], map[y + dy[i], x + dx[i]]] = 1;
                    }
                }
            }
        }

        return adj;
    }

    private static void GenerateMap(string pathPrefix)
    {
        // 프리팹의 맵 찾기
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");
        Dictionary<string, int> nodeNumber = new Dictionary<string, int>();

        int[,] map;
        int nodeCount = 0;
        int[,] adj;
        int[,] floydPath;

        // 순회
        foreach (GameObject go in gameObjects)
        {
            #region MakeMap
            Tilemap tmBase = go.FindChild<Tilemap>("Tilemap_Base", true);       // 맵전체 크기정보
            Tilemap tmColl = go.FindChild<Tilemap>("Tilemap_Collision", true);  // 충돌체크 정보가있는 콜리션

            int sizeX = tmBase.cellBounds.xMax - tmBase.cellBounds.xMin + 1;
            int sizeY = tmBase.cellBounds.yMax - tmBase.cellBounds.yMin + 1;

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
                        map[tmBase.cellBounds.yMax - y, x - tmBase.cellBounds.xMin] = n;
                    }
                    // 없으면 0
                    else
                    {
                        map[tmBase.cellBounds.yMax - y, x - tmBase.cellBounds.xMin] = 0;
                    }
                }
            }
            #endregion

            #region MakePath
            // 최단거리 캐싱
            adj = MakeAdj(ref map, nodeCount, sizeY, sizeX);
            floydPath = new int[nodeCount + 1, nodeCount + 1];

            // memset 없나?
            for (int i = 1; i <= nodeCount; i++)
                for (int j = 1; j <= nodeCount; j++)
                    floydPath[i, j] = -1;

            // FLOYD algorithm
            for (int k = 1; k <= nodeCount; k++)
                for (int i = 1; i <= nodeCount; i++)
                    for (int j = 1; j <= nodeCount; j++)
                        if (adj[i, j] > adj[i, k] + adj[k, j])
                        {
                            floydPath[i, j] = k;
                            adj[i, j] = adj[i, k] + adj[k, j];
                        }
            #endregion

            #region Print
            // 추출한거 출력하기
            using (var writer = File.CreateText($"{pathPrefix}/{go.name}.txt"))
            {
                #region PrintMap
                writer.WriteLine(tmBase.cellBounds.xMin);
                writer.WriteLine(tmBase.cellBounds.xMax);
                writer.WriteLine(tmBase.cellBounds.yMin);
                writer.WriteLine(tmBase.cellBounds.yMax);

                // 위에서 아래로
                for (int y = 0; y < sizeY; y++)
                {
                    // 왼쪽에서 오른쪽으로
                    for (int x = 0; x < sizeX; x++)
                    {
                        writer.Write($"{map[y, x]} ");
                    }
                    writer.WriteLine();
                }
                #endregion

                #region PrintPath
                {
                    // 최단거리 출력
                    // TODO 갈수없는 곳(고립된곳) 처리
                    writer.WriteLine($"{nodeCount}");
                    List<int> path = new List<int>();
                    for (int i = 1; i <= nodeCount; i++)
                    {
                        for (int j = 1; j <= nodeCount; j++)
                        {
                            //writer.Write($"{i} {j} ");
                            // path.size path.list[]
                            if (i == j) { writer.WriteLine($"1 {j} "); continue; }
                            path.Clear();
                            path.Add(i); // here
                            GetPath(i, j, ref path, ref floydPath);
                            path.Add(j); // dest
                            writer.Write($"{path.Count} ");
                            foreach (int v in path)
                            {
                                writer.Write($"{v} ");
                            }

                            writer.WriteLine();
                        }
                    }
                }

                #endregion
            }
            #endregion
        }
    }

    public static void GetPath(int u, int v, ref List<int> path, ref int[,] dp)
    {
        if (dp[u, v] != -1)
        {
            GetPath(u, dp[u, v], ref path, ref dp);
            path.Add(dp[u, v]);
            GetPath(dp[u, v], v, ref path, ref dp);
        }
    }
#endif
}
