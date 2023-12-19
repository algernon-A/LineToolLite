// <copyright file="LineToolTooltipSystem.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the Apache Licence, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace LineTool
{
    using System.Collections.Generic;
    using Game.UI.Tooltip;
    using Game.UI.Widgets;
    using Unity.Collections;
    using Unity.Mathematics;
    using static Game.Rendering.GuideLinesSystem;

    /// <summary>
    /// The Line Tool tooltip system.
    /// </summary>
    public partial class LineToolTooltipSystem : TooltipSystemBase
    {
        private LineToolSystem _lineToolSystem;
        private List<TooltipGroup> _tooltipGroups;

        /// <summary>
        /// Called when the system is created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            _lineToolSystem = World.GetOrCreateSystemManaged<LineToolSystem>();
            _tooltipGroups = new List<TooltipGroup>();
        }

        /// <summary>
        /// Called every tool update.
        /// </summary>
        protected override void OnUpdate()
        {
            // Iterate through all tooltips in buffer.
            NativeList<TooltipInfo> tooltips = _lineToolSystem.Tooltips;
            for (int i = 0; i < tooltips.Length; ++i)
            {
                // Create new tooltip template and add to list if needed.
                TooltipInfo tooltipInfo = tooltips[i];
                if (_tooltipGroups.Count <= i)
                {
                    _tooltipGroups.Add(new TooltipGroup
                    {
                        path = $"guideLineTooltip{i}",
                        horizontalAlignment = TooltipGroup.Alignment.Center,
                        verticalAlignment = TooltipGroup.Alignment.Center,
                        children = { (IWidget)new IntTooltip() },
                    });
                }

                // Set tooltip position.
                TooltipGroup tooltipGroup = _tooltipGroups[i];
                float2 tooltipPos = TooltipSystemBase.WorldToTooltipPos(tooltipInfo.m_Position);
                if (!tooltipGroup.position.Equals(tooltipPos))
                {
                    tooltipGroup.position = tooltipPos;
                    tooltipGroup.SetChildrenChanged();
                }

                // Set tooltip content.
                IntTooltip intTooltip = tooltipGroup.children[0] as IntTooltip;
                switch (tooltipInfo.m_Type)
                {
                    case TooltipType.Angle:
                        intTooltip.icon = "Media/Glyphs/Angle.svg";
                        intTooltip.value = tooltipInfo.m_IntValue;
                        intTooltip.unit = "angle";
                        break;
                    case TooltipType.Length:
                        intTooltip.icon = "Media/Glyphs/Length.svg";
                        intTooltip.value = tooltipInfo.m_IntValue;
                        intTooltip.unit = "length";
                        break;
                }

                // Add tooltop group. to UI.
                AddGroup(tooltipGroup);
            }
        }
    }
}
