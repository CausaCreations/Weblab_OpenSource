using UnityEngine;

public class TileDeletion : MonoBehaviour
{
    private TileSpawner _tileSpawner;
    private TileDeletionArea _tileDeletionArea;
    private DragAndDrop _dragAndDrop;
    
    public void SetSpawner(TileSpawner tileSpawner)
    {
        _tileSpawner = tileSpawner;
    }

    public void SetTileDeletionArea(TileDeletionArea deletionArea)
    {
        _tileDeletionArea = deletionArea;
    }

    public void SetDragAndDrop(DragAndDrop dragAndDrop)
    {
        _dragAndDrop = dragAndDrop;
        _dragAndDrop.OnEndDragging.AddListener(CheckTilePositionOverTileDeletionArea);
    }
    
    public void CheckTilePositionOverTileDeletionArea(Transform tile)
    {
        if (_tileDeletionArea.GetPointerIsInDeletionArea() && tile == this.transform)
        {
            DeleteTile();
        }
    }

    public void DeleteTile()
    {
        _dragAndDrop.OnEndDragging.RemoveListener(CheckTilePositionOverTileDeletionArea);
        Destroy(this.gameObject);
        FindObjectOfType<TileGrid>().UnregisterTile(GetComponent<TileData>());
        _tileSpawner.OnTileDeleted();
    }
}
