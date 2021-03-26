using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager
{
    #region Map
    public Grid CurrentGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    public int SizeX { get { return MaxX - MinX + 1; } }
    public int SizeY { get { return MaxY - MinY + 1; } }

    bool[,] _collision;//장애물 위치 저장할 불리언 배열 (y, x)

    // 갈수있냐 (충돌할 물체가 있냐)
    public bool CanGo(Vector3Int cellPos)
    {
        return CanGo(new Vector2Int(cellPos.x, cellPos.y));
    }
    public bool CanGo(Vector2Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x + 1 > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;
        return !_collision[y, x];
    }

    public bool LoadMap(int mapId)
    {
        // 맵이 있으면 삭제
        DestroyMap();
        // 맵 프리팹 이름을 Map_001, Map_002... 
        string mapName = "Map_" + mapId.ToString("000");
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}");
        if (go == null) return false;

        go.name = "Map";

        // Tilemap_Collision 안보이게 비활성화
        GameObject collision = Util.FindChild(go, "Tilemap_Collision", true);
        if (collision != null)
            collision.SetActive(false);

        // 맵좌표 계산할 그리드
        CurrentGrid = go.GetComponent<Grid>();

        // Collision 관련 파일 리소스에 있음(원본이 2개)
        TextAsset txt = Managers.Resource.Load<TextAsset>($"Map/{mapName}");
        if (txt == null) return false;

        StringReader reader = new StringReader(txt.text);

        // 맵크기
        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        // 충돌 판정위한  불리언 배열..
        // TODO 비트마스크로?
        _collision = new bool[SizeY, SizeX];

        // 맵순회
        for (int y = 0; y < SizeY; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < SizeX; x++)
            {
                // 1이면 충돌체 있음
                _collision[y, x] = (line[x] == '1' ? true : false);
            }
        }
        return true;
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }
    #endregion

    #region PathFinding
    public struct Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;

        public static bool operator ==(Pos l, Pos r)
        {
            return l.X == r.X && l.Y == r.Y;
        }
        public static bool operator !=(Pos l, Pos r)
        {
            return !(l == r);
        }

        public override bool Equals(object obj)
        {
            return (Pos)obj == this;
        }

        public override int GetHashCode()
        {
            long value = (Y << 32) | X;
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }

    // cell에서 pos로
    Pos Cell2Pos(Vector2Int cell)
    {
        // CellPos -> ArrayPos
        return new Pos(MaxY - cell.y, cell.x - MinX);
    }

    // pos 에서  cell로
    Vector2Int Pos2Cell(Pos pos)
    {
        // ArrayPos -> CellPos
        return new Vector2Int(pos.X + MinX, MaxY - pos.Y);
    }

    // U D L R
    int[] _deltaY = new int[] { 1, -1, 0, 0, 1, 1, -1, -1 };
    int[] _deltaX = new int[] { 0, 0, -1, 1, -1, 1, -1, 1 };
    int[] _cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 };
    // U D L R UL UR DL DR
    //int[] _deltaY = new int[] { 1, -1, 0, 0, 1, 1, -1, -1 };
    //int[] _deltaX = new int[] { 0, 0, -1, 1, -1, 1, -1, 1 };
    //int[] _cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 };

    public List<Vector2Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, bool ignoreDestCollision = false)
    {
        return FindPath(new Vector2Int(startCellPos.x, startCellPos.y), new Vector2Int(destCellPos.x, destCellPos.y), ignoreDestCollision);
    }
    public List<Vector2Int> FindPath(Vector2Int startCellPos, Vector2Int destCellPos, bool ignoreDestCollision = false)
    {
        // Astar
        //TODO : 못가는 곳 탐색 근처 경로로 이동..
        List<Pos> path = new List<Pos>();

        // 점수 매기기
        // F = G + H
        // F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
        // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
        // H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

        // (y, x) 이미 방문했는지 여부 (방문 = closed 상태)
        HashSet<Pos> closedList = new HashSet<Pos>(); // CloseList

        // (y, x) 가는 길을 한 번이라도 발견했는지
        // 발견X => MaxValue
        // 발견O => F = G + H
        Dictionary<Pos, int> openList = new Dictionary<Pos, int>();
        Dictionary<Pos, Pos> parent = new Dictionary<Pos, Pos>();

        // 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // CellPos -> ArrayPos
        Pos pos = Cell2Pos(startCellPos);
        Pos dest = Cell2Pos(destCellPos);

        int minh = Int32.MaxValue;
        Pos tempPos = Cell2Pos(destCellPos);

        // 시작점 발견 (예약 진행)
        openList.Add(pos, CalcG(0) + CalcH(pos, dest));
        pq.Push(new PQNode() { F = CalcG(0) + CalcH(pos, dest), G = 0, Y = pos.Y, X = pos.X });
        parent.Add(pos, pos);

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다
            PQNode pqNode = pq.Pop();
            Pos node = new Pos(pqNode.Y, pqNode.X);

            // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
            if (closedList.Contains(node))
                continue;

            // 방문한다
            closedList.Add(node);
            // 목적지가 아닌 최단거리 저장
            if (minh > pqNode.F - pqNode.G)
            {
                minh = pqNode.F - pqNode.G;
                tempPos = node;
            }
            // TODO 목적지가 갈 수 없는곳 커팅
            else
            {
                if(CanGo(destCellPos) == false)
                {
                    break;
                }
            }
            // 목적지 도착했으면 바로 종료
            if (node == dest)
                break;



            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
            for (int i = 0; i < _deltaY.Length; i++)
            {
                Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);



                // 유효 범위를 벗어났으면 스킵
                // 벽으로 막혀서 갈 수 없으면 스킵
                if (!ignoreDestCollision || next != dest)
                {
                    if (CanGo(Pos2Cell(next)) == false) // CellPos
                        continue;
                }

                // 이미 방문한 곳이면 스킵
                if (closedList.Contains(next))
                    continue;

                // 비용 계산
                int g = CalcG(pqNode.G, _cost[i]);
                int h = CalcH(next, dest);
                // 다른 경로에서 더 빠른 길 이미 찾았으면 스킵
                int open = 0;
                if (openList.TryGetValue(next, out open) == false)
                    open = Int32.MaxValue;
                if (open < g + h)
                    continue;

                // 예약 진행

                if (openList.TryAdd(next, g + h) == false)
                    openList[next] = g + h;

                pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
                if (parent.TryAdd(next, node) == false)
                    parent[next] = node;
            }
        }
        if (dest != tempPos)
            return CalcCellPathFromParent(parent, tempPos);
        return CalcCellPathFromParent(parent, dest);
    }



    int CalcG(int g, int cost = 0)
    {
        //TODO cost
        return g + cost;
    }
    int CalcH(Pos next, Pos dest)
    {
        int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
        return h;
    }

    // 찾은경로 따라가기
    List<Vector2Int> CalcCellPathFromParent(Dictionary<Pos, Pos> parent, Pos dest)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        Pos pos = dest;
        while (parent[pos] != pos)
        {
            cells.Add(Pos2Cell(pos));
            pos = parent[pos];
        }
        cells.Add(Pos2Cell(pos));
        cells.Reverse();

        return cells;
    }

    #endregion
}
