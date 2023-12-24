// <copyright file="ui.js" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the Apache Licence, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// See LICENSE.txt file in the project root for full license information.
// </copyright>


// Function to activate point mode.
if (typeof lineTool.handlePoint !== 'function') {
    lineTool.handlePoint = function () {
        document.getElementById("line-tool-simplecurve").classList.remove("selected");
        document.getElementById("line-tool-circle").classList.remove("selected");
        document.getElementById("line-tool-straight").classList.remove("selected");
        document.getElementById("line-tool-point").classList.add("selected");
        engine.trigger('SetPointMode');
    }
}

// Function to activate straight mode.
if (typeof lineTool.handleStraight !== 'function') {
    lineTool.handleStraight = function () {
        document.getElementById("line-tool-point").classList.remove("selected");
        document.getElementById("line-tool-simplecurve").classList.remove("selected");
        document.getElementById("line-tool-circle").classList.remove("selected");
        document.getElementById("line-tool-straight").classList.add("selected");
        engine.trigger('SetStraightMode');
    }
}

// Function to activate simple curve mode.
if (typeof lineTool.handleSimpleCurve !== 'function') {
    lineTool.handleSimpleCurve = function () {
        document.getElementById("line-tool-point").classList.remove("selected");
        document.getElementById("line-tool-straight").classList.remove("selected");
        document.getElementById("line-tool-circle").classList.remove("selected");
        document.getElementById("line-tool-simplecurve").classList.add("selected");
        engine.trigger('SetSimpleCurveMode');
    }
}

// Function to activate circle mode.
if (typeof lineTool.handleCircle !== 'function') {
    lineTool.handleCircle = function () {
        document.getElementById("line-tool-point").classList.remove("selected");
        document.getElementById("line-tool-straight").classList.remove("selected");
        document.getElementById("line-tool-simplecurve").classList.remove("selected");
        document.getElementById("line-tool-circle").classList.add("selected");
        engine.trigger('SetCircleMode');
    }
}

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
    lineTool.addTreeControl = function (event, adjustment) {
        try {
            if (typeof yyTreeController != 'undefined' && typeof yyTreeController.buildTreeAgeItem == 'function') {
                let modeLine = document.getElementById("line-tool-mode");
                yyTreeController.buildTreeAgeItem(modeLine, "afterend");
                document.getElementById("YYTC-change-age-buttons-panel").onclick = function () { engine.trigger('LineToolTreeControlUpdated') };
            }
        }
        catch {
            // Don't do anything.
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

// Set initial figures.
lineTool.adjustSpacing(null, 0);
lineTool.adjustRotation(null, 0);
lineTool.adjustRandomOffset(null, 0);
lineTool.adjustRandomSpacing(null, 0);

// Add button event handlers.
lineTool.setupClickButton("line-tool-point", lineTool.handlePoint, "PointMode");
lineTool.setupClickButton("line-tool-straight", lineTool.handleStraight, "StraightLine");
lineTool.setupClickButton("line-tool-simplecurve", lineTool.handleSimpleCurve, "SimpleCurve");
lineTool.setupClickButton("line-tool-circle", lineTool.handleCircle, "Circle");

lineTool.setupClickButton("line-tool-fence", lineTool.fenceMode, "FenceMode");

lineTool.setupClickButton("line-tool-measure-even", lineTool.measureEven, "FixedLength");
lineTool.setupClickButton("line-tool-rotation-random", lineTool.randomRotation, "RandomRotation");

lineTool.setupClickButton("line-tool-spacing-down", (event) => { lineTool.adjustSpacing(event, -1); }, "SpacingDown");
lineTool.setupClickButton("line-tool-spacing-up", (event) => { lineTool.adjustSpacing(event, 1); }, "SpacingUp");
lineTool.setupClickButton("line-tool-rotation-down", (event) => { lineTool.adjustRotation(event, -1); }, "AntiClockwise");
lineTool.setupClickButton("line-tool-rotation-up", (event) => { lineTool.adjustRotation(event, 1); }, "Clockwise");

lineTool.setupClickButton("line-tool-xOffset-down", (event) => { lineTool.adjustRandomSpacing(event, -1); }, "RandomSpacingDown");
lineTool.setupClickButton("line-tool-xOffset-up", (event) => { lineTool.adjustRandomSpacing(event, 1); }, "RandomSpacingUp");
lineTool.setupClickButton("line-tool-zOffset-down", (event) => { lineTool.adjustRandomOffset(event, -1); }, "RandomOffsetDown");
lineTool.setupClickButton("line-tool-zOffset-up", (event) => { lineTool.adjustRandomOffset(event, 1); }, "RandomOffsetUp");

lineTool.setTooltip("line-tool-spacing-field", "Spacing");
lineTool.setTooltip("line-tool-rotation-field", "Rotation");
lineTool.setTooltip("line-tool-xOffset-field", "SpacingVariation");
lineTool.setTooltip("line-tool-zOffset-field", "OffsetVariation");

// Apply translations.
lineTool.applyLocalization(lineTool.div);

// Clear any stale tooltip reference.
lineTool.tooltip = null;

// Hide any existing modes panel.
if (document.getElementById("line-tool-modes")) {
    let modePanel = document.getElementById("line-tool-modes");
    if (modePanel.parentElement) {
        modePanel.parentElement.removeChild(modePanel);
    }
}

// Set panel additional class for editor.
if (document.getElementsByClassName("editor-main-screen_m89").length > 0) {
    document.getElementById("line-tool-panel").classList.add("tool-options_Cqd");
}