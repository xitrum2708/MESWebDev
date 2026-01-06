// Excel-like color list
const COLORS = [
  "#FFFFFF","#000000","#FF0000","#00FF00","#0000FF",
  "#FFFF00","#FF00FF","#00FFFF","#C0C0C0","#808080",
  "#800000","#808000","#008000","#800080","#008080",
  "#000080","#FFA500","#A52A2A","#FFD700","#ADD8E6",
  "#90EE90","#FA8072","#F5DEB3","#D2691E","#696969",
  "#2F4F4F","#708090","#B0C4DE","#4B0082","#4682B4",
  "#32CD32","#FF6347","#B22222","#EEE8AA","#7FFF00"
];

// Initialize color picker for a specific input
function initColorPicker(inputId, buttonId, popupId) {
    const input = document.getElementById(inputId);
    const popup = document.getElementById(popupId);

    // create color grid if not exist
    if (!popup.dataset.loaded) {
        const grid = document.createElement("div");
        grid.className = "color-grid";
        COLORS.forEach(c => {
            const cell = document.createElement("div");
            cell.className = "color-cell";
            cell.style.background = c;
            cell.onclick = () => selectColor(input, popup, c);
            grid.appendChild(cell);
        });
        popup.appendChild(grid);
        popup.dataset.loaded = "true";
    }

    document.getElementById(buttonId).addEventListener("click", () => {
        popup.style.display = popup.style.display === "block" ? "none" : "block";
    });
}

function selectColor(input, popup, color) {
    input.value = color;
    input.style.background = color;
    input.style.color = isLight(color) ? "#000" : "#FFF";
    popup.style.display = "none";
}

// auto text color based on background
function isLight(hex) {
    const r = parseInt(hex.substr(1,2), 16);
    const g = parseInt(hex.substr(3,2), 16);
    const b = parseInt(hex.substr(5,2), 16);
    return (r + g + b) > 382;
}
