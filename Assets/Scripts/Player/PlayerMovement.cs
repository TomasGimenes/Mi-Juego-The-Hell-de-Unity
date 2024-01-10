using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer spr;

    [Header("Movement")]
    private float inputX;
    private float horizontal = 0f;
    [SerializeField] private float normalSpeed;
    [SerializeField] private float movSpeed;
    [SerializeField] private float reverseSpeed;
    [Range(0, 0.3f)][SerializeField] private float movSmoothing;
    private Vector3 vel = Vector3.zero;
    private float reverseWalk;
    

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask isGround;
    [SerializeField] private Transform groundController;
    [SerializeField] private Vector3 boxDimensions;
    [SerializeField] private bool onGround;
    private bool jump = false;

    [Header("Guns")]
    [SerializeField] private GameObject subFusilPrefab;
    [SerializeField] private bool onGuns = false;

    [Header("Se Deve Desactivar")]
    private BoxCollider2D boxCollider2D;
    [SerializeField] private Vector3 boxDimensionsDesc;
    [SerializeField] private Transform desacController;
    [SerializeField] private LayerMask desactivador;
    [SerializeField] private bool onDesactive;

    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        subFusilPrefab.SetActive(false); //Hace que el prefab del arma empieze desactivado
        //initialGravity = rb2D.gravityScale;
    }
    void Update()
    {
        //Establece el input de movimiento horizontal y lo multiplica por la velocidad de movimiento
        inputX = Input.GetAxisRaw("Horizontal");
        horizontal = inputX * movSpeed;
        
        //-----------------------------------------------------------------------

        //Animaciones Caminar y Salto
        anim.SetFloat("SpeedX", Mathf.Abs(horizontal));
        anim.SetFloat("SpeedY", rb2D.velocity.y);
        //-----------------------------------------------------------------------

        //Si toca una tecla W o Space salto(jump) es verdadero
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        //-----------------------------------------------------------------------
        
        //Para el arma (input y Activa o desactiva el objeto del arma)
        if (Input.GetButtonDown("SubFusil") && onGuns == false)
        {
            subFusilPrefab.SetActive(true);
            onGuns = true;
        }
        else if (Input.GetButtonDown("SubFusil") && onGuns == true)
        {
            subFusilPrefab.SetActive(false);
            onGuns = false;
        }
        //Animacion Para el arma
        anim.SetBool("onGuns", onGuns);
        //-----------------------------------------------------------------------

        reverseWalk = inputX;
    }
    private void FixedUpdate() 
    {
        //Crea una caja para detectar el suelo y detecta si esta en el suelo para aplicar la animacion de salto
        onGround = Physics2D.OverlapBox(groundController.position, boxDimensions, 0f, isGround);
        onDesactive = Physics2D.OverlapBox(desacController.position, boxDimensionsDesc, 0f, desactivador);
        anim.SetBool("onGround", onGround);
        //-----------------------------------------------------------------------

        Move(horizontal * Time.fixedDeltaTime, jump); //Funcion creada para el movimiento del jugador
        
        jump = false;
    }

    private void Move(float move, bool leap)
    { 
        //Aplica el movimiento horizontal
        Vector3 targetSpeed = new Vector2(move, rb2D.velocity.y); //crea un objetivo de velocidad
        rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, targetSpeed, ref vel, movSmoothing);//Mueve al personaje izq y der con un suavizado de movimiento
        anim.SetFloat("ReverseWalk", reverseWalk);


        //-----------------------------------------------------------------------

        //Crea un circunferencia de 360 grados en base a la posicion que esta el mouse
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //print(angle);
        //-----------------------------------------------------------------------

        //Si no tiene el arma el giro se basa en movimiento   (GIRO)
        if(onGuns == false)
        {
            if (move > 0)
            {
                spr.flipX = false;
                //Rotate();
            }
            else if (move < 0)
            {
                spr.flipX = true;
                //Rotate();
            }
        }
        
        //Si tiene el arma el giro se basa en el angulo(Donde apunte el mouse)  (GIRO)
        else if(onGuns == true)
        {
            if (angle > 90 || angle < -90)
            {
                spr.flipX = true;
                reverseWalk = inputX * -1;
            }
            else
            {
                spr.flipX = false;
                reverseWalk = inputX;
            }
            anim.SetFloat("ReverseWalk", reverseWalk);
            print(reverseWalk);
        }
        //-----------------------------------------------------------------------
        if(onGuns == false)
        {
            movSpeed = normalSpeed;
        }

        if(reverseWalk == -1 && onGuns == true)
        {
            movSpeed = reverseSpeed;

        }
        else
        {
            movSpeed = normalSpeed;
        }

        //Dependiendo de los booleanos jump y onGround se ejecuta la funcion Jump()
        if(jump && onGround)
        {
            Jump();
        }
        //-----------------------------------------------------------------------
        if(onDesactive)
        {
            boxCollider2D.enabled = false;
        }
        else
        {
            boxCollider2D.enabled = true;
        }
    }

    //Funcion De ratacion multiplicando la escala por -1 (Guardado por las dudas)
    private void Rotate()
    {
        //facingRight = !facingRight;
        //Vector3 scale = transform.localScale;
        //scale.x *= -1;
        //transform.localScale = scale;
    }
    //-----------------------------------------------------------------------

    //Funcion que aplica el salto
    private void Jump()
    {
        onGround = false;
        rb2D.AddForce(new Vector2(0f, jumpForce));//Aplica el salto agragando una fuerza
    }
    //-----------------------------------------------------------------------

    //Dibuja referencias de colo amarillo para verlas en el editor
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundController.position, boxDimensions);//Referencia de la caja que detecta si hay piso o no
        Gizmos.DrawWireCube(desacController.position, boxDimensionsDesc);
    }
    //-----------------------------------------------------------------------
}
