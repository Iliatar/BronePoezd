using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BronePoezd.Train
{
    class SegmentsPathLibrary
    {
        #region SingleTon
        private static SegmentsPathLibrary instance;
        public static SegmentsPathLibrary GetInstance()
        {
            if (instance == null)
            {
                instance = new SegmentsPathLibrary();
            }
            return instance;
        }
        #endregion

        //константы для больших кривых
        const float rMax = 1.5f;
        const float rMin = 1.4142135623f;
        const float lMaxStraigthCoeff = 0.975f;
        const float lMaxDiagonalCoeff = 1.025f;

        public SegmentPathData GetSegmentPathData(byte exitFrom, byte exitTo)
        {
            Func<float, float> xFunc = null;
            Func<float, float> yFunc = null;
            float lMax = 0;

            string codeFromTo = exitFrom.ToString() + exitTo.ToString();
            switch (codeFromTo)
            {
                #region Depot
                case "93":
                    {
                        const float magicLMax = 15;
                        xFunc = new Func<float, float>(l => 0.5f);
                        yFunc = new Func<float, float>(l =>
                        {
                            const float magicPoint = 0.5f;
                            float y;
                            if (l > magicLMax - magicPoint)
                            {
                                y = 1 + l - magicLMax;
                            }
                            else
                            {
                                y = magicPoint;
                            }
                            return y;
                        });
                        lMax = magicLMax;
                        break;
                    }

                case "39":
                    {
                        const float magicLMax = 15;
                        xFunc = new Func<float, float>(l => 0.5f);
                        yFunc = new Func<float, float>(l =>
                        {
                            const float magicPoint = 0.5f;
                            float y;
                            if (l < magicPoint)
                            {
                                y = 1 - l;
                            }
                            else
                            {
                                y = 1 - magicPoint;
                            }
                            return y;
                        });
                        lMax = magicLMax;
                        break;
                    }
                #endregion

                #region Straight
                case "73": //проверено
                    {
                        xFunc = new Func<float, float>(l => 0.5f);
                        yFunc = new Func<float, float>(l => l);
                        lMax = 1;
                        break;
                    }

                case "37": //проверено
                    {
                        xFunc = new Func<float, float>(l => 0.5f);
                        yFunc = new Func<float, float>(l => 1 - l);
                        lMax = 1;
                        break;
                    }

                case "15": //проверено
                    {
                        xFunc = new Func<float, float>(l => l);
                        yFunc = new Func<float, float>(l => 0.5f);
                        lMax = 1;
                        break;
                    }

                case "51": //проверено
                    {
                        xFunc = new Func<float, float>(l => 1 - l);
                        yFunc = new Func<float, float>(l => 0.5f);
                        lMax = 1;
                        break;
                    }
                #endregion

                #region Diagonal
                case "04": //проверено
                    {
                        xFunc = new Func<float, float>(l => l / (float)Math.Sqrt(2));
                        yFunc = new Func<float, float>(l => l / (float)Math.Sqrt(2));
                        lMax = (float)Math.Sqrt(2);
                        break;
                    }

                case "40": //проверено
                    {
                        xFunc = new Func<float, float>(l => 1 - l / (float)Math.Sqrt(2));
                        yFunc = new Func<float, float>(l => 1 - l / (float)Math.Sqrt(2));
                        lMax = (float)Math.Sqrt(2);
                        break;
                    }

                case "26": //проверено
                    {
                        xFunc = new Func<float, float>(l => l / (float)Math.Sqrt(2));
                        yFunc = new Func<float, float>(l => 1 - l / (float)Math.Sqrt(2));
                        lMax = (float)Math.Sqrt(2);
                        break;
                    }

                case "62": //проверено
                    {
                        xFunc = new Func<float, float>(l => 1 - l / (float)Math.Sqrt(2));
                        yFunc = new Func<float, float>(l => l / (float)Math.Sqrt(2));
                        lMax = (float)Math.Sqrt(2);
                        break;
                    }
                #endregion

                #region SmallCurves
                case "75": //проверено
                    {
                        xFunc = new Func<float, float>(l => (float)Math.Cos(-l * 2 + Math.PI) / 2 + 1);
                        yFunc = new Func<float, float>(l => (float)Math.Sin(-l * 2 + Math.PI) / 2);
                        lMax = (float)Math.PI / 4;
                        break;
                    }

                case "57": //проверено
                    {
                        xFunc = new Func<float, float>(l => (float)Math.Cos(l * 2 + Math.PI / 2) / 2 + 1);
                        yFunc = new Func<float, float>(l => (float)Math.Sin(l * 2 + Math.PI / 2) / 2);
                        lMax = (float)Math.PI / 4;
                        break;
                    }

                case "71": //проверено
                    {
                        xFunc = new Func<float, float>(l => (float)Math.Cos(l * 2) / 2);
                        yFunc = new Func<float, float>(l => (float)Math.Sin(l * 2) / 2);
                        lMax = (float)Math.PI / 4;
                        break;
                    }

                case "17": //проверено
                    {
                        xFunc = new Func<float, float>(l => (float)Math.Cos(Math.PI / 2 - l * 2) / 2);
                        yFunc = new Func<float, float>(l => (float)Math.Sin(Math.PI / 2 - l * 2) / 2);
                        lMax = (float)Math.PI / 4;
                        break;
                    }

                case "13": //проверено
                    {
                        xFunc = new Func<float, float>(l => (float)Math.Cos(l * 2 - Math.PI / 2) / 2);
                        yFunc = new Func<float, float>(l => (float)Math.Sin(l * 2 - Math.PI / 2) / 2 + 1f);
                        lMax = (float)Math.PI / 4;
                        break;
                    }

                case "31": //проверено
                    {
                        xFunc = new Func<float, float>(l => (float)Math.Cos(-l * 2) / 2);
                        yFunc = new Func<float, float>(l => (float)Math.Sin(-l * 2) / 2 + 1f);
                        lMax = (float)Math.PI / 4;
                        break;
                    }

                case "35"://проверено
                    {
                        xFunc = new Func<float, float>(l => (float)Math.Cos(l * 2 + Math.PI) / 2 + 1);
                        yFunc = new Func<float, float>(l => (float)Math.Sin(l * 2 + Math.PI) / 2 + 1);
                        lMax = (float)Math.PI / 4;
                        break;
                    }

                case "53"://проверено
                    {
                        xFunc = new Func<float, float>(l => (float)Math.Cos(-l * 2 + Math.PI * 3 / 2) / 2 + 1);
                        yFunc = new Func<float, float>(l => (float)Math.Sin(-l * 2 + Math.PI * 3 / 2) / 2 + 1);
                        lMax = (float)Math.PI / 4;
                        break;
                    }
                #endregion

                #region BigCurves
                case "74"://проверено
                    {

                        const float angleOffset = (float)Math.PI;
                        const float xOffset = 2;
                        const float yOffset = 0;
                        const float sign = -1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxDiagonalCoeff;

                        break;
                    }

                case "16"://проверено
                    {

                        const float angleOffset = (float)Math.PI / 2;
                        const float xOffset = 0;
                        const float yOffset = -1;
                        const float sign = -1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxDiagonalCoeff;

                        break;
                    }

                case "30"://проверено
                    {

                        const float angleOffset = (float)Math.PI / 2 * 0;
                        const float xOffset = -1;
                        const float yOffset = 1;
                        const float sign = -1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxDiagonalCoeff;

                        break;
                    }

                case "52"://проверено
                    {

                        const float angleOffset = (float)Math.PI / 2 * 3;
                        const float xOffset = 1;
                        const float yOffset = 2;
                        const float sign = -1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxDiagonalCoeff;

                        break;
                    }

                case "72"://проверено
                    {

                        const float angleOffset = (float)Math.PI / 2 * 0;
                        const float xOffset = -1;
                        const float yOffset = 0;
                        const float sign = 1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxDiagonalCoeff;

                        break;
                    }

                case "14"://проверено
                    {

                        const float angleOffset = (float)Math.PI / 2 * 3;
                        const float xOffset = 0;
                        const float yOffset = 2;
                        const float sign = 1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxDiagonalCoeff;

                        break;
                    }

                case "36"://проверено
                    {

                        const float angleOffset = (float)Math.PI / 2 * 2;
                        const float xOffset = 2;
                        const float yOffset = 1;
                        const float sign = 1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxDiagonalCoeff;

                        break;
                    }

                case "50"://проверено
                    {

                        const float angleOffset = (float)Math.PI / 2 * 1;
                        const float xOffset = 1;
                        const float yOffset = -1;
                        const float sign = 1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMax - (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxDiagonalCoeff;

                        break;
                    }

                case "47"://проверено
                    {

                        const float angleOffset = (float)Math.PI / 4 * 3;
                        const float xOffset = 2;
                        const float yOffset = 0;
                        const float sign = 1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxStraigthCoeff;

                        break;
                    }

                case "61"://проверено
                    {

                        const float angleOffset = (float)Math.PI / 4 * 1;
                        const float xOffset = 0;
                        const float yOffset = -1;
                        const float sign = 1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxStraigthCoeff;

                        break;
                    }

                case "03"://проверено
                    {

                        const float angleOffset = (float)Math.PI / 4 * 7;
                        const float xOffset = -1;
                        const float yOffset = 1;
                        const float sign = 1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxStraigthCoeff;

                        break;
                    }

                case "25"://проверено
                    {
                        const float angleOffset = (float)Math.PI / 4 * 5;
                        const float xOffset = 1;
                        const float yOffset = 2;
                        const float sign = 1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxStraigthCoeff;

                        break;
                    }

                case "63"://проверено
                    {
                        const float angleOffset = (float)Math.PI / 4 * 5;
                        const float xOffset = 2;
                        const float yOffset = 1;
                        const float sign = -1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxStraigthCoeff;

                        break;
                    }

                case "41"://проверено
                    {
                        const float angleOffset = (float)Math.PI / 4 * 7;
                        const float xOffset = 0;
                        const float yOffset = 2;
                        const float sign = -1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxStraigthCoeff;

                        break;
                    }

                case "27"://проверено
                    {
                        const float angleOffset = (float)Math.PI / 4 * 1;
                        const float xOffset = -1;
                        const float yOffset = 0;
                        const float sign = -1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxStraigthCoeff;

                        break;
                    }

                case "05"://проверено
                    {
                        const float angleOffset = (float)Math.PI / 4 * 3;
                        const float xOffset = 1;
                        const float yOffset = -1;
                        const float sign = -1;

                        xFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Cos(sign * l / rCur + angleOffset) * rCur + xOffset;
                        });

                        yFunc = new Func<float, float>(l =>
                        {
                            float rCur = rMin + (rMax - rMin) * 8 / ((rMax + rMin) * (float)Math.PI) * l;
                            return (float)Math.Sin(sign * l / rCur + angleOffset) * rCur + yOffset;
                        });

                        lMax = (rMax + rMin) / 2 * (float)Math.PI / 4 / lMaxStraigthCoeff;

                        break;
                    }
                #endregion
                default:
                    {
                        throw new ArgumentException("Invalid combination of exitFrom and exitTo");
                    }
            }

            SegmentPathData result = new SegmentPathData(lMax, xFunc, yFunc);
            return result;
        }



        public class SegmentPathData
        {
            public float LMax { get; private set; }
            public Func<float, float> XFunction { get; private set; }
            public Func<float, float> YFunction { get; private set; }

            public SegmentPathData(float lMax, Func<float, float> xFunc, Func<float, float> yFunc)
            {
                this.LMax = lMax;
                this.XFunction = xFunc;
                this.YFunction = yFunc;
            }
        }



    }
}
