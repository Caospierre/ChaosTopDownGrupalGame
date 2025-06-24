using System.Collections;
using Prefab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Tooltip("¿Está recibiendo daño actualmente?")]
    public bool isReceivingDamage = false;

    [Header("Daño del arma")]
    [SerializeField] private float minWeaponDamage = 5f;
    [SerializeField] private float maxWeaponDamage = 20f;

    public bool Interactuando { get => interactuando; set => interactuando = value; }

    public float Lives
    {
        get => GameSceneController.GetLives();
        set => GameSceneController.SetLives(Mathf.FloorToInt(value));
    }

    public float Health
    {
        get => GameSceneController.GetHealth();
        set => GameSceneController.SetHealth(Mathf.FloorToInt(value));
    }

    public bool isGameOver => Lives <= 0;

    private MonsterAI ultimoMonsterDetectado;

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

    private void LecturaInputs()
    {
        if (inputV == 0)
            inputH = Input.GetAxisRaw("Horizontal");
        if (inputH == 0)
            inputV = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.E))
            LanzarInteraccion();
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
                StartCoroutine(Mover());
        }
        else if (inputH == 0 && inputV == 0)
        {
            anim.SetBool("andando", false);
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

    private void LanzarInteraccion()
    {
        colliderDelante = LanzarCheck();
        if (colliderDelante && colliderDelante.TryGetComponent(out Interactuable interactuable))
            interactuable.Interactuar();
    }

    private Collider2D LanzarCheck()
    {
        return Physics2D.OverlapCircle(puntoInteraccion, radioInteraccion);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(puntoInteraccion, radioInteraccion);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Monster"))
            return;

        Debug.Log("Monster collision");

        GameObject armaActual = GameSceneController.Instance.currentWeapon;
        var itemHolder = armaActual.GetComponent<ItemHolder>();
        if (itemHolder == null || itemHolder.item == null)
            return;

        if (!float.TryParse(itemHolder.item.health, out float weaponHealth) || weaponHealth <= 0f)
            return;

        int damage = Mathf.FloorToInt(Random.Range(minWeaponDamage, maxWeaponDamage));

        if (collision.gameObject.TryGetComponent(out MonsterAI monster))
        {
            monster.TakeDamage(damage);

            if (ultimoMonsterDetectado != null && ultimoMonsterDetectado != monster)
                ultimoMonsterDetectado.CambiarColorArea(Color.red);

            monster.CambiarColorArea(Color.blue);
            ultimoMonsterDetectado = monster;
        }

        int desgaste = Mathf.FloorToInt(damage / 100f);
        GameSceneController.Instance.ReducirDurabilidadActualWeapon(desgaste);
    }

    public void TakeDamage(float amount)
    {
        if (isGameOver)
            return;

        isReceivingDamage = true;
        Health -= amount;

        if (Health <= 0)
        {
            Lives--;

            if (Lives > 0)
            {
                Debug.Log("☠️ Perdiste una vida. Reiniciando salud.");
                Health = 100;
            }
            else
            {
                Health = 0;
                Debug.Log("💀 Game Over");
                GameSceneController.Instance.GameOver();
            }
        }

        isReceivingDamage = false;
    }
}
