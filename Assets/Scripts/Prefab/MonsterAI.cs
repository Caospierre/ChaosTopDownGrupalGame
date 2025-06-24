using UnityEngine;

namespace Prefab
{
    public class MonsterAI : MonoBehaviour
    {
        [Header("Config")]
        public float speed = 2f;
        public float wallDamagePerSecond = 1f;

        [Header("Daño al jugador")]
        public float playerDamagePerSecond = 10f;
        public float damageInterval = 3f;
        private float damageTimer = 0f;

        [Header("Movimiento")]
        public float standbyMoveDuration = 2f;
        public float standbyWaitDuration = 1f;

        [Header("SeekWall Timeout")]
        public float seekWallMaxTime = 30f;
        private float seekWallTimer = 0f;

        [Header("Health")]
        public int maxHealth = 100;
        public int currentHealth;

        [Header("Debug")]
        public string actualState;

        [Header("Recompensas")]
        public GameObject[] dropItems;

        private enum State { Standby, SeekWall, AttackWall, ChasePlayer }
        private State currentStateEnum = State.Standby;

        private GameObject wallTarget;
        private GameObject playerTarget;
        private AreaDetector areaDetector;

        private Vector2 standbyDirection;
        private float standbyTimer;
        private bool isWaiting;

        void Start()
        {
            currentHealth = maxHealth;

            areaDetector = GetComponentInChildren<AreaDetector>();
            if (areaDetector != null)
            {
                areaDetector.OnEnter += HandlePlayerEnter;
                areaDetector.OnExit += HandlePlayerExit;
            }

            currentStateEnum = State.Standby;
            StartNewStandbyCycle();
        }

        void Update()
        {
            actualState = currentStateEnum.ToString();
            damageTimer += Time.deltaTime;

            switch (currentStateEnum)
            {
                case State.Standby:
                    UpdateStandbyMovement();
                    break;

                case State.SeekWall:
                    seekWallTimer += Time.deltaTime;

                    if (seekWallTimer >= seekWallMaxTime)
                    {
                        wallTarget = null;
                        currentStateEnum = State.Standby;
                        StartNewStandbyCycle();
                        break;
                    }

                    if (wallTarget == null)
                        wallTarget = FindNearestWall();

                    if (wallTarget != null)
                        MoveTowards(wallTarget.transform.position);
                    break;

                case State.AttackWall:
                    if (wallTarget != null)
                        DamageWall(wallTarget);
                    break;

                case State.ChasePlayer:
                    if (playerTarget != null)
                        MoveTowards(playerTarget.transform.position);
                    break;
            }

            if (currentStateEnum != State.ChasePlayer && playerTarget != null)
            {
                playerTarget = null;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (currentHealth <= 0)
                return;

            if (collision.gameObject.CompareTag("Wall"))
            {
                currentStateEnum = State.AttackWall;
                wallTarget = collision.gameObject;
            }
            else if (collision.gameObject.CompareTag("Colisiones"))
            {
                if (currentStateEnum == State.SeekWall)
                {
                    wallTarget = null;
                    currentStateEnum = State.Standby;
                    StartNewStandbyCycle();
                }
                else if (currentStateEnum == State.Standby)
                {
                    wallTarget = FindNearestWall();
                    currentStateEnum = wallTarget ? State.SeekWall : State.Standby;
                    if (currentStateEnum == State.SeekWall)
                        seekWallTimer = 0f;
                    if (currentStateEnum == State.Standby)
                        StartNewStandbyCycle();
                }
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                HandlePlayerEnter(collision.gameObject);
                AplicarDanioAlJugador(collision.gameObject);
            }
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if (currentHealth <= 0)
                return;

            if (collision.gameObject.CompareTag("Player"))
            {
                AplicarDanioAlJugador(collision.gameObject);
            }
        }

        private void AplicarDanioAlJugador(GameObject target)
        {
            if (damageTimer < damageInterval)
                return;

            Player player = target.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(playerDamagePerSecond);
                damageTimer = 0f;
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject == wallTarget)
            {
                wallTarget = null;
                currentStateEnum = State.Standby;
                StartNewStandbyCycle();
            }
        }

        private void MoveTowards(Vector3 target)
        {
            Vector2 dir = (target - transform.position).normalized;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                dir = new Vector2(Mathf.Sign(dir.x), 0);
            else
                dir = new Vector2(0, Mathf.Sign(dir.y));

            transform.position += (Vector3)(dir * speed * Time.deltaTime);
        }

        private void DamageWall(GameObject wall)
        {
            WallBlock wallBlock = wall.GetComponent<WallBlock>();
            if (wallBlock != null && wallBlock.IsAlive())
            {
                int damage = Mathf.CeilToInt(wallDamagePerSecond * Time.deltaTime);
                wallBlock.TakeDamage(damage);
            }
            else
            {
                wallTarget = null;
                currentStateEnum = State.Standby;
                StartNewStandbyCycle();
            }
        }

        private GameObject FindNearestWall()
        {
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            GameObject nearest = null;
            float minDist = Mathf.Infinity;

            foreach (GameObject wall in walls)
            {
                float dist = Vector2.Distance(transform.position, wall.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = wall;
                }
            }

            return nearest;
        }

        private void HandlePlayerEnter(GameObject player)
        {
            playerTarget = player;
            currentStateEnum = State.ChasePlayer;
        }

        private void HandlePlayerExit(GameObject player)
        {
            if (player == playerTarget)
            {
                playerTarget = null;
                currentStateEnum = State.Standby;
                StartNewStandbyCycle();
            }
        }

        private void UpdateStandbyMovement()
        {
            standbyTimer -= Time.deltaTime;

            if (standbyTimer <= 0)
            {
                if (isWaiting)
                {
                    wallTarget = FindNearestWall();
                    currentStateEnum = wallTarget ? State.SeekWall : State.Standby;
                    if (currentStateEnum == State.SeekWall)
                        seekWallTimer = 0f;
                    if (currentStateEnum == State.Standby)
                        StartNewStandbyCycle();
                }
                else
                {
                    isWaiting = true;
                    standbyTimer = standbyWaitDuration;
                }
            }

            if (!isWaiting)
            {
                transform.position += (Vector3)(standbyDirection * (speed * 0.5f * Time.deltaTime));
            }
        }

        private void StartNewStandbyCycle()
        {
            int dir = Random.Range(0, 4);
            switch (dir)
            {
                case 0: standbyDirection = Vector2.up; break;
                case 1: standbyDirection = Vector2.down; break;
                case 2: standbyDirection = Vector2.left; break;
                case 3: standbyDirection = Vector2.right; break;
            }

            standbyTimer = standbyMoveDuration;
            isWaiting = false;
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;

            if (currentHealth <= 0)
            {
                DropRandomItem();
                CambiarColorArea(Color.gray);
                Destroy(gameObject);
            }
            else
            {
                CambiarColorArea(Color.yellow);
            }
        }

        private void DropRandomItem()
        {
            if (dropItems == null || dropItems.Length == 0) return;

            int index = Random.Range(0, dropItems.Length);
            GameObject item = dropItems[index];

            if (item != null)
                Instantiate(item, transform.position, Quaternion.identity);
        }

        public void CambiarColorArea(Color nuevoColor)
        {
            if (areaDetector != null)
                areaDetector.SetColor(nuevoColor);
        }
    }
}
