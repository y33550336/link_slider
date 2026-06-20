using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChanger : MonoBehaviour
{
    public Image button2;
    public Image button4;

    public void OnPlayerNumberClicked(int number)
    {
        SceneTraveler.SetPlayerNumber(number);

        if (number == 2)
        {
            button2.color = Color.red;
            button4.color = new Color(255f / 255f, 172f / 255f, 172f / 255f);
        }
        else if (number == 4)
        {
            button2.color = new Color(255f / 255f, 172f / 255f, 172f / 255f);
            button4.color = Color.red;
        }
    }
}
