// <copyright file="ui.js" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// </copyright>

// Function to adjust spacing.
if (typeof lineToolAdjustSpacing !== 'function') {
    function lineToolAdjustSpacing(event, adjustment) {
        // Adjust for modifier keys - multiplying adjustment by 10 for FP rounding.
        var finalAdjustment = adjustment;
        if (event) {
            if (event.shiftKey)
                finalAdjustment *= 100;
            else if (!event.ctrlKey)
                finalAdjustment *= 10;
        }

        // Don't apply if adjutment will bring us below zero.
        newSpacing = lineToolSpacing + finalAdjustment;
        if (newSpacing < 1) return;

        // Apply spacing.
        lineToolSpacing = newSpacing;
        var roundedSpacing = newSpacing / 10;
        engine.trigger('SetLineToolSpacing', roundedSpacing);
        document.getElementById("line-tool-spacing-field").innerHTML = roundedSpacing + " m";
    }
}

// Function to adjust rotation.
if (typeof lineToolAdjustRotation !== 'function') {
    function lineToolAdjustRotation(event, adjustment) {
        // Adjust for modifier keys.
        var finalAdjustment = adjustment;
        if (event) {
            if (event.shiftKey)
                finalAdjustment *= 90;
            else if (!event.ctrlKey)
                finalAdjustment *= 10;
        }

        // Bounds check rotation.
        lineToolRotation += finalAdjustment;
        if (lineToolRotation >= 360) {
            lineToolRotation -= 360;
        }
        if (lineToolRotation < 0) {
            lineToolRotation += 360;
        }

        // Apply rotation.
        engine.trigger('SetLineToolRotation', lineToolRotation);
        document.getElementById("line-tool-rotation-field").innerHTML = lineToolRotation + "&deg;";
    }
}

// Function to implement random rotation selection.
if (typeof lineToolRandomRotation !== 'function') {
    function lineToolRandomRotation() {
        var adjustRotationButton = document.getElementById("line-tool-rotation-random");
        if (adjustRotationButton.classList.contains("selected")) {
            adjustRotationButton.classList.remove("selected");
            engine.trigger('SetLineToolRandomRotation', false);

            // Show rotation tools.
            lineToolSetRotationVisibility(true);
        }
        else {
            adjustRotationButton.classList.add("selected");
            engine.trigger('SetLineToolRandomRotation', true);

            // Hide rotation tools.
            lineToolSetRotationVisibility(false);
        }
    }
}

// Function to show the Tree Control age panel.
if (typeof addLineToolTreeControl !== 'function') {
    function addLineToolTreeControl(event, adjustment) {
        if (typeof buildTreeAgeItem == 'function') {
            var modeLine = document.getElementById("line-tool-mode");
            buildTreeAgeItem(modeLine, "afterend");
            document.getElementById("YYTC-change-age-buttons-panel").onclick = function () { engine.trigger('LineToolTreeControlUpdated') };
        }
    }
}

// Function to activate straight mode.
if (typeof lineToolHandleStraightMode !== 'function') {
    function lineToolHandleStraightMode() {
        document.getElementById("line-tool-simplecurve").classList.remove("selected");
        document.getElementById("line-tool-circle").classList.remove("selected");
        document.getElementById("line-tool-straight").classList.add("selected");
        engine.trigger('SetStraightMode');
    }
}

// Function to activate simple curve mode.
if (typeof lineToolHandleSimpleCurveMode !== 'function') {
    function lineToolHandleSimpleCurveMode() {
        document.getElementById("line-tool-straight").classList.remove("selected");
        document.getElementById("line-tool-circle").classList.remove("selected");
        document.getElementById("line-tool-simplecurve").classList.add("selected");
        engine.trigger('SetSimpleCurveMode');
    }
}

// Function to activate circle mode.
if (typeof lineToolHandleCircleMode !== 'function') {
    function lineToolHandleCircleMode() {
        document.getElementById("line-tool-straight").classList.remove("selected");
        document.getElementById("line-tool-simplecurve").classList.remove("selected");
        document.getElementById("line-tool-circle").classList.add("selected");
        engine.trigger('SetCircleMode');
    }
}

// Function to set rotation selection control visibility
if (typeof lineToolSetRotationVisibility !== 'function') {
    function lineToolSetRotationVisibility(isVisible) {
        lineToolSetButtonVisibility(document.getElementById("line-tool-rotation-up"), isVisible);
        lineToolSetButtonVisibility(document.getElementById("line-tool-rotation-down"), isVisible);
        if (isVisible) {
            document.getElementById("line-tool-rotation-field").style.visibility = "visible";
        }
        else {
            document.getElementById("line-tool-rotation-field").style.visibility = "hidden";
        }
    }
}

// Function to set the visibility status of a button with icon child.
if (typeof lineToolSetButtonVisibility !== 'function') {
    function lineToolSetButtonVisibility(button, isVisible) {
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
lineToolAdjustSpacing(null, 0);
lineToolAdjustRotation(null, 0);

// Add button event handlers.
document.getElementById("line-tool-spacing-down").onmousedown = (event) => { lineToolAdjustSpacing(event, -1); }
document.getElementById("line-tool-spacing-up").onmousedown = (event) => { lineToolAdjustSpacing(event, 1); }

document.getElementById("line-tool-rotation-random").onmousedown = () => { lineToolRandomRotation(); }
document.getElementById("line-tool-rotation-up").onmousedown = (event) => { lineToolAdjustRotation(event, 1); }
document.getElementById("line-tool-rotation-down").onmousedown = (event) => { lineToolAdjustRotation(event, -1); }

document.getElementById("line-tool-straight").onclick = lineToolHandleStraightMode;
document.getElementById("line-tool-simplecurve").onclick = lineToolHandleSimpleCurveMode;
document.getElementById("line-tool-circle").onclick = lineToolHandleCircleMode;
