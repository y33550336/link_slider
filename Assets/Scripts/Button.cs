using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    public Image button1, button2, button3, button4;

    public void OnPlayerNumberClicked(int number)
    {
        SceneTraveler.SetPlayerNumber(number);

        if (number == 2)
        {
            button1.color = Color.red;
            button2.color = new Color(255f / 255f, 172f / 255f, 172f / 255f);
        }
        else if (number == 4)
        {
            button2.color = Color.red;
            button1.color = new Color(255f / 255f, 172f / 255f, 172f / 255f);
        }
    }

    public void OnBackableClicked(bool backable)
    {
        SceneTraveler.SetBackable(backable);

        if (backable)
        {
            button3.color = Color.red;
            button4.color = new Color(255f / 255f, 172f / 255f, 172f / 255f);
        }
        else
        {
            button3.color = new Color(255f / 255f, 172f / 255f, 172f / 255f);
            button4.color = Color.red;
        }
    }
}
