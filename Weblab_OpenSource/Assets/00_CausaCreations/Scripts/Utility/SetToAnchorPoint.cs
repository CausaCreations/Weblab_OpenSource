using UnityEngine;

public class SetToAnchorPoint : MonoBehaviour
{
    [SerializeField] private Transform _anchor;
    
    void Update()
    {
        transform.position = _anchor.position;
    }
}
