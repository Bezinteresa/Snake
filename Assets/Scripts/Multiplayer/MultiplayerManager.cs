using Colyseus;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultiplayerManager : ColyseusManager<MultiplayerManager>
{
    #region Server

    [field: SerializeField] public Skins _skins;

    private const string  GameRoomName = "state_handler";

    private ColyseusRoom<State> _room;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeClient();
        Connection();
    }

    private  async void Connection() {

        Dictionary<string, object> data = new Dictionary<string, object> {

            { "skins", _skins.length } 
            
        };


        _room = await client.JoinOrCreate<State>(GameRoomName, data);

        _room.OnStateChange += OnStateChange;
    }

    private void OnStateChange(State state, bool isFirstState) {
        if (isFirstState == false) return;
        _room.OnStateChange -= OnStateChange;

        state.players.ForEach((key, player) => {
        if (key == _room.SessionId) CreatePlayer(player);
        else CreateEnemy(key,player);
        });

        _room.State.players.OnAdd += CreateEnemy;
        _room.State.players.OnRemove += RemoveEnemy;
        
    }

    protected override void OnApplicationQuit() {
        base.OnApplicationQuit();
        LeaveRoom();
    }

    public void LeaveRoom() { 
        _room?.Leave();
    }

    public void  SendMessage( string key, Dictionary<string, object> data) {

        _room.Send(key,data  );
    }

    #endregion

    #region Player

    [SerializeField] private PlayerAim _playerAim;
    [SerializeField] private Controller _controllerPrefab;
    [SerializeField] private Snake _snakePrefab;

    private void CreatePlayer(Player player) {
        Vector3 position = new Vector3(player.x, 0, player.z);
        Quaternion quaternion = Quaternion.identity;

        Snake snake = Instantiate(_snakePrefab, position, quaternion);
        snake.Init(player.d);

        PlayerAim aim = Instantiate(_playerAim, position, quaternion);
        aim.Init(snake.Speed);

        Controller controller = Instantiate(_controllerPrefab);
        controller.Init( aim, player, snake);

        snake.SetSkin(_skins.GetMaterial(player.skin));
    }

    #endregion
    
    #region Enemy
    Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();
    private void CreateEnemy(string key, Player player) {
        Vector3 position = new Vector3(player.x, 0, player.z);

        Snake snake = Instantiate(_snakePrefab, position, Quaternion.identity);
        snake.Init(player.d);

        EnemyController enemy = snake.AddComponent<EnemyController>();
        enemy.Init(player, snake);

        _enemies.Add(key, enemy);

        snake.SetSkin(_skins.GetMaterial(player.skin));

        //Test (Сделать удаление точки(_worldPointer) при удалении енеми)
        //snake.AddComponent<EnemyPointer>().Init();
    }
    private void RemoveEnemy(string key, Player value) {
        if (_enemies.ContainsKey(key) == false) {
            Debug.Log("попытка уничтожения енеми которого нет вы словаре");
            return;
        }
        EnemyController  enemy = _enemies[key];
        _enemies.Remove(key);
        enemy.Destroy();
    }

    #endregion



}
