﻿var width = 960,
    height = 600,
    padding = 100

var iscancel = false;
var iszoom = false;
var city;

var color = ["#8dd3c7", "#ffffb3", "#bebada", "#fb8072", "#80b1d3", "#fdb462", "#b3de69", "#fccde5", "#fbb4ae", "#b3cde3", "#ccebc5", "#decbe4", "#fed9a6", "#ffffcc", "#e5d8bd", "#fddaec", "#e41a1c", "#377eb8", "#4daf4a", "#984ea3", "#ff7f00", "#ffff33", "#a65628", "#f781bf"];

var projection = d3.geoMercator().scale(1200).translate([(width / 40) - padding, height / 3]);

var path = d3.geoPath().projection(projection);

var zoom = d3.zoom().scaleExtent([1, 8]).on("zoom", zoomed);

var color = d3.scaleThreshold().domain([0.028, 0.038, 0.048, 0.058, 0.068, 0.078]).range(['#4d9221', '#a1d76a', '#e6f5d0', '#f7f7f7', '#fde0ef', '#e9a3c9', '#c51b7d']);

//d3.select("svg").remove();
var svg = d3.select("#map").append("svg")
    .attr("width", width)
    .attr("height", height)
    .style("background", "#5d5959")
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
        .on("mouseout", function () { return tooltip.style("visibility", "hidden"); })
        .on("click", function (d) { clicked(d) });
    g.selectAll("text")
        .data(topojson.feature(cod, cod.objects.Provinces).features)
        .enter()
        .append("svg:text")
        .text(function (d) { return d.properties.NAME_1.slice(0, 8); })
        .attr("x", function (d) { return path.centroid(d)[0]; })
        .attr("y", function (d) { return path.centroid(d)[1]; })
        .attr("text-anchor", "middle")
        .attr('font-size', '6pt');
    FillProvinceColor();
});

function clicked(d) {
    var state = states.find(function (e) { return e.properties.GID_1.slice(4) === d.properties.GID_1.slice(4) })
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

    statePaths.transition(t)
        .attr('d', path)
        .attr('class', 'deactive')
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
        var tltip = TerritoiresResult.filter(function (e) { return e.GUI_2 === res.properties.GID_2.slice(4) });
        if (tltip.length > 0) {
            $.each(tltip, function (i, v) {
                if (i === 0)
                    $(table).find('tbody').append("<tr><td class=\"legend-color-guide\"><div style=\"background-color:" + v.Color + ";\"></div></td><td class=\"key\" colspan=\"2\" style=\"background-color:" + v.Color + ";\">" + v.Candidat + "</td><td class=\"key\">" + v.Party + "</td><td class=\"key\">" + v.Votants + "</td><td class=\"value\">" + v.Perce + " %</td></tr>");
                else
                    $(table).find('tbody').append("<tr><td class=\"legend-color-guide\"><div style=\"background-color:" + v.Color + ";\"></div></td><td class=\"key\" colspan=\"2\" \">" + v.Candidat + "</td><td class=\"key\">" + v.Party + "</td><td class=\"key\">" + v.Votants + "</td><td class=\"value\">" + v.Perce + " %</td></tr>");
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
            console.log(tvote);

            $.each(tltip, function (i, v) {
                debugger


                if (i === 0)
                    $(table).find('tbody').append("<tr><td class=\"legend-color-guide\"><div style=\"background-color:" + v.Color + ";\"></div></td><td class=\"key\" colspan=\"2\" style=\"background-color:" + v.Color + ";\">" + v.Candidat + "</td><td class=\"key\">" + v.Party + "</td><td class=\"key\">" + v.Votants + "</td><td class=\"value\">" + (parseInt(v.Votants) * 100 / tvote).toFixed(2) + " %</td></tr>");
                else
                    $(table).find('tbody').append("<tr><td class=\"legend-color-guide\"><div style=\"background-color:" + v.Color + ";\"></div></td><td class=\"key\" colspan=\"2\" \">" + v.Candidat + "</td><td class=\"key\">" + v.Party + "</td><td class=\"key\">" + v.Votants + "</td><td class=\"value\">" + (parseInt(v.Votants) * 100 / tvote).toFixed(2) + " %</td></tr>");
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
});
function reset() {
    usZoom();
}
function usZoom() {
    iscancel = false;
    $("#btncancel").prop('disabled', true);
    iszoom = false;
    var t = d3.transition().duration(800)
    projection.scale(1200).translate([(width / 40) - padding, height / 3])
    debugger
    statePaths.transition(t).attr('d', path).attr('class', '');
    //statePaths.transition(t).attr('d', path).style('fill', "white")

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
        console.log(updatedresult)
        //$("[stateCode=1_1]").css("fill", "#e2e2e2");
        //$("[stateCode=1_1]").css("fill", color[1]);
        var updated = JSON.parse(updatedresult);
        ProvinceResult = updated.Table
        TerritoiresResult = updated.Table1
        FillProvinceColor()
        toastr.success("Record Updated");
    };
    $.connection.hub.start().done(function () { console.log("hub done"); });
});

function FillProvinceColor() {
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
function GetTerritoiresColor(tid) {
    var j = TerritoiresResult.find(function (e) { return e.GUI_2 == tid.properties.GID_2.slice(4) })
    if (j) {
        return j.Color;
    }
    else {
        return "white";
    }
}