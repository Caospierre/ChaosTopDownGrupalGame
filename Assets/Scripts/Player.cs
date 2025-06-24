using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float inputH;
    private float inputV;
    private bool moviendo;
    private Vector3 puntoDestino;
    private Vector3 ultimoInput;
    private Vector3 puntoInteraccion;
    private Collider2D colliderDelante;
    private Animator anim;

    [SerializeField] private float velocidadMocimiento;
    [SerializeField] private float radioInteraccion;
    private bool interactuando;

    [Tooltip("Vidas")]
    [SerializeField] public float lives = 3f;

    [Tooltip("Salud")]
    [SerializeField] public float health = 100;

    [Tooltip("¿El jugador ya perdió todas las vidas?")]
    public bool isGameOver = false;

    [Tooltip("¿Está recibiendo daño actualmente?")]
    public bool isReceivingDamage = false;

    public bool Interactuando { get => interactuando; set => interactuando = value; }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        transform.position = LocationManager.Instance.LastSavedPosition;
        anim.SetFloat("inputH", LocationManager.Instance.LastSavedRotation.x);
        anim.SetFloat("inputV", LocationManager.Instance.LastSavedRotation.y);
    }

    void Update()
    {
        LecturaInputs();
        MovimientoYAnimaciones();
    }

    private void MovimientoYAnimaciones()
    {
        if (!interactuando && !moviendo && (inputH != 0 || inputV != 0))
        {
            anim.SetBool("andando", true);
            anim.SetFloat("inputH", inputH);
            anim.SetFloat("inputV", inputV);
            ultimoInput = new Vector3(inputH, inputV, 0);
            puntoDestino = transform.position + ultimoInput;
            puntoInteraccion = puntoDestino;

            colliderDelante = LanzarCheck();

            if (!colliderDelante)
            {
                StartCoroutine(Mover());
            }
        }
        else if (inputH == 0 && inputV == 0)
        {
            anim.SetBool("andando", false);
        }
    }

    private void LecturaInputs()
    {
        if (inputV == 0)
        {
            inputH = Input.GetAxisRaw("Horizontal");
        }
        if (inputH == 0)
        {
            inputV = Input.GetAxisRaw("Vertical");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            LanzarInteraccion();
        }
    }

    private void LanzarInteraccion()
    {
        colliderDelante = LanzarCheck();
        if (colliderDelante)
        {
            if (colliderDelante.TryGetComponent(out Interactuable interactuable))
            {
                interactuable.Interactuar();
            }
        }
    }

    IEnumerator Mover()
    {
        moviendo = true;
        while (transform.position != puntoDestino)
        {
            transform.position = Vector3.MoveTowards(transform.position, puntoDestino, velocidadMocimiento * Time.deltaTime);
            yield return null;
        }
        puntoInteraccion = transform.position + ultimoInput;
        moviendo = false;
    }

    private Collider2D LanzarCheck()
    {
        return Physics2D.OverlapCircle(puntoInteraccion, radioInteraccion);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(puntoInteraccion, radioInteraccion);
    }

    public void TakeDamage(float amount)
    {
        if (isGameOver)
            return;

        isReceivingDamage = true;

        health -= amount;

        if (health <= 0)
        {
            lives--;

            if (lives > 0)
            {
                Debug.Log("☠️ Perdiste una vida. Reiniciando salud.");
                health = 100;
            }
            else
            {
                health = 0;
                isGameOver = true;
                Debug.Log("💀 Game Over");
            }
        }

        isReceivingDamage = false;
    }
}
