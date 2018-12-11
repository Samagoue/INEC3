
$(document).ready(function () {
    $(".preloader").fadeOut();
    $("#btnsignup").click(function () {
        $('.preloader').show();
        if ($('#frmsignup').valid()) {
            var UserModel = {
                UserName: $('#Email').val(),
                Email: $('#Email').val(),
                Password: $('#Password').val(),
                ConfirmPassword: $('#ConfirmPassword').val(),
                UserProfile: { FirstName: $('#Name').val(), LastName: '' }
            };
            $.ajax({
                type: "POST",
                url: "/api/Account/Register",
                data: UserModel,
                success: function (data) {
                    $(".preloader").fadeOut();
                    if (data.succeeded) {
                        $.toast({
                            heading: 'Welcome to shadow',
                            text: 'Check your email and confirm.',
                            position: 'top-right',
                            loaderBg: '#ff6849',
                            icon: 'success',
                            hideAfter: 3500,
                            stack: 6
                        });
                        window.location.href = '/Account/Login';
                    }
                },
                error: function (ex) {
                    $(".preloader").fadeOut();
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
        else {
            $(".preloader").fadeOut();
        }
    });
});

$("#frmsignup").validate({
    rules: {
        Name: { required: true },
        Email: { required: true, email: true },
        Password: { required: "Please Enter Password", minlength: 5 },
        ConfirmPassword: { minlength: 5, equalTo: "#Password" }

    },
    messages: {
        Name: "Please Enter Name",
        Email: "Please Enter Valide Email",
        Password: { required: "Please Enter Password", minlength: "Enter at least {0} characters" },
        ConfirmPassword: "Please Enter Confirm Password",
    },
}
);