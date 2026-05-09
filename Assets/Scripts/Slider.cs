using UnityEngine;

public class Slider : MonoBehaviour
{
    public GameObject[] cells = new GameObject[49];
    // 7*7のセルでいくつかのスライダーがある
    public GameObject[] sliders = new GameObject[12];
    public GameObject[] players = new GameObject[2];
    private int boardSize = 7;
    private int[,] cellValues = new int[7, 7];
    // 0は空のセル、1はプレイヤー1の位置、2はプレイヤー2の位置、3はスライダーの位置
    private int[,] sliderValues = new int[7, 7];
    private Vector2Int[] playerPositions = new Vector2Int[2];
    int PlayerTurn = 0;
    int PlayerNumber = 2; //プレイヤーの数
    bool locked = false; //操作のロック（盤面を操作できないかどうか）

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (locked && Input.GetKeyDown(KeyCode.R))
        {
            Initialize();
        }

        if (locked) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            MovePlayer(0, -1, PlayerTurn);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            MovePlayer(-1, 0, PlayerTurn);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            MovePlayer(0, 1, PlayerTurn);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            MovePlayer(1, 0, PlayerTurn);
        }

    }

    void MovePlayer(int dx, int dy, int playerIndex)
    {
        if (playerPositions[playerIndex].x + dx < 0 || playerPositions[playerIndex].x + dx >= boardSize) return;
        if (playerPositions[playerIndex].y + dy < 0 || playerPositions[playerIndex].y + dy >= boardSize) return;
        if (cellValues[playerPositions[playerIndex].x + dx, playerPositions[playerIndex].y + dy] == 3)
        {
            cellValues[playerPositions[playerIndex].x, playerPositions[playerIndex].y] = 3;
            cellValues[playerPositions[playerIndex].x + dx, playerPositions[playerIndex].y + dy] = 1;
            playerPositions[playerIndex].x += dx;
            playerPositions[playerIndex].y += dy;

            players[playerIndex].transform.position += new Vector3(dx, -dy, 0);
        }

        else if (cellValues[playerPositions[playerIndex].x + dx, playerPositions[playerIndex].y + dy] == 0)
        {
            // スライダーを動かす
            bool canMove = true;
            // 今乗っているスライダーの番号を取得
            int currentSliderValue = sliderValues[playerPositions[playerIndex].x, playerPositions[playerIndex].y];
            // 今乗っているスライダーの値を-1にして、スライダーが動いた後に更新する
            sliderValues[playerPositions[playerIndex].x, playerPositions[playerIndex].y] = -1;
            while (canMove)
            {
                canMove = false;
                // スライダーが動けるか確認
                if (playerPositions[playerIndex].x + dx < 0 || playerPositions[playerIndex].x + dx >= boardSize) break;
                if (playerPositions[playerIndex].y + dy < 0 || playerPositions[playerIndex].y + dy >= boardSize) break;
                if (cellValues[playerPositions[playerIndex].x + dx, playerPositions[playerIndex].y + dy] == 0)
                {
                    // セルの値を更新
                    cellValues[playerPositions[playerIndex].x, playerPositions[playerIndex].y] = 0;
                    cellValues[playerPositions[playerIndex].x + dx, playerPositions[playerIndex].y + dy] = 1;
                    // プレイヤーの位置を更新
                    playerPositions[playerIndex].x += dx;
                    playerPositions[playerIndex].y += dy;

                    // プレイヤーとスライダーの表示位置を更新
                    sliders[currentSliderValue].transform.position += new Vector3(dx, -dy, 0);
                    players[playerIndex].transform.position += new Vector3(dx, -dy, 0);
                    canMove = true;
                }
            }
            // スライダーの値を更新
            sliderValues[playerPositions[playerIndex].x, playerPositions[playerIndex].y] = currentSliderValue;
            EndTurn();
        }
    }

    void EndTurn()
    {
        if (playerPositions[PlayerTurn].x == boardSize / 2 && playerPositions[PlayerTurn].y == boardSize / 2)
        {
            Debug.Log("Player " + (PlayerTurn + 1) + " wins!");
            locked = true;
        }
        PlayerTurn = (PlayerTurn + 1) % PlayerNumber;
    }

    void Initialize()
    {
        // ボードの初期化
        boardSize = 7;

        // セルの値を初期化
        cellValues = new int[,]
        {
            {1, 3, 0, 0, 0, 3, 3},
            {3, 0, 0, 0, 0, 0, 3},
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0},
            {3, 0, 0, 0, 0, 0, 3},
            {3, 3, 0, 0, 0, 3, 2}
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
        sliders[8].transform.position = cells[42].transform.position + new Vector3(0, 0, -0.1f);
        sliders[9].transform.position = cells[43].transform.position + new Vector3(0, 0, -0.1f);
        sliders[10].transform.position = cells[47].transform.position + new Vector3(0, 0, -0.1f);
        sliders[11].transform.position = cells[48].transform.position + new Vector3(0, 0, -0.1f);


        // プレイヤーの初期位置を設定
        playerPositions[0] = new Vector2Int(0, 0);
        playerPositions[1] = new Vector2Int(6, 6);

        players[0].transform.position = cells[0].transform.position + new Vector3(0, 0, -0.2f);
        players[1].transform.position = cells[48].transform.position + new Vector3(0, 0, -0.2f);

        // プレイヤー1のターンから開始
        PlayerTurn = 0;
        locked = false;
    }
}
