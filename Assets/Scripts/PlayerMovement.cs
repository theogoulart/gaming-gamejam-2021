using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rig;
    private Animator _anim;
    private Vector2 _direction;
    private Vector2 _lastXDirection = Vector2.right;
    private Vector2 _dashDirection = Vector2.zero;
    private Vector2 _wallJumpDirection;

    private bool _jump;
    private bool _isMovementFreezed;
    private bool _isDashing;
    private bool _isDashingEnabled = true;
    private bool _isJumping;
    private bool _isWallJumping;
    private bool _isWallSliding;
    private bool _isEarlyJumpEnabled;
    private bool _hasEarlyJumped;

    public LayerMask floorLayer;
    public Vector2 direction
    { 
        get { return _direction; }
        set { _direction = value; }
    }

    public float speed;
    public float jumpForce;
    public float wallJumpSpeed;
    public float wallJumpInterval;

    // Start is called before the first frame update
    void Start()
    {
        _rig = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void Update() {
        CheckEarlyJump();
        OnInput();
    }

    void FixedUpdate()
    {
        CheckJump();
        Move();
        WallSliding();
        Dash();
        WallJump();
    }

    void CheckEarlyJump()
    {
        if (!_isEarlyJumpEnabled) {
            return;
        }

        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0,0.5f,0), new Vector3(.3f,.4f,0), 0, floorLayer);
        if (hit != null && Input.GetKeyDown(KeyCode.Space)) {
            _hasEarlyJumped = true;
            Debug.Log(hit.name);
        }
    }

    void CheckJump()
    {
        if (_jump) {
            _jump = false;
            bool isGrounded = IsGrounded();
            if (isGrounded) {
                _rig.velocity = Vector2.zero;
                Jump();
                return;
            }

            bool isTouchingWall = IsTouchingWall();
            if (isTouchingWall) {
                _rig.velocity = Vector2.zero;
                StartCoroutine(OnWallJump());
                return;
            }
        }
    }

    bool CheckTouchingGround()
    {
        return true;
    }

    void Dash()
    {
        if (_isDashing) {
            if (_dashDirection == Vector2.zero) {
                if (IsGrounded()) {
                    if (_direction.x > 0) {
                        _dashDirection = Vector2.right;
                    } else if (_direction.x < 0) {
                        _dashDirection = Vector2.left;
                    } else if (_direction == Vector2.up) {
                        _dashDirection = Vector2.up;
                    } else {
                        _dashDirection = _lastXDirection;
                    }
                } else {
                    _dashDirection = _direction;
                }
            }

            _rig.velocity = _dashDirection * 600f * Time.deltaTime;
        }
    }

    void Die()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/char/char_death", transform.position);
        GameManager.instance.GameOver();
    }

    void ExecuteEarlyJump()
    {
        if (_hasEarlyJumped) {
            Debug.Log("this is a early jump");
            Jump();
        }
    }

    bool IsGrounded()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0,0.23f,0), new Vector3(.35f,.2f,0), 0, floorLayer);
        if (hit != null) {
            return true;
        }

        return false;
    }

    bool IsTouchingWall()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0,0.1f,0), new Vector3(0.85f,.2f,0), 0, floorLayer);
        if (hit != null) {
            return true;
        }

        return false;
    }

    void Jump()
    {
        StartCoroutine(OnJump());
        _rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Move()
    {
        if (_isMovementFreezed || _isDashing) {
            return;
        }

        if (_isWallSliding) {
            _rig.velocity = new Vector2(_rig.velocity.x, -0.5f);
            return;
        }

        _rig.velocity = new Vector2(_direction.x * speed, _rig.velocity.y);

        // idle
        if (_direction == Vector2.zero) {
            _anim.SetInteger("transition", 0);
            return;
        } else if (IsGrounded()) {
            _anim.SetInteger("transition", 1);
        }

        _lastXDirection = _direction.x > 0 ? Vector2.right : Vector2.left;
        transform.eulerAngles = _direction.x > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);

    }

    private void OnCollisionEnter2D(Collision2D other) {
        _isDashingEnabled = true;
        _isEarlyJumpEnabled = false;
        _isJumping = false;
        ExecuteEarlyJump();
        FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/char/char_land", transform.position);
    }

    private void OnDrawGizmos() {
        // EarlyJump
        // Gizmos.DrawWireCube(transform.position - new Vector3(0,0.5f,0), new Vector3(.3f,.4f,0));
        Gizmos.DrawWireCube(transform.position - new Vector3(0,0.5f,0), new Vector3(.3f,.4f,0));
        Gizmos.color = Color.green;
        // Grounded
        Gizmos.DrawWireCube(transform.position - new Vector3(0,0.23f,0), new Vector3(.35f,.2f,0));
        // WallCling
        Gizmos.DrawWireCube(transform.position - new Vector3(0,0.1f,0), new Vector3(0.85f,.2f,0));
    }

    void OnInput()
    {
        _direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if (_isDashingEnabled && Input.GetKeyDown(KeyCode.X)) {
            StartCoroutine(OnDash());
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            _jump = true;
            return;
        }
    }

    IEnumerator OnDash()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/char/char_dash", transform.position);
        _isDashingEnabled = false;
        _isMovementFreezed = true;
        yield return new WaitForSeconds(0.1f);
        _isMovementFreezed = false;
        _isDashing = true;
        yield return new WaitForSeconds(0.15f);
        _dashDirection = Vector2.zero;
        _isDashing = false;
    }

    IEnumerator OnJump()
    {
        _hasEarlyJumped = false;
        _isJumping = true;
        FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/char/char_jump", transform.position);
        yield return new WaitForSeconds(0.2f);
        _isEarlyJumpEnabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard")) {
            Die();
            return;
        }

        if (other.CompareTag("Stone")) {
            other.GetComponent<Stone>().Pick();
            return;
        }
    }

    IEnumerator OnWallJump()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0,0.1f,0), new Vector3(0.85f,.2f,0), 0, floorLayer);
        Vector2 closestPoint = hit.ClosestPoint(transform.position);
        _wallJumpDirection = closestPoint.x > transform.position.x ? (Vector2.up*2 + Vector2.left) : (Vector2.up*2 + Vector2.right);
        _isWallJumping = true;
        _isMovementFreezed = true;
        FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/char/char_jump", transform.position);
        yield return new WaitForSeconds(wallJumpInterval);
        _isWallJumping = false;
        _isMovementFreezed = false;
    }

    void WallJump()
    {
        if (!_isWallJumping) {
            return;
        }

        Debug.Log(_wallJumpDirection);
        _rig.AddForce(_wallJumpDirection * wallJumpSpeed * Time.deltaTime, ForceMode2D.Impulse);
    }

    void WallSliding()
    {
        if (IsTouchingWall() && !IsGrounded() && _rig.velocity.y < 0 && _direction.x != 0) {
            _isWallSliding = true;
            return;
        }

        _isWallSliding = false;
    }
}
