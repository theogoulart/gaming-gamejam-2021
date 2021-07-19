using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rig;
    private Vector2 _direction;

    private bool _isJumping;
    private bool _isWallJumping;
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

    // Start is called before the first frame update
    void Start()
    {
        _rig = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        CheckEarlyJump();
        CheckJump();
        OnInput();
    }

    void FixedUpdate()
    {
        Move();
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
        if (Input.GetKeyDown(KeyCode.Space)) {
            bool isGrounded = IsGrounded();
            if (isGrounded) {
                Jump();
                return;
            }

            bool isWallCling = IsWallCling();
            if (isWallCling) {
                WallJump();
                return;
            }
        }
    }

    bool CheckTouchingGround()
    {
        return true;
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
        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0,0.2f,0), new Vector3(.35f,.2f,0), 0, floorLayer);
        if (hit != null) {
            Debug.Log("grounded");
            return true;
        }

        return false;
    }

    bool IsWallCling()
    {
        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0,0.1f,0), new Vector3(0.55f,.2f,0), 0, floorLayer);
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
        _rig.velocity = new Vector2(_direction.x * speed, _rig.velocity.y);

        // idle 
        if (_direction.x == 0) {
            return;
        }

        transform.eulerAngles = _direction.x > 0 ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        _isEarlyJumpEnabled = false;
        _isJumping = false;
        ExecuteEarlyJump();
    }

    private void OnDrawGizmos() {
        // EarlyJump
        Gizmos.DrawWireCube(transform.position - new Vector3(0,0.5f,0), new Vector3(.3f,.4f,0));
        Gizmos.color = Color.green;
        // Grounded
        Gizmos.DrawWireCube(transform.position - new Vector3(0,0.2f,0), new Vector3(.35f,.2f,0));
        // WallCling
        Gizmos.DrawWireCube(transform.position - new Vector3(0,0.1f,0), new Vector3(0.55f,.2f,0));
    }

    void OnInput()
    {
        _direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    IEnumerator OnJump()
    {
        _hasEarlyJumped = false;
        _isJumping = true;
        yield return new WaitForSeconds(0.2f);
        _isEarlyJumpEnabled = true;
    }

    IEnumerator OnWallJump()
    {
        _isWallJumping = true;
        yield return new WaitForSeconds(0.2f);
        _isWallJumping = false;
    }

    void WallJump()
    {
        Debug.Log("wall jump");
        StartCoroutine(OnWallJump());
        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0,0.1f,0), new Vector3(0.55f,.2f,0), 0, floorLayer);
        Vector2 closestPoint = hit.ClosestPoint(transform.position);
        Vector2 jumpDirection = closestPoint.x > transform.position.x ? new Vector2(-1f,0f) : new Vector2(1f,0f);
        _rig.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
    }
}
