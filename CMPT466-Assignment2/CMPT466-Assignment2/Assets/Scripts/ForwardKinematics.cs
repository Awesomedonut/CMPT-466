using UnityEngine;

public static class ForwardKinematics
{
    public static void UpdateJointPositions(Actor actor, float[] frameData)
    {
        Debug.Log("test 123");

                int channelIndex = 0;
        
        for (int i = 0; i < actor.Joints.Count; i++)
        {
            Joint joint = actor.Joints[i];
            
            
            if (joint.Name == "Site") // skip ends
                continue;
                
            
            // rotation vals
            // Vector3   Vector3.zero;
            // Vector3 position = Vector3.zero;
            
           


            if (i == 0)
            {
                // // root pos
                                float x_p = frameData[channelIndex++];
                float y_p = frameData[channelIndex++];
                float z_p = frameData[channelIndex++];


                // position.x = frameData[channelIndex++];
                // position.y = frameData[channelIndex++];
                // position.z = frameData[channelIndex++];

                Vector3 position = new Vector3(x_p, y_p, -z_p);
                
          
                joint.LocalPosition = position;
            }
            
            
            //rotation vals
            float[] rotValues = new float[3];
            for (int j = 0; j < 3; j++)
            {
                rotValues[j] = frameData[channelIndex++];
            }
            
           
          //  Quaternion localRot = Quaternion.identity;
            Quaternion localRotation;
            
            // switch (joint.RotateOrder)
            // {
            //     case Joint.RotationOrder.XYZ:
            //         localRotation = Quaternion.Euler(rotValues[0], rotValues[1], rotValues[2]);
            //         break;
            //     case Joint.RotationOrder.XZY:
            //         localRotation = Quaternion.Euler(rotValues[0], rotValues[2], rotValues[1]);
            //         break;
            //     case Joint.RotationOrder.YXZ:
            //         localRotation = Quaternion.Euler(rotValues[1], rotValues[0], rotValues[2]);
            //         break;
            //     case Joint.RotationOrder.YZX:
            //         localRotation = Quaternion.Euler(rotValues[1], rotValues[2], rotValues[0]);
            //         break;
            //     case Joint.RotationOrder.ZXY:
            //         localRotation = Quaternion.Euler(rotValues[2], rotValues[0], rotValues[1]);
            //         break;
            //     case Joint.RotationOrder.ZYX:
            //         localRotation = Quaternion.Euler(rotValues[2], rotValues[1], rotValues[0]);
            //         break;
            //     default:
            //         localRotation = Quaternion.Euler(rotValues[0], rotValues[1], rotValues[2]);
            //         break;
            // }

            localRotation = Quaternion.Euler(rotValues[2], rotValues[1], rotValues[0]);

                        Quaternion coordinateSystemCorrection = Quaternion.Euler(-90, 0, 0);
            
            if (i == 0) // root joint
            {
                joint.LocalQuaternion = coordinateSystemCorrection * localRotation;
            }
            else
            {
                joint.LocalQuaternion = localRotation;
            }
            
            // quaternion stuff
            if (joint.ParentIdx == -1)
            {
         
                joint.GlobalQuaternion = joint.LocalQuaternion;
                joint.GlobalPosition = joint.LocalPosition;
            }
            else
            {
                // childd
                Joint parentJoint = actor.Joints[joint.ParentIdx];
                
       
                joint.GlobalQuaternion = parentJoint.GlobalQuaternion * joint.LocalQuaternion;
                
      
              //  Vector3 rotation2 = parentJoint.GlobalQuaternion * joint.LocalPosition;
              //  joint.GlobalPosition = parentJoint.GlobalPosition + rotation2;

              joint.GlobalPosition = parentJoint.GlobalPosition + (parentJoint.GlobalQuaternion * joint.LocalPosition);
            }
        }
    }
}

