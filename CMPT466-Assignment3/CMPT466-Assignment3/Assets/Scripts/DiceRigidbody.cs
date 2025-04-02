using UnityEngine;

public class DiceRigidbody : BaseRigidbody
{
    protected override Vector3[] LocalVertices => new Vector3[]
    {
        new(0.5f, 0.5f, 0.5f),
        new(0.5f, 0.5f, -0.5f),
        new(0.5f, -0.5f, 0.5f),
        new(0.5f, -0.5f, -0.5f),
        new(-0.5f, 0.5f, 0.5f),
        new(-0.5f, 0.5f, -0.5f),
        new(-0.5f, -0.5f, 0.5f),
        new(-0.5f, -0.5f, -0.5f)
    };

    protected override Matrix3x3 InertiaRefMatrix => new Matrix3x3(
        /*** Part 1: Inertia matrix of the dice ***/


        // new Vector3(1.0f/6.0f, 0, 0),
        
        
      
      
        //   new Vector3(1.0f, 0, 0),
        // new Vector3(0, 1.0f, 0),
        // new Vector3(0, 0, 1.0f)
      
      
      
        // new Vector3(0.2f, 0, 0),
        // new Vector3(0, 0.2f, 0),

   
      //  new Vector3(2, 0, 0),
//   new Vector3(0, 1, 0),
   
        
        // new Vector3(1.0f/6.0f, 0, 0),
        // new Vector3(0, 1.0f/6.0f, 0),
        // new Vector3(0, 0, 1.0f/6.0f)
 //new Vector3(0.5f, 0, 0),
        new Vector3(2f, 0, 0),
        new Vector3(0, 2f, 0),
    new Vector3(0, 0, 2f)
    );
}

