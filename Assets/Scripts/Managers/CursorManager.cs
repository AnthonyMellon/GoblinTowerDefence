using System;
using UnityEngine;
using Zenject;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Transform _cursorTracker;
    [SerializeField] private SpriteRenderer _attachedSpriteRenderer;
    private CursorAttachedObject _attachedObject;

    private InputProvider _inputProvider;

    [Inject]
    private void Initialize(InputProvider inputProvider)
    {
        _inputProvider = inputProvider;
    }

    private void OnEnable()
    {
        _inputProvider.OnNewMousePosition += MoveCursor;
        _inputProvider.OnInteract += UseCurrentAttachedObject;
    }

    private void OnDisable()
    {
        _inputProvider.OnNewMousePosition -= MoveCursor;
        _inputProvider.OnInteract -= UseCurrentAttachedObject;
    }

    public void SetAttachedObject(CursorAttachedObject attachedObject)
    {
        _attachedObject = attachedObject;
        if (_attachedObject == null)
        {
            _attachedSpriteRenderer.sprite = null;            
        }
        else
        {
            _attachedSpriteRenderer.sprite = _attachedObject.CurrSprite;
        }
    }

    public void RemoveAttachedObject(CursorAttachedObject attachedObject)
    {
        if (_attachedObject == attachedObject)
        {
            SetAttachedObject(null);
        }
    }

    private void UseCurrentAttachedObject(Vector2Int position)
    {
        if (_attachedObject == null) return;

        _attachedObject.UseObject(position);
    }

    private void MoveCursor(Vector2Int position)
    {
        _cursorTracker.localPosition = new Vector3(
            position.x,
            position.y,
            0
            );

        if(_attachedObject != null)
        {
            _attachedObject.UpdateValidity(position);
            _attachedSpriteRenderer.sprite = _attachedObject.CurrSprite;
        }
    }


    public class CursorAttachedObject
    {
        private Sprite _validSprite;
        private Sprite _invalidSprite;
        public Sprite CurrSprite { get; private set; }

        public delegate bool ValidityCheck(Vector2Int position);
        private ValidityCheck _validityCheck;

        public delegate void Use(Vector2Int position);
        private Use _use;

        public CursorAttachedObject(Sprite validSprite, Sprite invalidSprite, ValidityCheck validityCheck, Use use)
        {
            _validSprite = validSprite;
            _invalidSprite = invalidSprite;
            CurrSprite = invalidSprite;

            _validityCheck = validityCheck;
            _use = use;
        }

        public void UpdateValidity(Vector2Int position)
        {
            if(_validityCheck != null && _validityCheck.Invoke(position))
            {
                CurrSprite = _validSprite;
            }
            else
            {
                CurrSprite = _invalidSprite;
            }
        }

        public void UseObject(Vector2Int position)
        {
            _use?.Invoke(position);
        }
    }
}
