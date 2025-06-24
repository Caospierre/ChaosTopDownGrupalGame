using UnityEngine;
using System;

namespace Prefab
{
    [RequireComponent(typeof(LineRenderer))]
    public class AreaDetector : MonoBehaviour
    {
        public enum ShapeType { Circle, Rectangle }
        public ShapeType shape = ShapeType.Circle;

        public float radius = 3f;
        public Vector2 size = new Vector2(4, 4);
        public LayerMask targetLayer;
        public int circleResolution = 32;

        [Header("Visual")]
        [Tooltip("Grosor de la línea del detector")]
        public float lineWidth = 0.05f;

        [Tooltip("Color de la línea del detector")]
        public Color lineColor = new Color(1, 0, 0, 0.6f);

        [Tooltip("Habilita el gizmo de depuración en la escena")]
        public bool debugGizmos = true;

        private LineRenderer lr;
        private bool wasInside = false;
        private GameObject detectedObject = null;

        public event Action<GameObject> OnEnter;
        public event Action<GameObject> OnExit;

        void Start()
        {
            lr = GetComponent<LineRenderer>();
            lr.loop = true;
            lr.useWorldSpace = false;
            lr.widthMultiplier = lineWidth;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lr.endColor = lineColor;
            lr.enabled = false;

            if (shape == ShapeType.Circle)
                DrawCircle();
            else
                DrawRectangle();
        }

        void Update()
        {
            Collider2D hit = shape == ShapeType.Circle
                ? Physics2D.OverlapCircle(transform.position, radius, targetLayer)
                : Physics2D.OverlapBox(transform.position, size, 0f, targetLayer);

            bool isInside = hit != null;

            if (isInside && !wasInside)
            {
                detectedObject = hit.gameObject;
                OnEnter?.Invoke(detectedObject);
                lr.enabled = true;
            }
            else if (!isInside && wasInside)
            {
                OnExit?.Invoke(detectedObject);
                lr.enabled = false;
                detectedObject = null;
            }

            wasInside = isInside;
        }

        void DrawCircle()
        {
            lr.positionCount = circleResolution;
            for (int i = 0; i < circleResolution; i++)
            {
                float angle = i * Mathf.PI * 2f / circleResolution;
                Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
                lr.SetPosition(i, pos);
            }
        }

        void DrawRectangle()
        {
            lr.positionCount = 5;
            float x = size.x / 2f;
            float y = size.y / 2f;
            lr.SetPosition(0, new Vector3(-x, -y));
            lr.SetPosition(1, new Vector3(-x, y));
            lr.SetPosition(2, new Vector3(x, y));
            lr.SetPosition(3, new Vector3(x, -y));
            lr.SetPosition(4, new Vector3(-x, -y));
        }

        void OnDrawGizmosSelected()
        {
            if (!debugGizmos) return;

            Gizmos.color = lineColor;

            if (shape == ShapeType.Circle)
                Gizmos.DrawWireSphere(transform.position, radius);
            else
                Gizmos.DrawWireCube(transform.position, size);
        }
        
        public void SetColor(Color newColor)
        {
            lineColor = newColor;

            if (lr == null)
                lr = GetComponent<LineRenderer>();

            lr.startColor = lr.endColor = newColor;
        }
    }
}
