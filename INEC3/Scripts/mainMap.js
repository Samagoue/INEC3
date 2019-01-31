var swidth = 0, sheight = 0, scale = 0;
var width = $('#mapcontainer').width(),
    height = $('#mapcontainer').height()
var iscancel = false;
var iszoom = false;
var activeProvince = 0;
sheight = height / 3;
scale = (height * 3);
if ($(window).width() > 1440) {
    swidth = width / 4
    scale = scale - 80
    var projection = d3.geoMercator().scale(scale).translate([swidth, sheight]);//For Tablet
    console.log('4k Display');
}
else if (1440 >= $(window).width() && $(window).width() > 768) {
    swidth = -70
    scale = scale - 80
    var projection = d3.geoMercator().scale(scale).translate([swidth, sheight]);
    console.log('Laptop or Desktop');
    //console.log(width / 11);
}
else if (768 >= $(window).width() && $(window).width() > 425) {
    swidth = -150;
    scale = width * 2;
    var projection = d3.geoMercator().scale(scale).translate([swidth, sheight]);//For Tablet
    console.log('Tablet View');
}
else {
    swidth = -180
    scale = height * 2;
    var projection = d3.geoMercator().scale(scale).translate([swidth, sheight]);
    console.log('Mobile View');
}


var path = d3.geoPath().projection(projection);

var zoom = d3.zoom().scaleExtent([1, 8]).on("zoom", zoomed);

var svg = d3.select("#map").append("svg")
    .attr("width", width)
    .attr("height", height)
    .call(zoom)
    .on("click", stopped, true);

var g = svg.append("g");

svg.call(zoom);

var gui = d3.select("#map");

var tooltip = d3.select("body")
    .append("div")
    .attr("class", "nvtooltip xy-tooltip")
    .attr("id", "tooltip")
    .style("position", "absolute")
    .style("z-index", "10")
    .style("visibility", "hidden");


var states, counties, statePaths;
$(document).ready(function () {
    d3.json("/Resources/COD_TOPO.json", function (error, cod) {
        if (error) throw error;
        states = topojson.feature(cod, cod.objects.Provinces).features
        counties = topojson.feature(cod, cod.objects.Territoires).features

        statePaths = g.selectAll('.state')
            .data(states)
            .enter().append('path')
            .attr('class', 'state')
            .attr('d', path)
            .style('fill', "white")
            .attr("stateCode", function (d) { return d.properties.GID_1.slice(4); })
            .on("mouseover", function (d) { if (GenerateTooltip(d)) { return tooltip.style("visibility", "visible"); } })
            .on("mousemove", function () { return tooltip.style("top", (d3.event.pageY - 10) + "px").style("left", (d3.event.pageX + 10) + "px"); })
            .on("mouseout", function () { FillToolTipChart(); return tooltip.style("visibility", "hidden"); })
            .on("click", function (d) { clicked(d) });
        g.selectAll("text")
            .data(topojson.feature(cod, cod.objects.Provinces).features)
            .enter()
            .append("svg:text")
            .text(function (d) { return d.properties.NAME_1.slice(0, 6); })
            .attr("x", function (d) { return path.centroid(d)[0]; })
            .attr("y", function (d) { return path.centroid(d)[1]; })
            .attr("text-anchor", "middle")
            .attr('font-size', '6pt');
        FillProvinceColor();
    });
});
function clicked(d) {
    activeProvince = d.properties.GID_1.slice(4);
    //var state = states.find(function (e) { return e.properties.GID_1.slice(4) === d.properties.GID_1.slice(4) })
    var stateCounties = counties.filter(function (e) {
        return e.properties.GID_1.slice(4) === d.properties.GID_1.slice(4)
    })

    var t = d3.transition().duration(800)

    var countyPaths = g.selectAll('.county')
        .data(stateCounties, function (e) { return e.properties.GID_1.slice(4) })

    var enterCountyPaths = countyPaths.enter().append('path')
        .attr('class', 'county')
        .attr('d', path)
        .style('fill', function (d) { return GetTerritoiresColor(d) })
        .attr("territoiryCode", function (d) { return d.properties.GID_2.slice(4).split('.')[1]; })
        //.style('fill', function (d) { var c = TerritoiresResult.find(function (e) { return e.GUI_2 === d.properties.GID_2.slice(4) });  return c.Color; })
        //.style('fill', "white")
        .style('opacity', 0)
        .on("mouseover", function (d) { if (GenerateTooltip(d)) { return tooltip.style("visibility", "visible"); } })
        .on("mousemove", function () { return tooltip.style("top", (d3.event.pageY - 10) + "px").style("left", (d3.event.pageX + 10) + "px"); })
        .on("mouseout", function () { return tooltip.style("visibility", "hidden"); })
        .on('click', function () { usZoom() })

    //countyPaths.enter().append("svg:text")
    //    .text(function (d) { if (d.properties.TYPE_2 !== "Ville") { return d.properties.NAME_2.slice(0, 4); } })
    //    .attr("x", function (d) { return path.centroid(d)[0]; })
    //    .attr("y", function (d) { return path.centroid(d)[1]; })
    //    .attr("text-anchor", "middle")
    //    .attr('font-size', '3pt');

    statePaths.transition(t).attr('d', path).attr('class', 'deactive')
    //.style('fill', '#444')

    enterCountyPaths.transition(t).attr('d', path).style('opacity', 1)

    countyPaths.exit().transition(t).attr('d', path).style('opacity', 0).remove()

    //----------------------------------------------------------
    iszoom = true;
    iscancel = false;
    $("#btncancel").prop('disabled', false);

    var bounds = path.bounds(d),
        dx = bounds[1][0] - bounds[0][0],
        dy = bounds[1][1] - bounds[0][1],
        x = (bounds[0][0] + bounds[1][0]) / 2,
        y = (bounds[0][1] + bounds[1][1]) / 2,
        scale = Math.max(1, Math.min(3.5, 0.6 / Math.max(dx / width, dy / height))),
        translate = [width / 2 - scale * x, height / 2 - scale * y];

    svg.transition().duration(750).call(zoom.transform, d3.zoomIdentity.translate(translate[0], translate[1]).scale(scale));

}

