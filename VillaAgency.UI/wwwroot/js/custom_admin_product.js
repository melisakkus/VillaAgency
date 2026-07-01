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