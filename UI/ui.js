// <copyright file="ui.js" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// </copyright>


// Function to apply modifiers to distance adjustments.
if (typeof lineTool.adjustDistance != 'function') {
    lineTool.adjustDistance = function (event, adjustment) {

        // Adjust for modifier keys.
        let finalAdjustment = adjustment;
        if (event) {
            if (event.shiftKey)
                finalAdjustment *= 90;
            else if (!event.ctrlKey)
                finalAdjustment *= 10;
        }

        return finalAdjustment;
    }
}

// Function to implement fence mode selection.
if (typeof lineTool.fenceMode !== 'function') {
    lineTool.fenceMode = function () {
        let fenceModeButton = document.getElementById("line-tool-fence");
        let activating = !fenceModeButton.classList.contains("selected");
        if (activating) {
            fenceModeButton.classList.add("selected");

            // Deselect random rotation.
            document.getElementById("line-tool-rotation-random").classList.remove("selected");
            engine.trigger('SetLineToolRandomRotation', false);
            lineTool.setRotationVisibility(true);
        }
        else {
            fenceModeButton.classList.remove("selected");
        }

        // Update control visibility.
        lineTool.setFenceVisibility(!activating);
        engine.trigger('SetLineToolFenceMode', activating);
    }
}

// Function to toggle visibility of controls based on fence mode state.
if (typeof lineTool.setFenceVisibility !== 'function') {
    lineTool.setFenceVisibility = function (isVisible) {
        lineTool.setDivVisiblity(isVisible, "line-tool-spacing");
        lineTool.setDivVisiblity(isVisible, "line-tool-rotation");
        lineTool.setDivVisiblity(isVisible, "line-tool-rotation-field");
        lineTool.setDivVisiblity(isVisible, "line-tool-offsets");
    }
}

// Function to adjust spacing.
if (typeof lineTool.adjustSpacing !== 'function') {
    lineTool.adjustSpacing = function (event, adjustment) {
        // Adjust for modifiers.
        let finalAdjustment = lineTool.adjustDistance(event, adjustment);

        // Don't apply if adjutment will bring us below zero.
        newSpacing = lineTool.spacing + finalAdjustment;
        if (newSpacing < 1) return;

        // Apply spacing.
        lineTool.spacing = newSpacing;
        let roundedSpacing = newSpacing / 10;
        engine.trigger('SetLineToolSpacing', roundedSpacing);
        document.getElementById("line-tool-spacing-field").innerHTML = roundedSpacing + " m";
    }
}

// Function to update displayed spacing.
if (typeof lineTool.refreshSpacing !== 'function') {
    lineTool.refreshSpacing = function () {
        if (lineTool.spacing == null) {
            return;
        }

        let spacingField = document.getElementById("line-tool-spacing-field");
        if (spacingField != null) {
            document.getElementById("line-tool-spacing-field").innerHTML = (lineTool.spacing / 10) + " m";
        }
    }
}

// Function to implement fixed-length even spacing.
if (typeof lineTool.measureEven !== 'function') {
    lineTool.measureEven = function () {
        let measureEvenButton = document.getElementById("line-tool-measure-even");
        if (measureEvenButton.classList.contains("selected")) {
            measureEvenButton.classList.remove("selected");
            engine.trigger('SetLineToolMeasureEven', false);
        }
        else {
            measureEvenButton.classList.add("selected");
            engine.trigger('SetLineToolMeasureEven', true);
        }
    }
}

