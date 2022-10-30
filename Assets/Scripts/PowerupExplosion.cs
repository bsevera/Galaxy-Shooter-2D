using UnityEngine;

public class PowerupExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 1.28f);
    }
}
