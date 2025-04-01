using UnityEngine;

public static class ForwardKinematics
{
    public static void UpdateJointPositions(Actor actor, float[] frameData)
    {
        int channelIndex = 0;
        
        for (int i = 0; i < actor.Joints.Count; i++)
        {
            Joint joint = actor.Joints[i];
            
            // Skip end sites (they don't have channels)
            if (joint.Name == "Site")
                continue;
                
            // Root joint may have position channels
            if (i == 0)
            {
                // Root position (if available)
                float xPos = frameData[channelIndex++];
                float yPos = frameData[channelIndex++];
                float zPos = frameData[channelIndex++];
                
                // If the skeleton is upside down, try flipping the Y coordinate
                Vector3 position = new Vector3(xPos, -yPos, -zPos);
                
                // Set root position
                joint.LocalPosition = position;
            }
            
            // Get rotation values based on channel order
            float[] rotValues = new float[3];
            for (int j = 0; j < 3; j++)
            {
                rotValues[j] = frameData[channelIndex++];
            }
            
            // Apply rotation based on rotation order
            // This respects the rotation order specified in the joint
            Quaternion localRotation;
            
            switch (joint.RotateOrder)
            {
                case Joint.RotationOrder.XYZ:
                    localRotation = Quaternion.Euler(rotValues[0], rotValues[1], rotValues[2]);
                    break;
                case Joint.RotationOrder.XZY:
                    localRotation = Quaternion.Euler(rotValues[0], rotValues[2], rotValues[1]);
                    break;
                case Joint.RotationOrder.YXZ:
                    localRotation = Quaternion.Euler(rotValues[1], rotValues[0], rotValues[2]);
                    break;
                case Joint.RotationOrder.YZX:
                    localRotation = Quaternion.Euler(rotValues[1], rotValues[2], rotValues[0]);
                    break;
                case Joint.RotationOrder.ZXY:
                    localRotation = Quaternion.Euler(rotValues[2], rotValues[0], rotValues[1]);
                    break;
                case Joint.RotationOrder.ZYX:
                    localRotation = Quaternion.Euler(rotValues[2], rotValues[1], rotValues[0]);
                    break;
                default:
                    // Default to XYZ which is Unity's default
                    localRotation = Quaternion.Euler(rotValues[0], rotValues[1], rotValues[2]);
                    break;
            }
            
            // Store the local quaternion
            joint.LocalQuaternion = localRotation;
            
            // Calculate global quaternion and position
            if (joint.ParentIdx == -1)
            {
                // Root joint - apply a different correction to fix the upside-down issue
                // Try a 180-degree rotation around the Z-axis
                Quaternion rootCorrection = Quaternion.Euler(0, 0, 180);
                joint.GlobalQuaternion = rootCorrection * joint.LocalQuaternion;
                joint.GlobalPosition = joint.LocalPosition;
            }
            else
            {
                // Child joint
                Joint parentJoint = actor.Joints[joint.ParentIdx];
                
                // Global rotation is parent's global rotation multiplied by local rotation
                joint.GlobalQuaternion = parentJoint.GlobalQuaternion * joint.LocalQuaternion;
                
                // Global position is parent's global position plus the rotated local position vector
                joint.GlobalPosition = parentJoint.GlobalPosition + (parentJoint.GlobalQuaternion * joint.LocalPosition);
            }
        }
    }
}

