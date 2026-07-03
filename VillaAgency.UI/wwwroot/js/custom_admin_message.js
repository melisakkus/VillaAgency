// custom_admin_message.js

// index 
document.addEventListener("DOMContentLoaded", function () {
    var previewModal = document.getElementById('messagePreviewModal');

    previewModal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget;

        var name = button.getAttribute('data-name');
        var email = button.getAttribute('data-email');
        var subject = button.getAttribute('data-subject');
        var content = button.getAttribute('data-content');
        var date = button.getAttribute('data-date');

        previewModal.querySelector('#modalSenderName').textContent = name;
        previewModal.querySelector('#modalSenderEmail').textContent = email;
        previewModal.querySelector('#modalSubject').textContent = subject;
        previewModal.querySelector('#modalContent').textContent = content || "No message content.";
        previewModal.querySelector('#modalMessageDate').textContent = date;
    });
});



// sweetalert
function deleteMessage(id) {
    Swal.fire({
        title: "Are you sure?",
        text: "This message will be moved to the Deleted Messages folder. You can restore it later.",
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
            window.location.href = `/Admin/Message/Delete/${id}`;
        }
    });
}