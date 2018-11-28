var user = {
    grant_type: 'password',
    UserName: $('#username').val(),
    Password: $('#password').val()
};
$(document).ready(function () {
    $("#btnlogin").click(function () {
        if ($('#loginform').valid()) {
            //Ref>https://medium.com/@manivannan_data/create-update-and-delete-cookies-using-jquery-5235b110d384
            $.ajax({
                type: "POST",
                url: "/token",
                data: "grant_type=password&UserName=" + $('#username').val() + "&Password=" + $('#password').val(),
                dataType: "x-www-form-urlencoded",
                complete: function (data) {
                    console.log(data);
                    debugger
                    var resp = JSON.parse(data.responseText);
                    if (resp.error) {
                        alert(resp.error_description);
                    }
                    else if (resp.access_token) {
                        var res = 'Bearer ' + resp.access_token;
                        Cookies.set('inecbearer', res, { expires: 1 });
                        window.location.href = '/Admin';
                        //Cookies.get(‘name’); 
                    }
                    else {
                        alert("Something Wrong Try Again");
                    }

                }
            });
        }
    });
    
    $("#to-recover").click(function () {
        $('#recoverform').show();
        $('#loginform').hide();
    });
    $("#to-login").click(function () {
        $('#recoverform').hide();
        $('#loginform').show();
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