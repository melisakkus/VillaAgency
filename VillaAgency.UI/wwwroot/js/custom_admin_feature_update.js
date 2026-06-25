const urlInput = document.getElementById('imageUrlInput');
const previewContainer = document.getElementById('previewContainer');
const imgPreview = document.getElementById('imagePreview');
const form = document.getElementById('updateFeatureForm');

function updateImagePreview() {
    const url = urlInput.value.trim();
    if (url) {
        imgPreview.src = url;
        previewContainer.classList.remove('d-none');

        imgPreview.onerror = function () {
            previewContainer.classList.add('d-none');
        };
    } else {
        previewContainer.classList.add('d-none');
        imgPreview.src = "#";
    }
}

urlInput.addEventListener('input', updateImagePreview);
urlInput.addEventListener('change', updateImagePreview);
window.addEventListener('DOMContentLoaded', updateImagePreview);

function addFaq() {
    const container = document.getElementById("faqContainer");
    const html = `
        <div class="row g-2 mb-3 faq-item align-items-start">
            <div class="col-5">
                <input name="FAQs[0].Question" class="form-control form-control-sm" placeholder="Question" />
                <span class="text-danger d-block mt-1" data-valmsg-for="FAQs[0].Question" data-valmsg-replace="true" style="font-size: 11px;"></span>
            </div>
            <div class="col-6">
                <input name="FAQs[0].Answer" class="form-control form-control-sm" placeholder="Answer" />
                <span class="text-danger d-block mt-1" data-valmsg-for="FAQs[0].Answer" data-valmsg-replace="true" style="font-size: 11px;"></span>
            </div>
            <div class="col-1 text-end pt-1">
                <button type="button" class="btn btn-sm text-danger p-0 border-0 lh-1" onclick="removeItem(this, 'faq-item', 'FAQs')">
                    <i class="bi bi-trash3-fill"></i>
                </button>
            </div>
        </div>`;
    container.insertAdjacentHTML("beforeend", html);
    reindex("faq-item", "FAQs");
}

function addDetail() {
    const container = document.getElementById("detailContainer");
    const html = `
        <div class="row g-2 mb-3 detail-item align-items-start">
            <input type="hidden" name="FeatureDetails[0].Icon" value="bi bi-star" />
            <div class="col-5">
                <input name="FeatureDetails[0].Title" class="form-control form-control-sm" placeholder="Title" />
                <span class="text-danger d-block mt-1" data-valmsg-for="FeatureDetails[0].Title" data-valmsg-replace="true" style="font-size: 11px;"></span>
            </div>
            <div class="col-6">
                <input name="FeatureDetails[0].SubTitle" class="form-control form-control-sm" placeholder="SubTitle" />
                <span class="text-danger d-block mt-1" data-valmsg-for="FeatureDetails[0].SubTitle" data-valmsg-replace="true" style="font-size: 11px;"></span>
            </div>
            <div class="col-1 text-end pt-1">
                <button type="button" class="btn btn-sm text-danger p-0 border-0 lh-1" onclick="removeItem(this, 'detail-item', 'FeatureDetails')">
                    <i class="bi bi-trash3-fill"></i>
                </button>
            </div>
        </div>`;
    container.insertAdjacentHTML("beforeend", html);
    reindex("detail-item", "FeatureDetails");
}

function removeItem(button, itemClass, rootName) {
    const row = button.closest('.row');
    if (row) row.remove();
    reindex(itemClass, rootName);
}

function reindex(itemClass, rootName) {
    const items = document.querySelectorAll('.' + itemClass);
    items.forEach((item, index) => {
        const q = item.querySelector('input[name$=".Question"]');
        const a = item.querySelector('input[name$=".Answer"]');
        const t = item.querySelector('input[name$=".Title"]');
        const s = item.querySelector('input[name$=".SubTitle"]');
        const i = item.querySelector('input[name$=".Icon"]');

        if (q) q.name = `${rootName}[${index}].Question`;
        if (a) a.name = `${rootName}[${index}].Answer`;
        if (t) t.name = `${rootName}[${index}].Title`;
        if (s) s.name = `${rootName}[${index}].SubTitle`;
        if (i) i.name = `${rootName}[${index}].Icon`;

        const spans = item.querySelectorAll('span[data-valmsg-for]');
        spans.forEach(span => {
            const currentAttr = span.getAttribute('data-valmsg-for').toLowerCase();
            if (currentAttr.includes('question')) span.setAttribute('data-valmsg-for', `${rootName}[${index}].Question`);
            if (currentAttr.includes('answer')) span.setAttribute('data-valmsg-for', `${rootName}[${index}].Answer`);
            if (currentAttr.includes('title')) span.setAttribute('data-valmsg-for', `${rootName}[${index}].Title`);
            if (currentAttr.includes('subtitle')) span.setAttribute('data-valmsg-for', `${rootName}[${index}].SubTitle`);
        });
    });
}

form.addEventListener('submit', function () {
    reindex("faq-item", "FAQs");
    reindex("detail-item", "FeatureDetails");
});