using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;


public class BirdHandler : MonoBehaviour
{
    
    [TitleGroup("Gameplay")]
    [SerializeField] private float _detachDelay = 0.15f;
    [SerializeField] private float _spawnDelay = 1.5f;
    
    [TitleGroup("References")]
    [SerializeField] private GameObject _birdPrefab;
    [SerializeField] private Rigidbody2D _birdPivot;
    [ReadOnly, SerializeField] private Rigidbody2D _currentBirdBody;
    [ReadOnly, SerializeField] private SpringJoint2D _currentBirdSpringJoint;
    
    [TitleGroup("Touch")]
    [ReadOnly, SerializeField] private bool _isPressing;
    [ReadOnly, SerializeField] private bool _isDragingBird;
    [ReadOnly, SerializeField] private Vector2 _touchPosScreen;
    [ReadOnly, SerializeField] private Vector2 _touchPosWorld;
    
    private Camera _mainCamera;
    
    void Start()
    {
        _mainCamera = Camera.main;
        SpawnNewBird();
    }
    
    void Update()
    {
        if (_currentBirdBody is null)
            return;
        
        UpdateInputs();
        UpdateCurrentBirdState();
    }

    private void UpdateInputs()
    {
        // primaryTouch is only the first registered touch at time.
        if (Touchscreen.current is null)
            return;
        
        _isPressing = Touchscreen.current.primaryTouch.press.isPressed;
        _touchPosScreen = Touchscreen.current.primaryTouch.position.ReadValue();
        _touchPosWorld = _mainCamera.ScreenToWorldPoint(_touchPosScreen);
        
        if (_isPressing) 
            _isDragingBird = true;
    }
    
    private void UpdateCurrentBirdState()
    {
        if (!_isPressing)
        {
            if (_isDragingBird)
                LaunchBird();
        }
        else
        {
            _currentBirdBody.isKinematic = true;
            _currentBirdBody.MovePosition(_touchPosWorld);
        }
    }

    private void LaunchBird()
    {
        _isDragingBird = false;
        _currentBirdBody.isKinematic = false;
        _currentBirdBody = null;
        
        // It needs to be detached from the spring a frame later,
        // otherwise, no tension is pulling the object, so, it doesn't move forward.
        Invoke(nameof(DetachBird), _detachDelay);
    }
    
    private void DetachBird()
    {
        _currentBirdSpringJoint.enabled = false;
        _currentBirdSpringJoint = null;
        
        Invoke(nameof(SpawnNewBird), _spawnDelay);
    }

    private void SpawnNewBird()
    {
        GameObject newBird = Instantiate(_birdPrefab, _birdPivot.position, Quaternion.identity);
        newBird.transform.SetParent(transform.parent);
        _currentBirdBody = newBird.GetComponent<Rigidbody2D>();
        _currentBirdSpringJoint = newBird.GetComponent<SpringJoint2D>();
        _currentBirdSpringJoint.connectedBody = _birdPivot;
    }
    
}
