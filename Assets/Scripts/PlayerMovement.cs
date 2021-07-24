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
    private CameraShake cameraShake;

    private string _currentColor = "Gray";
    private bool _jump;
    private bool _isMovementFreezed;
    private bool _isDashing;
    private bool _isDashingEnabled = true;
    private bool _isJumping;
    private bool _isWallJumping;
    private bool _isWallSliding;
    private bool _isEarlyJumpEnabled;
    private bool _hasEarlyJumped;

    public GameObject defaultBody;
    public GameObject blueBody;
    public GameObject greenBody;
    public GameObject redBody;
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
        cameraShake = GameObject.FindGameObjectWithTag("CameraPrefab").GetComponent<CameraShake>();
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

    void ChangeColor(string color)
    {
        _currentColor = color;
        blueBody.SetActive(color == "Blue");
        defaultBody.SetActive(color == "Gray");
        greenBody.SetActive(color == "Green");
        redBody.SetActive(color == "Red");
    }

    void CheckEarlyJump()
    {
        if (!_isEarlyJumpEnabled) {
            return;
        }

        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0,0.5f,0), new Vector3(.3f,.4f,0), 0, floorLayer);
        if (hit != null && Input.GetKeyDown(KeyCode.Space)) {
            _hasEarlyJumped = true;
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

            _rig.velocity = _dashDirection * 450f * Time.deltaTime;
        }
    }

    void Die()
    {
        _rig.velocity = Vector2.zero;
        _rig.bodyType = RigidbodyType2D.Kinematic;
        _isMovementFreezed = true;
        FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/char/char_death", transform.position);
        _anim.SetTrigger("die");
        GameManager.instance.GameOver();
    }

    void ExecuteEarlyJump()
    {
        if (_hasEarlyJumped) {
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
        } else {
            _anim.SetInteger("transition", 0);
        }

        _lastXDirection = _direction.x > 0 ? Vector2.right : Vector2.left;
        transform.eulerAngles = _direction.x > 0 ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (IsTouchingWall() || IsGrounded()) {
            _isDashingEnabled = true;
            _isEarlyJumpEnabled = false;
            _isJumping = false;
        }

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
        // Damage detection
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3.up * 0.5f), 0.5f);
        Gizmos.DrawWireSphere(transform.position + (Vector3.right * 0.5f), 0.5f);
        Gizmos.DrawWireSphere(transform.position + (Vector3.down * 0.5f), 0.5f);
        Gizmos.DrawWireSphere(transform.position + (Vector3.left * 0.5f), 0.5f);
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
        yield return new WaitForSeconds(0.15f);
        _isMovementFreezed = false;
        _isDashing = true;
        cameraShake.Shake(0.02f, 0.1f);
        yield return new WaitForSeconds(0.15f);
        _dashDirection = Vector2.zero;
        _isDashing = false;
        if (IsTouchingWall() || IsGrounded()) {
            _isDashingEnabled = true;
        }
    }

    IEnumerator OnJump()
    {
        _hasEarlyJumped = false;
        _isJumping = true;
        FMODUnity.RuntimeManager.PlayOneShot("event:/sfx/char/char_jump", transform.position);
        _anim.SetTrigger("jump");
        yield return new WaitForSeconds(0.2f);
        _isEarlyJumpEnabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard")) {
            Spikes spike = other.GetComponent<Spikes>();
            if (spike != null) {
                if (spike.pointingDirection == Vector3.up) {
                    Collider2D[] col = Physics2D.OverlapCircleAll(transform.position + (Vector3.down * 0.5f), 0.5f);
                    foreach (var item in col)
                    {
                        if (item.name == "UpwardSpikes") {
                            Die();
                        }
                    }
                } else if (spike.pointingDirection == Vector3.right) {
                    Collider2D[] col = Physics2D.OverlapCircleAll(transform.position + (Vector3.left * 0.5f), 0.5f);
                    foreach (var item in col)
                    {
                        if (item.name == "RightySpikes") {
                            Die();
                        }
                    }
                } else if (spike.pointingDirection == Vector3.down) {
                    Collider2D[] col = Physics2D.OverlapCircleAll(transform.position + (Vector3.up * 0.5f), 0.5f);
                    foreach (var item in col)
                    {
                        if (item.name == "DownwardSpikes") {
                            Die();
                        }
                    }
                } else if (spike.pointingDirection == Vector3.left) {
                    Collider2D[] col = Physics2D.OverlapCircleAll(transform.position + (Vector3.right * 0.5f), 0.5f);
                    foreach (var item in col)
                    {
                        if (item.name == "LeftySpikes") {
                            Die();
                        }
                    }
                }
                return;
            }
            Die();
            return;
        }

        if (other.CompareTag("Stone")) {
            Stone stone = other.GetComponent<Stone>();
            if (stone.color == _currentColor) {
                return;
            }

            _anim.SetTrigger("transform");
            ChangeColor(stone.color);
            stone.Pick();
            return;
        }

        if (other.CompareTag("Coin")) {
            Coin coin = other.GetComponent<Coin>();
            cameraShake.Shake(0.02f, 0.1f);
            coin.PickCoin();
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
        _anim.SetTrigger("jump");
        yield return new WaitForSeconds(wallJumpInterval);
        _isWallJumping = false;
        _isMovementFreezed = false;
    }

    void WallJump()
    {
        if (!_isWallJumping) {
            return;
        }

        _rig.AddForce(_wallJumpDirection * wallJumpSpeed * Time.deltaTime, ForceMode2D.Impulse);
    }

    void WallSliding()
    {
        if (IsTouchingWall() && !IsGrounded() && _rig.velocity.y < 0 && _direction.x != 0) {
            _isWallSliding = true;
            transform.eulerAngles = _lastXDirection == Vector2.left ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
            _anim.SetInteger("transition", 2);
            return;
        }

        _isWallSliding = false;
    }
}
