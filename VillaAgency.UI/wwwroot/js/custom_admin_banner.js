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



// sweetalert
function deleteBanner(id) {
    Swal.fire({
        title: "Delete Banner?",
        text: "This action cannot be undone.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Delete",
        cancelButtonText: "Cancel",
        confirmButtonColor: "#dc2626",
        cancelButtonColor: "#6c757d",
        reverseButtons: true,
        customClass: {
            popup: "rounded-4 shadow-lg"
        }
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `/Admin/Banner/DeleteBanner/${id}`;
        }
    });
}