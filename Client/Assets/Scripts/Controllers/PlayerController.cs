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
            UpdateAnimation();                  // 바뀔때 자동업데이트
        }
    }
    private MoveDir _dir = MoveDir.Down;       // 이동방향
    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value) return; _dir = value;
            UpdateAnimation();                 // 바뀔떄 자동업데이트
        }
    }
    bool _isKeyPress = false;// 이동키 눌렀는지
    void Start()
    {
        //초기화
        Init();
    }

    void Init()
    {
        transform.position = _grid.CellToWorld(_cellPos) + _sprightCorrecrion;
        _sprite = GetComponent<SpriteRenderer>();

        Dir = MoveDir.Down;
        State = PlayerState.Idle;
    }

    void Update()
    {
        PlayerUpdateController();
    }
    
    private void LateUpdate()
    {
        // 카메라 따라가게
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
    void PlayerUpdateController()
    {
        switch (State)
        {
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
            case PlayerState.Attack:
                UpdateAttack();
                break;
        }

    }
    // STATE가 IDLE일떄 업데이트 할것들
    void UpdateIdle()
    {
        GetDirInput();
        if(_isKeyPress == true)
        {
            State = PlayerState.Moving;
        }
    }

    // STATE가 Move일때 업데이트 할것들
    // 스르륵 움직이게!
    void UpdateMoving()
    {
        Vector3 destPos = _grid.CellToWorld(_cellPos) + _sprightCorrecrion; // 목적지
        Vector3 moveDir = destPos - transform.position;                     // 목적지까지의 방향백터

        // 도착이냐
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime; // 정규화 방향으로 스피드만큼 이동
        }
    }
    // STATE가 Attack일때 업데이트 할것들
    void UpdateAttack() { 
        // TODO 공격!
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
            _isKeyPress = false;
        }
    }

    // 오브젝트 판정 실제로 이동
    void MoveToNextPos()
    {
        // 이동키 땠을때
        if (_isKeyPress == false)
        {
            State = PlayerState.Idle;
            return;
        }

        Vector3Int destPos = _cellPos;
        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        // 충돌체크
        if (Managers.Map.CanGo(destPos) == true)
        {
            //TODO 오브젝트 충돌
            _cellPos = destPos;
        }
    }


    void UpdateAnimation()
    {
        if (State == PlayerState.Idle)
        {
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
        else if(State == PlayerState.Attack)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = true;
                    break;
                default:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
    }
}
