using UnityEngine;

public class TargetArrow : MonoBehaviour
{
    [SerializeField] private Renderer _circleRenderer;
    [SerializeField] private Renderer _arrowRenderer;
    [SerializeField] private float _minCircleScale;
    [SerializeField] private float _maxCircleScale;
    [SerializeField] private float _arrowMinOffset;
    [SerializeField] private float _arrowMaxOffset;
    [SerializeField] private float _arrowMaxScale;

    public void ShowDrag(Vector3 position, Vector3 normalizedDragDelta)
    {
        _arrowRenderer.enabled = true;
        _circleRenderer.enabled = true;
        
        var scale = normalizedDragDelta.magnitude;
        var outRotation = GetAngle(normalizedDragDelta);

        transform.position = position;
        transform.eulerAngles = new Vector3(0, 0, outRotation);
        
        var circeScale = Mathf.Lerp(_minCircleScale, _maxCircleScale, scale);
        _circleRenderer.transform.localScale = new Vector3(circeScale, circeScale, 0.001f);

        var arrowScale = _arrowMaxScale * scale;
        _arrowRenderer.transform.localScale = new Vector3(arrowScale, arrowScale, 0.001f);
        _arrowRenderer.transform.localPosition = Vector3.right * Mathf.Lerp(_arrowMinOffset, _arrowMaxOffset, scale);
    }

    private float GetAngle(Vector3 vector)
    {
        vector.z = 0;
        var direction = vector.normalized;
        var acos = Mathf.Acos(direction.x);
        if (vector.y < 0)
        {
            acos *= -1f;
        }
        return acos * 180f / Mathf.PI;;
    }

    public void Hide()
    {
        _arrowRenderer.enabled = false;
        _circleRenderer.enabled = false;
    }
}