// <copyright file="LineToolUISystem.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the Apache Licence, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace LineTool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using cohtml.Net;
    using Colossal.Logging;
    using Game.Prefabs;
    using Game.SceneFlow;
    using Game.Tools;
    using Game.UI;
    using Unity.Entities;

    /// <summary>
    /// A tool UI system for LineTool.
    /// </summary>
    public sealed partial class LineToolUISystem : UISystemBase
    {
        // Cached references.
        private View _uiView;
        private ToolSystem _toolSystem;
        private LineToolSystem _lineToolSystem;
        private ILog _log;

        // Internal status.
        private bool _toolIsActive = false;
        private bool _activateTool = false;
        private bool _restorePreviousTool = false;
        private ToolBaseSystem _previousSystem = null;

        // Event binding.
        private List<BoundEventHandle> _eventHandles;

        // UI injection data.
        private string _injectedHTML;
        private string _injectedJS;
        private string _injectedCSS;
        private string _modeHTML;
        private string _modeJS;
        private string _commonJS;

        /// <summary>
        ///  Updates the displayed spacing amount.
        /// </summary>
        internal void UpdateSpacing()
        {
            // Multiply spacing by 10 for accuracy conversion)
            ExecuteScript(_uiView, $"if (typeof(lineTool) == 'object') {{ lineTool.spacing = {_lineToolSystem.Spacing * 10}; if (lineTool.refreshSpacing) lineTool.refreshSpacing();}}");
        }

        /// <summary>
        /// Clears any displayed tooltip.
        /// </summary>
        internal void ClearTooltip()
        {
            ExecuteScript(_uiView, "if (typeof(lineTool) == 'object') {{ lineTool.hideTooltip(); }}");
        }

        /// <summary>
        /// Called when the system is created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            // Set log.
            _log = Mod.Instance.Log;

            // Set references.
            _uiView = GameManager.instance.userInterface.view.View;
            _toolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
            _lineToolSystem = World.GetOrCreateSystemManaged<LineToolSystem>();

            // Read injection data.
            _commonJS = ReadJS("LineToolLite.UI.common.js");
            _modeHTML = ReadHTML("LineToolLite.UI.modes.html", "modeDiv", "if (!document.getElementById(\"line-tool-modes\") && !document.getElementById(\"line-tool-panel\")) { lineTool.modeParent = document.getElementsByClassName(\"tool-options-panel_Se6\"); if (lineTool.modeParent.length != 0) lineTool.modeParent[0].appendChild(lineTool.modeDiv); }");
            _modeJS = ReadJS("LineToolLite.UI.modes.js");
            _injectedHTML = ReadHTML("LineToolLite.UI.ui.html", "div", "lineTool.div.className = \"tool-options-panel_Se6\"; lineTool.div.id = \"line-tool-panel\"; lineTool.targetParent = document.getElementsByClassName(\"tool-side-column_l9i\"); if (lineTool.targetParent.length == 0) lineTool.targetParent = document.getElementsByClassName(\"main_k4u\"); if (lineTool.targetParent.length != 0) lineTool.targetParent[0].appendChild(lineTool.div);");
            _injectedJS = ReadJS("LineToolLite.UI.ui.js");
            _injectedCSS = ReadCSS("LineToolLite.UI.ui.css");

            // Initialize event handle list.
            _eventHandles = new ();

            _toolSystem.EventPrefabChanged = (Action<PrefabBase>)Delegate.Combine(_toolSystem.EventPrefabChanged, new Action<PrefabBase>(OnPrefabChanged));
        }

        /// <summary>
        /// Called every UI update.
        /// </summary>
        protected override void OnUpdate()
        {
            base.OnUpdate();

            // Check for tool activation trigger.
            if (_activateTool)
            {
                // Trigger set - clear it and activate tool.
                _activateTool = false;
                if (_toolSystem.activeTool != _lineToolSystem)
                {
                    _log.Debug("enabling tool");
                    _lineToolSystem.EnableTool();
                    return;
                }
            }

            // Check for previous tool restoration trigger.
            if (_restorePreviousTool)
            {
                // Trigger set - clear it and restore previous tool.
                _restorePreviousTool = false;
                if (_toolSystem.activeTool == _lineToolSystem)
                {
                    _log.Debug("restoring previous tool");
                    _lineToolSystem.RestorePreviousTool();
                    return;
                }
            }

            // Check for line tool activation.
            if (_toolSystem.activeTool == _lineToolSystem)
            {
                // Activate tool.
                if (!_toolIsActive)
                {
                    // Tool is now active but previously wasn't; update previous tool system record.
                    _previousSystem = _lineToolSystem;

                    // Ensure JS setup.
                    ExecuteScript(_uiView, _commonJS);

                    // Set initial rotation and offset variables in UI (multiply distances by 10 for accuracy conversion).
                    ExecuteScript(_uiView, $"lineTool.rotation = {_lineToolSystem.Rotation}; lineTool.randomSpacing = {_lineToolSystem.RandomSpacing * 10}; lineTool.randomOffset = {_lineToolSystem.RandomOffset * 10};");

                    // Attach our custom controls.
                    // Inject scripts.
                    _log.Debug("injecting component data");
                    ExecuteScript(_uiView, _injectedCSS);
                    ExecuteScript(_uiView, _injectedHTML);
                    ExecuteScript(_uiView, _injectedJS);

                    // Determine active tool mode.
                    string modeElement = _lineToolSystem.Mode switch
                    {
                        LineMode.SimpleCurve => "line-tool-simplecurve",
                        LineMode.Circle => "line-tool-circle",
                        _ => "line-tool-straight",
                    };

                    // Select active tool button.
                    ExecuteScript(_uiView, $"document.getElementById(\"{modeElement}\").classList.add(\"selected\");");

                    // Select random rotation button if needed.
                    if (_lineToolSystem.RandomRotation)
                    {
                        ExecuteScript(_uiView, $"document.getElementById(\"line-tool-rotation-random\").classList.add(\"selected\");");

                        // Hide rotation buttons.
                        ExecuteScript(_uiView, "lineTool.setRotationVisibility(false);");
                    }

                    // Select fence mode button if needed and update visibility states.
                    if (_lineToolSystem.CurrentSpacingMode == SpacingMode.FenceMode)
                    {
                        ExecuteScript(_uiView, $"document.getElementById(\"line-tool-fence\").classList.add(\"selected\"); lineTool.setFenceVisibility(false);");
                    }
                    else if (_lineToolSystem.CurrentSpacingMode == SpacingMode.FullLength)
                    {
                        // Otherwise, select fixed-length even spacing button if needed.
                        ExecuteScript(_uiView, $"document.getElementById(\"line-tool-measure-even\").classList.add(\"selected\");");
                    }

                    // Show tree control menu if tree control is active.
                    if (EntityManager.HasComponent<TreeData>(_lineToolSystem.SelectedEntity))
                    {
                        ExecuteScript(_uiView, "lineTool.addTreeControl();");
                    }

                    // Set initial spacing.
                    UpdateSpacing();

                    // Register event callbacks.
                    _eventHandles.Add(_uiView.RegisterForEvent("SetLineToolFenceMode", (Action<bool>)SetFenceMode));
                    _eventHandles.Add(_uiView.RegisterForEvent("SetLineToolSpacing", (Action<float>)SetSpacing));
                    _eventHandles.Add(_uiView.RegisterForEvent("SetLineToolMeasureEven", (Action<bool>)SetFixedLength));
                    _eventHandles.Add(_uiView.RegisterForEvent("SetLineToolRandomRotation", (Action<bool>)SetRandomRotation));
                    _eventHandles.Add(_uiView.RegisterForEvent("SetLineToolRotation", (Action<int>)SetRotation));
                    _eventHandles.Add(_uiView.RegisterForEvent("SetLineToolRandomSpacing", (Action<float>)SetRandomSpacing));
                    _eventHandles.Add(_uiView.RegisterForEvent("SetLineToolRandomOffset", (Action<float>)SetRandomOffset));
                    _eventHandles.Add(_uiView.RegisterForEvent("LineToolTreeControlUpdated", (Action)TreeControlUpdated));
                    _eventHandles.Add(_uiView.RegisterForEvent("SetLineToolSpacing", (Action<float>)SetSpacing));

                    _eventHandles.Add(_uiView.RegisterForEvent("SetPointMode", (Action)SetPointMode));
                    _eventHandles.Add(_uiView.RegisterForEvent("SetStraightMode", (Action)SetStraightMode));
                    _eventHandles.Add(_uiView.RegisterForEvent("SetSimpleCurveMode", (Action)SetSimpleCurveMode));
                    _eventHandles.Add(_uiView.RegisterForEvent("SetCircleMode", (Action)SetCircleMode));

                    // Record current tool state.
                    _toolIsActive = true;
                }
            }
            else
            {
                // Line tool not active - clean up if this is the first update after deactivation.
                if (_toolIsActive)
                {
                    // Remove DOM activation.
                    ExecuteScript(_uiView, "{ let panel = document.getElementById(\"line-tool-panel\"); if (panel) panel.parentElement.removeChild(panel); }");

                    // Remove event callbacks.
                    foreach (BoundEventHandle eventHandle in _eventHandles)
                    {
                        _uiView.UnregisterFromEvent(eventHandle);
                    }

                    // Record current tool state.
                    _toolIsActive = false;
                }
                else
                {
                    // Check to see if another tool change has occurred.
                    if (_toolSystem.activeTool != _previousSystem)
                    {
                        // Active tool has changed - record new tool.
                        _previousSystem = _toolSystem.activeTool;

                        // Check for object tool system activation.
                        if (_previousSystem is ObjectToolSystem)
                        {
                            // Object tool is now active.
                            _log.Debug("object tool system activated");

                            // Attach our custom controls.
                            // Inject scripts.
                            _log.Debug("injecting component data");
                            ExecuteScript(_uiView, _commonJS);
                            ExecuteScript(_uiView, _modeHTML);
                            ExecuteScript(_uiView, _modeJS);

                            _eventHandles.Add(_uiView.RegisterForEvent("SetPointMode", (Action)SetPointMode));
                            _eventHandles.Add(_uiView.RegisterForEvent("SetStraightMode", (Action)SetStraightMode));
                            _eventHandles.Add(_uiView.RegisterForEvent("SetSimpleCurveMode", (Action)SetSimpleCurveMode));
                            _eventHandles.Add(_uiView.RegisterForEvent("SetCircleMode", (Action)SetCircleMode));
                        }
                        else
                        {
                            // Remove any stale modes panel.
                            ExecuteScript(_uiView, "{ let modePanel = document.getElementById(\"line-tool-modes\"); if (modePanel) modePanel.parentElement.removeChild(modePanel); }");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles changes in the selected prefab.
        /// </summary>
        /// <param name="prefab">New selected prefab.</param>
        private void OnPrefabChanged(PrefabBase prefab)
        {
            // If the line tool is currently activated and the new prefab is a placeable object, reactivate it (the game will reset the tool to the relevant object tool).
            if (_toolSystem.activeTool == _lineToolSystem && prefab is StaticObjectPrefab)
            {
                _activateTool = true;
            }
        }

        /// <summary>
        /// Executes JavaScript in the given View.
        /// </summary>
        /// <param name="view"><see cref="View"/> to execute in.</param>
        /// <param name="script">Script to execute.</param>
        private void ExecuteScript(View view, string script)
        {
            // Null check.
            if (!string.IsNullOrEmpty(script))
            {
                view?.ExecuteScript(script);
            }
        }

        /// <summary>
        /// Load CSS from an embedded UI file.
        /// </summary>
        /// <param name="fileName">Embedded UI file name to read.</param>
        /// <returns>JavaScript <see cref="string"/> embedding the CSS (<c>null</c> if empty or error).</returns>
        private string ReadCSS(string fileName)
        {
            try
            {
                // Attempt to read file.
                string css = ReadUIFile(fileName);

                // Don't do anything if file wasn't read.
                if (!string.IsNullOrEmpty(css))
                {
                    // Return JavaScript code with CSS embedded.
                    return $"lineTool.style = document.createElement('style'); lineTool.style.type = 'text/css'; lineTool.style.innerHTML = \"{EscapeToJavaScript(css)}\"; document.head.appendChild(lineTool.style);";
                }
            }
            catch (Exception e)
            {
                _log.Error(e, $"exception reading CSS file {fileName}");
            }

            // If we got here, something went wrong.; return null.
            _log.Error($"failed to read embedded CSS file {fileName}");
            return null;
        }

        /// <summary>
        /// Load HTML from an embedded UI file.
        /// </summary>
        /// <param name="fileName">Embedded UI file name to read.</param>
        /// <param name="variableName">JavaScript variable name to use for the div.</param>
        /// <param name="injectionPostfix">Injection JavaScript postfix text.</param>
        /// <returns>JavaScript <see cref="string"/> embedding the HTML (<c>null</c> if empty or error).</returns>
        private string ReadHTML(string fileName, string variableName, string injectionPostfix)
        {
            try
            {
                // Attempt to read file.
                string html = ReadUIFile(fileName);

                // Don't do anything if file wasn't read.
                if (!string.IsNullOrEmpty(html))
                {
                    // Return JavaScript code with HTML embedded.
                    return $"lineTool.{variableName} = document.createElement('div'); lineTool.{variableName}.innerHTML = \"{EscapeToJavaScript(html)}\"; {injectionPostfix}";
                }
            }
            catch (Exception e)
            {
                _log.Error(e, $"exception reading embedded HTML file {fileName}");
            }

            // If we got here, something went wrong.; return null.
            _log.Error($"failed to read embedded HTML file {fileName}");
            return null;
        }

        /// <summary>
        /// Load JavaScript from an embedded UI file.
        /// </summary>>
        /// <param name="fileName">UI file name to read.</param>
        /// <returns>JavaScript as <see cref="string"/> (<c>null</c> if empty or error).</returns>
        private string ReadJS(string fileName)
        {
            try
            {
                // Attempt to read file.
                string js = ReadUIFile(fileName);

                // Don't do anything if file wasn't read.
                if (!string.IsNullOrEmpty(js))
                {
                    // Return JavaScript code with HTML embedded.
                    return js;
                }
            }
            catch (Exception e)
            {
                _log.Error(e, $"exception reading embedded JavaScript file {fileName}");
            }

            // If we got here, something went wrong; return null.
            _log.Error($"failed to read embedded JavaScript file {fileName}");
            return null;
        }

        /// <summary>
        /// Reads an embedded UI text file.
        /// </summary>
        /// <param name="fileName">Embedded UI file name to read.</param>
        /// <returns>File contents (<c>null</c> if none or error).</returns>
        private string ReadUIFile(string fileName)
        {
            try
            {
                // Read file.
                using Stream embeddedStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
                using StreamReader reader = new (embeddedStream);
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                _log.Error(e, $"exception reading embedded UI file {fileName}");
            }

            return null;
        }

        /// <summary>
        /// Escapes HTML or CSS input for in-lining into JavaScript.
        /// </summary>
        /// <param name="sourceString">HTML source.</param>
        /// <returns>Escaped HTML as <see cref="string"/>.</returns>
        private string EscapeToJavaScript(string sourceString)
        {
            // Create output StringBuilder.
            int length = sourceString.Length;
            StringBuilder stringBuilder = new (length * 2);

            // Iterate through each char.
            int index = -1;
            while (++index < length)
            {
                char ch = sourceString[index];

                // Just skip line breaks.
                if (ch == '\n' || ch == '\r')
                {
                    continue;
                }

                // Escape any double or single quotes.
                if (ch == '"' || ch == '\'')
                {
                    stringBuilder.Append('\\');
                }

                // Add character to output.
                stringBuilder.Append(ch);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Event callback to set fence mode.
        /// </summary>
        /// <param name="isActive">Value to set.</param>
        private void SetFenceMode(bool isActive) => _lineToolSystem.CurrentSpacingMode = isActive ? SpacingMode.FenceMode : SpacingMode.Manual;

        /// <summary>
        /// Event callback to set single item mode.
        /// </summary>
        private void SetPointMode()
        {
            // Restore previously-used tool.
            _restorePreviousTool = true;
        }

        /// <summary>
        /// Event callback to set straight line mode.
        /// </summary>
        private void SetStraightMode()
        {
            // Ensure tool is activated.
            _activateTool = true;
            _lineToolSystem.Mode = LineMode.Straight;
        }

        /// <summary>
        /// Event callback to set simple curve mode.
        /// </summary>
        private void SetSimpleCurveMode()
        {
            // Ensure tool is activated.
            _activateTool = true;
            _lineToolSystem.Mode = LineMode.SimpleCurve;
        }

        /// <summary>
        /// Event callback to set circle mode.
        /// </summary>
        private void SetCircleMode()
        {
            // Ensure tool is activated.
            _activateTool = true;
            _lineToolSystem.Mode = LineMode.Circle;
        }

        /// <summary>
        /// Event callback to set current spacing.
        /// </summary>
        /// <param name="spacing">Value to set.</param>
        private void SetSpacing(float spacing) => _lineToolSystem.Spacing = spacing;

        /// <summary>
        /// Event callback to set fixed-length even spacing mode.
        /// </summary>
        /// <param name="isActive">Value to set.</param>
        private void SetFixedLength(bool isActive) => _lineToolSystem.CurrentSpacingMode = isActive ? SpacingMode.FullLength : SpacingMode.Manual;

        /// <summary>
        /// Event callback to set the random rotation override.
        /// </summary>
        /// <param name="isRandom">Value to set.</param>
        private void SetRandomRotation(bool isRandom) => _lineToolSystem.RandomRotation = isRandom;

        /// <summary>
        /// Event callback to set current rotation.
        /// </summary>
        /// <param name="rotation">Value to set.</param>
        private void SetRotation(int rotation) => _lineToolSystem.Rotation = rotation;

        /// <summary>
        /// Event callback to set the random spacing offset maximum.
        /// </summary>
        /// <param name="randomSpacing">Value to set.</param>
        private void SetRandomSpacing(float randomSpacing) => _lineToolSystem.RandomSpacing = randomSpacing;

        /// <summary>
        /// Event callback to set the random lateral offset maximum.
        /// </summary>
        /// <param name="randomOffset">Value to set.</param>
        private void SetRandomOffset(float randomOffset) => _lineToolSystem.RandomOffset = randomOffset;

        /// <summary>
        /// Event callback to update Tree Control settings.
        /// </summary>
        private void TreeControlUpdated() => _lineToolSystem.RefreshTreeControl();
    }
}
