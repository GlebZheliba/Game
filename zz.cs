using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class Zz : MonoBehaviour
{
    [SerializeField] GameObject attackHitBox;
    [SerializeField] private int hp = 100;
    [SerializeField] private int stamina = 100;
    Animator animator;
    public Image HPBar;
    public Image StaminaBar;
    public string groundTag = "Ground";
    public string playerTag = "Player2";
    public string HPTag = "HP";
    public bool checkGround = true;
    private bool canJump = true;
    public KeyCode Jkey = KeyCode.I;
    public KeyCode Pkey = KeyCode.RightShift;
    public KeyCode left = KeyCode.J;
    public KeyCode right = KeyCode.L;
    public KeyCode Attack = KeyCode.O;
    bool isAttacking = false;
    private Rigidbody2D rb;
    SpriteRenderer sprite;
    public float speed = 5f; 
    public float jump = 5f;
    public float horizontalDamping = 2f;
    public float push = 5f;
    private float increaseRate = 1f;
    public Transform player1; // Трансформ игрока 1
    public Transform player2; // Трансформ игрока 2
    [SerializeField] private bool isInvincible = false; 
    public KeyCode protectionKey = KeyCode.RightControl;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
    if (Input.GetKeyDown(protectionKey )  &&!isInvincible && !isAttacking )
            {
                StartCoroutine(EnableInvincibilityForOneSecond());
                
            }

    if (canJump)
    {
        if (Input.GetKey(left))
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
           
        }
        else if (Input.GetKey(right))
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
           
        }
        else
        {
            // Постепенное уменьшение скорости после отжатия кнопки
            float deceleration = 5f; // Коэффициент уменьшения скорости
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, Time.deltaTime * deceleration), rb.velocity.y);
            
        }
    }

    if (canJump && Input.GetKeyDown(Jkey) && stamina >= 10)
    {
        // Применить силу прыжка
        rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
        canJump =!checkGround;
        stamina -= 10;
        StaminaBar.fillAmount = stamina*0.01f;
       

        // Применить затухание горизонтальной скорости
        rb.velocity = new Vector2(rb.velocity.x * horizontalDamping, rb.velocity.y);
    }

    if ( Input.GetKeyDown(Pkey) && stamina >= 10 && !IsPlayer1JumpingOverPlayer2() )
    {
        rb.velocity = new Vector2(speed * push, rb.velocity.y);
        stamina -= 10;
        StaminaBar.fillAmount = stamina*0.01f;
    }

     if ( Input.GetKeyDown(Pkey) && stamina >= 10  && IsPlayer1JumpingOverPlayer2())
    {
        rb.velocity = new Vector2(-speed * push, rb.velocity.y);
        stamina -= 10;
        StaminaBar.fillAmount = stamina*0.01f;
    }

    if (Input.GetKeyDown(Attack) && !isAttacking && stamina >= 10 && !isInvincible )
    {
        isAttacking = true;
        StartCoroutine(DoAttack());
        stamina -= 10;
        StaminaBar.fillAmount = stamina*0.01f;
        
    }

    if (IsPlayer1JumpingOverPlayer2())
    {
        player1.rotation = Quaternion.Euler(0, 0, 0);
    }
    else
    {
        player1.rotation = Quaternion.Euler(0, 180, 0);
    }
}

private IEnumerator EnableInvincibilityForOneSecond()
{
    isInvincible = true;
    yield return new WaitForSeconds(1f);
    isInvincible = false;
    yield return new WaitForSeconds(2f); 
}


private bool IsPlayer1JumpingOverPlayer2()
    {
        return player1.position.x > player2.position.x;
    }

private void Start()
    {
        isInvincible = false;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(IncreaseStaminaOverTime());
        // При старте игры выключить коллайдер атаки
        attackHitBox.SetActive(false);
    }

    

    private IEnumerator IncreaseStaminaOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (stamina < 100)
            {
                stamina += 1; 
                StaminaBar.fillAmount = stamina*0.01f;
            }
        }
    }

    private IEnumerator DoAttack()
    {
        attackHitBox.SetActive(true);
        yield return new WaitForSeconds(1f);
        attackHitBox.SetActive(false);
        isAttacking = false; 
    }

    private void OnCollisionEnter2D(Collision2D collisionData)
    {
        if (checkGround && collisionData.gameObject.CompareTag(groundTag) )
        {
            canJump = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("attackHitBox"))
        {
            TakeDamage(3); 
        }
    }

    void TakeDamage(int damage)
    {
        hp -= damage; 
        HPBar.fillAmount = hp*0.01f;
        if (hp <= 0)
        {
            Die(); 
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
}