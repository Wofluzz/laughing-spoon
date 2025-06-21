using System;
using System.Collections;
using UnityEngine;

public class CircleFadeEffectOnPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;
    Transform circleFade;
    Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponentInChildren<CutoffMaskUI>().enabled = true;
        circleFade = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        
        PlayerMovements.OnPlayerDied += OnPlayerDied;
        StartCoroutine( OnGameStart() );
        
    }

    private void OnPlayerDied()
    {
        GetComponent<Animator>().enabled = true;
        gameObject.GetComponentInChildren<CutoffMaskUI>().enabled = true;
        anim.Play("CircFadeIn");
    }

    private IEnumerator OnGameStart()
    {
        GetComponent<Animator>().enabled = true; 
        gameObject.GetComponentInChildren<CutoffMaskUI>().enabled = true;
        anim.Play("CircFadeOut");
        yield return new WaitForSeconds(5f);
        gameObject.GetComponentInChildren<CutoffMaskUI>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(player.position); 
        circleFade.position = new Vector3(Math.Clamp(screenPos.x, 0, Screen.width), Math.Clamp(screenPos.y, 0, Screen.height), circleFade.position.z);
    }
}
