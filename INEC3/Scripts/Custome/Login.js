var user = {
    grant_type: 'password',
    UserName: $('#username').val(),
    Password: $('#password').val()
};
$(document).ready(function () {
    $(".preloader").fadeOut();
    var ReturnUrl = getUrlParameter('ReturnUrl');

    $("#btnlogin").click(function () {
        $('.preloader').show();
        if ($('#loginform').valid()) {
            $.ajax({
                type: "POST",
                url: "/api/Account/Login",
                data: "Email=" + $('#username').val() + "&Password=" + $('#password').val(),
                dataType: "x-www-form-urlencoded",
                complete: function (data) {
                    var resp = JSON.parse(data.responseText);
                    if (data.status >= 200 && data.status < 400) {
                        if (resp.access_token) {
                            $.toast({ heading: 'Welcome to shadow', text: 'Successfully login. redirecting to homepage', position: 'top-right', loaderBg: '#ff6849', icon: 'success', hideAfter: 3500, stack: 6 });
                            $.ajax({
                                type: "GET", url: "/Account/Index",
                                data: "userid=" + resp.userid + "&access_token=" + resp.access_token + "&token_type=" + resp.token_type,
                                success: function () {
                                    if (ReturnUrl)
                                        window.location.href = ReturnUrl;
                                    else
                                        window.location.href = '/'
                                },
                                error: function () {
                                    $.toast({ heading: 'Error', text: 'Something wrong please try again.', position: 'top-right', loaderBg: '#ff6849', icon: 'error', hideAfter: 3500, stack: 6 });
                                }
                            });

                        }
                        else {
                            $.toast({ text: 'Something Wrong Try Again', position: 'top-right', loaderBg: '#ff6849', icon: 'error', hideAfter: 3500, stack: 6 });
                        }
                        $(".preloader").fadeOut();
                    }
                    else if (resp.Message) {
                        $(".preloader").fadeOut();
                        $.toast({
                            text: resp.Message,
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
                        $('#alerttop').show();
                        $('#paraemail').html('<b>Success </b> account reset password sent to :' + $('#txtresetemail').val());
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
    if ($('#email').val() != "") {
        $('#alerttop').show();
        $('#paraemail').html('<b>Email Verification Pending. </b>Check your email : ' + $('#email').val());
    }
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
function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
};
