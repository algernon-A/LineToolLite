// <copyright file="Plugin.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// </copyright>

namespace LineTool
{
    using System.Reflection;
    using BepInEx;
    using Game;
    using Game.Common;
    using Game.SceneFlow;
    using HarmonyLib;

    /// <summary>
    /// BepInEx plugin to substitute for IMod support.
    /// </summary>
    [BepInPlugin(GUID, "Line Tool Lite", "1.0.9")]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        /// <summary>
        /// Plugin unique GUID.
        /// </summary>
        public const string GUID = "com.github.algernon-A.CS2.LineToolLite";

        // IMod instance reference.
        private Mod _mod;

        /// <summary>
        /// Called when the plugin is loaded.
        /// </summary>
        public void Awake()
        {
            // Ersatz IMod.OnLoad().
            _mod = new ();
            _mod.OnLoad();

            _mod.Log.Info("Plugin.Awake");

            // Apply Harmony patches.
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
        }

        /// <summary>
        /// Harmony postfix to <see cref="SystemOrder.Initialize"/> to substitute for IMod.OnCreateWorld.
        /// </summary>
        /// <param name="updateSystem"><see cref="GameManager"/> <see cref="UpdateSystem"/> instance.</param>
        [HarmonyPatch(typeof(SystemOrder), nameof(SystemOrder.Initialize))]
        [HarmonyPostfix]
        private static void InjectSystems(UpdateSystem updateSystem) => Mod.Instance.OnCreateWorld(updateSystem);
    }
}
