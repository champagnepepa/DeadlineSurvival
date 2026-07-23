using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : StateMachineBehaviour
{
    float timer;
    List<Transform> waypoints = new List<Transform>();
    NavMeshAgent agent;
    Transform player;

    float chaseRange = 3f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        player = PlayerMovement.PlayerTransform;

        if (player == null)
        {
            Debug.LogWarning("PlayerTransform not found in PatrolState!");
        }

        var audio = animator.GetComponent<EnemyAudioController>();
        if (audio != null)
            audio.PlaySound(audio.chaseClip, 0.1f, true);

        agent.speed = 0.25f;
        timer = 0f;

        // Cari objek waypoint
        GameObject go = GameObject.FindGameObjectWithTag("Waypoints");
        if (go != null)
        {
            waypoints.Clear();
            foreach (Transform t in go.transform)
                waypoints.Add(t);

            if (waypoints.Count > 0)
                agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
        }
        else
        {
            Debug.LogWarning("GameObject with tag 'Waypoints' not found!");
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
            return;
        var audio = animator.GetComponent<EnemyAudioController>();

        if (agent.remainingDistance <= agent.stoppingDistance && waypoints.Count > 0)
        {
            agent.SetDestination(waypoints[Random.Range(0, waypoints.Count)].position);
            if (audio != null)
                audio.PlaySound(audio.patrolClip, 0.1f, true);
        }

        timer += Time.deltaTime;
        if (timer > 60f)
        {
            var fsmTimer = animator.GetComponent<FSMTimerManager>();
            if (fsmTimer != null)
                fsmTimer.EndTimer("Patrol");

            animator.SetBool("IsPatrolling", false);
        }

        
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance < chaseRange)
        {
            var fsmTimer = animator.GetComponent<FSMTimerManager>();
            if (fsmTimer != null)
                fsmTimer.StartTimer("Chase");

            if (!animator.GetBool("IsChasing"))
            {
                Debug.Log("AI Log: Player terdeteksi dalam jarak " + distance + ". Berpindah ke Chase State");
            }

            animator.SetBool("IsChasing", true);
            if (audio != null)
                audio.PlaySound(audio.chaseClip, 0.1f, true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
        EnemyAudioController audio = animator.GetComponent<EnemyAudioController>();
        if (audio != null)
        {
            audio.StopSound(); // Pastikan method ini tersedia di EnemyAudioController
        }
    }
}