using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float crouchSpeed = 1f;
    [SerializeField] private float rollSpeed = 7f;
    [SerializeField] private float airSpeed = 3f;
    [SerializeField] private float jumpImpulse = 10f;
    private Rigidbody2D rb;
    private Animator animator;
    private TouchingDirections touchingDirections;
    private PlayerMagicSpells playerMagicSpells;
    private Vector2 moveInput;

    [HideInInspector] public bool ledgeDetected;
    [SerializeField] private Vector2 offset1Right;
    [SerializeField] private Vector2 offset2Right;
    [SerializeField] private Vector2 offset1Left;
    [SerializeField] private Vector2 offset2Left;


    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLedge = true;
    private bool canClimb;
    public bool CanClimb
    {
        get
        {
            return canClimb;
        }
        private set
        {
            canClimb = value;
            animator.SetBool(AnimationStrings.canClimb, canClimb);
        }
    }
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool LockVelocity
    {
        private set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
    }
    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }

    public float CurrentMoveSpeed { 
        get { 
            if (isMoving && CanMove && !touchingDirections.IsOnWall)
            {
                if (!touchingDirections.IsGrounded)
                {
                    return airSpeed;
                }
                else if (isRolling)
                {
                    return rollSpeed;
                }
                else if (IsCrouch &&  !isRunning)
                {
                    return crouchSpeed;
                }
                else if (isRunning)
                {
                    return runSpeed;
                }
                
                return walkSpeed;
            }
            else
            {
                return 0;
            }
        }
    }
    private bool isMoving = false;
    public bool IsMoving { 
        get {
            return isMoving;
        }
        private set { 
            isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, isMoving);
        } 
    }


    private bool isFacingRight = true;

    public bool IsFacingRight
    {
        get { return isFacingRight; }
        private set {
            if (isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            isFacingRight = value; 
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        playerMagicSpells = GetComponent<PlayerMagicSpells>();
    }

    private bool isRunning = false;

    public bool IsRunning
    {
        get { return isRunning; }
        private set { 
            isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, isRunning);
        }
    }

    
    private bool isCrouch = false;

    public bool IsCrouch
    {
        get { return animator.GetBool(AnimationStrings.isCrouch); }
        private set
        {
            isCrouch = value;
            animator.SetBool(AnimationStrings.isCrouch, isCrouch);
        }
    }
    void FixedUpdate()
    {
        if (CanMove)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
            
        }
        

    }

    private void Update()
    {
        SetFacingDirection(moveInput);
        CheckForLedge();
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput =  context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            
        }
        



    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if(context.canceled)
        {
            IsRunning = false;
        }

    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            if (IsCrouch && !touchingDirections.IsOnCeiling)
            {
                IsCrouch = false;
            }
            else
            {
                IsCrouch = true;
            }
        }

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
       if(canGrabLedge && IsAlive)
        {
            if (moveInput.x > 0 && !IsFacingRight)
            {
                IsFacingRight = true;
            }
            else if (moveInput.x < 0 && IsFacingRight)
            {
                IsFacingRight = false;
            }
        }
           
      
        
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }
    private void ResetAttack() => animator.ResetTrigger(AnimationStrings.attackTrigger);
    
    public void OnMagicAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            foreach(var magicSpellName in playerMagicSpells.GetSpellNames())
            {
                if(magicSpellName != playerMagicSpells.GetMagicSpell())
                {
                    animator.SetBool(magicSpellName, false);
                }
                
            }
            animator.SetBool(playerMagicSpells.GetMagicSpell(), true);
            animator.SetTrigger(AnimationStrings.magicAttackTrigger);
        }
    }


    private bool isRolling = false;

    public bool IsRolling
    {
        get { return isRolling; }
        private set
        {
            isRolling = value;
            animator.SetBool(AnimationStrings.isRolling, isRolling);
        }
    }

    public Animator Animator { get => animator; set => animator = value; }

    private bool isRollInProgress = false;
    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded) {
            if (isMoving)
            {
                if (!isRollInProgress)
                {
                    IsCrouch = false;
                    IsRolling = true;
                    StartCoroutine(RollCoroutine());
                }
            } 
        }
    }
    [SerializeField] private float rollDistance = 1.4f;
    private IEnumerator RollCoroutine()
    {


        isRollInProgress = true;
        float distanceTraveled = 0f;
        

        // Пока не пройдена вся дистанция подката
        while (distanceTraveled < rollDistance)
        {
            // Перемещение в направлении, в котором смотрит персонаж
            float moveDelta = rollSpeed * Time.fixedDeltaTime;
            rb.position += new Vector2((isFacingRight ? moveDelta : -moveDelta), 0f);
            distanceTraveled += moveDelta;

            yield return new WaitForFixedUpdate();
        }
        IsRolling = false;
        yield return new WaitForSeconds(0.5f);
        // Восстановление оригинальной скорости и завершение подката
        
        isRollInProgress = false;
    }

    private void CheckOnCeiling()
    {
        if (touchingDirections.IsOnCeiling)
        {
            Debug.LogWarning("isCrouch");
            IsCrouch = true;
        }
    }
    public void OnHit(int damage, Vector2 knockback)
    {

        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge)
        {
            LockVelocity = true;
            canGrabLedge = false;

            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            if (isFacingRight)
            {
                climbBegunPosition = ledgePosition + offset1Right;
                climbOverPosition = ledgePosition + offset2Right;
            }
            else
            {
                climbBegunPosition = ledgePosition + offset1Left;
                climbOverPosition = ledgePosition + offset2Left;
            }
           

            CanClimb = true;
        }

        if(CanClimb)
        {
            transform.position = climbBegunPosition;
        }
    }

    public void LedgeClimbOver()
    {
            CanClimb = false;
            isClimbed = true;
            LockVelocity = false;
            Invoke("AllowLedgeGrab", 0.5f);
            
        
    }

    private void AllowLedgeGrab() => canGrabLedge = true;

    private bool isClimbed;
    private void ResetPositionAfterClimb() 
    {
        if (isClimbed)
        {
            transform.position = climbOverPosition;
            isClimbed = false;
        }
        
    }

    private bool isChooseMagicSpells = false;
    public void OnChooseMagicSpells(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!isChooseMagicSpells)
            {
                CharacterEvents.showMagicSpellsMenu.Invoke();
                isChooseMagicSpells = true;
            }
            else
            {
                CharacterEvents.closeMagicSpellsMenu.Invoke();
                isChooseMagicSpells = false;
            }
        }
        
        
    }

}
