using UnityEngine;

public class Rotator : MonoBehaviour
{
    public int dir = 1;
    public int speed = 40;

    // Update is called once per frame
    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 180, Time.time * speed * dir);
    }
}