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

        foreach (Vector3 local in LocalVertices)
        {
            Vector3 global = position + rotation * local;
           
           
            if (global.y < 0){
                CollidedVertices.Add(global);
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
        foreach (Vector3 global in collidedVertices)
        {
          

          
            float penetration = -global.y;
            
          
          
            Vector3 r = global - position;
            Vector3 relative = linear_velocity + Vector3.Cross(angular_velocity, r);
            
          
          
            Vector3 normal = new Vector3(0, ks * penetration, 0);
            
           

           // Vector3 damp = -kd * relative.y * Vector3.up;
           // Vector3 damp = -kd * relative;
           
                           //Vector3 totalForce = normal + damp + friction;

           
           
        //    Vector3 verticalDamp = -kd * relative.y * Vector3.up;
        //    Vector3 horizontalDamp = -kd * 0.5f * new Vector3(relative.x, 0, relative.z);

           
            Vector3 damp = -kd * relative.y * Vector3.up;
            
           
           
            Vector3 horizVelocity = new Vector3(relative.x, 0, relative.z);
            Vector3 friction = -mu * normal.magnitude * horizVelocity.normalized;
            
       
       
            Vector3 totalForce = normal + damp + friction;
            
         
         
            netForce += totalForce;
            
          
          
            netTorque += Vector3.Cross(r, totalForce);
        }
        /*** part 3 coding ends ***/

        return (netForce, netTorque);
    }

    private void Integrate(float deltaTime)
    {
        var (force, torque) = ComputeForceAndTorque();

        /*** Part 4: Integrate Timestep ***/
        
        Matrix3x3 i = InertiaRefMatrix;
        Matrix3x3 r = new Matrix3x3(rotation);
        Matrix3x3 i_global = r * i * r.transpose;

        //  linVelocity += (force / mass) * deltaTime;
        linear_velocity += (force / mass) * deltaTime;
        angular_velocity += i_global.inverse * torque * deltaTime;

        
    //      //  position += linVelocity * deltaTime;
    //   position += linear_velocity * deltaTime;
    //     rotation = rotation* Quaternion.Euler(angular_velocity * deltaTime * Mathf.Rad2Deg);
        
        
        position += linear_velocity * deltaTime;
        
        Vector3 rotationDelta = angular_velocity * deltaTime * Mathf.Rad2Deg;
        Quaternion deltaRotation = Quaternion.Euler(rotationDelta);
        rotation = deltaRotation * rotation;
        /*** part 4 coding ends ***/
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
