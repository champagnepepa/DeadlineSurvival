using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;


//[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public static Transform PlayerTransform { get; private set; }
    public Transform[] groundCheck = new Transform[5];
    public static PlayerMovement Instance { get; set; }

    public PlayerMovement pm;

    public CharacterController controller;

    public Vector3 MoveUpdate;
    public Animator Anim;
    public InputPlayModule InputPlay;
    GameInput gameInput;
    GameInput.GamePlayActions input;
    public float[] ActionTime;

    public bool IsWalk;
    public bool IsDeath;
    public bool IsIdle;
    public bool IsJump;
    public bool IsFall;
    public bool IsReadyIdle;
    public bool IsAttack;
    public bool IsRun;

    private float footstepTimer = 0f;
    public float footstepIntervalWalk = 0.5f;
    public float footstepIntervalRun = 0.3f;

    public float speed = 4f;
    public float walkSpeed = 4f;
    public float runSpeed = 8f;

    private float thirstDrainTimer = 0f;
    public float thirstDrainInterval = 1f;
    public float thirstDrainRate = 5f;

    public float gravity = -7.5f;
    //[SerializeField] Transform[] groundCheck = new Transform[10];
    //public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private AudioSource audioSource;
    public AudioClip walkClip;
    public AudioClip runClip;

    public GameObject deathPanel;

    Vector3 velocity;
    bool isGrounded;
    float xAxis;
    float zAxis;

    public bool isInterrupted;

    public Camera cam;

    public Transform handParent;

    //public InventorySlot selectedItemSlot;
    //public Transform handParent;

    private void Awake()
    {
        if (Instance ==null)
        {
            Instance = this;
            PlayerTransform = transform;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        controller = GetComponent<CharacterController>();
        Anim = GetComponentInChildren<Animator>();

        //audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }


        gameInput = new GameInput();
        input = gameInput.GamePlay;
        //AssignInputs();
    }
    private void Action(ActionState State)
    {
        switch (State)
        {
            case ActionState.Walk:
                IsWalk = !IsWalk;
                break;
            case ActionState.Running:
                IsRun = true;
                break;
            case ActionState.Jump:
                StartCoroutine(JumpCoroutine());
                break;
            case ActionState.Attack:
                IsAttack = ActionTime[0] == 0;
                break;
            case ActionState.Death:
                break;
        }
    }
    public float Idle()
    {
        Anim.SetFloat("Move", 0f);
        Anim.SetFloat("AxisX", 0f);
        Anim.SetFloat("AxisZ", 0f);
        //Sound.Stop();
        return 0;
    }

    public void ReadyIdle()
    {
        bool hasItemInHand = false;

        for (int i = 0; i < handParent.childCount; i++)
        {
            if (handParent.GetChild(i).gameObject.activeSelf)
            {
                hasItemInHand = true;
                break;
            }
        }

        Anim.SetBool("IsEquip", hasItemInHand);
        Debug.Log($"🎯 IsEquip set to: {hasItemInHand}");
    }

    //private float Move(Vector3 vector)
    //{
    //    Anim.SetBool("IsMove", IsWalk);
    //    Anim.SetFloat("Move", IsWalk ? 1f : 2f);
    //    Anim.SetFloat("AxisX", IsWalk ? vector.x : 0f);
    //    Anim.SetFloat("AxisZ", IsWalk ? vector.z : 0f);
    //    //if (!Sound.isPlaying) {
    //    //    Sound.clip = Resources.Load<AudioClip>("Audio/Run");
    //    //    Sound.Play();
    //    //}
    //    return IsWalk ? 2f : 3f;
    //}

    private void Move(Vector3 vector)
    {
        Anim.SetBool("IsMove", true);
        Anim.SetFloat("Move", IsRun && IsWalk ? 2f : 1f);
        Anim.SetFloat("AxisX", vector.x);
        Anim.SetFloat("AxisZ", vector.z);
    }

    private IEnumerator AttackCoroutine()
    {
        speed = Idle();
        Anim.SetFloat("Variety", 0f);
        Anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.75f);
    }
    private Quaternion Rotation(Vector3 vector)
    {
        var forward = Camera.main.transform.forward;
        var right = Camera.main.transform.right;
        forward.y = right.y = 0;
        MoveUpdate = (vector.z * forward.normalized) + (vector.x * right.normalized);
        var rotate = Quaternion.LookRotation(MoveUpdate);
        return Quaternion.Slerp(rotate, transform.rotation, Time.deltaTime);
    }

    private Quaternion Rotation(Vector3 vector, float speed)
    {
        var lookPos = Quaternion.LookRotation(vector);
        return Quaternion.Slerp(transform.rotation, lookPos, Time.deltaTime * speed);
    }

    public IEnumerator RunCoroutine()
    {
        speed = 10f;
        Anim.SetBool("IsRun", true);
        yield return new WaitForSeconds(0.05f);
        IsRun = false;
    }

    private IEnumerator JumpCoroutine()
    {
        if (!IsJump)
        {
            IsFall = false;
            IsJump = true;
            Anim.SetBool("IsJump", true);
            MoveUpdate.y++;
            controller.Move(MoveUpdate * Time.deltaTime);

            yield return new WaitForSeconds(0.5f);
            IsFall = true;
        }
    }

    //private void Movement()
    //{
    //    if (!IsDeath)
    //    {
    //        Fall();
    //        var vector = InputPlay ? InputPlay.MoveHandler.normalized : Vector3.zero;
    //        IsIdle = (vector.x, vector.z) == (0, 0);
    //        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    //        if (isGrounded && velocity.y < 0)
    //        {
    //            velocity.y = -2f;
    //        }
    //        if (IsIdle)
    //        {
    //            Idle();
    //        }
    //        else
    //        {
    //            speed = Move(vector);
    //            if (IsRun && !IsWalk && !IsJump)
    //            {
    //                StartCoroutine(RunCoroutine());
    //            }
    //        }
    //        
    //        //Vector3 MoveUpdate = transform.right * x + transform.forward * z;
    //        //velocity.y += gravity * Time.deltaTime;
    //        //MoveUpdate.Normalize();
    //    }
    //    float xAxis = Input.GetAxisRaw("Horizontal");
    //    float zAxis = Input.GetAxisRaw("Vertical");
    //    Vector3 MoveUpdate = transform.forward * zAxis + transform.right * xAxis;
    //    velocity.y += gravity * Time.deltaTime;
    //    controller.Move(speed * Time.deltaTime * MoveUpdate);
    //}

    private void Movement()
    {
        if (IsDeath) return;

        Fall();

        Vector3 inputVector = InputPlay ? InputPlay.MoveHandler.normalized : Vector3.zero;
        IsIdle = inputVector == Vector3.zero;
        IsWalk = inputVector.magnitude > 0.1f;
        isGrounded = Physics.CheckSphere(groundCheck[0].position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (IsIdle)
        {
            Idle();
        }
        else
        {
            Move(inputVector);
        }

        velocity.y += gravity * Time.deltaTime;

        // Hitung arah pergerakan
        Vector3 moveDir = transform.forward * inputVector.z + transform.right * inputVector.x;
        controller.Move((moveDir * speed + new Vector3(0, velocity.y, 0)) * Time.deltaTime);

        PlayFootstepSound();
    }

    //private void Movement()
    //{
    //
    //    if (!IsDeath)
    //    {
    //        //Fall();
    //        //var vector = InputPlay ? InputPlay.MoveHandler.normalized : Vector3.zero;
    //        //IsIdle = (vector.x, vector.z) == (0, 0);
    //        if (IsIdle)
    //        {
    //            Idle();
    //        }
    //        else
    //        {
    //            controller.Move(move * speed * Time.deltaTime);
    //            //if (IsRun && !IsWalk && !IsJump)
    //            //{
    //            //    StartCoroutine(DashCoroutine());
    //            //}
    //            //if (!IsJump)
    //            //{
    //            //    MoveUpdate = new Vector3(vector.x, MoveUpdate.y, vector.z);
    //            //    if (!IsWalk)
    //            //    {
    //            //        transform.rotation = Rotation(vector);
    //            //    }
    //            //}
    //        }
    //        controller.Move(speed * Time.deltaTime * move);
    //    }
    //}
    private void PlayFootstepSound()
    {
        if (!isGrounded || IsIdle || IsJump || IsFall) return;
        if (audioSource == null) return;

        footstepTimer += Time.deltaTime;

        float interval = IsRun ? footstepIntervalRun : footstepIntervalWalk;
        AudioClip clipToPlay = IsRun ? runClip : walkClip;

        if (clipToPlay == null)
        {
            Debug.LogWarning("❗ Footstep clip is null (runClip or walkClip belum diassign).");
            return;
        }

        if (footstepTimer >= interval)
        {
            Debug.Log("👣 Footstep: Playing sound: " + clipToPlay.name + ", Volume: " + audioSource.volume);

            audioSource.clip = clipToPlay;
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();
            footstepTimer = 0f;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        PlayerState.Instance.currentHealth -= damageAmount;

        if (PlayerState.Instance.currentHealth <= 0)
        {
            IsDeath = true;
            deathPanel.SetActive(true);
        }
    }

    private void Death()
    {
        //Anim.SetBool("IsDeath", IsDeath = CurrentHp <= 0);
        if (IsDeath)
        {
            Idle();
            Anim.SetBool("IsDeath", true);
            pm.isInterrupted = true;
        }
    }

    private void Fall()
    {
        if (IsFall)
        {
            MoveUpdate.y += gravity * Time.deltaTime;
            controller.Move(MoveUpdate * Time.deltaTime);
            if (controller.isGrounded)
            {
                MoveUpdate = Vector3.zero;
                Anim.SetBool("IsJump", false);
                IsJump = false;
            }
            else
            {
                IsJump = true;
            }
        }
    }

    //public void ClearHandItem()
    //{
    //    Debug.Log("🔧 Memanggil ClearHandItem");
    //
    //    for (int i = 0; i < handParent.childCount; i++)
    //    {
    //        GameObject child = handParent.GetChild(i).gameObject;
    //        Debug.Log($"Cek {child.name} aktif: {child.activeSelf}");
    //
    //        if (child.activeSelf)
    //        {
    //            Debug.Log($"🚫 Menonaktifkan: {child.name}");
    //            child.SetActive(false);
    //        }
    //    }
    //}

    //public void DisableHandItem(ItemSO item)
    //{
    //    for (int i = 0; i < handParent.childCount; i++)
    //    {
    //        var handObj = handParent.GetChild(i).gameObject;
    //        var itemHand = handObj.GetComponent<ItemHand>();
    //        if (itemHand != null && itemHand.itemScriptableObject == item)
    //        {
    //            handObj.SetActive(false);
    //        }
    //    }
    //}

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Anim = GetComponentInChildren<Animator>();

        if (InputPlay == null)
        {
            InputPlay = new InputPlayModule();
        }

        InputPlay.OnAction = Action;

        gameInput = new GameInput();
        input = gameInput.GamePlay;
        gameInput.Enable();

        // ✅ CARI groundCheck otomatis
        if (groundCheck == null || groundCheck.Length == 0 || groundCheck[0] == null)
        {
            Transform ground = transform.Find("GroundCheck");
            if (ground != null)
            {
                groundCheck = new Transform[1];
                groundCheck[0] = ground;
                Debug.Log("✅ GroundCheck ditemukan otomatis di prefab Player.");
            }
            else
            {
                Debug.LogWarning("⚠️ GroundCheck tidak ditemukan di Player.");
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }


    // Update is called once per frame

    private void Update()
    {
        if (IsRun)
        {
            if (PlayerState.Instance.currentThirst > 0)
            {
                thirstDrainTimer += Time.deltaTime;
                if (thirstDrainTimer >= thirstDrainInterval)
                {
                    PlayerState.Instance.currentThirst -= thirstDrainRate;
                    PlayerState.Instance.currentThirst = Mathf.Clamp(PlayerState.Instance.currentThirst, 0, PlayerState.Instance.maxThirst);
                    thirstDrainTimer = 0f;

                    // Kalau thirst habis, matikan run
                    if (PlayerState.Instance.currentThirst <= 0)
                    {
                        IsRun = false;
                        speed = walkSpeed;
                        Anim.SetBool("IsRun", false);
                    }
                }
            }
            else
            {
                // Jika sudah habis thirst dan masih mencoba lari
                IsRun = false;
                speed = walkSpeed;
                Anim.SetBool("IsRun", false);
            }
        }

        if (isInterrupted && !Cursor.visible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (!isInterrupted && Cursor.visible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // Cek apakah masih punya stamina / thirst
            if (PlayerState.Instance.currentThirst > 0)
            {
                IsRun = true;
                speed = runSpeed;
                Anim.SetBool("IsRun", true);
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            IsRun = false;
            speed = walkSpeed;
            Anim.SetBool("IsRun", false);
        }

        if (isInterrupted)
            return;

        //if (input.Attack.IsPressed())
        //{ Attack(); }
        //
        //
        //SetAnimations();
        Movement();
        Death();
        ReadyIdle();
    }

    //void OnEnable()
    //{ input.Enable(); }
    //
    //void OnDisable()
    //{ input.Disable(); }
    //
    //void AssignInputs()
    //{
    //    input.Attack.started += ctx => Attack();
    //}
    //
    //public const string IDLE = "IsMove";
    //public const string WALK = "IsMove";
    //public const string ATTACK1 = "AttackSlash";
    //
    //string currentAnimationState;
    //
    //public void ChangeAnimationState(string newState)
    //{
    //    // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
    //    if (currentAnimationState == newState) return;
    //
    //    // PLAY THE ANIMATION //
    //    currentAnimationState = newState;
    //    Anim.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    //}
    //
    //void SetAnimations()
    //{
    //    // If player is not attacking
    //    if (!attacking)
    //    {
    //        var vector = InputPlay ? InputPlay.MoveHandler.normalized : //Vector3.zero;
    //        IsIdle = (vector.x, vector.z) == (0, 0);
    //        if (IsIdle)
    //        {
    //            Idle();
    //        }
    //        else
    //        {
    //            speed = Move(vector);
    //        }
    //    }
    //}
    //
    //[Header("Attacking")]
    //public float attackDistance = 3f;
    //public float attackDelay = 0.4f;
    //public float attackSpeed = 1f;
    //public int attackDamage = 1;
    //public LayerMask attackLayer;
    //
    //public GameObject hitEffect;
    //
    //bool attacking = false;
    //bool readyToAttack = true;
    //int attackCount;
    //
    //public void Attack()
    //{
    //    if (!readyToAttack || attacking) return;
    //
    //    readyToAttack = false;
    //    attacking = true;
    //
    //    Invoke(nameof(ResetAttack), attackSpeed);
    //    Invoke(nameof(AttackRaycast), attackDelay);
    //
    //    //audioSource.pitch = Random.Range(0.9f, 1.1f);
    //    //audioSource.PlayOneShot(swordSwing);
    //
    //    if (attackCount == 0)
    //    {
    //        ChangeAnimationState(ATTACK1);
    //        attackCount++;
    //    }
    //    else
    //    {
    //        ChangeAnimationState(ATTACK1);
    //        attackCount = 0;
    //    }
    //}
    //
    //void ResetAttack()
    //{
    //    attacking = false;
    //    readyToAttack = true;
    //}
    //
    //void AttackRaycast()
    //{
    //    if (Physics.Raycast(cam.transform.position, cam.transform.forward, /out/ RaycastHit hit, attackDistance, attackLayer))
    //    {
    //        HitTarget(hit.point);
    //
    //        if (hit.transform.TryGetComponent<Actor>(out Actor T))
    //        { T.TakeDamage(attackDamage); }
    //    }
    //}
    //
    //void HitTarget(Vector3 pos)
    //{
    //    //audioSource.pitch = 1;
    //    //audioSource.PlayOneShot(hitSound);
    //
    //    GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
    //    Destroy(GO, 20);
    //}
}
