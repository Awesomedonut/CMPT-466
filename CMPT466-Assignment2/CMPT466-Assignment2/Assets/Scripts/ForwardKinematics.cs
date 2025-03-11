using UnityEngine;

public static class ForwardKinematics{
    public static void UpdateJointPositions(Actor actor, float[] frameData)   {
        Debug.Log("test 123");

        int channelIndex = 0;
        
        // loop thru joints



        for (int i = 0; i < actor.Joints.Count; i++)  {
            Joint joint = actor.Joints[i];
            

            if (joint.Name == "Site") // skip ends
                continue;
                
            if (i == 0) {// root pos

            
            // rotation vals
            // Vector3   Vector3.zero;
            // Vector3 position = Vector3.zero;
            
                
                float x = frameData[channelIndex++]; // first 3 values for hip translational data
                float y = frameData[channelIndex++];
                float z = frameData[channelIndex++];
                   // position.x = frameData[channelIndex++];
                // position.y = frameData[channelIndex++];
                // position.z = frameData[channelIndex++];
              
                joint.LocalPosition = new Vector3(x, y, -z);
            }
            
            //rotation vals
            float x_rot = frameData[channelIndex++];
            float y_rot = frameData[channelIndex++];
            float z_rot = frameData[channelIndex++];
            
   
  //  Quaternion local_rot = Quaternion.identity;
            Quaternion local_rot = Quaternion.identity;
            
            switch (joint.RotateOrder)   {


                case Joint.RotationOrder.YZX:
          
                    local_rot = Quaternion.identity;
        local_rot *= Quaternion.AngleAxis(y_rot, Vector3.up);
        local_rot *= Quaternion.AngleAxis(z_rot, Vector3.forward);
        local_rot *= Quaternion.AngleAxis(x_rot, Vector3.right);
                    break;

                case Joint.RotationOrder.XZY:
                
                    local_rot = Quaternion.identity;
        local_rot *= Quaternion.AngleAxis(x_rot, Vector3.right);
        local_rot *= Quaternion.AngleAxis(z_rot, Vector3.forward);
        local_rot *= Quaternion.AngleAxis(y_rot, Vector3.up);
                    break;
                case Joint.RotationOrder.YXZ:
                
                    local_rot = Quaternion.identity;
        local_rot *= Quaternion.AngleAxis(y_rot, Vector3.up);
        local_rot *= Quaternion.AngleAxis(x_rot, Vector3.right);
        local_rot *= Quaternion.AngleAxis(z_rot, Vector3.forward);
                    break;
                case Joint.RotationOrder.XYZ:
           
                    local_rot = Quaternion.identity;
        local_rot *= Quaternion.AngleAxis(x_rot, Vector3.right);
        local_rot *= Quaternion.AngleAxis(y_rot, Vector3.up);
        local_rot *= Quaternion.AngleAxis(z_rot, Vector3.forward);
                    break;


                case Joint.RotationOrder.ZXY:
                
                    local_rot = Quaternion.identity;
        local_rot *= Quaternion.AngleAxis(z_rot, Vector3.forward);
        local_rot *= Quaternion.AngleAxis(x_rot, Vector3.right);
        local_rot *= Quaternion.AngleAxis(y_rot, Vector3.up);
                    break;
                case Joint.RotationOrder.ZYX:
                 
                    local_rot = Quaternion.identity;
        local_rot *= Quaternion.AngleAxis(z_rot, Vector3.forward);
        local_rot *= Quaternion.AngleAxis(y_rot, Vector3.up);
        local_rot *= Quaternion.AngleAxis(x_rot, Vector3.right);
                    break;
                default:
                
                    local_rot = Quaternion.identity;
        local_rot *= Quaternion.AngleAxis(x_rot, Vector3.right);
        local_rot *= Quaternion.AngleAxis(y_rot, Vector3.up);
        local_rot *= Quaternion.AngleAxis(z_rot, Vector3.forward);
                    break;
            }
            
      
            joint.LocalQuaternion = local_rot;
            
          


            if (joint.ParentIdx == -1) { // root
            
                //joint.GlobalQuaternion = joint.LocalQuaternion;


                
                Quaternion rootCorrection = Quaternion.Euler(-90, 0, 0);
                joint.GlobalQuaternion = rootCorrection * joint.LocalQuaternion;
                joint.GlobalPosition = joint.LocalPosition;
            }else{
                // childd
                Joint parentJoint = actor.Joints[joint.ParentIdx];
                
             
                joint.GlobalQuaternion = parentJoint.GlobalQuaternion * joint.LocalQuaternion;
                
            
                joint.GlobalPosition = parentJoint.GlobalPosition + (parentJoint.GlobalQuaternion * joint.LocalPosition);
            }
        }   //  Vector3 rotation2 = parentJoint.GlobalQuaternion * joint.LocalPosition;
              //  joint.GlobalPosition = parentJoint.GlobalPosition + rotation2;
    }
}
