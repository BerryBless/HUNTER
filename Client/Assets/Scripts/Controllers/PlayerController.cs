using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    // 임시 오브젝트 나중에 커지면 옮길것
    public Grid _grid;          // 실제 게임맵
    public Animator _animator;  // 애니메이션 재생할 애니메이터
    public SpriteRenderer _sprite;// 플레이어 스프라이트 좌우 뒤집기

    public Vector3 _sprightCorrecrion = new Vector3(0.5f, 0, 0);
    [SerializeField]
    private float _speed = 6f;  // 이동스피드

    // 타일맵에서 있는 좌표 - 밖에서 계산할것, 월드좌표로 계산안함
    Vector3Int _cellPos = Vector3Int.zero;

    PlayerState _state = PlayerState.Moving;      //플레이어 상태 가만히/이동/공격
    PlayerState State
    {
        get { return _state; }
        set
        {
            if (_state == value) return; _state = value;
        }
    }
    private MoveDir _dir = MoveDir.Down;       // 이동방향
    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value) return; _dir = value;
        }
    }
    bool _isMoving = false;// 판정좌표는 이동했으나, 스프라이트는 움직이는 중
    bool _isKeyPress = false;// 이동키 눌렀는지
    void Start()
    {
        //초기화

        transform.position = _grid.CellToWorld(_cellPos) + _sprightCorrecrion;
        _sprite = GetComponent<SpriteRenderer>();

        Dir = MoveDir.Down;
        State = PlayerState.Idle;
    }

    void Update()
    {
        GetDirInput();
        UpdateIsMoveing();
        UpdateMoving();
        UpdateAnimation();
    }

    // 키보드 입력으로 방향 결정
    void GetDirInput()
    {
        _isKeyPress = true;
        float axisV = Input.GetAxisRaw("Vertical");     // 수직
        float axisH = Input.GetAxisRaw("Horizontal");   // 수평
        if (axisV != 0)
        {
            Dir = axisV > 0 ? MoveDir.Up : MoveDir.Down;   // + : -
        }
        else if (axisH != 0)
        {
            Dir = axisH > 0 ? MoveDir.Right : MoveDir.Left;// + : -
        }
        else
        {
            State = PlayerState.Idle;
            _isKeyPress = false;
        }
    }

    // 오브젝트 판정 실제로 이동
    void UpdateIsMoveing()
    {
        if (_isKeyPress == true && _isMoving == false)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _cellPos += Vector3Int.up;
                    _isMoving = true; State = PlayerState.Moving;
                    break;
                case MoveDir.Down:
                    _cellPos += Vector3Int.down;
                    _isMoving = true; State = PlayerState.Moving;
                    break;
                case MoveDir.Left:
                    _cellPos += Vector3Int.left;
                    _isMoving = true; State = PlayerState.Moving;
                    break;
                case MoveDir.Right:
                    _cellPos += Vector3Int.right;
                    _isMoving = true; State = PlayerState.Moving;
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
            State = PlayerState.Idle;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime; // 정규화 방향으로 스피드만큼 이동
            _isMoving = true;
        }
    }

    void UpdateAnimation()
    {
        if (State == PlayerState.Idle)
        {
            if (_isMoving == true || _isKeyPress == true) return;
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true; // true일경우 x축을 flip(뒤집다)!
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == PlayerState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true; // true일경우 x축을 flip(뒤집다)!
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
    }
}
