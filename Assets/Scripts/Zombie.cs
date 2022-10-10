using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private StateMachine brain;
    private Animator animator;
    [SerializeField]
    private Text stateNote;
    private NavMeshAgent agent;
    private Player player;

    private bool playerIsNear;
    private bool withinAttackRange;

    private float changeMind;
    private float attackTimer;

    void Start(){
        player = FindObjectOfType<Player>();
        agent = GetComponent<NavMeshAgent>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        brain = GetComponent<StateMachine>();
        playerIsNear = false;
        withinAttackRange = false;

        brain.PushState(Idle, OnIdleEnter, OnIdleExit);
    }

    void Update(){
        playerIsNear = Vector3.Distance(transform.position, player.transform.position) < 5;
        withinAttackRange = Vector3.Distance(transform.position, player.transform.position) < 1.4f;
    }

    void OnIdleEnter(){
        agent.ResetPath();
        stateNote.text = "Idle";
    }

    void Idle(){
        changeMind -= Time.deltaTime;
        
        if (playerIsNear){
            brain.PushState(Chase, OnChaseEnter, OnChaseExit);
        } else if (changeMind <= 0){
            brain.PushState(Wander, OnWanderEnter, OnWanderExit);
            changeMind = Random.Range(4, 10);
        }
    }

    void OnIdleExit(){

    }

    void OnWanderEnter(){
        stateNote.text = "Wander";
        animator.SetBool("Chase", true);
        Vector3 wanderDirection = (Random.insideUnitSphere * 4f +
        transform.position);
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(wanderDirection, out navMeshHit, 3f, NavMesh.AllAreas);
        Vector3 destination = navMeshHit.position;
        agent.SetDestination(destination);
    }

    void Wander(){
        if (agent.remainingDistance <= .25f){
            agent.ResetPath();
            brain.PushState(Idle, OnIdleEnter, OnIdleExit);
        }
        if (playerIsNear){
            brain.PushState(Chase, OnChaseEnter, OnChaseExit);
        }
    }

    void OnWanderExit(){
        animator.SetBool("Chase", false);
    }

    void OnChaseEnter(){
        stateNote.text = "Chase";
        animator.SetBool("Chase", true);
    }

    void Chase(){
        agent.SetDestination(player.transform.position);
        if (Vector3.Distance(transform.position, player.transform.position) > 5.5f){
            brain.PopState();
            brain.PushState(Idle, OnIdleEnter, OnIdleExit);
        }
        if (withinAttackRange){
            brain.PushState(Attack, OnAttackEnter, null);
        }
    }

    void OnChaseExit(){
        animator.SetBool("Chase", false);
    }

    void OnAttackEnter(){
        agent.ResetPath();
        stateNote.text  = "Attack";
    }

    void Attack(){
        attackTimer -= Time.deltaTime;
        if (!withinAttackRange){
            brain.PopState();
        } else if (attackTimer <= 0){
            animator.SetTrigger("Attack");
            player.Hurt(2, 1);
            attackTimer = 2f;
        }
    }

    void OnAttackExit(){
        if(!withinAttackRange){
            brain.PopState();
        }
    }

}
