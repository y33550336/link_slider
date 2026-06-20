using UnityEngine;
using TMPro;

public class Slider : MonoBehaviour
{
    [SerializeField]
    private GrpcClient client;

    public GameObject[] cells = new GameObject[49];
    // 7*7のセルでいくつかのスライダーがある
    public GameObject[] sliders = new GameObject[12];
    public GameObject[] players = new GameObject[2];
    private int boardSize = 7;
    private int[,] cellValues;
    // 0は空のセル、1はプレイヤー1の位置、2はプレイヤー2の位置、-1はスライダーの位置
    private int[,] sliderValues;
    private Vector2Int[] playerPositions = new Vector2Int[2];
    int PlayerTurn = 0;
    int PlayerNumber = 2; //プレイヤーの数
    bool locked = false; //操作のロック（盤面を操作できないかどうか）
    public TextMeshPro Text; //ターン表示用のテキスト
    public GameObject SquareObject;

    private int[,,] GameLog; //ゲームの状態を保存する配列、CellValuesの記録
    private int[,,] SliderGameLog; //スライダーの状態を保存する配列、SliderValuesの記録
    private int TurnIndex = 0; //GameLogのインデックス
    bool isRewinding = false; //巻き戻し中かどうか

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (locked && Input.GetKeyDown(KeyCode.R) && !isRewinding)
        {
            Initialize();
        }
        if (Input.GetKeyDown(KeyCode.Z) && locked)
        {
            isRewinding = true;
            SquareObject.SetActive(false);
            Text.text = "Rewinding... Use Left/Right Arrow to navigate turns, X to stop rewinding";
        }
        if (Input.GetKeyDown(KeyCode.X) && isRewinding)
        {
            isRewinding = false;
            Text.text = "Pless Z to Rewind, R to Restart";
        }
        if (isRewinding && Input.GetKeyDown(KeyCode.LeftArrow) && TurnIndex > 0)
        {
            Reflection(TurnIndex - 1); //1ターン前の状態に戻る
            TurnIndex = Mathf.Max(TurnIndex - 1, 0); //TurnIndexを1減らす
        }
        if (isRewinding && Input.GetKeyDown(KeyCode.RightArrow) && TurnIndex < GameLog.GetLength(0) - 1)
        {
            Reflection(TurnIndex + 1); //1ターン後の状態に進む
            TurnIndex = Mathf.Min(TurnIndex + 1, GameLog.GetLength(0) - 1); //TurnIndexを1増やす
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
        Text.text = "";
        if (playerPositions[playerIndex].x + dx < 0 || playerPositions[playerIndex].x + dx >= boardSize) return;
        if (playerPositions[playerIndex].y + dy < 0 || playerPositions[playerIndex].y + dy >= boardSize) return;
        if (cellValues[playerPositions[playerIndex].x + dx, playerPositions[playerIndex].y + dy] == -1)
        {
            cellValues[playerPositions[playerIndex].x, playerPositions[playerIndex].y] = -1;
            cellValues[playerPositions[playerIndex].x + dx, playerPositions[playerIndex].y + dy] = playerIndex + 1;
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
                    cellValues[playerPositions[playerIndex].x + dx, playerPositions[playerIndex].y + dy] = playerIndex + 1;
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
        Record(); //ターン終了時の状態を記録
        if (playerPositions[PlayerTurn].x == boardSize / 2 && playerPositions[PlayerTurn].y == boardSize / 2)
        {
            SquareObject.SetActive(true);
            if (PlayerTurn == 0) Text.color = Color.red;
            else Text.color = Color.blue;
            Text.text = "Player " + (PlayerTurn + 1) + " wins! Press Z to Rewind, R to Restart";
            locked = true;
        }
        PlayerTurn = (PlayerTurn + 1) % PlayerNumber;
        if (!locked) Text.text = "               Player " + (PlayerTurn + 1) + "'s turn";
    }

    void Record()
    {
        // GameLogのサイズを1増やす
        int[,,] newGameLog = new int[TurnIndex + 1, boardSize, boardSize];
        for (int i = 0; i < TurnIndex; i++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    newGameLog[i, x, y] = GameLog[i, x, y];
                }
            }
        }
        // 現在のCellValuesをGameLogに保存
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                newGameLog[TurnIndex, x, y] = cellValues[x, y];
            }
        }
        GameLog = newGameLog;

        int[,,] newSliderGameLog = new int[TurnIndex + 1, boardSize, boardSize];
        for (int i = 0; i < TurnIndex; i++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    newSliderGameLog[i, x, y] = SliderGameLog[i, x, y];
                }
            }
        }
        // 現在のSliderValuesをSliderGameLogに保存
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                newSliderGameLog[TurnIndex, x, y] = sliderValues[x, y];
            }
        }
        SliderGameLog = newSliderGameLog;
        TurnIndex++;
    }

    void Reflection(int index)
    {
        if (index < 0 || index >= GameLog.GetLength(0)) return;
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                cellValues[x, y] = GameLog[index, x, y];
                sliderValues[x, y] = SliderGameLog[index, x, y];
            }
        }
        // プレイヤーの位置を更新
        // スライダーの位置を更新
        for (int i = 0; i < PlayerNumber; i++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (cellValues[x, y] == i + 1)
                    {
                        playerPositions[i] = new Vector2Int(x, y);
                        players[i].transform.position = cells[y * boardSize + x].transform.position + new Vector3(0, 0, -0.2f);
                    }
                    // SliderGameLogを使用してスライダーの位置を復元
                    int sliderIndex = SliderGameLog[index, x, y];
                    if (sliderIndex != -1)
                    {
                        sliders[sliderIndex].transform.position = cells[y * boardSize + x].transform.position + new Vector3(0, 0, -0.1f);
                    }
                }
            }
        }
    }

    void Initialize()
    {
        // ボードの初期化
        boardSize = 7;

        // セルの値を初期化
        cellValues = new int[,]
        {
            {1, -1, 0, 0, 0, -1, -1},
            {-1, 0, 0, 0, 0, 0, -1},
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0},
            {-1, 0, 0, 0, 0, 0, -1},
            {-1, -1, 0, 0, 0, -1, 2}//転置していることに注意
        };

        sliderValues = new int[,]
        {
            {0, 4, -1, -1, -1, 6, 8},
            {1, -1, -1, -1, -1, -1, 9},
            {-1, -1, -1, -1, -1, -1, -1},
            {-1, -1, -1, -1, -1, -1, -1},
            {-1, -1, -1, -1, -1, -1, -1},
            {2, -1, -1, -1, -1, -1, 10},
            {3, 5, -1, -1, -1, 7, 11}//転置していることに注意
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

        Text.text = "               Player 1's turn"; Text.color = Color.black;
        SquareObject.SetActive(false);

        TurnIndex = 0;
        GameLog = new int[0, boardSize, boardSize];
        SliderGameLog = new int[0, boardSize, boardSize];
    }
}