function zoomed() {
    g.style("stroke-width", 1.5 / d3.event.transform.k + "px");
    g.attr("transform", d3.event.transform);
    FillToolTipChart();
}
function stopped() {
    if (d3.event.defaultPrevented) d3.event.stopPropagation();
}

function GenerateTooltip(res) {
    //debugger
    var tltipdiv = document.getElementById('tooltip');
    var table = document.getElementById('nvtable');
    table.removeAttribute('hidden');
    tltipdiv.appendChild(table);
    $("#nvtable tbody tr").remove();
    if (iszoom && !iscancel) {
        $('#header').text(res.properties.NAME_2);
        //debugger
        var tltip = TerritoiresResult.filter(function (e) { if (res.properties.GID_2) { return e.GUI_2 === res.properties.GID_2.slice(4) } });
        if (tltip.length > 0) {
            $('#LblToltiptitle').text('Results of ' + res.properties.NAME_2);
            $('#TooltipChart').find("li").remove();
            $.each(tltip, function (i, v) {
                $('#lblreporting').text(v.ReportedPerc + '% Reporting');
                if (i === 0)
                    $(table).find('tbody').append("<tr><td class=\"legend-color-guide\"><div style=\"background-color:" + v.Color + ";\"></div></td><td class=\"key\" colspan=\"2\" style=\"background-color:" + v.Color + ";\">" + v.Candidat + "</td><td class=\"key\">" + v.Party + "</td><td class=\"key\">" + v.Votants + "</td><td class=\"value\">" + v.Perce + " %</td></tr>");
                else
                    $(table).find('tbody').append("<tr><td class=\"legend-color-guide\"><div style=\"background-color:" + v.Color + ";\"></div></td><td class=\"key\" colspan=\"2\" \">" + v.Candidat + "</td><td class=\"key\">" + v.Party + "</td><td class=\"key\">" + v.Votants + "</td><td class=\"value\">" + v.Perce + " %</td></tr>");
                //Set Tooltip Chart
                $('#TooltipChart').append("<li><h2>" + v.Candidat + "</h2> <small>" + v.Votants + "</small><div class=\"pull-right\">" + v.Perce + "% <i class=\"fa fa-level-up text-success\"></i></div><div class=\"progress\"><div class=\"progress-bar progress-bar-info\" role=\"progressbar\" aria-valuenow=\"50\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width:" + v.Perce + "%;background-color:" + v.Color + ";\"> <span class=\"sr-only\">20% Complete</span></div></div></li>");
            });
            tltipdiv.appendChild(table);
            return true;
        }
        return false;
    }
    else {
        $('#header').text(res.properties.NAME_1);

        var tltip = ProvinceResult.filter(function (e) { return e.GUI_1 === res.properties.GID_1.slice(4) });
        var tvote = 0;
        if (tltip.length > 0) {
            $.each(tltip, function (i, v) {
                tvote += parseInt(v.Votants);
            });
            $('#LblToltiptitle').text('Results of ' + res.properties.NAME_1);
            $('#TooltipChart').find("li").remove();
            $.each(tltip, function (i, v) {
                $('#lblreporting').text(v.ReportedPerc + '% Reporting');
                var per = (parseInt(v.Votants) * 100 / tvote).toFixed(2);
                if (i === 0)
                    $(table).find('tbody').append("<tr><td class=\"legend-color-guide\"><div style=\"background-color:" + v.Color + ";\"></div></td><td class=\"key\" colspan=\"2\" style=\"background-color:" + v.Color + ";\">" + v.Candidat + "</td><td class=\"key\">" + v.Party + "</td><td class=\"key\">" + v.Votants + "</td><td class=\"value\">" + per + " %</td></tr>");
                else
                    $(table).find('tbody').append("<tr><td class=\"legend-color-guide\"><div style=\"background-color:" + v.Color + ";\"></div></td><td class=\"key\" colspan=\"2\" \">" + v.Candidat + "</td><td class=\"key\">" + v.Party + "</td><td class=\"key\">" + v.Votants + "</td><td class=\"value\">" + per + " %</td></tr>");
                //Set Tooltip Chart
                $('#TooltipChart').append("<li><h2>" + v.Candidat + "</h2> <small>" + v.Votants + "</small><div class=\"pull-right\">" + per + "% <i class=\"fa fa-level-up text-success\"></i></div><div class=\"progress\"><div class=\"progress-bar progress-bar-info\" role=\"progressbar\" aria-valuenow=\"50\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width:" + per + "%;background-color:" + v.Color + ";\"> <span class=\"sr-only\">20% Complete</span></div></div></li>");
            });
            tltipdiv.appendChild(table);
            return true;
        }
        return false;
    }
    tltipdiv.appendChild(table);
    return true;
}
function activearea() {
    if (iscancel) {
        iscancel = false;
        $("#btncancel").prop('disabled', false);
        iszoom = true;
    }
    else {
        iscancel = true;
        $("#btncancel").prop('disabled', true);
        iszoom = false;
        reset();
    }
}

