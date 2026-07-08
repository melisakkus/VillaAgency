
$(function () {
    $("#contact-form").submit(function (e) {
        e.preventDefault();
        $(".validation-error").removeClass("show").text("");
        var form = $(this);

        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: "success",
                        title: "Message Sent",
                        text: response.message,
                        confirmButtonColor: "#f35525"
                    });
                    form[0].reset();
                } else {
                    if (response.errors) {
                        $.each(response.errors, function (key, messages) {
                            var errorSpan = $(".error-" + key);
                            errorSpan.text(messages[0]).addClass("show");
                        });
                    }
                }
            },
            error: function (xhr, status, error) {
                Swal.fire({
                    icon: "error",
                    title: "System Error",
                    text: "An unexpected error occurred. Please try again later.",
                    confirmButtonColor: "#333"
                });
            }
        });
    });
});
