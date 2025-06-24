using UnityEngine;

namespace Prefab
{
    public class MonsterAI : MonoBehaviour
    {
        [Header("Config")]
        public float speed = 2f;
        public float wallDamagePerSecond = 1f;
        public float playerDamagePerSecond = 10f;
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

                Player player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    int damage = Mathf.CeilToInt(playerDamagePerSecond * Time.deltaTime);
                    player.TakeDamage(damage);
                }
            }
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Player player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    int damage = Mathf.CeilToInt(playerDamagePerSecond * Time.deltaTime);
                    player.TakeDamage(damage);
                }
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
                Destroy(gameObject);
            }
        }
    }
}
