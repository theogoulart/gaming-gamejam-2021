using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rig;
    private bool _isJumping;

    public float speed;
    public float jumpForce;

    // Start is called before the first frame update
    void Start()
    {
        _rig = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        Jump();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Jump()
    {
        if (!_isJumping && Input.GetKeyDown(KeyCode.Space)) {
            _isJumping = true;
            _rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void Move()
    {
        float movement = Input.GetAxis("Horizontal");
        _rig.velocity = new Vector2(movement * speed, _rig.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        _isJumping = false;
    }
}
