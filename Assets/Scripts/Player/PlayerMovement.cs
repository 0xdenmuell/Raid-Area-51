using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D PlayerRb;
    private Animator anim;

    private SpriteRenderer rend;
    public Sprite normalPlayer;
    public Sprite slidingPlayer;

    public Transform groundCheck;
    public Transform wallCheck1;
    public Transform wallCheck2;
    public Transform wallCheck3;

    public LayerMask whatIsGround;

    public bool isGrounded;
    public bool isSlidingWall;
    public bool isTouchingWall;
    public int facingDirection = 0;
    private bool checkJumpHigher;
    public bool isJumping;
    public bool isWallJumping;
    private bool isWalking;

    private int amountOfJumps = 1;
    private int amountOfJumpsLeft;

    private float movementInput;
    private float movementSpeedFactor = 6f;
    private float jumpSpeedFactor = 9f;

    public float groundCheckRadius = 0.15f;
    public float wallCheckDistance = 0.2f;

    private float jumpForce = 10f;
    private float wallSlideSpeed = 2f;
    private float wallJumpForce = 7f;
    public float airDragMultiplier = 0.97f;
    public float jumpHigher = 0.5f;

    private float jumpTimer;
    private float jumpTimerSet = 0.15f;

    public float isJumpingTimer;
    private float isJumpingTimerSet = 0.5f;

    private Vector2 jumpForceOfWall;
    private Vector2 wallJumpDirection = new Vector2(1.5f, 1f);
    private float deltaTime;

    void Start()
    {
        PlayerRb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        amountOfJumpsLeft = amountOfJumps;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
        UpdateAnimations();
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        if (Physics2D.Raycast(wallCheck1.position, transform.right, wallCheckDistance, whatIsGround) ||
                Physics2D.Raycast(wallCheck2.position, transform.right, wallCheckDistance, whatIsGround) ||
                Physics2D.Raycast(wallCheck3.position, transform.right, wallCheckDistance, whatIsGround))
        {
            isTouchingWall = true;
        }
        else
        {
            isTouchingWall = false;
        }

        if (isJumping)
        {
            isJumpingTimer -= Time.deltaTime;
            if (jumpTimer <= 0)
            {
                isJumping = false;
            }
        }
        if (isWallJumping && isGrounded)
        {
            isWallJumping = false;
        }
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isJumping)
        {
            PlayerRb.velocity = new Vector2(0.0f, PlayerRb.velocity.y);

            isSlidingWall = true;
            rend.sprite = slidingPlayer;
            isWallJumping = false;
        }

        else
        {
            isSlidingWall = false;
            rend.sprite = normalPlayer;
        }
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && PlayerRb.velocity.y <= 0 || isTouchingWall)
        {
            amountOfJumpsLeft = amountOfJumps;
            checkJumpHigher = false;
        }
    }

    void CheckInput()
    {
        movementInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && amountOfJumpsLeft > 0)
        {
            if (!isGrounded && isTouchingWall && movementInput != 0 && movementInput != facingDirection)
            {
                WallJump();
            }
            else if (isGrounded || !isTouchingWall)
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
            }
        }

        if (checkJumpHigher && !Input.GetKey(KeyCode.Space))
        {
            checkJumpHigher = false;
            PlayerRb.velocity = new Vector2(PlayerRb.velocity.x, PlayerRb.velocity.y * jumpHigher);
        }

    }

    void CheckJump()
    {
        if (jumpTimer > 0)
        {
            if (!isGrounded && isTouchingWall)
            {
                WallJump();
                jumpTimer = 0;

            }
            else if (isGrounded || !isTouchingWall)
            {
                NormalJump();
                jumpTimer = 0;
            }
        }
        jumpTimer -= Time.deltaTime;
    }

    void ApplyMovement()
    {
        if (!isSlidingWall)
        {

            //movement when i am not grounded and have no movement input -- reduce my speed
            if (!isGrounded && movementInput == 0)
            {
                PlayerRb.velocity = new Vector2(PlayerRb.velocity.x * airDragMultiplier * airDragMultiplier, PlayerRb.velocity.y);
            }

            else if (isWallJumping && movementInput == 0)
            {
                PlayerRb.velocity = new Vector2(PlayerRb.velocity.x * airDragMultiplier, PlayerRb.velocity.y);
            }

            else if (isWallJumping && movementInput != 0)
            {
                PlayerRb.velocity = new Vector2(movementSpeedFactor * wallJumpDirection.x *  movementInput, PlayerRb.velocity.y);
            }
            else
            {
                PlayerRb.velocity = new Vector2(movementSpeedFactor * movementInput, PlayerRb.velocity.y);
            }

            //ground movement and jump movement when there is a movementInput

        }
        if (isSlidingWall && PlayerRb.velocity.y < 0.01)
        {
            if (PlayerRb.velocity.y < -wallSlideSpeed)
            {
                PlayerRb.velocity = new Vector2(0.0f, -wallSlideSpeed);
            }
        }

    }
    /*
    //Reduces the velocity of x when u are not moving (slowing down)
    if (movementInput != 0 && !isSlidingWall)
    {
        PlayerRb.velocity = new Vector2(movementSpeedFactor * movementInput, PlayerRb.velocity.y);
    }
    else
    {
        movementMomentumX = PlayerRb.velocity.x;
        if (!isSlidingWall)
        {
            if (movementMomentumX > 0 && facingDirection == 1)
            {
                facingDirection *= -1;
                movementMomentumX -= 0.4f;
            }
            else if (movementMomentumX < 0 && facingDirection == -1)
            {
                facingDirection *= -1;
                movementMomentumX += 0.4f;
            }
            PlayerRb.velocity = new Vector2(movementMomentumX, PlayerRb.velocity.y);
        }
    }
       */

    void NormalJump()
    {
        PlayerRb.velocity = new Vector2(PlayerRb.velocity.x, jumpForce);
        amountOfJumpsLeft--;
        checkJumpHigher = true;
        isJumping = true;
    }

    void WallJump()
    {
        PlayerRb.velocity = new Vector2(PlayerRb.velocity.x, 0.0f);
        ChangeFacingDirection();
        jumpForceOfWall = new Vector2(movementSpeedFactor * wallJumpDirection.x * facingDirection, movementSpeedFactor * wallJumpDirection.y);
        PlayerRb.AddForce(jumpForceOfWall, ForceMode2D.Impulse);
        amountOfJumpsLeft = amountOfJumps;
        amountOfJumpsLeft--;
        checkJumpHigher = true;
        isJumping = true;
        isWallJumping = true;
        isJumpingTimer = isJumpingTimerSet;
        rend.sprite = normalPlayer;
    }


    void CheckMovementDirection()
    {
        // Left = -1
        if (Input.GetKeyDown(KeyCode.A) && !isSlidingWall)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
            facingDirection = -1;
        }

        // Right = 1
        if (Input.GetKeyDown(KeyCode.D) && !isSlidingWall)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            facingDirection = 1;
        }

        if (PlayerRb.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
    }

    void ChangeFacingDirection()
    {
        if (facingDirection == 1)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
            facingDirection = -1;
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            facingDirection = 1;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck1.position, new Vector3(wallCheck1.position.x + wallCheckDistance, wallCheck1.position.y, wallCheck1.position.z));
        Gizmos.DrawLine(wallCheck2.position, new Vector3(wallCheck2.position.x + wallCheckDistance, wallCheck2.position.y, wallCheck2.position.z));
        Gizmos.DrawLine(wallCheck3.position, new Vector3(wallCheck3.position.x + wallCheckDistance, wallCheck3.position.y, wallCheck3.position.z));
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}

/*
transform.position += -movement * Time.deltaTime * moveSpeed;

if (isWallSliding && PlayerRb.velocity.y < -wallSlideSpeed)
{
PlayerRb.velocity = new Vector2(PlayerRb.velocity.x, -wallSlideSpeed);
}

}

void CheckSurroundings()
{
isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
}


void CheckIfWallSliding()
{
if (isTouchingWall && !isGrounded && PlayerRb.velocity.y < 0)
{
isWallSliding = true;
}
else
{
isWallSliding = false;
}
}

void Jump()
{
if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true && remainingAmountJump >= 1)
{
PlayerRb.AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
}
if (Input.GetKeyDown(KeyCode.W) && isGrounded == true && remainingAmountJump >= 1)
{
PlayerRb.AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
}
}

void Walljump()
{
isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
}



private void OnCollisionEnter2D(Collision2D collision)
{
if (collision.collider.tag == "Ground")
{
isGrounded = true;
remainingAmountJump = 1;
}
if (collision.collider.tag == "Side")
{

}
}

private void OnCollisionExit2D(Collision2D collision)
{
if (collision.collider.tag == "Ground")
{
isGrounded = false;
remainingAmountJump = 0;
}
if (collision.collider.tag == "Side")
{

}
}


*/


