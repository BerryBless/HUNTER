using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Define;

public class PlayerController : BaseController
{
    private List<GameObject> _pathUi = new List<GameObject>();
    private Vector3Int _destCellPos;
    private List<Vector3Int> _movePath = null;
    //bool _isKeyPress = false;// 이동키 눌렀는지
    private void LateUpdate()
    {
        // 카메라 따라가게
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
    #region override
    protected override void Init()
    {
        base.Init();
        //Camera.main.orthographicSize = 30;
        //_speed = 30;
        //transform.localScale = new Vector3(5, 5, 0);
    }
    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                //GetDirInputKeyboard();
                GetInputMouse();
                UpdateIdle();
                break;
            case CreatureState.Moving:
                //GetDirInputKeyboard();
                GetInputMouse();
                UpdateMoving();
                break;
            case CreatureState.Attack:
                UpdateAttack();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }

    }

    protected override void UpdateIdle()
    {
    }
    protected override void UpdateAttack()
    {
        // TODO 공격!
    }
    protected override void UpdateDead()
    {
    }
    protected override void UpdateAnimation()
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

    protected override void MoveToNextPos()
    {
        if (_movePath == null)
        {
            return;
        }
        if (_movePath.Count <= 1)
        {
            _movePath = null;
            ClearPath();
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = _movePath[1];
        _movePath.RemoveAt(0);
        Managers.Resource.Destroy(_pathUi[0]);
        _pathUi.RemoveAt(0);

        Vector3Int moveCellDir = nextPos - CellPos;

        Dir = GetDirFromVector(moveCellDir);

        if (Managers.Map.CanGo(nextPos))
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }
    #endregion

    // 키보드 입력으로 방향 결정
    //private void GetDirInputKeyboard()
    //{
    //    //_isKeyPress = true;
    //    float axisV = Input.GetAxisRaw("Vertical");     // 수직
    //    float axisH = Input.GetAxisRaw("Horizontal");   // 수평
    //    if (axisV != 0)
    //    {
    //        Dir = axisV > 0 ? MoveDir.Up : MoveDir.Down;   // + : -
    //    }
    //    else if (axisH != 0)
    //    {
    //        Dir = axisH > 0 ? MoveDir.Right : MoveDir.Left;// + : -
    //    }
    //    else
    //    {
    //        //_isKeyPress = false;
    //    }
    //}

    private void GetInputMouse()
    {

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _destCellPos = Managers.Map.CurrentGrid.WorldToCell(mousePos);

            if (Managers.Map.CanGo(_destCellPos))
            {
                State = CreatureState.Moving;
                _movePath = Managers.Map.FindPath(CellPos, _destCellPos);
                ViewPath();
            }
        }
    }

    private void ViewPath()
    {

        ClearPath();

        for (int i = 0; i < _movePath.Count; i++)
        {
            GameObject go = Managers.Resource.Instantiate("Ui/Path");
            go.transform.position = _movePath[i] + new Vector3(0.5f, 0.5f, 0);
            //  GameObject tm = FindChild()
            _pathUi.Add(go);
        }
    }

    private void ClearPath()
    {
        foreach (GameObject go in _pathUi)
        {
            Managers.Resource.Destroy(go);
        }
        _pathUi.Clear();
    }
}
