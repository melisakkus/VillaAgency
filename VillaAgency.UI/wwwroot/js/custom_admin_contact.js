// custom_admin_contact.js

// create-update
document.addEventListener("DOMContentLoaded", function () {
    const mapUrlInput = document.getElementById("mapUrlInput");
    const mapPreview = document.getElementById("mapPreview");
    const mapPlaceholder = document.getElementById("mapPlaceholder");

    function updateMapPreview() {
        let url = mapUrlInput.value.trim();

        if (url.includes('<iframe')) {
            const match = url.match(/src="([^"]+)"/);
            if (match && match[1]) {
                url = match[1];
                mapUrlInput.value = url;
            }
        }

        if (url && url.startsWith('http')) {
            if (url.includes('/embed') || url.includes('google.com/maps/embed')) {
                mapPreview.src = url;
                mapPreview.classList.remove("d-none");
                mapPlaceholder.classList.add("d-none");
            }
            else {

                mapPreview.classList.add("d-none");
                mapPlaceholder.classList.remove("d-none");
                mapPlaceholder.innerHTML = `
                        <div class="p-3">
                            <i class="bi bi-info-circle display-6 text-info mb-3 d-block"></i>
                            <span class="text-body small fw-bold d-block">Short Link Detected</span>
                            <span class="text-muted small">Google doesn't allow live preview for short links.
                            But don't worry, it will work fine for users!</span>
                            <br>
                            <a href="${url}" target="_blank" class="btn btn-xs btn-link mt-2">Test Link <i class="bi bi-box-arrow-up-right"></i></a>
                        </div>`;
            }
        } else {
            mapPreview.classList.add("d-none");
            mapPlaceholder.classList.remove("d-none");
        }
    }

    mapUrlInput.addEventListener("input", updateMapPreview);
    updateMapPreview();
});


//index.html
function handleMapModal(url) {
    const iframe = document.getElementById('modalIframe');
    const placeholder = document.getElementById('modalPlaceholder');
    const externalLink = document.getElementById('modalExternalLink');

    // Check if the URL is an embeddable Google Maps URL
    const isEmbeddable = url.includes("/embed") || url.includes("google.com/maps/embed");

    if (isEmbeddable) {
        // Show Iframe and Hide Placeholder
        iframe.src = url;
        iframe.classList.remove('d-none');
        placeholder.classList.add('d-none');
    } else {
        // Short link detected: Hide Iframe (Google blocks it) and Show Placeholder
        iframe.src = "";
        iframe.classList.add('d-none');
        placeholder.classList.remove('d-none');
        externalLink.href = url;
    }

    // Trigger the Modal
    const myModal = new bootstrap.Modal(document.getElementById('mapModal'));
    myModal.show();
}

// Reset iframe on modal close to prevent background processing
document.getElementById('mapModal').addEventListener('hidden.bs.modal', function () {
    document.getElementById('modalIframe').src = '';
});