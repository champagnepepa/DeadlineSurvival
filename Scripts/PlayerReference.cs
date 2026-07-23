using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    public static GameObject player;

    void Awake()
    {
        // inisialisasi player static
        player = this.gameObject;

        // agar tidak hilang saat pindah scene
        DontDestroyOnLoad(this.gameObject);
    }
}
