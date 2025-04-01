using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRigidbody : MonoBehaviour
{
    [HideInInspector]
    public bool simPaused;

    public Vector3 initialPosition;
    public Vector3 initialRotation;
    public Vector3 initialVelocity;

    public float dt;
    public float mass;
    public float ks;
    public float kd;
    public float mu;
    public float g = 9.81f;

    protected Vector3 position;
    protected Quaternion rotation;
    protected Vector3 linear_velocity;
    protected Vector3 angular_velocity;

    protected abstract Vector3[] LocalVertices { get; }
    protected abstract Matrix3x3 InertiaRefMatrix { get; }

    private List<Vector3> GetCollidedVertices()
    {
        List<Vector3> CollidedVertices = new List<Vector3>();

        /*** Part 2: Collision Detection ***/
        foreach (Vector3 localVertex in LocalVertices)
        {
            Vector3 globalVertex = position + rotation * localVertex;
            if (globalVertex.y < 0)
            {
                CollidedVertices.Add(globalVertex);
            }
        }
        /*** part 2 coding ends ***/

        return CollidedVertices;
    }

    private (Vector3, Vector3) ComputeForceAndTorque()
    {
        Vector3 netForce = Vector3.zero;
        Vector3 netTorque = Vector3.zero;

        /*** Part 3: Calculate Forces and Torques ***/
   
   

        netForce += new Vector3(0, -mass * g, 0);



        List<Vector3> collidedVertices = GetCollidedVertices();
        foreach (Vector3 globalVertex in collidedVertices)
        {
          
          
            float penetration = -globalVertex.y;
            
          
          
            Vector3 r = globalVertex - position;
            Vector3 relativeVelocity = linear_velocity + Vector3.Cross(angular_velocity, r);
            
          
          
            Vector3 normalForce = new Vector3(0, ks * penetration, 0);
            
           
           
            Vector3 dampingForce = -kd * relativeVelocity.y * Vector3.up;
            
           
           
            Vector3 horizontalVelocity = new Vector3(relativeVelocity.x, 0, relativeVelocity.z);
            Vector3 frictionForce = -mu * normalForce.magnitude * horizontalVelocity.normalized;
            
       
       
            Vector3 totalForce = normalForce + dampingForce + frictionForce;
            
         
         
            netForce += totalForce;
            
          
          
            netTorque += Vector3.Cross(r, totalForce);
        }
        /*** part 3 coding ends ***/

        return (netForce, netTorque);
    }

    private void Integrate(float deltaTime)
    {
        var (force, torque) = ComputeForceAndTorque();

        Matrix3x3 R = new Matrix3x3(rotation);
        Matrix3x3 I = InertiaRefMatrix;
        Matrix3x3 I_global = R * I * R.transpose;

        linear_velocity += (force / mass) * deltaTime;
        angular_velocity += I_global.inverse * torque * deltaTime;

        position += linear_velocity * deltaTime;
        rotation = Quaternion.Euler(angular_velocity * deltaTime * Mathf.Rad2Deg) * rotation;
    }

    public void AdvanceTimeStep()
    {
        Integrate(dt);
    }

    public void ResetState()
    {
        position = initialPosition;
        linear_velocity = initialVelocity;
        rotation = Quaternion.Euler(initialRotation);
        angular_velocity = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetState();
    }

    // FixedUpdate is called every fixed frame-rate frame
    // Read more here: https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html
    void FixedUpdate()
    {
        if (!simPaused)
        {
            AdvanceTimeStep();
        }
        transform.position = position;
        transform.rotation = rotation;
        Time.fixedDeltaTime = dt;
    }
}
