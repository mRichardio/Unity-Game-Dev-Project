using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public float ShrinkSpeed = .3f;
    public List<AudioClip> Clips;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource sfx = GetComponent<AudioSource>();
        sfx.pitch = Random.Range(0.95f, 1.05f);
        sfx.clip = Clips[Random.Range(0, Clips.Count)];
        sfx.Play();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * Time.deltaTime * ShrinkSpeed;
        if (transform.localScale.x <= 0.00001f)
        {
            Destroy(gameObject);
        }
    }
}
