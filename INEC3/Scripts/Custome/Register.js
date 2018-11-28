
$(document).ready(function () {

    $("#btnsignup").click(function () {
        if ($('#frmsignup').valid()) {
            var UserModel = {
                UserName: $('#Email').val(),
                Email: $('#Email').val(),
                Password: $('#Password').val(),
                ConfirmPassword: $('#ConfirmPassword').val()
            };

            $.ajax({
                type: "POST",
                url: "/api/Account/Register",
                data: UserModel,
                success: function (data) {
                    //debugger
                    if (data.succeeded) {
                        window.location.href = '/Account/Login';
                    }
                    //console.log(data);
                },
                error: function (ex) {
                    if (ex.responseJSON) {
                        if (ex.responseJSON.modelState) {
                            var eror = ex.responseJSON.modelState[""];
                            alert(eror[0]);
                            console.log('Error ' + ex);
                        }
                        else if (ex.responseJSON.message) {
                            alert(ex.responseJSON.message);
                        }
                        else {
                            alert("Something Wrong Try Again");
                        }
                    }
                }
            });
        }
    });
});

$("#frmsignup").validate({
    rules: {
        Name: "required",
        Email: { required: true, email: true },
        Password: "required",
        ConfirmPassword: "required",

    },
    messages: {
        Name: "Please Enter Name",
        Email: "Please Enter Valide Email",
        Password: "Please Enter Password",
        ConfirmPassword: "Please Enter Confirm Password",
    },
}
);