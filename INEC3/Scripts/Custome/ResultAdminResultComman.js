function SelectParty() {
    if ($('#ID_Candidat').val() > 0) {
        $(".preloader").show();
        $.ajax({
            url: '/api/Results/GetParty',
            type: "GET",
            headers: { Authorization: bearer },
            data: { candidateid: $('#ID_Candidat').val() },
            success: function (resp) {
                if (resp.ContentType != 'error') {
                    if (resp.Data) {
                        $("#ID_Party").val(resp.Data.ID_Party);
                        if (resp.Data.Color != null)
                            $("#ID_Party").css("background", resp.Data.Color);
                        else $("#ID_Party").css("background", '#e2e2e2');
                    }
                    else {
                        console.log("SelectParty(): Something Wrong")
                    }
                }
                $(".preloader").fadeOut();
            },
            error: function () {
                $(".preloader").fadeOut();
            }
        });
    }
}
function PolStationCahngeGet() {
    $(".preloader").show();
    $.ajax({
        url: '/api/Results/PolStationCahngeGet',
        type: "GET",
        headers: { Authorization: bearer },
        data: { polingstationid: $('#ID_Bureauvote').val() },
        success: function (result) {

            $('#Abstentions').val(0);
            $('#Nuls').val(0);
            $('#Exprimes').val(0);
            $('#Total_Votes').val(0);
            $('#Code_SV').val(0);
            if (result.ContentType != 'error') {
                var resp = result.Data
                if (resp) {
                    $('#Votants').val(resp.Total_Voters);
                    $('#Abstentions').val(resp.Abstentions);
                    $('#Exprimes').val(resp.Exprimes);
                    $('#Nuls').val(resp.Nuls);
                    $('#Total_Votes').val(resp.Total_Votes);
                    $('#Code_SV').val(resp.Code_SV);
                }
                var table = $('#tbllist tbody');
                if (resp.ResultList) {
                    $.each(resp.ResultList, function (i, v) {
                        table.append('<tr><td><input type="hidden" class="ResultId" value="' + v.ID_Result + '" /><input type="hidden" class="CandidateId" value="' + v.ID_Candidat + '" />' + v.Candidate + '</td><td><input type="hidden" class="PartyId" value="' + v.ID_Party + '" />' + v.Party + '</td><td class="votes">' + v.Votes + '</td><td class="percen">' + v.Pourcentage + '</td><td><button type="button" onClick="removerecord($(this))" class="btn btn-sm btn-danger"> X </button></td></tr>')
                    });
                }
            }

            else {
                $('#Votants').val(0);
                console.log("PolStationCahngeGet(): Something Wrong")
            }
            $(".preloader").fadeOut();
        },
        error: function () {
            $(".preloader").fadeOut();
        }
    });
}
function GetTerritoireList() {
    if ($('#ID_Province').val() > 0) {
        $('#ID_Territoire').html('');
        $('#ID_Territoire').append($('<option></option>').val('').html('Select Territoire'));
        $(".preloader").show();
        $.ajax({
            url: '/api/Results/GetTerritoireList',
            type: 'GET',
            headers: { Authorization: bearer },
            data: { ProvinceId: $('#ID_Province').val() },
            success: function (res) {
                if (res) {
                    $.each(res, function (i, v) {
                        $('#ID_Territoire').append($('<option></option>').val(v.ID_Territoire).html(v.Nom));
                    });
                }
                $(".preloader").fadeOut();
            },
            error: function () {
                $(".preloader").fadeOut();
            }
        });
    }
}

