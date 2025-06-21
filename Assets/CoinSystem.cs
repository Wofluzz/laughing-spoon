using System;
using System.Collections;
using UnityEngine;

public class CoinSystem : MonoBehaviour
{
    public static int coinCount = 0;

    public static event Action OnCoinCollected;

    [SerializeField]
    private TMPro.TextMeshProUGUI coinText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coinCount = PlayerPrefs.GetInt("Coins", 0);
        OnCoinCollected += UpdateCoinText;

    }

    private void UpdateCoinText()
    {
        coinText.text = coinCount.ToString();
        StartCoroutine(HideCoinCounter());
    }

    private IEnumerator HideCoinCounter()
    { 
        yield return new WaitForSeconds(2f);
        coinText.gameObject.SetActive(false);
    }


}
