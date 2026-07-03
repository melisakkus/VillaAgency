// custom_admin_product.js

const imageUrlInput = document.getElementById('imageUrlInput');
const imagePreviewImg = document.getElementById('imagePreviewImg');
const previewPlaceholder = document.getElementById('previewPlaceholder');

if (imageUrlInput && imagePreviewImg && previewPlaceholder) {

    function updateImagePreview() {
        const url = imageUrlInput.value.trim();

        if (url && (url.startsWith('http') || url.startsWith('/'))) {
            imagePreviewImg.src = url;
            imagePreviewImg.onload = () => {
                imagePreviewImg.classList.remove('d-none');
                previewPlaceholder.classList.add('d-none');
            };
            imagePreviewImg.onerror = () => resetImagePreview();
        } else {
            resetImagePreview();
        }
    }

    function resetImagePreview() {
        imagePreviewImg.src = "#";
        imagePreviewImg.classList.add('d-none');
        previewPlaceholder.classList.remove('d-none');
    }

    imageUrlInput.addEventListener('input', updateImagePreview);

    window.addEventListener('DOMContentLoaded', updateImagePreview);
}

// sweetalert
function deleteProduct(id) {
    Swal.fire({
            title: "Are you sure?",
            text: "This product will be moved to the Archived Products folder. You can restore it later.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, move to trash!",
            cancelButtonText: "Cancel",
            confirmButtonColor: "#dc2626", 
            cancelButtonColor: "#6c757d",
            reverseButtons: true,
            customClass: {
                popup: "rounded-4 shadow-lg"
            }
        }).then((result) => {
            if (result.isConfirmed) {
                window.location.href = `/Admin/Product/Delete/${id}`;
            }
        });
    }