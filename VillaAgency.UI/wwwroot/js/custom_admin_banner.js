// custom_admin_banner.js

document.addEventListener("DOMContentLoaded", function () {
    const inputCity = document.getElementById("inputCity");
    const inputTitle = document.getElementById("inputTitle");
    const inputImageUrl = document.getElementById("inputImageUrl");

    const previewCard = document.getElementById("previewCard");
    const previewPlaceholder = document.getElementById("previewPlaceholder");
    const previewImg = document.getElementById("previewImg");
    const previewCity = document.getElementById("previewCity");
    const previewTitle = document.getElementById("previewTitle");

    function updatePreview() {
        const urlValue = inputImageUrl.value.trim();
        const cityValue = inputCity.value.trim();
        const titleValue = inputTitle.value.trim();

        if (urlValue) {
            previewImg.src = urlValue;
            previewCity.textContent = cityValue || "City Name";
            previewTitle.textContent = titleValue || "Banner Title";

            previewCard.style.display = "block";
            previewPlaceholder.style.display = "none";
        } else {
            previewCard.style.display = "none";
            previewPlaceholder.style.display = "block";
        }
    }

    inputCity.addEventListener("input", updatePreview);
    inputTitle.addEventListener("input", updatePreview);
    inputImageUrl.addEventListener("input", updatePreview);

    previewImg.addEventListener("error", function () {
        previewCard.style.display = "none";
        previewPlaceholder.style.display = "block";
    });

    updatePreview();
});