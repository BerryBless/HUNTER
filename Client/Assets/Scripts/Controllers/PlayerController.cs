using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    public Grid _grid;
    public Vector3 _sprightCorrecrion = new Vector3(0.5f, 0, 0);
    [SerializeField]
    private float _speed = 6f;  // 이동스피드

    // 타일맵에서 있는 좌표 - 밖에서 계산할것, 월드좌표로 계산안함
    Vector3Int _cellPos = Vector3Int.zero; 
    MoveDir _dir = MoveDir.Idle;       // 이동방향
    bool _isMoving = false;// 판정좌표는 이동했으나, 스프라이트는 움직이는 중

    void Start()
    {
        transform.position = _grid.CellToWorld(_cellPos) + _sprightCorrecrion;
    }

    void Update()
    {
        GetDirInput();
        UpdateIsMoveing();
        UpdateMoving();
    }

    // 키보드 입력으로 방향 결정
    void GetDirInput()
    {
        float axisV = Input.GetAxisRaw("Vertical");     // 수직
        float axisH = Input.GetAxisRaw("Horizontal");   // 수평
        if (axisV != 0)
        {
            _dir = axisV > 0 ? MoveDir.Up : MoveDir.Down;   // + : -
        }
        else if (axisH != 0)
        {
            _dir = axisH > 0 ? MoveDir.Right : MoveDir.Left;// + : -
        }
        else
        {
            _dir = MoveDir.Idle;
        }
    }

    // 오브젝트 판정 실제로 이동
    void UpdateIsMoveing()
    {
        if(_isMoving == false)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _cellPos += Vector3Int.up;
                    _isMoving = true;
                    break;
                case MoveDir.Down:
                    _cellPos += Vector3Int.down;
                    _isMoving = true;
                    break;
                case MoveDir.Left:
                    _cellPos += Vector3Int.left;
                    _isMoving = true;
                    break;
                case MoveDir.Right:
                    _cellPos += Vector3Int.right;
                    _isMoving = true;
                    break;

            }
        }
    }

    // 유니티 스프라이트 이동
    void UpdateMoving()
    {
        if (_isMoving == false) return;

        Vector3 destPos = _grid.CellToWorld(_cellPos) + _sprightCorrecrion; // 목적지
        Vector3 moveDir = destPos - transform.position;                     // 목적지까지의 방향백터

        // 도착이냐
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            _isMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime; // 정규화 방향으로 스피드만큼 이동
            _isMoving = true;
        }
    }
}
