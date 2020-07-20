/*
In this script:
- Publicly available simple message system
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenMessager : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    public static ScreenMessager smInstance;

    public void Awake()
    {
        smInstance = this;
    }

    public void ShowMessage(string messageContent)
    {
        messageText.text = messageContent;
        StartCoroutine(showMessageBriefly());
    }

    private IEnumerator showMessageBriefly()
    {
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.75f);
        messageText.gameObject.SetActive(false);
    }
}
