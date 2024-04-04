using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUi : MonoBehaviour
{

    private PlayerSettings _playerSettings;

    private void Start() {
        _playerSettings = PlayerSettings.Instance;
    }

    public void InputLogin(string login) {
        _playerSettings.SetLogin(login);
    }

    public void ClickConnect() {
        if (string.IsNullOrEmpty(_playerSettings.Login)) return;

        SceneManager.LoadScene("Game");

    }

}
