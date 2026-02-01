using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class FootstepController : MonoBehaviour
{
    public AudioSource footstepAudio; // Inspector'dan baðlayacaðýz
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Cycle üzerindeki agent
    }

    void Update()
    {
        // Agent hareket ediyor mu kontrol
        if (agent.velocity.magnitude > 0.1f)
        {
            if (!footstepAudio.isPlaying)
            {
                footstepAudio.pitch = Random.Range(0.95f, 1.05f); // doðal tekrar
                footstepAudio.Play();
            }
        }
        else
        {
            footstepAudio.Stop();
        }
    }
}
