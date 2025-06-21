using Inventory.Model;
using Inventory2D.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateOpen : MonoBehaviour
{
    [SerializeField]
    private ItemSO_2D[] itemData;
    [SerializeField]
    private GameObject itemObject;
    [SerializeField]
    private int OpenningScore = 100;
    [SerializeField]
    private ParticleSystem ParticleSystem;

    [SerializeField]
    private bool Opened = false;

    public void OpenCrate()
    {
        if (Opened) return;
        Opened = true;
        StartCoroutine(SpawnLootAndDestroy(itemData));
    }

    private void Explode()
    {
        GameObject crate_ps = Instantiate(ParticleSystem, transform.position, Quaternion.identity).gameObject;
        var exp = crate_ps.GetComponent<ParticleSystem>();
        exp.Play();
        Destroy(gameObject);
    }

    IEnumerator SpawnLoot(ItemSO_2D[] itemData)
    {
        // M�langer la liste des items  
        for (int i = itemData.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = itemData[i];
            itemData[i] = itemData[randomIndex];
            itemData[randomIndex] = temp;
        }

        foreach (var item in itemData)
        {
            GameObject n_item = Instantiate(itemObject, new Vector3(transform.position.x, transform.position.y + 1, 0), Quaternion.identity);
            float rm_posX = Random.Range(0, 2);

            PowerUpSpawner pSpawn = n_item.GetComponent<PowerUpSpawner>();

            pSpawn.powerUpSO = item;
            pSpawn.SetupPowerUp(item);
            StartCoroutine(EnableItemSO(pSpawn));
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator SpawnLootAndDestroy(ItemSO_2D[] itemData)
    {
        yield return StartCoroutine(SpawnLoot(itemData));
        Explode(); // maintenant seulement !
    }

    IEnumerator EnableItemSO(PowerUpSpawner pSpawn)
    {
        pSpawn.enabled = false; // D�sactiver le power-up pour emp�cher l'interaction imm�diate  
        Collider2D collider = pSpawn.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false; // D�sactiver le collider pour emp�cher la r�cup�ration imm�diate  
        }
        yield return new WaitForSeconds(1); // Attendre la fin de l'animation de sortie (ajuster la dur�e si n�cessaire)  
        if (collider != null)
        {
            collider.enabled = true; // R�activer le collider pour permettre la r�cup�ration  
        }
        pSpawn.enabled = true; // R�activer le power-up pour permettre l'interaction  
    }

    private void OnDestroy()
    {
        GameManager.instance.AddScore(OpenningScore);
        GameManager.instance.ShowScore(OpenningScore.ToString(), gameObject);
    }



}
