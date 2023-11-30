// <copyright file="StraightLine.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// </copyright>

namespace LineTool
{
    /// <summary>
    /// Straight-line placement mode.
    /// </summary>
    public class StraightLine : LineBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StraightLine"/> class.
        /// </summary>
        public StraightLine()
        {
            // Basic state.
            m_validStart = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StraightLine"/> class.
        /// </summary>
        /// <param name="mode">Mode to copy starting state from.</param>
        public StraightLine(LineBase mode)
            : base(mode)
        {
        }
    }
}