$(function () {
    $('#btnzoomin').click(function () { zoom.scaleBy(svg, 2); });
    $('#btnzoomout').click(function () { zoom.scaleBy(svg, 0.5); });
    FillTopCandidate();
});
function reset() {
    usZoom();

}
function usZoom() {
    iscancel = false;
    $("#btncancel").prop('disabled', true);
    iszoom = false;
    var t = d3.transition().duration(800)

    projection.scale(scale).translate([swidth, sheight])

    statePaths.transition(t).attr('d', path).attr('class', 'state');

    g.selectAll('.county')
        .data([])
        .exit().transition(t)
        .attr('d', path)
        .style('opacity', 0)
        .remove()
    svg.transition().duration(750).call(zoom.transform, d3.zoomIdentity);

}

$(document).ready(function () {
    // Declare a proxy to reference the hub.
    var realTimeMapHub = $.connection.realTimeMapHub;
    // Create a function that the hub can call to broadcast messages.
    realTimeMapHub.client.mapUpdate = function (updatedresult) {
        //$("[stateCode=1_1]").css("fill", "#e2e2e2");
        var updated = JSON.parse(updatedresult);
        ProvinceResult = updated.Table
        TerritoiresResult = updated.Table1
        TopCandidate = updated.Table2
        ReportPolStation = updated.Table3
        LastUpdatedPoolstn = updated.Table4
        FillProvinceColor()
        FillTopCandidate()
        if (iszoom) {
            FillTerritoiresColor();
        }
        toastr.success("Record Updated");
    };
    $.connection.hub.start().done(function () { console.log("hub done"); });

    $(window).resize(function () {
        location.reload(true);
    });
});

