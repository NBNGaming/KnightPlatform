using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //public GameDataScripts gameData;

    public GameObject Menu;
    public GameObject DeadMenu;


    //Для перемещения
    // private float maxSpeed = 10f;//макс скорость, если будет ускорение
    private float speed = 2f;
    private bool canMove = true;
    private bool viewRight = true;


    //Для рывка
    private float dashSpeed = 55f;
    private float scaleGravity = 2f;
    public bool canDash = true;
    public float dashStopSec = 1.5f;


    //Для здоровья
    [SerializeField] public int hpPlayer;
    [SerializeField] public int hpPlayerMax = 10;


    public float bubbleSecundMax = 0.5f;
    public LayerMask whatIsPlayer;

    //Для получение урона
    private float checkRadiusDamage = 0.2f;
    public bool canDamage = true; //для неуз

    public Material matBlink; //для мигания!!!!!!!!!!
    public Material matDefault;
    private SpriteRenderer spriteRender;


    //Отображние здоровья
    public Text counterHp;
    public Slider sliderHp;

    //Для отскока !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!значение подправить при релизе!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private int reboundTime = 5;
    private int rebound = 4;
    private int reboundAttack = 5;


    //для маны
    [SerializeField] public int mpPlayer;

    [SerializeField] public int mpPlayerMax = 4;

    //отображением маны
    public Text counterMp;

    public Slider sliderMp;

    //для хилки
    public bool canHeal = true;
    public float stopHealing = 0.01f;


    //Для атаки
    [SerializeField] public int damagePlayer = 1;
    private bool canAtt = true; //для проверки возможности выполнения атак
    public Collider2D[] isEnemy;
    public Transform attackPosRight;
    public Transform attackPosLeft;
    public Transform attackPosUp;
    public Transform attackPosDown;
    private float checkRadiusAttack = 0.45f;
    public LayerMask whatIsEnemy;

    private float
        stopAtack = 0.2f; //на релиз проверить на анимации!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


    //Для прыжка
    private float jumpHeight = 1f;
    private float jumpForce;
    private float jumpSecundMax = 0.5f;
    private float jumpCounter;
    private bool isJumping;


    private Rigidbody2D rb;
    private BoxCollider2D bx;


    //Для проверки поверхности под игроком
    private bool isGround;
    public Transform feetPos;
    private float checkRadius = 0.45f;
    public LayerMask whatIsGroud;

    //Скольжение
    private float checkRadiusWall = 0.45f;
    public LayerMask whatIsWall;
    private bool isWall;

    public Animator animat; // анимация персонажа


    private AudioSource _adSource; //звуки игрока

    private AudioListener _adListener;

    public AudioClip sword;
    public AudioClip hitHazard;
    public AudioClip hitSword;
    public AudioClip damageCl;


    //движение Персонажа
    private void MovePlayer()
    {
        if (Math.Abs(Input.GetAxis("Horizontal")) != 0)
            animat.SetFloat("HorizontalMove", Math.Abs(Input.GetAxis("Horizontal")));

        if (canMove)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0); //поворот
                rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
                viewRight = true;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0); //поворот
                //тут мб -180 лучше сделать, зависит от того как поведет себя спрайт при повороте
                rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
                viewRight = false;
            }
        }
    }

    //рывок персонажа, мб добавить кадр неуязвимости! и отключать коллайдер
    private void DashPlayer()
    {
        //animat.SetBool("Dashing", false);
        if (canDash)
        {
            if (Input.GetButton("Dash"))
            {
                animat.SetBool("Dashing", true);

                // rb.velocity = Vector2.zero;

                rb.gravityScale = 0;
                //эффект рывка начало

                //неуявз мб? 


                rb.velocity =
                    new Vector2(Input.GetAxis("Horizontal") * dashSpeed,
                        rb.velocity.y); //рывок при передвижение в стороны
                //rb.velocity = Vector2.right * dashSpeed;
                //эффект рывка конец

                StartCoroutine(StopCollision());
                StartCoroutine(StopDashing());
            }
        }
        else
        {
        }

        canMove = false;
        rb.gravityScale = scaleGravity;
        canMove = true;
    }

    private IEnumerator StopCollision()
    {
        Physics2D.IgnoreLayerCollision((int)Math.Log(whatIsPlayer.value, 2), (int)Math.Log(whatIsEnemy.value, 2), true);
        yield return new WaitForSeconds(0.2f);
        Physics2D.IgnoreLayerCollision((int)Math.Log(whatIsPlayer.value, 2), (int)Math.Log(whatIsEnemy.value, 2),
            false);
        animat.SetBool("Dashing", false);
    }

    private IEnumerator StopDashing()
    {
        canDash = false;
        yield return new WaitForSeconds(dashStopSec);
        canDash = true;
    }


    //Прыжок Персонажа (нету анимации!)
    private void JumpPlayer()
    {
        isGround = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGroud); //касание пола

        //с пола
        if (((isGround) || (isWall)) && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            jumpCounter = jumpSecundMax;
            rb.velocity = Vector2.up * jumpForce;
        }

        //динамический
        if (Input.GetButton("Jump") && isJumping == true)
        {
            // animat.SetBool("Jumping", true);
            //animat.SetFloat("HorizontalMove", 0);
            if (jumpCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;

                jumpCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        //при отпускании
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if (((isGround) || (isWall)))
        {
            //animat.SetBool("Jumping", false);
        }
    }


    //Получение урона
    private void CanDamage()
    {
        int dir;
        Collider2D dirDamageUp;
        Collider2D dirDamageLeft;
        Collider2D dirDamageRight;
        Collider2D dirDamageDown;
        if (canDamage)
        {
            dirDamageRight = Physics2D.OverlapCircle(attackPosRight.position, checkRadiusDamage, whatIsEnemy);
            dirDamageLeft = Physics2D.OverlapCircle(attackPosLeft.position, checkRadiusDamage, whatIsEnemy);
            dirDamageUp = Physics2D.OverlapCircle(attackPosUp.position, checkRadiusDamage, whatIsEnemy);
            dirDamageDown = Physics2D.OverlapCircle(attackPosDown.position, checkRadiusDamage, whatIsEnemy);

            if (dirDamageRight != null)
            {
                StartCoroutine(StopMove());
                //Debug.Log("Справа ударили");
                if (viewRight)
                    dir = 2;
                else
                    dir = 0;
                TakeDamage(dirDamageRight.tag, dirDamageRight.transform.GetComponent<EnemyHealth>().EnemyDamage);
                handleMovePlayerDamage(dir);
                StartCoroutine(StopDamage());
            }
            else if (dirDamageLeft != null)
            {
                StartCoroutine(StopMove());
                //Debug.Log("Слева ударили");
                dir = 0;
                TakeDamage(dirDamageLeft.tag, dirDamageLeft.transform.GetComponent<EnemyHealth>().EnemyDamage);
                handleMovePlayerDamage(dir);
                StartCoroutine(StopDamage());
            }
            else if (dirDamageUp != null)
            {
                StartCoroutine(StopMove());
                //Debug.Log("Сверху ударили");
                dir = 1;
                TakeDamage(dirDamageUp.tag, dirDamageUp.transform.GetComponent<EnemyHealth>().EnemyDamage);
                handleMovePlayerDamage(dir);
                StartCoroutine(StopDamage());
            }
            else if (dirDamageDown != null)
            {
                StartCoroutine(StopMove());
                //Debug.Log("Снизу ударили");
                dir = -1;
                TakeDamage(dirDamageDown.tag, dirDamageDown.transform.GetComponent<EnemyHealth>().EnemyDamage);
                handleMovePlayerDamage(dir);
                StartCoroutine(StopDamage());
            }
            //анимация и/или эффект получения урона

            //counterHp.text = hpPlayer.ToString() + "/" + hpPlayerMax.ToString();
        }

        //spriteRender.material = matDefault;
        // хз нужно ли тут, если в респавне лежит?
        if (hpPlayer <= 0)
        {
            _adListener.enabled = false;
            DeadMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void handleMovePlayerDamage(int dir)
    {
        _adSource.PlayOneShot(damageCl);
        if (dir == 1)
            rb.velocity = Vector2.up * rebound;
        else if (dir == -1)
            rb.velocity = Vector2.down * rebound;
        else if (dir == 0)
            rb.velocity = Vector2.right * rebound;
        else
        {
            rb.velocity = Vector2.left * rebound;
        }
    }

    private void TakeDamage(string typeDamage, int enemyDamage)
    {
        if (typeDamage == "Hazard")
        {
            hpPlayer -= 1;
            //sliderHp.value -= 1;
        }
        else if (typeDamage == "Enemy")
        {
            hpPlayer -= enemyDamage;
            //sliderHp.value -= enemyDamage;
        }

        spriteRender.material = matBlink;
        FlashingDamage();
    }

    private IEnumerator StopMove()
    {
        canMove = false;
        canDash = false;
        yield return new WaitForSeconds(reboundTime);
        canMove = true;
        canDash = true;
    }


    private IEnumerator StopDamage()
    {
        canDamage = false;
        //Physics2D.IgnoreLayerCollision((int)Mathf.Log((whatIsPlayer.value), 2), (int)Mathf.Log((whatIsEnemy.value), 2), true);


        yield return new WaitForSeconds(bubbleSecundMax);

        canDamage = true;
        //Physics2D.IgnoreLayerCollision((int)Mathf.Log((whatIsPlayer.value), 2), (int)Mathf.Log((whatIsEnemy.value), 2), false);
    }

    public void FlashingDamage()
    {
        Invoke("ResetMaterial", 0.3f);
    }

    public void ResetMaterial()
    {
        spriteRender.material = matDefault;
    }

    //скольжение
    private void Sliding()
    {
        isWall = Physics2D.OverlapCircle(feetPos.position, checkRadiusWall, whatIsWall); //касание стены
        if (isWall)
        {
            animat.SetBool("Sliding", true);
        }

        if (!isWall || isGround)
        {
            animat.SetBool("Sliding", false);
        }
    }

    //атака персонажа, не доделана!!!!!!!!!!!!
    private void AttackWeapon()
    {
        if (Input.GetButton("Attack") && canAtt)
        {
            _adSource.PlayOneShot(sword);
            //Vector2 directionEnemy;
            int dir;
            bool isHazard = false;
            //Debug.Log("Удар был");

            if (Input.GetAxis("Vertical") < 0)
            {
                isEnemy = Physics2D.OverlapCircleAll(attackPosDown.position, checkRadiusAttack, whatIsEnemy);
                animat.SetBool("AttackDown", true);
                dir = -1;
            }
            else if (Input.GetAxis("Vertical") > 0)
            {
                isEnemy = Physics2D.OverlapCircleAll(attackPosUp.position, checkRadiusAttack, whatIsEnemy);
                animat.SetBool("AttackUp", true);
                dir = 1;
            }
            else
            {
                //animat.SetBool("AttackRight", true); 
                animat.SetBool("AttackRight", true);
                isEnemy = Physics2D.OverlapCircleAll(attackPosRight.position, checkRadiusAttack, whatIsEnemy);
                if (viewRight)
                    dir = 0;
                else
                    dir = 2;
            }

            /*
            else
            {
                isEnemy = Physics2D.OverlapCircleAll(attackPosRight.position, checkRadiusAttack, whatIsEnemy);
                dir = 0;
            }
            */

            //ооталкивание и нанесениу урона
            for (int i = 0; i < isEnemy.Length; i++)
            {
                //Debug.Log("массив есть");
                if (isEnemy[i].tag == "Enemy")
                {
                    _adSource.PlayOneShot(hitSword);
                    isEnemy[i].GetComponent<EnemyHealth>().EnemyHp -= damagePlayer;
                    // Debug.Log("урон есть");

                    if (dir == 1)
                    {
                        isEnemy[i].GetComponent<EnemyBehaviour>().rb.velocity = Vector2.up * reboundAttack;
                    }
                    else if (dir == -1)
                    {
                        isEnemy[i].GetComponent<EnemyBehaviour>().rb.velocity = Vector2.down * reboundAttack;
                    }
                    else if (dir == 0)
                    {
                        isEnemy[i].GetComponent<EnemyBehaviour>().rb.velocity = Vector2.right * reboundAttack;
                    }
                    else
                    {
                        isEnemy[i].GetComponent<EnemyBehaviour>().rb.velocity = Vector2.left * reboundAttack;
                    }
                }
                else if (isEnemy[i].tag == "Hazard" && !isHazard)
                {
                    _adSource.PlayOneShot(hitHazard);
                    handleMovePlayer(dir);
                    isHazard = true;
                }
            }

            StartCoroutine(StopAttack());
        }
        else if (!canAtt)
        {
            animat.SetBool("AttackRight", false);
            animat.SetBool("AttackUp", false);
            animat.SetBool("AttackDown", false);
        }
    }

    private IEnumerator StopAttack()
    {
        //animat.SetBool("AttackRight", false);
        canAtt = false;
        yield return new WaitForSeconds(stopAtack);
        canAtt = true;
    }

    private void handleMovePlayer(int dir)
    {
        if (dir == 1)
            rb.velocity = Vector2.down * rebound;
        else if (dir == -1)
            rb.velocity = Vector2.up * rebound;
        else if (dir == 0)
            rb.velocity = Vector2.left * rebound;
        else
        {
            rb.velocity = Vector2.right * rebound;
        }
    }

    //Использование Маны (хилка лишь)
    public void manaUse()
    {
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            if (mpPlayer > 0 && Input.GetButton("Heal") && canHeal && (hpPlayer < hpPlayerMax))
            {
                animat.SetBool("Healing", true);


                if (hpPlayer < hpPlayerMax)
                    hpPlayer += 1;
                else
                    animat.SetBool("Healing", false);
                sliderHp.value = hpPlayer;
                counterHp.text = hpPlayer.ToString() + "/" + hpPlayerMax.ToString();

                mpPlayer -= 1;
                sliderMp.value = mpPlayer;
                counterMp.text = mpPlayer.ToString() + "/" + mpPlayerMax.ToString();

                StartCoroutine(StopHealing());
                //animat.SetBool("Healing", false);
            }
            else if (!Input.GetButton("Heal"))
            {
                animat.SetBool("Healing", false);
            }
        }
    }

    private IEnumerator StopHealing()
    {
        canHeal = false;
        yield return new WaitForSeconds(stopHealing);
        canHeal = true;
        // animat.SetBool("Healing", false);
    }


    void Start()
    {
        _adListener = GetComponent<AudioListener>();
        _adSource = GetComponent<AudioSource>();


        spriteRender = GetComponent<SpriteRenderer>();
        matBlink = Resources.Load("PlayerBlink", typeof(Material)) as Material;
        matDefault = spriteRender.material;


        rb = GetComponent<Rigidbody2D>();
        bx = GetComponent<BoxCollider2D>();

        jumpCounter = jumpSecundMax;
        jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rb.gravityScale));

        rb.gravityScale = scaleGravity;
        canMove = true;


        hpPlayer = hpPlayerMax;

        counterHp.text = hpPlayer.ToString() + "/" + hpPlayerMax.ToString();

        sliderHp.maxValue = hpPlayerMax;
        sliderHp.minValue = 0;
        sliderHp.value = hpPlayer;

        mpPlayer = 4;

        counterMp.text = mpPlayer.ToString() + "/" + mpPlayerMax.ToString();

        sliderMp.maxValue = mpPlayerMax;
        sliderMp.minValue = 0;
        sliderMp.value = mpPlayer;
    }

    void FixedUpdate()
    {
        MovePlayer();
        DashPlayer();
        //AttackWeapon();
    }

    void Update()
    {
        AttackWeapon();
        JumpPlayer();
        Sliding();
        CanDamage();
        manaUse();

        counterHp.text = hpPlayer.ToString() + "/" + hpPlayerMax.ToString();
        counterMp.text = mpPlayer.ToString() + "/" + mpPlayerMax.ToString();
        sliderHp.value = hpPlayer;
        sliderMp.value = mpPlayer;
        
        if (hpPlayer <= 0)
        {
            _adListener.enabled = false;
            DeadMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        //collision.collider= this;
        //проверка на касание опасности
    }

    void OnCollisionEnter2D(Collision2D collision) //для передвигающихся платформ и конца уровня
    {
        if (collision.gameObject.tag == "FloatingPlatform")
        {
            transform.parent = collision.gameObject.transform;
        }

        if (collision.gameObject.tag == "EndLvl")
        {
            SceneManager.LoadScene(0);
        }
    }


    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "FloatingPlatform")
        {
            transform.parent = null;
        }
    }
}