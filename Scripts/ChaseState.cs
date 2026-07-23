using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateMachineBehaviour
{
    NavMeshAgent agent;
    Transform player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        player = PlayerMovement.PlayerTransform;

        var audio = animator.GetComponent<EnemyAudioController>();
        if (audio != null)
            audio.PlaySound(audio.chaseClip, 0.1f, true);

        if (player == null)
        {
            Debug.LogWarning("PlayerTransform not found in ChaseState!");
            return;
        }

        agent.speed = 1.5f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
            return;

        agent.SetDestination(player.position);

        var audio = animator.GetComponent<EnemyAudioController>();
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance > 6f)
        {
            var fsmTimer = animator.GetComponent<FSMTimerManager>();
            if (fsmTimer != null)
                fsmTimer.EndTimer("Chase");

            animator.SetBool("IsChasing", false);
        }
        if (distance < 1.5f)
        {
            var fsmTimer = animator.GetComponent<FSMTimerManager>();
            if (fsmTimer != null)
                fsmTimer.StartTimer("Attack");

            if (!animator.GetBool("Attack"))
            {
                Debug.Log("AI Log: Player terdeteksi dalam jarak " + distance + ". Berpindah ke Attack State");
            }

            animator.SetBool("Attack", true);
            if (audio != null)
                audio.PlaySound(audio.attackClip, 0.1f, true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(animator.transform.position);
        EnemyAudioController audio = animator.GetComponent<EnemyAudioController>();
        if (audio != null)
        {
            audio.StopSound(); // Pastikan method ini tersedia di EnemyAudioController
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
