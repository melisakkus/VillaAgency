// custom_admin_videoview.js

// sweetalert
function deleteVideo(id) {
    Swal.fire({
        title: "Delete Video",
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
            window.location.href = `/Admin/Video/Delete/${id}`;
        }
    });
}



document.addEventListener("DOMContentLoaded", function () {

    // 1. INDEX 
    document.querySelectorAll(".video-preview-bg").forEach(container => {
        const url = container.getAttribute("data-video-url");
        const ytMatch = url?.match(/(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/ ]{11})/i);
        if (ytMatch) {
            container.style.backgroundImage = `url('https://img.youtube.com/vi/${ytMatch[1]}/mqdefault.jpg')`;
            container.style.backgroundSize = "cover";
            container.style.backgroundPosition = "center";
        }
    });

    // 2. MODAL (Index & Create & Update)

    const modalVideo = document.getElementById('modalVideoContainer');
    const videoModalEl = document.getElementById('videoPlayerModal');

    if (videoModalEl && modalVideo) {
        videoModalEl.addEventListener('show.bs.modal', function (e) {
            const url = e.relatedTarget.getAttribute('data-video-src');
            if (!url) return;

            const ytMatch = url.match(/(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/ ]{11})/i);
            modalVideo.innerHTML = ytMatch
                ? `<iframe src="https://www.youtube.com/embed/${ytMatch[1]}?autoplay=1" allow="autoplay; encrypted-media" allowfullscreen class="border-0 w-100 h-100"></iframe>`
                : `<video src="${url}" controls autoplay class="w-100 h-100"></video>`;
        });

        videoModalEl.addEventListener('hide.bs.modal', () => {
            modalVideo.innerHTML = "";
        });
    }

    // 3. CREATE & UPDATE 
    const videoInput = document.querySelector('input[name="VideoUrl"]');
    const previewContainer = document.getElementById('liveVideoPreviewContainer');
    const previewBg = document.getElementById('liveVideoPreviewBg');
    const previewPlayBtn = document.getElementById('livePreviewPlayBtn');

    if (videoInput && previewContainer && previewBg) {

        function handleVideoPreview(url) {
            if (!url || url.trim() === "") {
                previewContainer.classList.add('d-none');
                return;
            }

            const ytMatch = url.match(/(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/ ]{11})/i);

            previewContainer.classList.remove('d-none');
            previewPlayBtn.setAttribute('data-video-src', url);

            if (ytMatch) {
                previewBg.innerHTML = ""; 
                previewBg.style.backgroundImage = `url('https://img.youtube.com/vi/${ytMatch[1]}/mqdefault.jpg')`;
            } else {
                previewBg.style.backgroundImage = "none";
                previewBg.innerHTML = `<video src="${url}" muted loop autoplay class="w-100 h-100" style="object-fit: cover; pointer-events: none;"></video>`;
            }
        }

        videoInput.addEventListener('input', (e) => handleVideoPreview(e.target.value));

        if (videoInput.value) {
            handleVideoPreview(videoInput.value);
        }
    }
});


