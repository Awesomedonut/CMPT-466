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
            Vector3 eulerAngles = Vector3.zero;
            Vector3 position = Vector3.zero;
            
           


            if (i == 0)
            {
                // root pos
                position.x = frameData[channelIndex++];
                position.y = frameData[channelIndex++];
                position.z = frameData[channelIndex++];
                
          
                joint.LocalPosition = position;
            }
            
            
            //rotation vals
            float[] rotValues = new float[3];
            for (int j = 0; j < 3; j++)
            {
                rotValues[j] = frameData[channelIndex++];
            }
            
           
            switch (joint.RotateOrder)
            {
                case Joint.RotationOrder.XYZ:
                    joint.LocalQuaternion = Quaternion.Euler(rotValues[0], rotValues[1], rotValues[2]);
                    break;
                case Joint.RotationOrder.XZY:
                    joint.LocalQuaternion = Quaternion.Euler(rotValues[0], rotValues[2], rotValues[1]);
                    break;
                case Joint.RotationOrder.YXZ:
                    joint.LocalQuaternion = Quaternion.Euler(rotValues[1], rotValues[0], rotValues[2]);
                    break;
                case Joint.RotationOrder.YZX:
                    joint.LocalQuaternion = Quaternion.Euler(rotValues[1], rotValues[2], rotValues[0]);
                    break;
                case Joint.RotationOrder.ZXY:
                    joint.LocalQuaternion = Quaternion.Euler(rotValues[2], rotValues[0], rotValues[1]);
                    break;
                case Joint.RotationOrder.ZYX:
                    joint.LocalQuaternion = Quaternion.Euler(rotValues[2], rotValues[1], rotValues[0]);
                    break;
                default:
                    joint.LocalQuaternion = Quaternion.Euler(rotValues[0], rotValues[1], rotValues[2]);
                    break;
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
                
      
                Vector3 rotatedOffset = parentJoint.GlobalQuaternion * joint.LocalPosition;
                joint.GlobalPosition = parentJoint.GlobalPosition + rotatedOffset;
            }
        }
    }
}

