var user = {
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
                            var res = 'Bearer ' + resp.access_token;
                            Cookies.set('inecbearer', res, { expires: 1 });
                            window.location.href = '/Admin';
                        }
                        else {
                            alert("Something Wrong Try Again");
                        }
                        $(".preloader").fadeOut();
                    }
                    else if (resp.message) {
                        $(".preloader").fadeOut();
                        alert(resp.message);
                    }
                }
            });
        }
        else {
            $(".preloader").fadeOut();
        }

    });

    $("#to-recover").click(function () {
        //$('#recoverform').show();
        //$('#loginform').hide();
        $("#loginform").slideUp();
        $("#recoverform").fadeIn();
    });
    $("#to-login").click(function () {
        //$('#recoverform').hide();
        //$('#loginform').show();
        $("#loginform").fadeIn();
        $("#recoverform").slideUp();
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