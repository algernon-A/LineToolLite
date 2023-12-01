// <copyright file="ui.js" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// </copyright>

// Function to adjust spacing.
if (typeof lineTool.adjustSpacing !== 'function') {
    lineTool.adjustSpacing = function(event, adjustment) {
        // Adjust for modifier keys - multiplying adjustment by 10 for FP rounding.
        var finalAdjustment = adjustment;
        if (event) {
            if (event.shiftKey)
                finalAdjustment *= 100;
            else if (!event.ctrlKey)
                finalAdjustment *= 10;
        }

        // Don't apply if adjutment will bring us below zero.
        newSpacing = lineTool.spacing + finalAdjustment;
        if (newSpacing < 1) return;

        // Apply spacing.
        lineTool.spacing = newSpacing;
        var roundedSpacing = newSpacing / 10;
        engine.trigger('SetLineToolSpacing', roundedSpacing);
        document.getElementById("line-tool-spacing-field").innerHTML = roundedSpacing + " m";
    }
}

// Function to adjust rotation.
if (typeof lineTool.adjustRotation !== 'function') {
    lineTool.adjustRotation = function(event, adjustment) {
        // Adjust for modifier keys.
        let finalAdjustment = adjustment;
        if (event) {
            if (event.shiftKey)
                finalAdjustment *= 90;
            else if (!event.ctrlKey)
                finalAdjustment *= 10;
        }

        // Bounds check rotation.
        lineTool.rotation += finalAdjustment;
        if (lineTool.rotation >= 360) {
            lineTool.rotation -= 360;
        }
        if (lineTool.rotation < 0) {
            lineTool.rotation += 360;
        }

        // Apply rotation.
        engine.trigger('SetLineToolRotation', lineTool.rotation);
        document.getElementById("line-tool-rotation-field").innerHTML = lineTool.rotation + "&deg;";
    }
}

// Function to implement random rotation selection.
if (typeof lineTool.randomRotation !== 'function') {
    lineTool.randomRotation = function() {
        var adjustRotationButton = document.getElementById("line-tool-rotation-random");
        if (adjustRotationButton.classList.contains("selected")) {
            adjustRotationButton.classList.remove("selected");
            engine.trigger('SetLineToolRandomRotation', false);

            // Show rotation tools.
            lineTool.setRotationVisibility(true);
        }
        else {
            adjustRotationButton.classList.add("selected");
            engine.trigger('SetLineToolRandomRotation', true);

            // Hide rotation tools.
            lineTool.setRotationVisibility(false);
        }
    }
}

// Function to show the Tree Control age panel.
if (typeof lineTool.addTreeControl !== 'function') {
    lineTool.addTreeControl = function(event, adjustment) {
        if (typeof buildTreeAgeItem == 'function') {
            var modeLine = document.getElementById("line-tool-mode");
            buildTreeAgeItem(modeLine, "afterend");
            document.getElementById("YYTC-change-age-buttons-panel").onclick = function () { engine.trigger('LineToolTreeControlUpdated') };
        }
    }
}

// Function to activate straight mode.
if (typeof lineTool.handleStraightMode !== 'function') {
    lineTool.handleStraightMode = function() {
        document.getElementById("line-tool-simplecurve").classList.remove("selected");
        document.getElementById("line-tool-circle").classList.remove("selected");
        document.getElementById("line-tool-straight").classList.add("selected");
        engine.trigger('SetStraightMode');
    }
}

// Function to activate simple curve mode.
if (typeof lineTool.handleSimpleCurveMode !== 'function') {
    lineTool.handleSimpleCurveMode = function() {
        document.getElementById("line-tool-straight").classList.remove("selected");
        document.getElementById("line-tool-circle").classList.remove("selected");
        document.getElementById("line-tool-simplecurve").classList.add("selected");
        engine.trigger('SetSimpleCurveMode');
    }
}

// Function to activate circle mode.
if (typeof lineTool.handleCircleMode !== 'function') {
    lineTool.handleCircleMode = function() {
        document.getElementById("line-tool-straight").classList.remove("selected");
        document.getElementById("line-tool-simplecurve").classList.remove("selected");
        document.getElementById("line-tool-circle").classList.add("selected");
        engine.trigger('SetCircleMode');
    }
}

// Function to set rotation selection control visibility
if (typeof lineTool.setRotationVisibility !== 'function') {
    lineTool.setRotationVisibility = function(isVisible) {
        lineTool.setButtonVisibility(document.getElementById("line-tool-rotation-up"), isVisible);
        lineTool.setButtonVisibility(document.getElementById("line-tool-rotation-down"), isVisible);
        if (isVisible) {
            document.getElementById("line-tool-rotation-field").style.visibility = "visible";
        }
        else {
            document.getElementById("line-tool-rotation-field").style.visibility = "hidden";
        }
    }
}

// Function to set the visibility status of a button with icon child.
if (typeof lineTool.setButtonVisibility !== 'function') {
    lineTool.setButtonVisibility = function(button, isVisible) {
        var firstChild = button.firstChild;
        if (isVisible) {
            button.classList.remove("hidden");
            firstChild.classList.remove("hidden");
            firstChild.style.display = "inline";
        }
        else {
            button.classList.add("hidden");
            firstChild.classList.add("hidden");
            firstChild.style.display = "none";
        }
    }
}


// Set initial figures.
lineTool.adjustSpacing(null, 0);
lineTool.adjustRotation(null, 0);

// Add button event handlers.
document.getElementById("line-tool-spacing-down").onmousedown = (event) => { lineTool.adjustSpacing(event, -1); }
document.getElementById("line-tool-spacing-up").onmousedown = (event) => { lineTool.adjustSpacing(event, 1); }

document.getElementById("line-tool-rotation-random").onmousedown = () => { lineTool.randomRotation(); }
document.getElementById("line-tool-rotation-up").onmousedown = (event) => { lineTool.adjustRotation(event, 1); }
document.getElementById("line-tool-rotation-down").onmousedown = (event) => { lineTool.adjustRotation(event, -1); }

document.getElementById("line-tool-straight").onclick = lineTool.handleStraightMode;
document.getElementById("line-tool-simplecurve").onclick = lineTool.handleSimpleCurveMode;
document.getElementById("line-tool-circle").onclick = lineTool.handleCircleMode;