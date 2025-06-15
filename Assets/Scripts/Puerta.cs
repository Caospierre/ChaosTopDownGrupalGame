using UnityEngine;

public class Puerta : MonoBehaviour
{
    [SerializeField] private int targetSceneIndex;
    [SerializeField] private Vector3 newPlayerPosition;
    [SerializeField] private Vector2 newPlayerRotation;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            LocationManager.Instance.LoadNewScene(targetSceneIndex, newPlayerPosition, newPlayerRotation);
        }
    }
}
