
using UnityEngine;

public class Snake : MonoBehaviour
{
    public float Speed { get{return _speed;}}

    [SerializeField] private int _playerLayer = 6;
    [SerializeField] private Tail _tailPrefab;
    [field: SerializeField] public Transform _head { get; private set; }
    [SerializeField] private float _speed = 2f;

    [SerializeField] private SetSkin _setSkin;
    [SerializeField] ShowName _showName;

    //Перенесли в PlayerAim
    //[SerializeField] private float _rotateSpeed = 90f;
    //что то удалили его
    // [SerializeField] private Transform _directionPoint;

    private Tail _tail;

    public void Init( int detailCount, bool isPlayer = false) {
        if (isPlayer) {
            gameObject.layer = _playerLayer;
            var childrens = GetComponentsInChildren<Transform>();
            for ( int i = 0; i < childrens.Length; i++) {
                childrens[i].gameObject.layer = _playerLayer;
            }
        }
        Tail tail = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        tail.Init(_head, _speed, detailCount, _playerLayer, isPlayer);
        _tail = tail;
        
    }

    public void SetDetailCount(int detailCount) {
        _tail.SetDetailCount(detailCount);
    }

    public void SetSkin(Material material) {
        _setSkin.Set(material);
        _tail.SetSkin(material);
    }

    public void SetName(string login) {
        _showName.SetText(login);
    }

    public void Destroy(string clientID) {

        var detailPositions = _tail.GetDetailPosition();
        detailPositions.id = clientID;
        string json = JsonUtility.ToJson(detailPositions);
        MultiplayerManager.Instance.SendMessage("gameOver", json);
        _tail.Destroy();
        Destroy(gameObject);

    }

    void Update() {
        Move();

        //Вызывается с сервера для синхронизации
        //Rotate();
    }


    //Вызывается с сервера для синхронизации
    //private void Rotate() {
    //    Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
    //    _head.rotation = Quaternion.RotateTowards(_head.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
    //}

    private void Move() {
        transform.position +=  _head.forward * Time.deltaTime * _speed;
    }

    //private Vector3 _targetDirection = Vector3.zero;

    public void SetRotation(Vector3 pointToLook) { 
        //Удалили
        //_directionPoint.LookAt(pointToLook);
        _head.LookAt(pointToLook);
    }


    //Вызывается с сервера для синхронизации
    //public void LerpRotation(Vector3 cursorPosition) {
    //    _targetDirection = cursorPosition - _head.position;
    //}

    
    

}
