using System;
using WaveProject.Station.Plates;

namespace WaveProject.Station
{
    internal class EmptyPhaseShiftPlate : PhaseShiftPlate
    {
        public EmptyPhaseShiftPlate(float plateLength, float plateThickness) : base(plateLength, plateThickness)
        {
        }

        public override double GetReceiverSignalLevel(float angleInRadians, double variantWavelength)
        {
            const float betta = 0f;
            const float r = 0f;

            var cosOfAngle = Math.Cos(angleInRadians - betta);
            return Math.Pow(Math.Abs(cosOfAngle) * (1 - r) + r, 2) * 100;
        }
    }
}