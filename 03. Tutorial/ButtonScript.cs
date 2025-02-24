using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{    
    public void OnPointerDown(PointerEventData eventData) 
    {
        DefaultTile targetDefaultTile = TileManager.Instance.defaultTiles[16].GetComponent<DefaultTile>();
        targetDefaultTile.OnStartButtonPress();
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        DefaultTile targetDefaultTile = TileManager.Instance.defaultTiles[16].GetComponent<DefaultTile>();
        targetDefaultTile.OnEndButtonPress();
    }    
}