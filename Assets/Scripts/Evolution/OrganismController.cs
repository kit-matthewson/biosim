using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class OrganismController : MonoBehaviour {
    public NavMeshAgent Agent;
    public MeshRenderer Mesh;
    public Material BaseMaterial;
    public Gradient Gradient;

    /// <summary>
    /// Initialise this <c>OrganismController</c> based on some <c>Organism</c>.
    /// </summary>
    /// <param name="organism">The <c>Organism</c> to extract values from.</param>
    public void Initialise(Organism organism) {
        SetStats(organism);

        gameObject.transform.SetPositionAndRotation(RandPos(), Quaternion.identity);
        Agent.SetDestination(RandPos());
    }

    /// <summary>
    /// Update the attributes of the <c>Organism</c> this controller is based off.
    /// </summary>
    /// <param name="organism">The <c>Organism</c> to extract values from.</param>
    public void SetStats(Organism organism) {
        float v = (float)((organism.Fitness + 1) / 2);

        Mesh.material = new Material(BaseMaterial) {
            color = Gradient.Evaluate(v)
        };

        Agent.speed = (float)(organism.AttributeValue("Agility") * organism.AttributeValue("Endurance") * 20 + 1);
    }

    [PublicAPI]
    private void Update() {
        if (Random.value < 0.05) {
            Agent.SetDestination(RandPos());
        }
    }

    public Vector3 RandPos() {
        const int width = 100;

        return new Vector3(Random.Range(-width, width), 0, Random.Range(-width, width));
    }
}