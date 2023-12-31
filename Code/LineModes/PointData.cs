﻿// <copyright file="PointData.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the Apache Licence, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace LineTool
{
    using Unity.Mathematics;

    /// <summary>
    /// Data struct for calculated point.
    /// </summary>
    public struct PointData
    {
        /// <summary>
        /// Point location.
        /// </summary>
        public float3 Position;

        /// <summary>
        /// Point rotation.
        /// </summary>
        public quaternion Rotation;
    }
}
