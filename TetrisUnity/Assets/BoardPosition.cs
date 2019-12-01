using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPosition : MonoBehaviour
{
    public GameObject winText;
    public GameObject loseText;
    public void lose()
    {
        loseText.SetActive(true);
    }

    public void win()
    {
        winText.SetActive(true);
    }
}
