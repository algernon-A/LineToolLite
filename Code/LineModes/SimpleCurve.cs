// <copyright file="SimpleCurve.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// </copyright>

namespace LineTool
{
    using Colossal.Mathematics;
    using Game.Net;
    using Game.Rendering;
    using Game.Simulation;
    using Unity.Collections;
    using Unity.Mathematics;
    using UnityEngine;
    using static Game.Rendering.GuideLinesSystem;

    /// <summary>
    /// Simple curve placement mode.
    /// </summary>
    public class SimpleCurve : LineBase
    {
        // Current elbow point.
        private bool m_validElbow = false;
        private float3 m_elbowPoint;

        // Calculated Bezier.
        private Bezier4x3 _thisBezier;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCurve"/> class.
        /// </summary>
        /// <param name="mode">Mode to copy starting state from.</param>
        public SimpleCurve(LineBase mode)
            : base(mode)
        {
        }

        /// <summary>
        /// Gets a value indicating whether we're ready to place (we have enough control positions).
        /// </summary>
        public override bool HasAllPoints => m_validStart & m_validElbow;

        /// <summary>
        /// Handles a mouse click.
        /// </summary>
        /// <param name="position">Click world position.</param>
        /// <returns>True if items are to be placed as a result of this click, false otherwise.</returns>
        public override bool HandleClick(float3 position)
        {
            // If no valid initial point, record this as the first point.
            if (!m_validStart)
            {
                m_startPos = position;
                m_validStart = true;
                return false;
            }

            // Otherwise, if no valid elbow point, record this as the elbow point.
            if (!m_validElbow)
            {
                m_elbowPoint = position;
                m_validElbow = true;
                return false;
            }

            // Place the items on the curve.
            return true;
        }

        /// <summary>
        /// Performs actions after items are placed on the current line, setting up for the next line to be set.
        /// </summary>
        /// <param name="position">Click world position.</param>
        public override void ItemsPlaced(float3 position)
        {
            // Update new starting location to the previous end point and clear elbow.
            m_startPos = position;
            m_validElbow = false;
        }

        /// <summary>
        /// Calculates the points to use based on this mode.
        /// </summary>
        /// <param name="currentPos">Selection current position.</param>
        /// <param name="fenceMode">Set to <c>true</c> if fence mode is active.</param>
        /// <param name="spacing">Spacing setting.</param>
        /// <param name="rotation">Rotation setting.</param>
        /// <param name="zBounds">Prefab zBounds.</param>
        /// <param name="pointList">List of points to populate.</param>
        /// <param name="heightData">Terrain height data reference.</param>
        public override void CalculatePoints(float3 currentPos, bool fenceMode, float spacing, int rotation, Bounds1 zBounds, NativeList<PointData> pointList, ref TerrainHeightData heightData)
        {
            // Don't do anything if we don't have valid start.
            if (!m_validStart)
            {
                return;
            }

            // If we have a valid start but no valid elbow, just draw a straight line.
            if (!m_validElbow)
            {
                base.CalculatePoints(currentPos, fenceMode, spacing, rotation, zBounds, pointList, ref heightData);
                return;
            }

            // Calculate Bezier.
            _thisBezier = NetUtils.FitCurve(new Line3.Segment(m_startPos, m_elbowPoint), new Line3.Segment(currentPos, m_elbowPoint));

            // Default rotation quaternion.
            quaternion qRotation = quaternion.Euler(0f, math.radians(rotation), 0f);

            float tFactor = 0f;
            while (tFactor < 1.0f)
            {
                // Calculate point.
                float3 thisPoint = MathUtils.Position(_thisBezier, tFactor);

                // Get next t factor.
                tFactor = BezierStep(tFactor, spacing);

                // Calculate applied rotation for fence mode.
                if (fenceMode)
                {
                    float3 difference = MathUtils.Position(_thisBezier, tFactor) - thisPoint;
                    qRotation = quaternion.Euler(0f, math.atan2(difference.x, difference.z), 0f);
                }

                // Calculate terrain height.
                thisPoint.y = TerrainUtils.SampleHeight(ref heightData, thisPoint);

                // Add point to list.
                pointList.Add(new PointData { Position = thisPoint, Rotation = qRotation, });
            }
        }

