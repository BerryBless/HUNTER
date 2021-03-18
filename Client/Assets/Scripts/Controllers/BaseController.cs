using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
    #region Values
    protected Animator _animator;                               // 크리쳐의 애니메이터
    protected SpriteRenderer _sprite;                           // 크리쳐의 스프라이트
    [SerializeField]
    protected Vector3 _sprightCorrecrion = new Vector3(0.5f, 0.5f, 0);// 스프라이트 오차값
    [SerializeField]
    protected float _speed = 6f;  // 이동스피드
    #endregion

    #region Properties
    private CreatureState _state = CreatureState.Idle;      // 오브젝트 상태 Idle/Moveing/Attack/Dead
    protected CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value) return; _state = value;
            UpdateAnimation();                  // 바뀔때 자동업데이트
        }
    }
    private MoveDir _dir = MoveDir.Down;       // 이동방향
    protected MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value) return; _dir = value;
            UpdateAnimation();                 // 바뀔떄 자동업데이트
        }
    }

    private Vector3Int _cellPos;            // 타일맵의 좌표로 계산
    protected Vector3Int CellPos
    {
        get { return _cellPos; }
        set
        {
            _cellPos.x = value.x;
            _cellPos.y = value.y;
            _cellPos.z = value.z;
        }
    }
    #endregion

    #region UnityEvent
    void Start()
    {
        Init();
    }
    void Update()
    {
        UpdateController();
    }
    #endregion

    #region Virtual Methods
    protected virtual void Init() {
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos);
        transform.position = pos;

        UpdateAnimation();
    }
    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Attack:
                UpdateAttack();
                break;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + _sprightCorrecrion; // 목적지
        Vector3 moveDir = destPos - transform.position;                     // 목적지까지의 방향백터

        // 도착이냐
        float dist = moveDir.magnitude;
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            // Debug.Log($"destPos({CellPos})");
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime; // 정규화 방향으로 스피드만큼 이동
        }
    }
    protected virtual void UpdateAttack() { }
    protected virtual void UpdateDead() { }
    protected virtual void UpdateAnimation()
    {
        if (State == CreatureState.Idle)
        {
            switch (Dir)
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
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
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
                    _sprite.flipX = true; // 오른쪽 바라보는거 뒤집기
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.Attack)
        {
            switch (Dir)
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
        else if (State == CreatureState.Dead)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("DEAD_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("DEAD_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("DEAD_RIGHT");
                    _sprite.flipX = true;
                    break;
                default:
                    _animator.Play("DEAD_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
    }

    protected virtual void MoveToNextPos() { }

    #endregion

    #region Methods
    // Dir 방향의 한칸앞 셀포지션 리턴
    public Vector3Int GetFrontCellPos()
    {
        Vector3Int frontPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                frontPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                frontPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                frontPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                frontPos += Vector3Int.right;
                break;
        }
        return frontPos;
    }

    // 벡터에 따른 방향 Dir 정하기
    public MoveDir GetDirFromVector(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        return MoveDir.Down;
    }
    #endregion
}

