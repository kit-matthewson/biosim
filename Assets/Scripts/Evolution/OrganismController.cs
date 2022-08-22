using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class OrganismController : MonoBehaviour {

    public NavMeshAgent agent;
    public MeshRenderer mesh;
    public Material baseMaterial;
    public Gradient gradient;

    public void Initialise(Organism organism) {
        SetStats(organism);

        gameObject.transform.SetPositionAndRotation(RandPos(), Quaternion.identity);
        agent.SetDestination(RandPos());
    }

    public void SetStats(Organism organism) {
        float v = (float)((organism.Fitness + 1) / 2);

        mesh.material = new(baseMaterial) {
            color = gradient.Evaluate(v)
        };

        agent.speed = (float)((organism.AttributeValue("Agility") * organism.AttributeValue("Endurance") * 20) + 1);
    }

    public Vector3 RandPos() {
        int width = 100;
        return new Vector3(Random.Range(-width, width), 0, Random.Range(-width, width));
    }
}
