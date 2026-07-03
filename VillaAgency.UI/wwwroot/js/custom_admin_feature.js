// custom_admin_feature.js

document.addEventListener("DOMContentLoaded", function () {
    // 1. Elements for the Preview logic
    const inputTitle = document.getElementById("inputTitle");
    const imageUrlInput = document.getElementById("imageUrlInput");
    const previewCard = document.getElementById("previewCard");
    const imagePreview = document.getElementById("imagePreview");
    const previewTitle = document.getElementById("previewTitle");
    const previewPlaceholder = document.getElementById("previewPlaceholder");

    // Target either the Create or Update form
    const form = document.getElementById('createFeatureForm') || document.getElementById('updateFeatureForm');

    // --- PREVIEW LOGIC ---
    function updatePreview() {
        if (!imageUrlInput || !imagePreview) return; // Exit if elements don't exist (e.g., on Index page)

        const url = imageUrlInput.value.trim();
        const title = inputTitle ? inputTitle.value.trim() : "";

        if (url) {
            imagePreview.src = url;
            if (previewTitle) previewTitle.textContent = title || "Feature Title";

            if (previewCard) previewCard.classList.remove("d-none");
            if (previewPlaceholder) previewPlaceholder.classList.add("d-none");
        } else {
            if (previewCard) previewCard.classList.add("d-none");
            if (previewPlaceholder) previewPlaceholder.classList.remove("d-none");
            imagePreview.src = "#";
        }
    }

    // Attach Preview Listeners safely
    if (imageUrlInput) {
        imageUrlInput.addEventListener("input", updatePreview);
        imageUrlInput.addEventListener("change", updatePreview);

        // imagePreview var mı kontrolü eklendi
        if (imagePreview) {
            imagePreview.addEventListener("error", function () {
                if (previewCard) previewCard.classList.add("d-none");
                if (previewPlaceholder) previewPlaceholder.classList.remove("d-none");
            });
        }

        updatePreview();
    }

    if (inputTitle) {
        inputTitle.addEventListener("input", updatePreview);
    }

    // --- FORM SUBMIT REINDEXING ---
    if (form) {
        form.addEventListener('submit', function () {
            reindex("faq-item", "FAQs");
            reindex("detail-item", "FeatureDetails");
        });
    }
});

// --- GLOBAL FUNCTIONS (Called by HTML buttons) ---

function addFaq() {

    const index = document.querySelectorAll(".faq-item").length;

    document.getElementById("faqContainer").insertAdjacentHTML("beforeend", `
        <div class="row g-2 mb-3 faq-item">

            <div class="col-5">
                <input name="FAQs[${index}].Question"
                       class="form-control form-control-sm"
                       placeholder="Question">
            </div>

            <div class="col-6">
                <input name="FAQs[${index}].Answer"
                       class="form-control form-control-sm"
                       placeholder="Answer">
            </div>

            <div class="col-1 text-end">
                <button type="button"
                        class="btn btn-sm text-danger border-0"
                        onclick="removeItem(this,'faq-item','FAQs')">
                    ✕
                </button>
            </div>

        </div>
    `);
}

function addDetail() {

    const index = document.querySelectorAll(".detail-item").length;

    document.getElementById("detailContainer").insertAdjacentHTML("beforeend", `
        <div class="row g-2 mb-3 detail-item">

            <input type="hidden"
                   name="FeatureDetails[${index}].Icon"
                   value="bi bi-star">

            <div class="col-5">
                <input name="FeatureDetails[${index}].Title"
                       class="form-control form-control-sm"
                       placeholder="Detail Title">
            </div>

            <div class="col-6">
                <input name="FeatureDetails[${index}].SubTitle"
                       class="form-control form-control-sm"
                       placeholder="Detail Subtitle">
            </div>

            <div class="col-1 text-end">
                <button type="button"
                        class="btn btn-sm text-danger border-0"
                        onclick="removeItem(this,'detail-item','FeatureDetails')">
                    ✕
                </button>
            </div>

        </div>
    `);
}

function removeItem(button, itemClass, rootName) {
    button.closest("." + itemClass).remove();
    reindex(itemClass, rootName);
}

// Corrects the [0] indices so C# List model binding works
function reindex(itemClass, rootName) {

    document.querySelectorAll("." + itemClass).forEach((item, index) => {

        item.querySelectorAll("input").forEach(input => {

            if (input.name.endsWith("Question"))
                input.name = `${rootName}[${index}].Question`;

            else if (input.name.endsWith("Answer"))
                input.name = `${rootName}[${index}].Answer`;

            else if (input.name.endsWith("SubTitle"))
                input.name = `${rootName}[${index}].SubTitle`;

            else if (input.name.endsWith("Title"))
                input.name = `${rootName}[${index}].Title`;

            else if (input.name.endsWith("Icon"))
                input.name = `${rootName}[${index}].Icon`;
        });
    });
}

// SweetAlert - For Index Page
function deleteFeature(id) {
    if (typeof Swal === 'undefined') {
        if (confirm("Are you sure you want to delete this feature?")) {
            window.location.href = `/Admin/Feature/Delete/${id}`;
        }
        return;
    }

    Swal.fire({
        title: "Delete Feature",
        text: "This action cannot be undone.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Delete",
        cancelButtonText: "Cancel",
        confirmButtonColor: "#dc2626",
        cancelButtonColor: "#6c757d",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = `/Admin/Feature/Delete/${id}`;
        }
    });
}