        /// <summary>
        /// Draws any applicable overlay.
        /// </summary>
        /// <param name="currentPos">Current cursor world position.</param>
        /// <param name="overlayBuffer">Overlay buffer.</param>
        /// <param name="tooltips">Tooltip list.</param>
        public override void DrawOverlay(float3 currentPos, OverlayRenderSystem.Buffer overlayBuffer, NativeList<TooltipInfo> tooltips)
        {
            if (m_validStart)
            {
                // Draw an elbow overlay if we've got valid starting and elbow positions.
                if (m_validElbow)
                {
                    // Calculate lines.
                    Line3.Segment line1 = new (m_startPos, m_elbowPoint);
                    Line3.Segment line2 = new (m_elbowPoint, currentPos);

                    // Draw lines.
                    DrawDashedLine(m_startPos, m_elbowPoint, line1, overlayBuffer, tooltips);
                    DrawDashedLine(m_elbowPoint, currentPos, line2, overlayBuffer, tooltips);

                    // Draw angle.
                    DrawAngleIndicator(line1, line2, 8f, 8f, overlayBuffer, tooltips);
                }
                else
                {
                    // Initial position only; just draw a straight line.
                    base.DrawOverlay(currentPos, overlayBuffer, tooltips);
                }
            }
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public override void Reset()
        {
            // Only clear elbow if we have one.
            if (m_validElbow)
            {
                m_validElbow = false;
            }
            else
            {
                base.Reset();
            }
        }

        /// <summary>
        /// Steps along a Bezier calculating the target t factor for the given starting t factor and the given distance.
        /// Code based on Alterran's PropLineTool (StepDistanceCurve, Utilities/PLTMath.cs).
        /// </summary>
        /// <param name="tStart">Starting t factor.</param>
        /// <param name="distance">Distance to travel.</param>
        /// <returns>Target t factor.</returns>
        private float BezierStep(float tStart, float distance)
        {
            const float Tolerance = 0.001f;
            const float ToleranceSquared = Tolerance * Tolerance;

            float tEnd = Travel(tStart, distance);
            float usedDistance = CubicBezierArcLengthXZGauss04(tStart, tEnd);

            // Twelve iteration maximum for performance and to prevent infinite loops.
            for (int i = 0; i < 12; ++i)
            {
                // Stop looping if the remaining distance is less than tolerance.
                float remainingDistance = distance - usedDistance;
                if (remainingDistance * remainingDistance < ToleranceSquared)
                {
                    break;
                }

                usedDistance = CubicBezierArcLengthXZGauss04(tStart, tEnd);
                tEnd += (distance - usedDistance) / CubicSpeedXZ(tEnd);
            }

            return tEnd;
        }

        /// <summary>
        /// Steps along a Bezier BACKWARDS from the end point, calculating the target t factor for the given spacing distance.
        /// Code based on Alterran's PropLineTool (StepDistanceCurve, Utilities/PLTMath.cs).
        /// </summary>
        /// <param name="distance">Distance to travel.</param>
        /// <returns>Target t factor.</returns>
        private float BezierStepReverse(float distance)
        {
            const float Tolerance = 0.001f;
            const float ToleranceSquared = Tolerance * Tolerance;

            float tEnd = Travel(1, -distance);
            float usedDistance = CubicBezierArcLengthXZGauss04(tEnd, 1.0f);

            // Twelve iteration maximum for performance and to prevent infinite loops.
            for (int i = 0; i < 12; ++i)
            {
                // Stop looping if the remaining distance is less than tolerance.
                float remainingDistance = distance - usedDistance;
                if (remainingDistance * remainingDistance < ToleranceSquared)
                {
                    break;
                }

                usedDistance = CubicBezierArcLengthXZGauss04(tEnd, 1.0f);
                tEnd -= (distance - usedDistance) / CubicSpeedXZ(tEnd);
            }

            return tEnd;
        }

        /// <summary>
        /// From Alterann's PropLineTool (CubicSpeedXZ, Utilities/PLTMath.cs).
        /// Returns the integrand of the arc length function for a cubic Bezier curve, constrained to the XZ-plane at a specific t.
        /// </summary>
        /// <param name="t"> t factor.</param>
        /// <returns>Integrand of arc length.</returns>
        private float CubicSpeedXZ(float t)
        {
            // Pythagorean theorem.
            float3 tangent = MathUtils.Tangent(_thisBezier, t);
            float derivXsqr = tangent.x * tangent.x;
            float derivZsqr = tangent.z * tangent.z;

            return math.sqrt(derivXsqr + derivZsqr);
        }

        /// <summary>
        /// From Alterann's PropLineTool (CubicBezierArcLengthXZGauss04, Utilities/PLTMath.cs).
        /// Returns the XZ arclength of a cubic Bezier curve between two t factors.
        /// Uses Gauss–Legendre Quadrature with n = 4.
        /// </summary>
        /// <param name="t1">Starting t factor.</param>
        /// <param name="t2">Ending t factor.</param>
        /// <returns>XZ arc length.</returns>
        private float CubicBezierArcLengthXZGauss04(float t1, float t2)
        {
            float linearAdj = (t2 - t1) / 2f;

            // Constants are from Gauss-Lengendre quadrature rules for n = 4.
            float p1 = CubicSpeedXZGaussPoint(0.3399810435848563f, 0.6521451548625461f, t1, t2);
            float p2 = CubicSpeedXZGaussPoint(-0.3399810435848563f, 0.6521451548625461f, t1, t2);
            float p3 = CubicSpeedXZGaussPoint(0.8611363115940526f, 0.3478548451374538f, t1, t2);
            float p4 = CubicSpeedXZGaussPoint(-0.8611363115940526f, 0.3478548451374538f, t1, t2);

            return linearAdj * (p1 + p2 + p3 + p4);
        }

        /// <summary>
        /// From Alterann's PropLineTool (CubicSpeedXZGaussPoint, Utilities/PLTMath.cs).
        /// </summary>
        /// <param name="x_i">X i.</param>
        /// <param name="w_i">W i.</param>
        /// <param name="a">a.</param>
        /// <param name="b">b.</param>
        /// <returns>Cubic speed.</returns>
        private float CubicSpeedXZGaussPoint(float x_i, float w_i, float a, float b)
        {
            float linearAdj = (b - a) / 2f;
            float constantAdj = (a + b) / 2f;
            return w_i * CubicSpeedXZ((linearAdj * x_i) + constantAdj);
        }

        /// <summary>
        /// Based on CS1's mathematics calculations for Bezier travel.
        /// </summary>
        /// <param name="start">Starting t-factor.</param>
        /// <param name="distance">Distance to travel.</param>
        /// <returns>Ending t-factor.</returns>
        private float Travel(float start, float distance)
        {
            Vector3 startPos = MathUtils.Position(_thisBezier, start);

            if (distance < 0f)
            {
                // Negative (reverse) direction.
                distance = 0f - distance;
                float t1 = 0f;
                float t2 = start;
                float f1 = Vector3.SqrMagnitude(_thisBezier.a - (float3)startPos);
                float f2 = 0f;

                // Eight steps max.
                for (int i = 0; i < 8; ++i)
                {
                    // Calculate current position.
                    float tMid = (t1 + t2) * 0.5f;
                    Vector3 midpoint = MathUtils.Position(_thisBezier, tMid);
                    float midDistance = Vector3.SqrMagnitude(midpoint - startPos);

                    // Check for nearer match.
                    if (midDistance < distance * distance)
                    {
                        t2 = tMid;
                        f2 = midDistance;
                    }
                    else
                    {
                        t1 = tMid;
                        f1 = midDistance;
                    }
                }

                // We've been using square magnitudes for comparison, so rest to true value.
                f1 = Mathf.Sqrt(f1);
                f2 = Mathf.Sqrt(f2);

                // Check for exact match.
                float fDiff = f1 - f2;
                if (fDiff == 0f)
                {
                    // Exact match found - return that.
                    return t2;
                }

                // Not an exact match - use an interpolation.
                return Mathf.Lerp(t2, t1, Mathf.Clamp01((distance - f2) / fDiff));
            }
            else
            {
                // Positive (forward) direction.
                float t1 = start;
                float t2 = 1f;
                float f1 = 0f;
                float f2 = Vector3.SqrMagnitude(_thisBezier.d - (float3)startPos);

                // Eight steps max.
                for (int i = 0; i < 8; ++i)
                {
                    // Calculate current position.
                    float tMid = (t1 + t2) * 0.5f;
                    Vector3 midPoint = MathUtils.Position(_thisBezier, tMid);
                    float midDistance = Vector3.SqrMagnitude(midPoint - startPos);

                    // Check for nearer match.
                    if (midDistance < distance * distance)
                    {
                        t1 = tMid;
                        f1 = midDistance;
                    }
                    else
                    {
                        t2 = tMid;
                        f2 = midDistance;
                    }
                }

                // We've been using square magnitudes for comparison, so rest to true value.
                f1 = Mathf.Sqrt(f1);
                f2 = Mathf.Sqrt(f2);

                // Check for exact match.
                float fDiff = f2 - f1;
                if (fDiff == 0f)
                {
                    // Exact match found - return that.
                    return t1;
                }

                // Not an exact match - use an interpolation.
                return Mathf.Lerp(t1, t2, Mathf.Clamp01((distance - f1) / fDiff));
            }
        }

        /// <summary>
        /// Draws an angle indicator between two lines.
        /// </summary>
        /// <param name="line1">Line 1.</param>
        /// <param name="line2">Line 2.</param>
        /// <param name="lineWidth">Overlay line width.</param>
        /// <param name="lineLength">Overlay line length.</param>
        /// <param name="overlayBuffer">Overlay buffer.</param>
        /// <param name="tooltips">Tooltip list.</param>
        private void DrawAngleIndicator(Line3.Segment line1, Line3.Segment line2, float lineWidth, float lineLength, OverlayRenderSystem.Buffer overlayBuffer, NativeList<TooltipInfo> tooltips)
        {
            bool angleSide = false;

            // Calculate line lengths.
            float line1Length = math.distance(line1.a.xz, line1.b.xz);
            float line2Length = math.distance(line2.a.xz, line2.b.xz);

            // Minimum line length check.
            if (line1Length > lineWidth * 7f && line2Length > lineWidth * 7f)
            {
                // Calculate line directions.
                float2 line1Direction = (line1.b.xz - line1.a.xz) / line1Length;
                float2 line2Direction = (line2.a.xz - line2.b.xz) / line2Length;

                // Display size.
                float size = math.min(lineLength, math.min(line1Length, line2Length)) * 0.5f;

                // Calculate angle and determine shortest side.
                int angle = Mathf.RoundToInt(math.degrees(math.acos(math.clamp(math.dot(line1Direction, line2Direction), -1f, 1f))));
                if (angle < 180)
                {
                    angleSide = math.dot(MathUtils.Right(line1Direction), line2Direction) < 0f;
                }

                // Check angle type - straight line, obtuse, right-angle, acute.
                if (angle == 180)
                {
                    // Straight line - three lines.
                    // Get perpendiculars.
                    float2 angle1Direction = angleSide ? MathUtils.Right(line1Direction) : MathUtils.Left(line1Direction);
                    float2 angle2Direction = angleSide ? MathUtils.Right(line2Direction) : MathUtils.Left(line2Direction);
                    float3 line1Start = line1.b;
                    line1Start.xz -= line1Direction * size;

                    // Calculate three lines.
                    float3 line1End = line1.b;
                    float3 line2Start = line1.b;
                    line1End.xz += (angle1Direction * (size - (lineWidth * 0.5f))) - (line1Direction * size);
                    line2Start.xz += (angle1Direction * size) - (line1Direction * (size + (lineWidth * 0.5f)));
                    float3 line2End = line2.a;
                    float3 line3Start = line2.a;
                    line2End.xz -= (angle2Direction * size) + (line2Direction * (size + (lineWidth * 0.5f)));
                    line3Start.xz -= (angle2Direction * (size - (lineWidth * 0.5f))) + (line2Direction * size);
                    float3 line3End = line2.a;
                    line3End.xz -= line2Direction * size;

                    // Draw lines.
                    overlayBuffer.DrawLine(Color.white, new Line3.Segment(line1Start, line1End), lineWidth);
                    overlayBuffer.DrawLine(Color.white, new Line3.Segment(line2Start, line2End), lineWidth);
                    overlayBuffer.DrawLine(Color.white, new Line3.Segment(line3Start, line3End), lineWidth);

                    // Add tooltip.
                    float3 tooltipPos = line1.b;
                    tooltipPos.xz += angle1Direction * (size * 1.5f);
                    TooltipInfo value = new (TooltipType.Angle, tooltipPos, angle);
                    tooltips.Add(in value);
                }
                else if (angle > 90)
                {
                    // Obtuse angle - two angle indicators.
                    float2 angleDirection = math.normalize(line1Direction + line2Direction);
                    float3 startPoint = line1.b;

                    // Calculate two sequential curves.
                    startPoint.xz -= line1Direction * size;
                    float3 startTangent = default;
                    startTangent.xz = angleSide ? MathUtils.Right(line1Direction) : MathUtils.Left(line1Direction);
                    float3 midPoint = line1.b;
                    midPoint.xz -= angleDirection * size;
                    float3 midTangent = default;
                    midTangent.xz = angleSide ? MathUtils.Right(angleDirection) : MathUtils.Left(angleDirection);
                    float3 endPoint = line2.a;
                    endPoint.xz -= line2Direction * size;
                    float3 endTangent = default;
                    endTangent.xz = angleSide ? MathUtils.Right(line2Direction) : MathUtils.Left(line2Direction);

                    // Draw curves.
                    overlayBuffer.DrawCurve(Color.white, NetUtils.FitCurve(startPoint, startTangent, midTangent, midPoint), lineWidth);
                    overlayBuffer.DrawCurve(Color.white, NetUtils.FitCurve(midPoint, midTangent, endTangent, endPoint), lineWidth);

                    // Add tooltip.
                    float3 tooltipPos = line1.b;
                    tooltipPos.xz -= angleDirection * (size * 1.5f);
                    TooltipInfo value = new (TooltipType.Angle, tooltipPos, angle);
                    tooltips.Add(in value);
                }
                else if (angle == 90)
                {
                    // Right angle - two lines.
                    float3 line1Start = line1.b;
                    line1Start.xz -= line1Direction * size;

                    // Calculate two lines.
                    float3 line1End = line1.b;
                    float3 line2Start = line1.b;
                    line1End.xz -= (line2Direction * (size - (lineWidth * 0.5f))) + (line1Direction * size);
                    line2Start.xz -= (line2Direction * size) + (line1Direction * (size + (lineWidth * 0.5f)));
                    float3 line2End = line2.a;
                    line2End.xz -= line2Direction * size;

                    // Draw lines.
                    overlayBuffer.DrawLine(Color.white, new Line3.Segment(line1Start, line1End), lineWidth);
                    overlayBuffer.DrawLine(Color.white, new Line3.Segment(line2Start, line2End), lineWidth);

                    // Add tooltip.
                    float3 tooltipPos = line1.b;
                    tooltipPos.xz -= math.normalizesafe(line1Direction + line2Direction) * (size * 1.5f);
                    TooltipInfo value = new (TooltipType.Angle, tooltipPos, angle);
                    tooltips.Add(in value);
                }
                else if (angle > 0)
                {
                    // Acute angle - one angle indicator.
                    float3 startPos = line1.b;
                    startPos.xz -= line1Direction * size;

                    // Calculate single curve.
                    float3 startTangent = default;
                    startTangent.xz = angleSide ? MathUtils.Right(line1Direction) : MathUtils.Left(line1Direction);
                    float3 endPos = line2.a;
                    endPos.xz -= line2Direction * size;
                    float3 endTangent = default;
                    endTangent.xz = angleSide ? MathUtils.Right(line2Direction) : MathUtils.Left(line2Direction);

                    // Draw curve.
                    overlayBuffer.DrawCurve(Color.white, NetUtils.FitCurve(startPos, startTangent, endTangent, endPos), lineWidth);

                    // Add tooltip.
                    float3 tooltipPos = line1.b;
                    tooltipPos.xz -= math.normalizesafe(line1Direction + line2Direction) * (size * 1.5f);
                    TooltipInfo value = new (TooltipType.Angle, tooltipPos, angle);
                    tooltips.Add(in value);
                }
            }
        }
    }
}
