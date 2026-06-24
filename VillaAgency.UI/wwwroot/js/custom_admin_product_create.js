const input = document.getElementById('imageUrlInput');
const img = document.getElementById('imagePreviewImg');
const placeholder = document.getElementById('previewPlaceholder');

input.addEventListener('input', function () {
    const url = this.value.trim();
    if (url && (url.startsWith('http') || url.startsWith('/'))) {
        img.src = url;
        img.onload = () => {
            img.classList.remove('d-none');
            placeholder.classList.add('d-none');
        };
        img.onerror = () => resetPreview();
    } else {
        resetPreview();
    }
});

function resetPreview() {
    img.src = "#";
    img.classList.add('d-none');
    placeholder.classList.remove('d-none');
}