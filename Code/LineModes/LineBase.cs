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
    using static LineToolSystem;

    /// <summary>
    /// Line placement mode.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Protected fields")]
    public abstract class LineBase
    {
        /// <summary>
        /// Selection radius of points.
        /// </summary>
        protected const float PointRadius = 8f;

        /// <summary>
        /// Indicates whether a valid starting position has been recorded.
        /// </summary>
        protected bool m_validStart;

        /// <summary>
        /// Records the current selection start position.
        /// </summary>
        protected float3 m_startPos;

        /// <summary>
        /// Records the current selection end position.
        /// </summary>
        protected float3 m_endPos;

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
        /// <param name="spacingMode">Active spacing mode.</param>
        /// <param name="spacing">Spacing distance.</param>
        /// <param name="randomSpacing">Random spacing offset maximum.</param>
        /// <param name="randomOffset">Random lateral offset maximum.</param>
        /// <param name="rotation">Rotation setting.</param>
        /// <param name="zBounds">Prefab zBounds.</param>
        /// <param name="pointList">List of points to populate.</param>
        /// <param name="heightData">Terrain height data reference.</param>
        public virtual void CalculatePoints(float3 currentPos, SpacingMode spacingMode, float spacing, float randomSpacing, float randomOffset, int rotation, Bounds1 zBounds, NativeList<PointData> pointList, ref TerrainHeightData heightData)
        {
            // Don't do anything if we don't have a valid start point.
            if (!m_validStart)
            {
                return;
            }

            // Calculate length.
            float3 difference = currentPos - m_startPos;
            float length = math.length(difference);
            System.Random random = new ((int)length * 1000);

            // Calculate applied rotation (in radians).
            float appliedRotation = math.radians(rotation);
            if (spacingMode == SpacingMode.FenceMode)
            {
                appliedRotation = math.atan2(difference.x, difference.z);
            }

            // Rotation quaternion.
            quaternion qRotation = quaternion.Euler(0f, appliedRotation, 0f);

            // Calculate even full-length spacing if needed.
            float adjustedSpacing = spacing;
            if (spacingMode == SpacingMode.FullLength)
            {
                adjustedSpacing = length / math.round(length / spacing);
            }

            // Create points.
            float currentDistance = spacingMode == SpacingMode.FenceMode ? -zBounds.min : 0f;
            float endLength = spacingMode == SpacingMode.FenceMode ? length - zBounds.max : length;
            while (currentDistance < endLength)
            {
                // Calculate interpolated point.
                float spacingAdjustment = 0f;
                if (randomSpacing > 0f && spacingMode != SpacingMode.FenceMode)
                {
                    spacingAdjustment = (float)(random.NextDouble() * randomSpacing * 2f) - randomSpacing;
                }

                float3 thisPoint = math.lerp(m_startPos, currentPos, (currentDistance + spacingAdjustment) / length);

                // Apply offset adjustment.
                if (randomOffset > 0f && spacingMode != SpacingMode.FenceMode)
                {
                    float3 left = math.normalize(new float3(-difference.z, 0f, difference.x));
                    thisPoint += left * ((float)(randomOffset * random.NextDouble() * 2f) - randomOffset);
                }

                thisPoint.y = TerrainUtils.SampleHeight(ref heightData, thisPoint);

                // Add point to list.
                pointList.Add(new PointData { Position = thisPoint, Rotation = qRotation, });
                currentDistance += adjustedSpacing;
            }

            // Final item for full-length mode if required (if there was a distance overshoot).
            if (spacingMode == SpacingMode.FullLength && currentDistance < length + adjustedSpacing)
            {
                float3 thisPoint = currentPos;
                thisPoint.y = TerrainUtils.SampleHeight(ref heightData, thisPoint);

                // Add point to list.
                pointList.Add(new PointData { Position = thisPoint, Rotation = qRotation, });
            }

            // Record end position for overlays.
            m_endPos = currentPos;
        }

        /// <summary>
        /// Draws any applicable overlay.
        /// </summary>
        /// <param name="overlayBuffer">Overlay buffer.</param>
        /// <param name="tooltips">Tooltip list.</param>
        public virtual void DrawOverlay(OverlayRenderSystem.Buffer overlayBuffer, NativeList<TooltipInfo> tooltips)
        {
            // Don't draw overlay if we don't have a valid start.
            if (m_validStart)
            {
                DrawDashedLine(m_startPos, m_endPos, new Line3.Segment(m_startPos, m_endPos), overlayBuffer, tooltips);
            }
        }

        /// <summary>
        /// Draws point overlays.
        /// </summary>
        /// <param name="overlayBuffer">Overlay buffer.</param>
        public virtual void DrawPointOverlays(OverlayRenderSystem.Buffer overlayBuffer)
        {
            Color softCyan = Color.cyan;
            softCyan.a *= 0.1f;

            overlayBuffer.DrawCircle(Color.cyan, softCyan, 0.3f, 0, new float2(0f, 1f), m_startPos, PointRadius * 2f);
            overlayBuffer.DrawCircle(Color.cyan, softCyan, 0.3f, 0, new float2(0f, 1f), m_endPos, PointRadius * 2f);
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public virtual void Reset()
        {
            m_validStart = false;
        }

        /// <summary>
        /// Checks to see if a click should initiate point dragging.
        /// </summary>
        /// <param name="position">Click position in world space.</param>
        /// <returns>Drag mode.</returns>
        internal virtual DragMode CheckDragHit(float3 position)
        {
            if (math.distancesq(position, m_startPos) < (PointRadius * PointRadius))
            {
                // Start point.
                return DragMode.StartPos;
            }
            else if (math.distancesq(position, m_endPos) < (PointRadius * PointRadius))
            {
                // End point.
                return DragMode.EndPos;
            }

            // No hit.
            return DragMode.None;
        }

        /// <summary>
        /// Handles dragging action.
        /// </summary>
        /// <param name="dragMode">Dragging mode.</param>
        /// <param name="position">New position.</param>
        internal virtual void HandleDrag(DragMode dragMode, float3 position)
        {
            // Drag start point.
            if (dragMode == DragMode.StartPos)
            {
                m_startPos = position;
            }
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
