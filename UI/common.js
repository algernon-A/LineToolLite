// Ensure container.
if (typeof lineTool != 'object') var lineTool = {};

// Function to setup buttons.
if (typeof lineTool.setupClickButton2 !== 'function') {
    lineTool.setupClickButton2 = function (id, onclick, toolTipKey) {
        let newButton = document.getElementById(id);
        if (newButton) {
            newButton.onclick = onclick;
        }
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

// Function to set the visibility status of a button with icon child.
if (typeof lineTool.setButtonVisibility !== 'function') {
    lineTool.setButtonVisibility = function (button, isVisible) {
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

// Function to apply translation strings.
if (typeof lineTool.applyLocalization !== 'function') {
    lineTool.applyLocalization = function (target) {
        if (!target) {
            return;
        }

        let targets = target.querySelectorAll('[localeKey]');
        targets.forEach(function (currentValue) {
            currentValue.innerHTML = engine.translate(currentValue.getAttribute("localeKey"));
        });
    }
}

// Function to setup buttons.
if (typeof lineTool.setupClickButton !== 'function') {
    lineTool.setupClickButton = function (id, onclick, toolTipKey) {
        let newButton = document.getElementById(id);
        if (newButton) {
            newButton.onclick = onclick;
            lineTool.setTooltip(id, toolTipKey);
        }
    }
}

// Function to setup controls with a scrollwheel component.
if (typeof lineTool.setupWheel !== 'function') {
    lineTool.setupWheel = function (id, onwheel) {
        let newControl = document.getElementById(id);
        if (newControl) {
            newControl.onwheel = onwheel;
        }
    }
}

// Function to setup tooltip.
if (typeof lineTool.setTooltip !== 'function') {
    lineTool.setTooltip = function (id, toolTipKey) {
        let target = document.getElementById(id);
        target.onmouseenter = () => lineTool.showTooltip(document.getElementById(id), toolTipKey);
        target.onmouseleave = lineTool.hideTooltip;
    }
}

// Function to show a tooltip, creating if necessary.
if (typeof lineTool.showTooltip !== 'function') {
    lineTool.showTooltip = function (parent, tooltipKey) {

        if (!lineTool.tooltip) {
            lineTool.tooltip = document.createElement("div");
            lineTool.tooltip.style.visibility = "hidden";
            lineTool.tooltip.classList.add("balloon_qJY", "balloon_H23", "up_ehW", "center_hug", "anchored-balloon_AYp", "up_el0");
            let boundsDiv = document.createElement("div");
            boundsDiv.classList.add("bounds__AO");
            let containerDiv = document.createElement("div");
            containerDiv.classList.add("container_zgM", "container_jfe");
            let contentDiv = document.createElement("div");
            contentDiv.classList.add("content_A82", "content_JQV");
            let arrowDiv = document.createElement("div");
            arrowDiv.classList.add("arrow_SVb", "arrow_Xfn");
            let broadDiv = document.createElement("div");
            lineTool.tooltipTitle = document.createElement("div");
            lineTool.tooltipTitle.classList.add("title_lCJ");
            let paraDiv = document.createElement("div");
            paraDiv.classList.add("paragraphs_nbD", "description_dNa");
            lineTool.tooltipPara = document.createElement("p");
            lineTool.tooltipPara.setAttribute("cohinline", "cohinline");

            paraDiv.appendChild(lineTool.tooltipPara);
            broadDiv.appendChild(lineTool.tooltipTitle);
            broadDiv.appendChild(paraDiv);
            containerDiv.appendChild(arrowDiv);
            contentDiv.appendChild(broadDiv);
            boundsDiv.appendChild(containerDiv);
            boundsDiv.appendChild(contentDiv);
            lineTool.tooltip.appendChild(boundsDiv);

            // Append tooltip to screen element.
            let screenParent = document.getElementsByClassName("game-main-screen_TRK");
            if (screenParent.length == 0) {
                screenParent = document.getElementsByClassName("editor-main-screen_m89");
            }
            if (screenParent.length > 0) {
                screenParent[0].appendChild(lineTool.tooltip);
            }
        }

        // Set text and position.
        lineTool.tooltipTitle.innerHTML = engine.translate("LINETOOL." + tooltipKey);
        lineTool.tooltipPara.innerHTML = engine.translate("LINETOOL_DESCRIPTION." + tooltipKey);

        // Set visibility tracking to prevent race conditions with popup delay.
        lineTool.tooltipVisibility = "visible";

        // Slightly delay popup by three frames to prevent premature activation and to ensure layout is ready.
        window.requestAnimationFrame(() => {
            window.requestAnimationFrame(() => {
                window.requestAnimationFrame(() => {
                    lineTool.setTooltipPos(parent);
                });

            });
        });
    }
}

// Function to adjust the position of a tooltip and make visible.
if (typeof lineTool.setTooltipPos !== 'function') {
    lineTool.setTooltipPos = function (parent) {
        if (!lineTool.tooltip) {
            return;
        }

        let tooltipRect = lineTool.tooltip.getBoundingClientRect();
        let parentRect = parent.getBoundingClientRect();
        let xPos = parentRect.left + ((parentRect.width - tooltipRect.width) / 2);
        let yPos = parentRect.top - tooltipRect.height;
        lineTool.tooltip.setAttribute("style", "left:" + xPos + "px; top: " + yPos + "px; --posY: " + yPos + "px; --posX:" + xPos + "px");

        lineTool.tooltip.style.visibility = lineTool.tooltipVisibility;
    }
}

// Function to hide the tooltip.
if (typeof lineTool.hideTooltip !== 'function') {
    lineTool.hideTooltip = function () {
        if (lineTool.tooltip) {
            lineTool.tooltipVisibility = "hidden";
            lineTool.tooltip.style.visibility = "hidden";
        }
    }
}