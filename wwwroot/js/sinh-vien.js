$(function () {
    const $modal = $("#student-modal");
    const modal = bootstrap.Modal.getOrCreateInstance($modal[0]);
    const $modalTitle = $("#student-modal-title");
    const $modalBody = $("#student-modal-body");
    const $message = $("#student-message");

    function showMessage(text, isError) {
        $message
            .toggleClass("alert-danger", isError)
            .toggleClass("alert-success", !isError)
            .text(text)
            .removeClass("d-none");
    }

    function refreshStudentTable() {
        return $.get($("#student-table").attr("data-student-list-url"))
            .done(function (html) {
                $("#student-table").html(html);
            })
            .fail(function () {
                showMessage("Đã lưu dữ liệu nhưng không tải lại được danh sách. Hãy tải lại trang.", true);
            });
    }

    $(document).on("click", "[data-student-modal-url]", function () {
        const $button = $(this);
        $modalTitle.text($button.data("student-modal-title"));
        $modalBody.html('<div class="text-center py-4" role="status">Đang tải...</div>');
        modal.show();

        $.get($button.attr("data-student-modal-url"))
            .done(function (html) {
                $modalBody.html(html);
            })
            .fail(function (xhr) {
                $modalBody.empty().append(
                    $("<div>", {
                        class: "alert alert-danger",
                        role: "alert",
                        text: xhr.status === 404
                            ? "Không tìm thấy sinh viên."
                            : "Không tải được nội dung. Vui lòng thử lại."
                    })
                );
            });
    });

    $(document).on("submit", "[data-student-ajax-form]", function (event) {
        event.preventDefault();

        const form = this;
        const $form = $(form);
        const $submit = $form.find(":submit").prop("disabled", true);

        $.ajax({
            url: form.action,
            method: form.method,
            data: new FormData(form),
            processData: false,
            contentType: false,
            dataType: "html"
        })
            .done(function () {
                modal.hide();
                showMessage($form.attr("data-success-message"), false);
                refreshStudentTable();
            })
            .fail(function (xhr) {
                if (xhr.status === 422) {
                    $modalBody.html(xhr.responseText);
                    $.validator.unobtrusive.parse($modalBody);
                    return;
                }

                const errorText = xhr.status === 404
                    ? "Không tìm thấy sinh viên."
                    : xhr.status === 400
                        ? "Yêu cầu không hợp lệ hoặc đã hết hạn. Hãy mở lại hộp thoại rồi thử lại."
                        : "Không thể lưu thay đổi. Vui lòng thử lại.";

                $modalBody.prepend(
                    $("<div>", {
                        class: "alert alert-danger",
                        role: "alert",
                        text: errorText
                    })
                );
            })
            .always(function () {
                $submit.prop("disabled", false);
            });
    });
});
