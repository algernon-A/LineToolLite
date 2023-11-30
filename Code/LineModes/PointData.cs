// <copyright file="PointData.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
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
