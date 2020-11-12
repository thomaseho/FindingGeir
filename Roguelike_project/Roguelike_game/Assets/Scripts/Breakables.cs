using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Breakables : MonoBehaviour
{
    public GameObject[] brokenPieces;

    public bool shouldDropItem;
    public GameObject[] itemsToDrop;
    public float itemDropPercent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Smash()
    {
        Destroy(gameObject);

        AudioManager.instance.PlaySFX(0);

        int piecesToDrop = Random.Range(1, 5);

        for (int i = 0; i < piecesToDrop; i++)
        {
            Instantiate(brokenPieces[Random.Range(0, brokenPieces.Length)], transform.position, transform.rotation);
        }

        if (shouldDropItem)
        {
            float dropChance = Random.Range(0f, 100f);

            if (dropChance < itemDropPercent)
            {
                int randomItem = Random.Range(0, itemsToDrop.Length);

                Instantiate(itemsToDrop[randomItem], transform.position, transform.rotation);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (PlayerController.instance.dashCounter > 0)
            {
                Smash();
            }
        }

        if (other.tag == "PlayerBullet")
        {
            Smash();
        }
    }
}