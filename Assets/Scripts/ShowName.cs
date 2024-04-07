
using UnityEngine;
using UnityEngine.UI;

public class ShowName : MonoBehaviour {
    [SerializeField] private Text _text;
    [SerializeField] private GameObject _canvas;

    public void SetText(string login) {
        _canvas.SetActive(true);
        _text.text = login;
    }
}
