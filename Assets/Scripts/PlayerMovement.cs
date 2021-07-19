using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rig;

    private bool _isJumping;
    private bool _isEarlyJumpEnabled;
    private bool _hasEarlyJumped;

    public LayerMask floorLayer;

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

        // Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0,0.35f,0), new Vector3(0.4f,0.1f,0), 0);
        Collider2D hit = Physics2D.OverlapBox(transform.position - new Vector3(0,0.35f,0), new Vector3(1f, 1f, 0), 0, floorLayer);
        if (hit != null && Input.GetKeyDown(KeyCode.Space)) {
            _hasEarlyJumped = true;
            Debug.Log(hit.name);
        }
    }

    void CheckJump()
    {
        float verticalVelocity = (int)_rig.velocity.y;
        if (!_isJumping && Input.GetKeyDown(KeyCode.Space) && verticalVelocity >= 0) {
            Jump();
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

    void Jump()
    {
        StartCoroutine(OnJump());
        _rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void Move()
    {
        float movement = Input.GetAxis("Horizontal");
        _rig.velocity = new Vector2(movement * speed, _rig.velocity.y);

        // idle 
        if (movement == 0) {
            return;
        }

        transform.eulerAngles = movement > 0 ? new Vector3(0, 180, 0) : new Vector3(0, 0, 0);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        _isEarlyJumpEnabled = false;
        _isJumping = false;
        ExecuteEarlyJump();
    }

    private void OnDrawGizmos() {
        // Gizmos.DrawWireCube(transform.position - new Vector3(0,0.35f,0), new Vector3(0.4f,0.1f,0));
        Gizmos.DrawWireCube(transform.position - new Vector3(0,0.35f,0), new Vector3(1f,1f,0));
    }

    IEnumerator OnJump()
    {
        _hasEarlyJumped = false;
        _isJumping = true;
        yield return new WaitForSeconds(0.2f);
        _isEarlyJumpEnabled = true;
    }
}
