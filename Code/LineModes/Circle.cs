// <copyright file="Circle.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// </copyright>

namespace LineTool
{
    using Colossal.Mathematics;
    using Game.Simulation;
    using Unity.Collections;
    using Unity.Mathematics;

    /// <summary>
    ///  Circle placement mode.
    /// </summary>
    public class Circle : LineBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> class.
        /// </summary>
        /// <param name="mode">Mode to copy starting state from.</param>
        public Circle(LineBase mode)
            : base(mode)
        {
        }

        /// <summary>
        /// Calculates the points to use based on this mode.
        /// </summary>
        /// <param name="currentPos">Selection current position.</param>
        /// <param name="fenceMode">Set to <c>true</c> if fence mode is active.</param>
        /// <param name="spacing">Spacing setting.</param>
        /// <param name="randomSpacing">Random spacing offset maximum.</param>
        /// <param name="randomOffset">Random lateral offset maximum.</param>
        /// <param name="rotation">Rotation setting.</param>
        /// <param name="zBounds">Prefab zBounds.</param>
        /// <param name="pointList">List of points to populate.</param>
        /// <param name="heightData">Terrain height data reference.</param>
        public override void CalculatePoints(float3 currentPos, bool fenceMode, float spacing, float randomSpacing, float randomOffset, int rotation, Bounds1 zBounds, NativeList<PointData> pointList, ref TerrainHeightData heightData)
        {
            // Don't do anything if we don't have valid start.
            if (!m_validStart)
            {
                return;
            }

            // Calculate length.
            float3 difference = currentPos - m_startPos;
            float radius = math.length(difference);

            // Calculate spacing.
            float circumference = radius * math.PI * 2f;
            float numPoints = math.floor(circumference / spacing);
            float increment = (math.PI * 2f) / numPoints;
            float startAngle = math.atan2(difference.z, difference.x);
            System.Random random = new ((int)circumference * 1000);

            // Create points.
            for (float i = startAngle; i < startAngle + (math.PI * 2f); i += increment)
            {
                // Apply spacing adjustment.
                float adjustedAngle = i;
                if (randomSpacing > 0f && !fenceMode)
                {
                    float distanceAdjustment = (float)(random.NextDouble() * randomSpacing * 2f) - randomSpacing;
                    adjustedAngle += (distanceAdjustment * math.PI * 2f) / circumference;
                }

                // Calculate point.
                float xPos = radius * math.cos(adjustedAngle);
                float yPos = radius * math.sin(adjustedAngle);
                float3 thisPoint = new (m_startPos.x + xPos, m_startPos.y, m_startPos.z + yPos);

                // Apply offset adjustment.
                if (randomOffset > 0f && !fenceMode)
                {
                    thisPoint += math.normalize(thisPoint - m_startPos) * ((float)(randomOffset * random.NextDouble() * 2f) - randomOffset);
                }

                // Calculate terrain height.
                thisPoint.y = TerrainUtils.SampleHeight(ref heightData, thisPoint);

                // Add point to list.
                pointList.Add(new PointData { Position = thisPoint, Rotation = quaternion.Euler(0f, math.radians(rotation) - i, 0f), });
            }
        }

        /// <summary>
        /// Performs actions after items are placed on the current line, setting up for the next line to be set.
        /// </summary>
        /// <param name="location">Click world location.</param>
        public override void ItemsPlaced(float3 location)
        {
            // Empty, to retain original start position (centre of circle).
        }
    }
}
