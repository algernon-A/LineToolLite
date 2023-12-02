// <copyright file="LineBase.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// </copyright>

namespace LineTool
{
    using Colossal.Mathematics;
    using Game.Rendering;
    using Game.Simulation;
    using Unity.Collections;
    using Unity.Mathematics;
    using UnityEngine;
    using static Game.Rendering.GuideLinesSystem;

    /// <summary>
    /// Line placement mode.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Protected fields")]
    public abstract class LineBase
    {
        /// <summary>
        /// Indicates whether a valid starting position has been recorded.
        /// </summary>
        protected bool m_validStart;

        /// <summary>
        /// Records the current selection start position.
        /// </summary>
        protected float3 m_startPos;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineBase"/> class.
        /// </summary>
        public LineBase()
        {
            // Basic state.
            m_validStart = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineBase"/> class.
        /// </summary>
        /// <param name="mode">Mode to copy starting state from.</param>
        public LineBase(LineBase mode)
        {
            m_validStart = mode.m_validStart;
            m_startPos = mode.m_startPos;
        }

        /// <summary>
        /// Gets a value indicating whether a valid starting position has been recorded.
        /// </summary>
        public bool HasStart => m_validStart;

        /// <summary>
        /// Gets a value indicating whether we're ready to place (we have enough control positions).
        /// </summary>
        public virtual bool HasAllPoints => m_validStart;

        /// <summary>
        /// Handles a mouse click.
        /// </summary>
        /// <param name="position">Click world position.</param>
        /// <returns><c>true</c> if items are to be placed as a result of this click, <c>false</c> otherwise.</returns>
        public virtual bool HandleClick(float3 position)
        {
            // If no valid start position is set, record it.
            if (!m_validStart)
            {
                m_validStart = true;
                m_startPos = position;

                // No placement at this stage (only the first click has been made).
                return false;
            }

            // Second click; we're placing items.
            return true;
        }

        /// <summary>
        /// Performs actions after items are placed on the current line, setting up for the next line to be set.
        /// </summary>
        /// <param name="position">Click world position.</param>
        public virtual void ItemsPlaced(float3 position)
        {
            // Update new starting location to the previous end point.
            m_startPos = position;
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
        public virtual void CalculatePoints(float3 currentPos, bool fenceMode, float spacing, int rotation, Bounds1 zBounds, NativeList<PointData> pointList, ref TerrainHeightData heightData)
        {
            // Don't do anything if we don't have a valid start point.
            if (!m_validStart)
            {
                return;
            }

            // Calculate length.
            float length = math.length(currentPos - m_startPos);

            // Calculate applied rotation (in radians).
            float appliedRotation = math.radians(rotation);
            if (fenceMode)
            {
                float3 difference = currentPos - m_startPos;
                appliedRotation = math.atan2(difference.x, difference.z);
            }

            // Rotation quaternion.
            quaternion rotationQuaternion = quaternion.Euler(0f, appliedRotation, 0f);

            // Create points.
            float currentDistance = fenceMode ? -zBounds.min : 0f;
            float endLength = fenceMode ? length - zBounds.max : length;
            while (currentDistance < endLength)
            {
                // Calculate interpolated point.
                float3 thisPoint = math.lerp(m_startPos, currentPos, currentDistance / length);
                thisPoint.y = TerrainUtils.SampleHeight(ref heightData, thisPoint);

                // Add point to list.
                pointList.Add(new PointData { Position = thisPoint, Rotation = rotationQuaternion, });
                currentDistance += spacing;
            }
        }

        /// <summary>
        /// Draws any applicable overlay.
        /// </summary>
        /// <param name="currentPos">Current cursor world position.</param>
        /// <param name="overlayBuffer">Overlay buffer.</param>
        /// <param name="tooltips">Tooltip list.</param>
        public virtual void DrawOverlay(float3 currentPos, OverlayRenderSystem.Buffer overlayBuffer, NativeList<TooltipInfo> tooltips)
        {
            DrawDashedLine(m_startPos, currentPos, new Line3.Segment(m_startPos, currentPos), overlayBuffer, tooltips);
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public virtual void Reset()
        {
            m_validStart = false;
        }

        /// <summary>
        /// Draws a dashed line overlay between the two given points.
        /// </summary>
        /// <param name="startPos">Line start position.</param>
        /// <param name="endPos">Line end position.</param>
        /// <param name="segment">Line segment.</param>
        /// <param name="overlayBuffer">Overlay buffer.</param>
        /// <param name="tooltips">Tooltip list.</param>
        protected void DrawDashedLine(float3 startPos, float3 endPos, Line3.Segment segment, OverlayRenderSystem.Buffer overlayBuffer, NativeList<TooltipInfo> tooltips)
        {
            const float LineWidth = 1f;

            float distance = math.distance(startPos.xz, endPos.xz);

            // Don't draw lines for short distances.
            if (distance > LineWidth * 8f)
            {
                // Offset segment, mimicking game simple curve overlay, to ensure dash spacing.
                float3 offset = (segment.b - segment.a) * (LineWidth * 4f / distance);
                Line3.Segment line = new (segment.a + offset, segment.b - offset);

                // Draw line - distance figures mimic game simple curve overlay.
                overlayBuffer.DrawDashedLine(Color.white, line, LineWidth * 3f, LineWidth * 5f, LineWidth * 3f);

                // Add length tooltip.
                int length = Mathf.RoundToInt(math.distance(startPos.xz, endPos.xz));
                if (length > 0)
                {
                    tooltips.Add(new TooltipInfo(TooltipType.Length, (startPos + endPos) * 0.5f, length));
                }
            }
        }
    }
}
