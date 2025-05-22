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
    private int quantity = 1;
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
        foreach (var item in itemData)
        {
            GameObject n_item = Instantiate(itemObject, new Vector3(transform.position.x, transform.position.y + 1, 0), Quaternion.identity);
            float rm_posX = Random.Range(0, 2);

            PowerUpSpawner pSpawn = n_item.GetComponent<PowerUpSpawner>();
            pSpawn.powerUpSO = item;
            pSpawn.SetupPowerUp(item);
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator SpawnLootAndDestroy(ItemSO_2D[] itemData)
    {
        yield return StartCoroutine(SpawnLoot(itemData));
        Explode(); // maintenant seulement !
    }



}
