// custom_admin_question.js

// sweetalert
function deleteQuestion(id) {
    Swal.fire({
        title: "Delete Question",
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
            window.location.href = `/Admin/Question/Delete/${id}`;
        }
    });
}