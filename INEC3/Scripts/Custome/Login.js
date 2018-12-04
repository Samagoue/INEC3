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
                //url: "/token",
                url: "/api/Account/Login",
                //data: "grant_type=password&UserName=" + $('#username').val() + "&Password=" + $('#password').val(),
                data: "Email=" + $('#username').val() + "&Password=" + $('#password').val(),
                dataType: "x-www-form-urlencoded",
                complete: function (data) {
                    $(".preloader").fadeOut();
                    debugger
                    var resp = JSON.parse(data.responseText);
                    // if (resp.error) {
                    if (data.status >= 200 && data.status < 400) {
                        if (resp.access_token) {
                            var res = 'Bearer ' + resp.access_token;
                            Cookies.set('inecbearer', res, { expires: 1 });
                            window.location.href = '/Admin';
                            //Cookies.get(‘name’);
                        }
                        else {
                            alert("Something Wrong Try Again");
                        }

                    }
                    else if (resp.message) {

                        alert(resp.message);
                    }


                }
                //error: function (err) {
                //    alert('error');
                //    $(".preloader").fadeOut();
                //    var resp = JSON.parse(err.responseText);
                //    if (resp.message) {
                //        alert(resp.message);
                //    }
                //}
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