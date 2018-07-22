using System.Collections;
using System.Collections.Generic;
using System;

namespace BronePoezd.Train
{
    public struct TrainPhysParams
    {
        float mass;
        float maxThrust;
        float maxSpeed;
        float maxBreakingForce;
        float dragAtMaxSpeed;

        public TrainPhysParams(float mass, float maxThrust, float maxSpeed, float maxBreakingForce, float dragAtMaxSpeed)
        {
            this.mass = mass;
            this.maxThrust = maxThrust;
            this.maxSpeed = maxSpeed;
            this.maxBreakingForce = maxBreakingForce;
            this.dragAtMaxSpeed = dragAtMaxSpeed;
        }

        public float GetMass()
        {
            return mass;
        }

        public float GetMaxThrust()
        {
            return maxThrust;
        }

        public float GetMaxSpeed()
        {
            return maxSpeed;
        }

        public float GetMaxBreak()
        {
            return maxBreakingForce;
        }

        public float GetMaxDrag()
        {
            return dragAtMaxSpeed;
        }

        public static TrainPhysParams CalculateTotalParams (List<PlatformController> platformList)
        {
            TrainPhysParams totalParams = new TrainPhysParams(0, 0, 0, 0, 0);
            foreach (PlatformController platform in platformList)
            {
                TrainPhysParams platformPhysParams = platform.GetPhysParams();
                totalParams.mass += platformPhysParams.GetMass();
                totalParams.maxThrust += platformPhysParams.GetMaxThrust();
                totalParams.maxSpeed = Math.Max(platformPhysParams.GetMaxSpeed(), totalParams.maxSpeed);
                totalParams.maxBreakingForce += platformPhysParams.GetMaxBreak();
                totalParams.dragAtMaxSpeed += platformPhysParams.GetMaxDrag();
            }


            return totalParams;
        }
    }
}
