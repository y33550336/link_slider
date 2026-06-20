using UnityEngine;

public static class SceneTraveler
{
    //ゲーム起動中において、変数をシーン間で共有するためのスタティッククラス
    public static int playerNumber = 2; //プレイヤーの数

    public static void SetPlayerNumber(int number)
    {
        playerNumber = number;
    }

    public static int GetPlayerNumber()
    {
        return playerNumber;
    }
}
