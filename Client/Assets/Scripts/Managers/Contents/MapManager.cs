using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager
{
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

}
