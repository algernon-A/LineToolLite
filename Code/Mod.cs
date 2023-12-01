// <copyright file="Mod.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// </copyright>

namespace LineTool
{
    using System.IO;
    using Colossal.Logging;
    using Game;
    using Game.Modding;
    using Game.SceneFlow;
    using Game.UI;

    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public sealed class Mod : IMod
    {
        /// <summary>
        /// The mod's default name.
        /// </summary>
        public const string ModName = "Line Tool";

        /// <summary>
        /// Gets the active instance reference.
        /// </summary>
        public static Mod Instance { get; private set; }

        /// <summary>
        /// Gets the mod's active log.
        /// </summary>
        internal ILog Log { get; private set; }

        /// <summary>
        /// Called by the game when the mod is loaded.
        /// </summary>
        public void OnLoad()
        {
            // Set instance reference.
            Instance = this;

            // Initialize logger.
            Log = LogManager.GetLogger(ModName);
            Log.Info("setting logging level to Debug");
            Log.effectivenessLevel = Level.Debug;

            Log.Info("loading");
        }

        /// <summary>
        /// Called by the game when the game world is created.
        /// </summary>
        /// <param name="updateSystem">Game update system.</param>
        public void OnCreateWorld(UpdateSystem updateSystem)
        {
            Log.Info("starting OnCreateWorld");

            // Activate systems.
            updateSystem.UpdateAt<LineToolSystem>(SystemUpdatePhase.ToolUpdate);
            updateSystem.UpdateAfter<LineToolUISystem>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAt<LineToolTooltipSystem>(SystemUpdatePhase.UITooltip);

            // Add mod UI icons to UI resource handler.
            GameUIResourceHandler uiResourceHandler = GameManager.instance.userInterface.view.uiSystem.resourceHandler as GameUIResourceHandler;
            uiResourceHandler?.HostLocationsMap.Add("linetool", new System.Collections.Generic.List<string> { Path.GetDirectoryName(typeof(Plugin).Assembly.Location) + "/" });
        }

        /// <summary>
        /// Called by the game when the mod is disposed of.
        /// </summary>
        public void OnDispose()
        {
            Log.Info("disposing");
            Instance = null;
        }
    }
}
