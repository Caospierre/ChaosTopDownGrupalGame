using UnityEngine;

namespace Prefab
{
    public class MonsterAI : MonoBehaviour
    {
        public enum State { Standby, Chasing, SeekWall, AttackingWall }
        public string action;

        public float speed = 2f;
        public float detectionRadius = 3f;
        public float wallSearchTime = 5f;
        public float standbyWalkTime = 2f;
        public float standbyDuration = 3f; // <-- tiempo en standby antes de volver a buscar pared
        public float damagePerSecond = 5f;

        private Rigidbody2D rb;
        private State currentState = State.Standby;
        private Vector2 randomDir;
        private float standbyTimer = 0f;
        private float wallSeekTimer = 0f;
        private GameObject targetWall;
        private GameObject player;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            randomDir = Random.insideUnitCircle.normalized;
            InvokeRepeating(nameof(UpdateRandomDirection), 0f, standbyWalkTime);
        }

        void Update()
        {
            action = currentState.ToString();

            switch (currentState)
            {
                case State.Standby:
                    standbyTimer += Time.deltaTime;
                    rb.linearVelocity = randomDir * speed;

                    if (standbyTimer >= standbyDuration)
                    {
                        currentState = State.SeekWall;
                        wallSeekTimer = wallSearchTime;
                        standbyTimer = 0f;
                    }
                    break;

                case State.Chasing:
                    if (player != null)
                    {
                        Vector2 direction = (player.transform.position - transform.position).normalized;
                        rb.linearVelocity = direction * speed;
                    }
                    break;

                case State.SeekWall:
                    wallSeekTimer -= Time.deltaTime;

                    if (targetWall == null)
                        targetWall = FindClosestWall();

                    if (targetWall != null)
                    {
                        Vector2 direction = (targetWall.transform.position - transform.position).normalized;
                        rb.linearVelocity = direction * speed;
                    }

                    if (wallSeekTimer <= 0f)
                    {
                        currentState = State.Standby;
                    }
                    break;

                case State.AttackingWall:
                    rb.linearVelocity = Vector2.zero;
                    break;
            }
        }

        void UpdateRandomDirection()
        {
            if (currentState == State.Standby)
                randomDir = Random.insideUnitCircle.normalized;
        }

        GameObject FindClosestWall()
        {
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            float minDist = float.MaxValue;
            GameObject closest = null;

            foreach (var wall in walls)
            {
                float dist = Vector2.Distance(transform.position, wall.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = wall;
                }
            }

            return closest;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                player = other.gameObject;
                currentState = State.Chasing;
            }
            else if (other.CompareTag("Wall"))
            {
                currentState = State.AttackingWall;
                InvokeRepeating(nameof(ApplyWallDamage), 0f, 1f);
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                player = null;
                currentState = State.SeekWall;
                wallSeekTimer = wallSearchTime;
            }
            else if (other.CompareTag("Wall"))
            {
                CancelInvoke(nameof(ApplyWallDamage));
                currentState = State.Standby;
            }
        }

        void ApplyWallDamage()
        {
            if (targetWall != null)
            {
                var block = targetWall.GetComponent<Prefab.WallBlock>();
                if (block != null)
                {
                    bool destroyed = block.TakeDamage((int)damagePerSecond);
                    if (destroyed)
                    {
                        targetWall = null;
                        currentState = State.SeekWall;
                        wallSeekTimer = wallSearchTime;
                    }
                }
            }
        }
    }
}
