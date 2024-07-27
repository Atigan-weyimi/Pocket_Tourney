using UnityEngine;

// Base class for all puck in the game
public class Puck : MonoBehaviour
{
    [SerializeField] private GameObject _selectionCircle;

    private bool canShoot;
    
    public bool CanShoot
    {
        get => canShoot;
        set
        {
            canShoot = value;
            SetSelectionActive(value);
        }
    }

    public void SetSelectionActive(bool isActive)
    {
        if (_selectionCircle == null)
        {
            return;
        }
        _selectionCircle.gameObject.SetActive(isActive);
    }

    public void SetTexture(Texture2D texture)
    {
        GetComponent<Renderer>().material.mainTexture = texture;
    }
}