function GetCommune() {
    if ($('#ID_Territoire').val() > 0) {
        $('#ID_Commune').html('');
        $('#ID_Commune').append($('<option></option>').val('').html('Select Commune'));
        $(".preloader").show();
        $.ajax({
            url: '/api/Results/GetCommune',
            type: 'GET',
            headers: { Authorization: bearer },
            data: { ProvinceId: $('#ID_Province').val(), TerritoireId: $('#ID_Territoire').val() },
            success: function (res) {
                if (res) {
                    $.each(res, function (i, v) {
                        $('#ID_Commune').append($('<option></option>').val(v.ID_Commune).html(v.Nom));
                    });
                }
                $(".preloader").fadeOut();
            },
            error: function () {
                $(".preloader").fadeOut();
            }
        });
    }
    resettable()
}
function GetPoolingStation() {
    if ($('#ID_Commune').val() > 0) {
        $('#ID_Bureauvote').html('');
        $('#ID_Bureauvote').append($('<option></option>').val('').html('Select Pooling Station'));
        $(".preloader").show();
        $.ajax({
            url: '/api/Results/GetPoolingStationList',
            type: 'GET',
            headers: { Authorization: bearer },
            data: { CommuneId: $('#ID_Commune').val() },
            success: function (res) {
                if (res) {
                    $.each(res, function (i, v) {
                        $('#ID_Bureauvote').append($('<option></option>').val(v.ID_Bureauvote).html(v.Nom));
                    });
                }
                $(".preloader").fadeOut();
            },
            error: function () {
                $(".preloader").fadeOut();
            }
        });
    }
}

function CountExprimes() {
    var Abstentions = $('#Abstentions').val() == "" ? 0 : $('#Abstentions').val()
    var Nuls = $('#Nuls').val() == "" ? 0 : $('#Nuls').val()

    var Votes = 0
    $('#tbllist >tbody tr').each(function () {
        Votes += parseInt($(this).find('.votes').html())
    });
    $('#Exprimes').val(parseInt(Votes) + parseInt(Abstentions) + parseInt(Nuls))
}

function removerecord(button) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover this record!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    }).then((willDelete) => {
        if (willDelete) {
            $(".preloader").show();
            $.ajax({
                url: '/api/Results/RemoveResult',
                type: 'GET',
                headers: { Authorization: bearer },
                data: { ResultId: button.closest("tr").find('.ResultId').val(), ID_Bureauvote: $('#ID_Bureauvote').val() },
                success: function (resp) {
                    if (resp) {
                        swal("Success! Record has been deleted!", { icon: "success", });
                        resettable()
                        var table = $('#tbllist tbody');
                        $.each(resp, function (i, v) {
                            if (i == 0) {
                                $('#Abstentions').val(v.Abstentions)
                                $('#Nuls').val(v.Nuls)
                                $('#Exprimes').val(v.Exprimes)
                                $('#Total_Votes').val(v.Total_Votes)
                            }
                            table.append('<tr><td><input type="hidden" class="ResultId" value="' + v.ID_Result + '" /><input type="hidden" class="CandidateId" value="' + v.ID_Candidat + '" />' + v.Candidate + '</td><td><input type="hidden" class="PartyId" value="' + v.ID_Party + '" />' + v.Party + '</td><td class="votes">' + v.Votes + '</td><td class="percen">' + v.Pourcentage + '</td><td><button type="button" onClick="removerecord($(this))" class="btn btn-sm btn-danger"> X </button></td></tr>')
                        });
                    }
                    else {
                        swal("Oops! something wrong please try again!", { icon: "error", });
                        //$.toast({ heading: 'Oops', text: 'something wrong please try again.', position: 'top-right', loaderBg: '#ff6849', icon: 'error', hideAfter: 3500, stack: 6 });
                    }
                    $(".preloader").fadeOut();
                },
                error: function () {
                    $(".preloader").fadeOut();
                }
            });

        } else {
            swal("Your record is safe!");
        }
    });
}
function resetList() {
    $('#ID_Candidat').val('');
    $('#ID_Party').val('');
    $('#ID_Party').css("background", "");
    $('#Voix').val(0);

}
function resettable() {
    $('#tbllist tbody>tr').remove();
}