// Function to implement random rotation selection.
if (typeof lineTool.randomRotation !== 'function') {
    lineTool.randomRotation = function () {
        let randomRotationButton = document.getElementById("line-tool-rotation-random");
        if (randomRotationButton.classList.contains("selected")) {
            randomRotationButton.classList.remove("selected");
            engine.trigger('SetLineToolRandomRotation', false);

            // Show rotation tools.
            lineTool.setRotationVisibility(true);
        }
        else {
            randomRotationButton.classList.add("selected");
            engine.trigger('SetLineToolRandomRotation', true);

            // Hide rotation tools.
            lineTool.setRotationVisibility(false);
        }
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

// Function to adjust random spacing offset.
if (typeof lineTool.adjustRandomSpacing !== 'function') {
    lineTool.adjustRandomSpacing = function (event, adjustment) {
        // Adjust for modifiers.
        let finalAdjustment = lineTool.adjustDistance(event, adjustment);

        // Bounds check.
        lineTool.randomSpacing += finalAdjustment;
        let maxSpacing = Math.round((lineTool.spacing / 3) - 1);
        if (lineTool.randomSpacing > maxSpacing) {
            lineTool.randomSpacing = maxSpacing;
        }
        if (lineTool.randomSpacing < 0) {
            lineTool.randomSpacing = 0;
        }

        // Apply spacing offset.
        engine.trigger('SetLineToolRandomSpacing', lineTool.randomSpacing / 10);
        document.getElementById("line-tool-xOffset-field").innerHTML = (lineTool.randomSpacing / 10) + " m";
    }
}

// Function to adjust random lateral offset.
if (typeof lineTool.adjustRandomOffset !== 'function') {
    lineTool.adjustRandomOffset = function (event, adjustment) {
        // Adjust for modifiers.
        let finalAdjustment = lineTool.adjustDistance(event, adjustment);

        // Bounds check.
        lineTool.randomOffset += finalAdjustment;
        if (lineTool.randomOffset > 1000) {
            lineTool.randomOffset = 1000;
        }
        if (lineTool.randomOffset < 0) {
            lineTool.randomOffset = 0;
        }

        // Apply spacing offset.
        engine.trigger('SetLineToolRandomOffset', lineTool.randomOffset / 10);
        document.getElementById("line-tool-zOffset-field").innerHTML = (lineTool.randomOffset / 10) + " m";
    }
}

// Function to show the Tree Control age panel.
if (typeof lineTool.addTreeControl !== 'function') {
    lineTool.addTreeControl = function(event, adjustment) {
        if (typeof buildTreeAgeItem == 'function') {
            let modeLine = document.getElementById("line-tool-mode");
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

// Function to set div visibility
if (typeof lineTool.setDivVisiblity !== 'function') {
    lineTool.setDivVisiblity = function (isVisible, divId) {
        if (isVisible) {
            document.getElementById(divId).style.visibility = "visible";
        }
        else {
            document.getElementById(divId).style.visibility = "hidden";
        }
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
lineTool.adjustRandomOffset(null, 0);
lineTool.adjustRandomSpacing(null, 0);

// Add button event handlers.
document.getElementById("line-tool-fence").onmousedown = () => { lineTool.fenceMode(); }
document.getElementById("line-tool-straight").onclick = lineTool.handleStraightMode;
document.getElementById("line-tool-simplecurve").onclick = lineTool.handleSimpleCurveMode;
document.getElementById("line-tool-circle").onclick = lineTool.handleCircleMode;

document.getElementById("line-tool-measure-even").onmousedown = () => { lineTool.measureEven(); }
document.getElementById("line-tool-spacing-down").onmousedown = (event) => { lineTool.adjustSpacing(event, -1); }
document.getElementById("line-tool-spacing-up").onmousedown = (event) => { lineTool.adjustSpacing(event, 1); }

document.getElementById("line-tool-rotation-random").onmousedown = () => { lineTool.randomRotation(); }
document.getElementById("line-tool-rotation-up").onmousedown = (event) => { lineTool.adjustRotation(event, 1); }
document.getElementById("line-tool-rotation-down").onmousedown = (event) => { lineTool.adjustRotation(event, -1); }

document.getElementById("line-tool-xOffset-down").onmousedown = (event) => { lineTool.adjustRandomSpacing(event, -1); }
document.getElementById("line-tool-xOffset-up").onmousedown = (event) => { lineTool.adjustRandomSpacing(event, 1); }
document.getElementById("line-tool-zOffset-down").onmousedown = (event) => { lineTool.adjustRandomOffset(event, -1); }
document.getElementById("line-tool-zOffset-up").onmousedown = (event) => { lineTool.adjustRandomOffset(event, 1); }