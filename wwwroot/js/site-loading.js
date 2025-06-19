function showLoadingOverlay() {
    const overlay = document.getElementById("loadingOverlay");
    if (overlay) overlay.style.display = "block";
}

function hideLoadingOverlay() {
    const overlay = document.getElementById("loadingOverlay");
    if (overlay) overlay.style.display = "none";
}