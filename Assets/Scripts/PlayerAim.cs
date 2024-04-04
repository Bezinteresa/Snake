
using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private LayerMask _colissionLayer;
    [SerializeField] private float _overlapRadius = 0.5f;
    [SerializeField] private float _rotateSpeed = 90f;
    private Transform _snakeHead;
    private Vector3 _targetDirection = Vector3.zero;
    private float _speed;


    public void Init(Transform snakeHead, float speed) {
        _snakeHead = snakeHead;
        _speed = speed;
    }

    void Update()
    {
        Rotate();
        Move();
        CheckExit();
    }

    private void FixedUpdate() {
        CheckColission();
    }

    private void CheckColission() {
        Collider[] colliders = Physics.OverlapSphere(_snakeHead.position, _overlapRadius, _colissionLayer);

        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].TryGetComponent(out Apple apple)) {
                apple.Collect();
            } else {


                if (colliders[i].GetComponentInParent<Snake>()) {
                    //Столкновение головы  с головой
                    Transform enemy = colliders[i].transform;
                    float playerAngle = Vector3.Angle(enemy.position - _snakeHead.position, transform.forward);
                    float enemyAngle = Vector3.Angle(_snakeHead.position - enemy.position, enemy.forward);
                    if (playerAngle > enemyAngle + 5) {
                        GameOver();
                    }
                } else {
                    //Столкновение в деталь
                    GameOver();
                }
            }
        }
    }

    private void GameOver() {
        FindObjectOfType<Controller>().Destroy();
        Destroy(gameObject);
    }

    private void Rotate() { 
       Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
    }

    private  void Move() {
        transform.position +=  transform.forward * Time.deltaTime * _speed;


    }

    private void CheckExit() {
        if (Math.Abs(_snakeHead.position.x) > 128 || Math.Abs(_snakeHead.position.z) > 128) GameOver();
    }

    public void SetTargetDirection(Vector3 pointToLook) {
        _targetDirection = pointToLook - transform.position;
    }

    public void GetMoveInfo(out Vector3 position) {
        position = transform.position;
    }

}