function addToListAndDataBase() {
    if ($('#ID_Candidat').val() != '' && $('#Voix').val() > 0) {
        var votes = parseInt($('#Voix').val());
        $('#Total_Votes').val(parseInt($('#Voix').val()) + parseInt($('#Total_Votes').val()));
        $('#Exprimes').val(parseInt($('#Total_Votes').val()) + parseInt($('#Abstentions').val()) + parseInt($('#Nuls').val()))
        var perc = 0;
        var tbl_Results = {
            ID_Result: $('#ID_Result').val(),
            ID_Bureauvote: $('#ID_Bureauvote').val(),
            Votants: $('#Votants').val(),
            Total_Votes: parseInt($('#Total_Votes').val()),
            Abstentions: $('#Abstentions').val(),
            Nuls: $('#Nuls').val(),
            Exprimes: $('#Exprimes').val(),
            ID_Candidat: $('#ID_Candidat').val(),
            ID_Party: $('#ID_Party').val(),
            Pourcentage: perc,
            Voix: votes
        };
        $(".preloader").show();
        $.ajax({
            url: '/api/Results/SaveListRecord',
            type: "POST",
            headers: { Authorization: bearer },
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(tbl_Results),
            success: function (resp) {
                if (resp.ContentType == null) {
                    resettable()
                    resp = resp.Data;
                    var table = $('#tbllist tbody');
                    $.each(resp, function (i, v) {
                        if (i == 0) {
                            $('#Abstentions').val(v.Abstentions)
                            $('#Nuls').val(v.Nuls)
                            $('#Exprimes').val(v.Exprimes)
                            $('#Total_Votes').val(v.Total_Votes)
                        }
                        table.append('<tr><td><input type="hidden" class="ResultId" value="' + v.ID_Result + '" /><input type="hidden" class="CandidateId" value="' + v.ID_Candidat + '" />' + v.Nom + '</td><td><input type="hidden" class="PartyId" value="' + v.ID_Party + '" />' + v.Party + '</td><td class="votes">' + v.Votes + '</td><td class="percen">' + v.Pourcentage + '</td><td><button type="button" onClick="removerecord($(this))" class="btn btn-sm btn-danger"> X </button></td></tr>')
                    });
                    //$.toast({ text: 'Record save successfully', position: 'top-right', loaderBg: '#ff6849', icon: 'success', hideAfter: 3500, stack: 2 });
                    swal("Record save successfully!", "", "success")
                    resetList()
                }
                else {
                    if (resp.ContentType == 'fail')
                        swal("Oops!", resp.Data, "error")
                        //$.toast({ text: resp.Data, position: 'top-right', loaderBg: '#ff6849', icon: 'warning', hideAfter: 3500, stack: 2 });
                    else
                        swal("Oops! something wrong please try again!", { icon: "error", });
                        //$.toast({ text: 'Oops something wrong try after sometime', position: 'top-right', loaderBg: '#ff6849', icon: 'error', hideAfter: 3500, stack: 2 });
                }
                $(".preloader").fadeOut();
            },
            error: function (err) {
                $(".preloader").fadeOut();
                alert('something wrong with addToListAndDataBase');
            }
        });


    }
    else {
        if ($('#Voix').val() == 0) {
            swal('Enter votes more than zero.')
            //$.toast({ heading: 'Invalid Value', text: 'Enter votes more than zero.', position: 'top-right', loaderBg: '#ff6849', icon: 'error', hideAfter: 3500, stack: 6 });
        }
        if ($('#ID_Candidat').val() == '') {
            swal('Select Candidate.')
            //$.toast({ heading: 'Invalid Candidate', text: 'Select Candidate.', position: 'top-right', loaderBg: '#ff6849', icon: 'error', hideAfter: 3500, stack: 6 });
        }
    }
}

$("#form").validate({
    rules: {
        ID_Province: { required: true },
        ID_Territoire: { required: true },
        ID_Commune: { required: true},
        ID_Bureauvote: { required: true},
        Abstentions: { required: true},
        Nuls: { required: true},
        Exprimes: { required: true},
        ID_Candidat: { required: true},
        Voix: { required: true}

    },
    messages: {
        ID_Province: "This field is required",
        ID_Territoire: "This field is required",
        ID_Commune: "This field is required",
        ID_Bureauvote: "This field is required",
        Abstentions: "This field is required",
        Nuls: "This field is required",
        Exprimes: "This field is required",
        ID_Candidat: "This field is required",
        Voix: "This field is required"
    },
});