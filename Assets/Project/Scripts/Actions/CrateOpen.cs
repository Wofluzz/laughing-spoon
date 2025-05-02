using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateOpen : MonoBehaviour
{
    [SerializeField]
    private GameObject[] items;
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
        StartCoroutine(SpawnLootAndDestroy(items));
    }

    private void Explode()
    {
        GameObject crate_ps = Instantiate(ParticleSystem, transform.position, Quaternion.identity).gameObject;
        var exp = crate_ps.GetComponent<ParticleSystem>();
        exp.Play();
        Destroy(gameObject);
    }

    IEnumerator SpawnLoot(GameObject[] items)
    {
        foreach (var item in items)
        {
            GameObject n_item = Instantiate(item, new Vector3(transform.position.x, transform.position.y + 1, 0), Quaternion.identity);
            float rm_posX = Random.Range(0, 2);

            n_item.GetComponent<Rigidbody2D>().AddForce(new Vector2((Random.Range(0, 2) * 2 - 1) * 3, 5), ForceMode2D.Impulse);
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator SpawnLootAndDestroy(GameObject[] items)
    {
        yield return StartCoroutine(SpawnLoot(items));
        Explode(); // maintenant seulement !
    }



}
