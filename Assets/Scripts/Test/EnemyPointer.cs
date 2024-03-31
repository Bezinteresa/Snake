using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointer : MonoBehaviour
{
    //Радар врагов, точка показывает в какой стороне враг https://www.youtube.com/watch?v=x88p8gUtuJI

    //13,54

    private Transform _player;
    private Camera _camera;

    [SerializeField] Transform _worldPointer;
    

    //Поменять на Init
    public void Init() {
        StartCoroutine(FindPlayer());
        _camera = Camera.main;
        _worldPointer.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null) return;
        Vector3 fromPlayerToEnemy = transform.position - _player.position;
        Ray ray = new Ray(_player.position, fromPlayerToEnemy);

        //[0] = Left, [1] = Right, [2] = Down, [3] = Up
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);

        float minDistance = Mathf.Infinity;

        for (int i = 0; i < 4; i++) {
            if (planes[i].Raycast(ray, out float distance)) {
                if (distance < minDistance) { 
                    minDistance = distance;
                }
            }
        }
        minDistance = Mathf.Clamp(minDistance, 0, fromPlayerToEnemy.magnitude);

        Vector3 worldPosition = ray.GetPoint(minDistance);
        Debug.Log(worldPosition);
        _worldPointer.position = worldPosition;

    }

    private IEnumerator FindPlayer() {

        while (true) {
            if (FindObjectOfType<Snake>() != null) {
                _player = FindObjectOfType<Snake>().transform;
                yield break; // остановить корутину после того, как игрок найден
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
