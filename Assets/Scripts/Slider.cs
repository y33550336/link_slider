using UnityEngine;

public class Slider : MonoBehaviour
{
    public GameObject[] cells = new GameObject[49];
    // 7*7のセルでいくつかのスライダーがある
    public GameObject[] sliders = new GameObject[12];
    public GameObject[] players = new GameObject[2];
    private int boardSize = 7;
    private int[,] cellValues = new int[7, 7];
    // 0は空のセル、1はスライダーがある、2はプレイヤー1の位置、3はプレイヤー2の位置
    private int[,] sliderValues = new int[7, 7];
    private (int x, int y) player1Position = (0, 0);
    private (int x, int y) player2Position = (0, 0);
    private Vector2Int[] playerPositions = new Vector2Int[2];
    bool isPlayer1Turn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            MovePlayerUp();
            Debug.Log("W key was pressed");
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            MovePlayerLeft();
            Debug.Log("A key was pressed");
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            MovePlayerDown();
            Debug.Log("S key was pressed");
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            MovePlayerRight();
            Debug.Log("D key was pressed");
        }
    }

    void MovePlayer(int dx, int dy)
    {
        if(player1Position.x + dx < 0 || player1Position.x + dx >= boardSize)return;
        if(cellValues[player1Position.x + dx, player1Position.y] == 1)
        {
            cellValues[player1Position.x, player1Position.y] = 1;
            cellValues[player1Position.x - 1, player1Position.y] = 2;
            player1Position.x -= 1;
            Debug.Log("Player moved left");
            players[0].transform.position += new Vector3(-1, 0, 0);
        }
    }

    void MovePlayerRight()
    {
        if(playerPositions[0].x + 1 >= boardSize)return;
        if(cellValues[playerPositions[0].x + 1, playerPositions[0].y] == 1)
        {
            cellValues[playerPositions[0].x, playerPositions[0].y] = 1;
            cellValues[playerPositions[0].x + 1, playerPositions[0].y] = 2;
            playerPositions[0].x += 1;
            Debug.Log("Player moved right");
            players[0].transform.position += new Vector3(1, 0, 0);
        }
        else if(cellValues[playerPositions[0].x + 1, playerPositions[0].y] == 0)
        {
            // スライダーを動かす
            bool canMove = true;
            int dx = 1;
            // 今乗っているスライダーの番号を取得
            int currentSliderValue = sliderValues[playerPositions[0].x, playerPositions[0].y];
            // 今乗っているスライダーの値を-1にして、スライダーが動いた後に更新する
            sliderValues[playerPositions[0].x, playerPositions[0].y] = -1;
            while(canMove)
            {
                canMove = false;
                // スライダーが動けるか確認
                if(playerPositions[0].x + dx < 0 || playerPositions[0].x + dx >= boardSize)break;
                if(cellValues[playerPositions[0].x + dx, playerPositions[0].y] == 0)
                {
                    // セルの値を更新
                    cellValues[playerPositions[0].x, playerPositions[0].y] = 0;
                    cellValues[playerPositions[0].x + dx, playerPositions[0].y] = 2;
                    // プレイヤーの位置を更新
                    playerPositions[0].x += dx;
                    Debug.Log("Player moved right");
                    // プレイヤーとスライダーの表示位置を更新
                    sliders[currentSliderValue].transform.position += new Vector3(dx, 0, 0);
                    players[0].transform.position += new Vector3(dx, 0, 0);
                    canMove = true;
                }
            }
            // スライダーの値を更新
            sliderValues[playerPositions[0].x, playerPositions[0].y] = currentSliderValue;
        }
    }
    void MovePlayerLeft()
    {
        if(playerPositions[0].x - 1 < 0)return;
        if(cellValues[playerPositions[0].x - 1, playerPositions[0].y] == 1)
        {
            cellValues[playerPositions[0].x, playerPositions[0].y] = 1;
            cellValues[playerPositions[0].x - 1, playerPositions[0].y] = 2;
            playerPositions[0].x -= 1;
            Debug.Log("Player moved left");
            players[0].transform.position += new Vector3(-1, 0, 0);
        }
        else if(cellValues[playerPositions[0].x - 1, playerPositions[0].y] == 0)
        {
            // スライダーを動かす
            bool canMove = true;
            int dx = -1;
            // 今乗っているスライダーの番号を取得
            int currentSliderValue = sliderValues[playerPositions[0].x, playerPositions[0].y];
            // 今乗っているスライダーの値を-1にして、スライダーが動いた後に更新する
            sliderValues[playerPositions[0].x, playerPositions[0].y] = -1;
            while(canMove)
            {
                canMove = false;
                // スライダーが動けるか確認
                if(playerPositions[0].x + dx < 0 || playerPositions[0].x + dx >= boardSize)break;
                if(cellValues[playerPositions[0].x + dx, playerPositions[0].y] == 0)
                {
                    // セルの値を更新
                    cellValues[playerPositions[0].x, playerPositions[0].y] = 0;
                    cellValues[playerPositions[0].x + dx, playerPositions[0].y] = 2;
                    // プレイヤーの位置を更新
                    playerPositions[0].x += dx;
                    Debug.Log("Player moved left");
                    // プレイヤーとスライダーの表示位置を更新
                    sliders[currentSliderValue].transform.position += new Vector3(dx, 0, 0);
                    players[0].transform.position += new Vector3(dx, 0, 0);
                    canMove = true;
                }
            }
            // スライダーの値を更新
            sliderValues[playerPositions[0].x, playerPositions[0].y] = currentSliderValue;
        }
    }
    void MovePlayerUp()
    {
        if(playerPositions[0].y - 1 < 0)return;
        if(cellValues[playerPositions[0].x, playerPositions[0].y - 1] == 1)
        {
            cellValues[playerPositions[0].x, playerPositions[0].y] = 1;
            cellValues[playerPositions[0].x, playerPositions[0].y - 1] = 2;
            playerPositions[0].y -= 1;
            Debug.Log("Player moved up");
            players[0].transform.position += new Vector3(0, 1, 0);
        }
        else if(cellValues[playerPositions[0].x, playerPositions[0].y - 1] == 0)
        {
            // スライダーを動かす
            bool canMove = true;
            int dy = -1;
            // 今乗っているスライダーの番号を取得
            int currentSliderValue = sliderValues[playerPositions[0].x, playerPositions[0].y];
            // 今乗っているスライダーの値を-1にして、スライダーが動いた後に更新する
            sliderValues[playerPositions[0].x, playerPositions[0].y] = -1;
            while(canMove)
            {
                canMove = false;
                // スライダーが動けるか確認
                if(playerPositions[0].y + dy < 0 || playerPositions[0].y + dy >= boardSize)break;
                if(cellValues[playerPositions[0].x, playerPositions[0].y + dy] == 0)
                {
                    // セルの値を更新
                    cellValues[playerPositions[0].x, playerPositions[0].y] = 0;
                    cellValues[playerPositions[0].x, playerPositions[0].y + dy] = 2;
                    // プレイヤーの位置を更新
                    playerPositions[0].y += dy;
                    Debug.Log("Player moved up");
                    // プレイヤーとスライダーの表示位置を更新
                    sliders[currentSliderValue].transform.position += new Vector3(0, -dy, 0);
                    players[0].transform.position += new Vector3(0, -dy, 0);
                    canMove = true;
                }
            }
            // スライダーの値を更新
            sliderValues[playerPositions[0].x, playerPositions[0].y] = currentSliderValue;
        }
    }
    void MovePlayerDown()
    {
        if(playerPositions[0].y + 1 >= boardSize)return;
        if(cellValues[playerPositions[0].x, playerPositions[0].y + 1] == 1)
        {
            cellValues[playerPositions[0].x, playerPositions[0].y] = 1;
            cellValues[playerPositions[0].x, playerPositions[0].y + 1] = 2;
            playerPositions[0].y += 1;
            Debug.Log("Player moved down");
            players[0].transform.position += new Vector3(0, -1, 0);
        }
        else if(cellValues[playerPositions[0].x, playerPositions[0].y + 1] == 0)
        {
            // スライダーを動かす
            bool canMove = true;
            int dy = 1;
            // 今乗っているスライダーの番号を取得
            int currentSliderValue = sliderValues[playerPositions[0].x, playerPositions[0].y];
            // 今乗っているスライダーの値を-1にして、スライダーが動いた後に更新する
            sliderValues[playerPositions[0].x, playerPositions[0].y] = -1;
            while(canMove)
            {
                canMove = false;
                // スライダーが動けるか確認
                if(playerPositions[0].y + dy < 0 || playerPositions[0].y + dy >= boardSize)break;
                if(cellValues[playerPositions[0].x, playerPositions[0].y + dy] == 0)
                {
                    // セルの値を更新
                    cellValues[playerPositions[0].x, playerPositions[0].y] = 0;
                    cellValues[playerPositions[0].x, playerPositions[0].y + dy] = 2;
                    // プレイヤーの位置を更新
                    playerPositions[0].y += dy;
                    Debug.Log("Player moved down");
                    // プレイヤーとスライダーの表示位置を更新
                    sliders[currentSliderValue].transform.position += new Vector3(0, -dy, 0);
                    players[0].transform.position += new Vector3(0, -dy, 0);
                    canMove = true;
                }
            }
            // スライダーの値を更新
            sliderValues[playerPositions[0].x, playerPositions[0].y] = currentSliderValue;
        }
    }

    void Initialize()
    {
        // ボードの初期化
        boardSize = 7;

        // セルの値を初期化
        cellValues = new int[,]
        {
            {2, 1, 0, 0, 0, 1, 1},
            {1, 0, 0, 0, 0, 0, 1},
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0},
            {1, 0, 0, 0, 0, 0, 1},
            {1, 1, 0, 0, 0, 1, 3}
        };

        sliderValues = new int[,]
        {
            {0, 4, -1, -1, -1, 6, 8},
            {1, -1, -1, -1, -1, -1, 9},
            {-1, -1, -1, -1, -1, -1, -1},
            {-1, -1, -1, -1, -1, -1, -1},
            {-1, -1, -1, -1, -1, -1, -1},
            {2, -1, -1, -1, -1, -1, 10},
            {3, 5, -1, -1, -1, 7, 11}
        };

        sliders[0].transform.position = cells[0].transform.position + new Vector3(0, 0, -0.1f);
        sliders[1].transform.position = cells[1].transform.position + new Vector3(0, 0, -0.1f);
        sliders[2].transform.position = cells[5].transform.position + new Vector3(0, 0, -0.1f);
        sliders[3].transform.position = cells[6].transform.position + new Vector3(0, 0, -0.1f);
        sliders[4].transform.position = cells[7].transform.position + new Vector3(0, 0, -0.1f);
        sliders[5].transform.position = cells[13].transform.position + new Vector3(0, 0, -0.1f);
        sliders[6].transform.position = cells[35].transform.position + new Vector3(0, 0, -0.1f);
        sliders[7].transform.position = cells[41].transform.position + new Vector3(0, 0, -0.1f);
        sliders[8].transform.position = cells[42].transform.position + new Vector3( 0, 0, -0.1f);
        sliders[9].transform.position = cells[43].transform.position + new Vector3(0, 0, -0.1f);
        sliders[10].transform.position = cells[47].transform.position + new Vector3(0, 0, -0.1f);
        sliders[11].transform.position = cells[48].transform.position + new Vector3(0, 0, -0.1f);

        
        // プレイヤーの初期位置を設定
        playerPositions[0] = new Vector2Int(0, 0);
        playerPositions[1] = new Vector2Int(6, 6);

        players[0].transform.position = cells[0].transform.position + new Vector3(0, 0, -0.2f);
        players[1].transform.position = cells[48].transform.position + new Vector3(0, 0, -0.2f);
        
        // プレイヤー1のターンから開始
        isPlayer1Turn = true;
    }
}
