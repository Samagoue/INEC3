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
                FirstName: $('#Name').val()
                //UserProfile: { FirstName: $('#Name').val(), LastName: '' }
            };
            $.ajax({
                type: "POST",
                url: "/api/Account/Register",
                data: UserModel,
                success: function (data) {
                    $(".preloader").fadeOut();
                    if (data.Succeeded) {
                        $.toast({ heading: 'Welcome to shadow', text: 'Check your email and confirm.', position: 'top-right', loaderBg: '#ff6849', icon: 'success', hideAfter: 3500, stack: 6 });
                        window.location.href = '/Account/Login?email=' + $('#Email').val()
                    }
                },
                error: function (ex) {
                    $(".preloader").fadeOut();
                    if (ex.responseJSON) {
                        if (ex.responseJSON.modelState) {
                            var eror = ex.responseJSON.modelState[""];
                            $.toast({ heading: 'Invalid Data !', text: eror[0], position: 'top-right', loaderBg: '#ff6849', icon: 'warning', hideAfter: 3500, stack: 6 });
                            //alert(eror[0]);
                            console.log('Error ' + ex);
                        }
                        else if (ex.responseJSON.Message) {
                            $.toast({ heading: 'Invalid Data !', text: ex.responseJSON.Message, position: 'top-right', loaderBg: '#ff6849', icon: 'warning', hideAfter: 3500, stack: 6 });
                            //alert(ex.responseJSON.Message);
                        }
                        else {
                            $.toast({ heading: 'Oopss..!', text: 'Something Wrong Try Again', position: 'top-right', loaderBg: '#ff6849', icon: 'error', hideAfter: 3500, stack: 6 });
                            //alert("Something Wrong Try Again");
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