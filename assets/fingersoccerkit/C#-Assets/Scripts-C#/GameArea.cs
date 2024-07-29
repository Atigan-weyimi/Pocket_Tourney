using System;
using UnityEngine;

public class GameArea : MonoBehaviour
{
    [SerializeField] private Rect _rect;

    public Rect Rect => _rect;

    private void OnDrawGizmos()
    {
        var colorWas = Gizmos.color;
        Gizmos.color = Color.blue;
        
        Gizmos.DrawLine(new Vector3(_rect.xMin, _rect.yMin), new Vector3(_rect.xMax, _rect.yMin));
        Gizmos.DrawLine(new Vector3(_rect.xMax, _rect.yMin), new Vector3(_rect.xMax, _rect.yMax));
        Gizmos.DrawLine(new Vector3(_rect.xMax, _rect.yMax), new Vector3(_rect.xMin, _rect.yMax));
        Gizmos.DrawLine(new Vector3(_rect.xMin, _rect.yMax), new Vector3(_rect.xMin, _rect.yMin));

        Gizmos.color = colorWas;
    }
}