function FillProvinceColor() {
    $('.state').css("fill", 'white');//Use To Reset Color
    if (ProvinceResult) {
        var last;
        $.each(ProvinceResult, function (i, v) {
            if (last !== ProvinceResult[i].GUI_1) {
                last = ProvinceResult[i].GUI_1
                $("[stateCode=" + ProvinceResult[i].GUI_1 + "]").css("fill", ProvinceResult[i].Color);
            }
        });
    }
}
function FillTerritoiresColor() {
    $('.county').css("fill", 'white');
    if (TerritoiresResult) {
        var last;
        $.each(TerritoiresResult, function (i, v) {

            if (activeProvince === TerritoiresResult[i].GUI_1) {
                if (last !== TerritoiresResult[i].GUI_2.split('.')[1]) {
                    last = TerritoiresResult[i].GUI_2.split('.')[1];
                    $("[territoiryCode=" + TerritoiresResult[i].GUI_2.split('.')[1] + "]").css("fill", TerritoiresResult[i].Color);
                }
            }
        });
    }
}
function GetTerritoiresColor(tid) {
    var j = TerritoiresResult.find(function (e) { return e.GUI_2 == tid.properties.GID_2.slice(4) })
    if (j) {
        return j.Color;
    }
    else {
        return "white";
    }
}
function FillTopCandidate() {
    if (TopCandidate) {
        //$.each(TopCandidate, function (i, v) {
        for (var i = 0; i < 2; i++) {
            $("#lblcandidate" + i).html((TopCandidate[i]) ? TopCandidate[i].Candidat : 'Top Candidate');
            $("#lblcandidatePer" + i).html((TopCandidate[i]) ? TopCandidate[i].Perc + ' %' : '0 %');
            $("#lblcandidateVote" + i).html((TopCandidate[i]) ? TopCandidate[i].Votants : '');
            if (TopCandidate[i])
                $("#imgcandidate" + i).attr("src", "/Content/image/Candimg_" + TopCandidate[i].Candidatimg + ".png");
            else
                $("#imgcandidate" + i).attr("src", "/Content/image/logo.png");
        }
        //);
        FillToolTipChart();
    }
    if (ReportPolStation) {
        if (ReportPolStation[0]) {
            $('#ReportPolstation').html(ReportPolStation[0].Reportstation);
            $('#PersPolstation').html(ReportPolStation[0].PersPolstation + ' %');
        }
        else {
            $('#ReportPolstation').html('');
            $('#PersPolstation').html('0 %');
        }
    }
    if (LastUpdatedPoolstn) {
        if (LastUpdatedPoolstn[0]) {
            $('#lbllastupdatedProvinceName').html(LastUpdatedPoolstn[0].ProvinceName);
            $('#lbllastupdatedProvince').html(LastUpdatedPoolstn[0].Province);
            $('#lbllastupdatedVotants').html(LastUpdatedPoolstn[0].Votants);
            $('#lbllastupdatedVotantsName').html(LastUpdatedPoolstn[0].Polingstation);
        }
        else {
            $('#lbllastupdatedProvinceName').html('Province');
            $('#lbllastupdatedProvince').html('0');
            $('#lbllastupdatedVotants').html('0');
            $('#lbllastupdatedVotantsName').html('Poling Stat..');
        }
    }
}
function FillToolTipChart() {
    $('#LblToltiptitle').text('GLOBAL RESULTS');
    var TltipChart = $('#TooltipChart');
    TltipChart.find("li").remove();
    $.each(TopCandidate, function (i, v) {
        TltipChart.append("<li><h2>" + v.Candidat + "</h2> <small>" + v.Votants + "</small><div class=\"pull-right\">" + v.Perc + "% <i class=\"fa fa-level-up text-success\"></i></div><div class=\"progress\"><div class=\"progress-bar progress-bar-info\" role=\"progressbar\" aria-valuenow=\"50\" aria-valuemin=\"0\" aria-valuemax=\"100\" style=\"width:" + v.Perc + "%;background-color:" + v.Color + ";\"> <span class=\"sr-only\">20% Complete</span></div></div></li>");
    });
}
