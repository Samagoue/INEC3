﻿var user = {
    grant_type: 'password',
    UserName: $('#username').val(),
    Password: $('#password').val()
};
$(document).ready(function () {
    $(".preloader").fadeOut();

    $("#btnlogin").click(function () {
        $('.preloader').show();
        if ($('#loginform').valid()) {
            //Ref>https://medium.com/@manivannan_data/create-update-and-delete-cookies-using-jquery-5235b110d384
            $.ajax({
                type: "POST",
                url: "/api/Account/Login",
                data: "Email=" + $('#username').val() + "&Password=" + $('#password').val(),
                dataType: "x-www-form-urlencoded",
                complete: function (data) {
                    var resp = JSON.parse(data.responseText);
                    if (data.status >= 200 && data.status < 400) {
                        if (resp.access_token) {
                            $.toast({
                                heading: 'Welcome to shadow',
                                text: 'Successfully login. redirecting to homepage',
                                position: 'top-right',
                                loaderBg: '#ff6849',
                                icon: 'success',
                                hideAfter: 3500,
                                stack: 6
                            });
                            var res = 'Bearer ' + resp.access_token;
                            Cookies.set('inecbearer', res, { expires: 1 });
                            //window.location.href = '/Admin';
                            window.location.href = resp.returnUrl;
                        }
                        else {
                            $.toast({
                                text: 'Something Wrong Try Again',
                                position: 'top-right',
                                loaderBg: '#ff6849',
                                icon: 'error',
                                hideAfter: 3500,
                                stack: 6
                            });
                            //alert("Something Wrong Try Again");
                        }
                        $(".preloader").fadeOut();
                    }
                    else if (resp.message) {
                        $(".preloader").fadeOut();
                        $.toast({
                            text: resp.message,
                            position: 'top-right',
                            loaderBg: '#ff6849',
                            icon: 'warning',
                            hideAfter: 3500,
                            stack: 6
                        });
                        //alert(resp.message);
                    }
                }
            });
        }
        else {
            $(".preloader").fadeOut();
        }

    });
    $("#btnreset").click(function () {
        $('.preloader').show();
        if ($('#loginform').valid()) {
            $.ajax({
                type: "GET",
                url: "/api/Account/ForgotPassword",
                data: "email=" + $('#txtresetemail').val(),
                success: function (resp) {
                    debugger
                    if (resp.contentType == 'error') {
                        $(".preloader").fadeOut();
                        $.toast({
                            text: 'Something Wrong Try Again',
                            position: 'top-right',
                            loaderBg: '#ff6849',
                            icon: 'error',
                            hideAfter: 3500,
                            stack: 6
                        });
                       
                    }
                    else if (resp.contentType == 'fail') {
                        $(".preloader").fadeOut();
                        $.toast({
                            text: resp.data,
                            position: 'top-right',
                            loaderBg: '#ff6849',
                            icon: 'warning',
                            hideAfter: 3500,
                            stack: 6
                        });
                    }
                    else {
                        $(".preloader").fadeOut();
                        $.toast({
                            text: 'Reset password send to your email.',
                            position: 'top-right',
                            loaderBg: '#ff6849',
                            icon: 'success',
                            hideAfter: 3500,
                            stack: 6
                        });
                    }
                },
                error: function (data) {
                    alert(data);
                }
            });
        }


    });

    $("#to-recover").click(function () {
        $('#loginform').trigger("reset");
        $("#loginform").slideUp();
        $("#recoverform").fadeIn();
    });
    $("#to-login").click(function () {
        $('#recoverform').trigger("reset");
        $("#loginform").fadeIn();
        $("#recoverform").slideUp();
    });
    $('#username').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            //$('input[name = btnlogin]').click();
            $('#btnlogin').trigger('click');
            return false;
        }
    });
    $('#password').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            $('#btnlogin').trigger('click');
            //$('input[name = btnlogin]').click();
            return false;
        }
    });
});

$("#loginform").validate({
    rules: {
        username: "required",
        password: "required",
    },
    messages: {
        username: "Please Enter Email",
        password: "Please Enter password",
    },
}
);