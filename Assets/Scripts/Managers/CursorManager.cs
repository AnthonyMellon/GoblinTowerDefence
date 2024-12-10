using UnityEngine;
using Zenject;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Transform _cursorTracker;
    [SerializeField] private SpriteRenderer _attachedSprite;

    private InputProvider _inputProvider;

    [Inject]
    private void Initialize(InputProvider inputProvider)
    {
        _inputProvider = inputProvider;
    }

    private void OnEnable()
    {
        _inputProvider.OnNewMousePosition += MoveCursor;
    }

    private void OnDisable()
    {
        _inputProvider.OnNewMousePosition -= MoveCursor;
    }

    public void SetAttachedSprite(Sprite newSprite)
    {
        _attachedSprite.sprite = newSprite;
    }

    public void ClearAttachedSprite()
    {
        _attachedSprite.sprite = null;
    }

    private void MoveCursor(Vector2Int position)
    {
        _cursorTracker.localPosition = new Vector3(
            position.x,
            position.y,
            0
            );
    }
}
