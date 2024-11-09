using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainController : MonoBehaviour
{
    public Camera camera;
    private RaycastHit hit;
    private NavMeshAgent agent;
    private string groundTag = "Ground";
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if(hit.collider.CompareTag(groundTag))
                {
                    agent.SetDestination(hit.point);
                }
            }
        }
    }
}
