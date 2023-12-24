// Function to activate point mode.
if (typeof lineTool.handlePointMode !== 'function') {
    lineTool.handlePointMode = function () {
        document.getElementById("line-tool-mode-simplecurve").classList.remove("selected");
        document.getElementById("line-tool-mode-circle").classList.remove("selected");
        document.getElementById("line-tool-mode-straight").classList.remove("selected");
        document.getElementById("line-tool-mode-point").classList.add("selected");
        engine.trigger('SetPointMode');
    }
}

// Function to activate straight mode.
if (typeof lineTool.handleStraightMode !== 'function') {
    lineTool.handleStraightMode = function () {
        document.getElementById("line-tool-mode-point").classList.remove("selected");
        document.getElementById("line-tool-mode-simplecurve").classList.remove("selected");
        document.getElementById("line-tool-mode-circle").classList.remove("selected");
        document.getElementById("line-tool-mode-straight").classList.add("selected");
        engine.trigger('SetStraightMode');
    }
}

// Function to activate simple curve mode.
if (typeof lineTool.handleSimpleCurveMode !== 'function') {
    lineTool.handleSimpleCurveMode = function () {
        document.getElementById("line-tool-mode-point").classList.remove("selected");
        document.getElementById("line-tool-mode-straight").classList.remove("selected");
        document.getElementById("line-tool-mode-circle").classList.remove("selected");
        document.getElementById("line-tool-mode-simplecurve").classList.add("selected");
        engine.trigger('SetSimpleCurveMode');
    }
}

// Function to activate circle mode.
if (typeof lineTool.handleCircleMode !== 'function') {
    lineTool.handleCircleMode = function () {
        document.getElementById("line-tool-mode-point").classList.remove("selected");
        document.getElementById("line-tool-mode-straight").classList.remove("selected");
        document.getElementById("line-tool-mode-simplecurve").classList.remove("selected");
        document.getElementById("line-tool-mode-circle").classList.add("selected");
        engine.trigger('SetCircleMode');
    }
}

// Add button event handlers.
lineTool.setupClickButton("line-tool-mode-point", lineTool.handlePointMode, "PointMode");
lineTool.setupClickButton("line-tool-mode-straight", lineTool.handleStraightMode, "StraightLine");
lineTool.setupClickButton("line-tool-mode-simplecurve", lineTool.handleSimpleCurveMode, "SimpleCurve");
lineTool.setupClickButton("line-tool-mode-circle", lineTool.handleCircleMode, "Circle");
