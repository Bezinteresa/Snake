
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    [SerializeField] private Transform _detailPrefab;
    [SerializeField] private float _detailDistance = 1f;

    [SerializeField] SetSkin _skin;

    private Transform _head;
    private float _snakeSpeed = 2f;
    private List< Transform> _details = new List<Transform>();
    private List<Vector3> _positionHistory = new List<Vector3>();
    private List<Quaternion> _rotationHistory = new List<Quaternion>();

    private Material _currentSkin;

    public void Init(Transform head, float speed, int detailCount) {
        _snakeSpeed = speed;
        _head = head;   

        _details.Add(transform);
        _positionHistory.Add(_head.position);
        _rotationHistory.Add(_head.rotation);
        _positionHistory.Add(transform.position);
        _rotationHistory.Add(transform.rotation);

        SetDetailCount(detailCount);
    }

    public void SetDetailCount(int detailCount) { 
        //���-�� ������� ����� ����� 1) 0;  2) 10;  
        if (detailCount == _details.Count - 1) return;

        // 1) 0-5 = -5;  2) 10 - 5 = 5;
        int diff = (_details.Count - 1) - detailCount;

        // 1) -5 < 1 
        if (diff < 1) {
            // diff = -5;  -diff = -(-5)= 5;
            for (int i = 0; i < -diff; i++) {
                AddDetail();
            }
        }
        // 2) 5 > 1
        else
        {
            for (int i = 0; i < diff; i++) {
                RemoveDetail();
            }
        }
    }

    public void SetSkin(Material material) {
        
        for (int i = 0; i < _details.Count; i++) {
            _details[i].GetComponent<SetSkin>().Set(material);
        }
    }

    private void AddDetail() {
        Vector3 position = _details[_details.Count - 1].position;
        Quaternion rotation = _details[_details.Count - 1].rotation;
        Transform detail = Instantiate(_detailPrefab, position, rotation);


        //����������� � ������ ������� �������� �� ������ �����
        _details.Insert(0, detail);
        //�������  ����������� � ����� �������
        _positionHistory.Add(position);
        _rotationHistory.Add(rotation);
    }

    private void RemoveDetail() {
        if (_details.Count <= 1) {
            Debug.LogError("�������� ������� ������ ������� ���");
            return;
        }

        Transform detail = _details[0];
        _details.Remove(detail);
        Destroy(detail.gameObject);
        _positionHistory.RemoveAt(_positionHistory.Count - 1);
        _rotationHistory.RemoveAt(_rotationHistory.Count - 1);
    }

    void Update()
    {
        //������
        float distance = (_head.position - _positionHistory[0]).magnitude;
        
        while (distance > _detailDistance) { 
            Vector3 direction = (_head.position - _positionHistory[0]).normalized;
            _positionHistory.Insert(0, _positionHistory[0] + direction * _detailDistance);
            _positionHistory.RemoveAt(_positionHistory.Count - 1);

            _rotationHistory.Insert(0, _head.rotation);
            _rotationHistory.RemoveAt(_rotationHistory.Count - 1);

            distance -= _detailDistance;
        }

        //����� �����������
        for (int i = 0; i < _details.Count; i++) {
            //���������� ����� ������ ������������ � ������� ����� ����� distance/detailDistance
            float percent = distance / _detailDistance;
            _details[i].position = Vector3.Lerp(_positionHistory[i+1], _positionHistory[i], percent);
            _details[i].rotation = Quaternion.Lerp(_rotationHistory[i + 1], _rotationHistory[i], percent);


            //Vector3 direction = (_positionHistory[i] - _positionHistory[i+1]).normalized;
            //_details[i].position += direction * Time.deltaTime * _snakeSpeed;
        }
    }

    internal void Destroy() {
        for (int i = 0; i < _details.Count; i++) {
            Destroy(_details[i].gameObject);
        }
    }
}
