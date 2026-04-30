// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

    $('#loginBtn').on('click', function() {
        var formData = {
        Account: $('#Account').val(),
    Password: $('#Password').val(),
    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

    $.ajax({
        url: '@Url.Action("Login", "Login")', // 確保這裡是你的 Login Controller
    type: 'POST',
    data: formData,
    success: function(response) {
                if (response.success) {
        window.location.href = response.redirectUrl; // 成功導向
                } else {
        $('#loginError').text(response.message).show(); // 顯示錯誤訊息
                }
            }
        });
    });
